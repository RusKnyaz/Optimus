using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebBrowser.Dom.Elements;
using WebBrowser.Environment;
using WebBrowser.ScriptExecuting;

namespace WebBrowser.Dom
{
	class StubSynchronizationContext : SynchronizationContext
	{
		public override void Post(SendOrPostCallback d, object state)
		{
			d(state);
		}

		public override void Send(SendOrPostCallback d, object state)
		{
			d(state);
		}
	}

	public enum DocumentReadyStates
	{
		Loading, Interactive, Complete
	}

	/// <summary>
	/// http://www.w3.org/html/wg/drafts/html/master/dom.html#document
	/// http://dev.w3.org/html5/spec-preview/dom.html
	/// all idls http://www.w3.org/TR/REC-DOM-Level-1/idl-definitions.html
	/// </summary>
	public class Document : DocumentFragment, IDocument
	{
		private readonly IResourceProvider _resourceProvider;
		public readonly SynchronizationContext Context;
		private readonly IScriptExecutor _scriptExecutor;

		internal Document() :this(null, null, null, null)
		{
			ReadyState = DocumentReadyStates.Loading;
		}

		internal Document(IResourceProvider resourceProvider, SynchronizationContext context, IScriptExecutor scriptExecutor, 
			Window window):base(null)
		{
			_resourceProvider = resourceProvider;
			Context = context ?? new StubSynchronizationContext();
			_scriptExecutor = scriptExecutor;
			_unresolvedDelayedResources = new List<IDelayedResource>();
			NodeType = DOCUMENT_NODE;

			DocumentElement = new Element(this, "html"){ParentNode = this};
			ChildNodes = new List<Node> { DocumentElement };

			EventTarget = new EventTarget(this, () => window);
		}

		public Element DocumentElement { get; private set; }
		public DocumentReadyStates ReadyState { get; private set; }
		public override string NodeName { get { return "#document"; }}

		private readonly List<IDelayedResource> _unresolvedDelayedResources;
		
		public void Write(string text)
		{
			using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
			{
				DocumentBuilder.Build(this, stream);
			}
		}

		internal void Complete()
		{
			Trigger("DOMContentLoaded");
			//todo: check is it right
			ReadyState = DocumentReadyStates.Complete;

			ResolveDelayedContent();

			RunScripts(ChildNodes.SelectMany(x => x.Flatten()).OfType<Script>());
		}

		private void Trigger(string type)
		{
			var evt = CreateEvent("Event");
			evt.InitEvent(type, false, false);
			Context.Send(state => DispatchEvent(evt), null);
		}

		public void ResolveDelayedContent()
		{
			Task.WaitAll(_unresolvedDelayedResources.Where(x => !x.Loaded).Select(x => x.LoadAsync(_resourceProvider)).ToArray());
			_unresolvedDelayedResources.Clear();
			Trigger("load");
		}

		internal void RunScripts(IEnumerable<Script> scripts)
		{
				//todo: what we should do if some script changes ChildNodes?
				//todo: optimize (create queue of not executed scripts);
			foreach (var script in scripts.ToArray())
			{
				if (script.Executed || string.IsNullOrEmpty(script.Text)) continue;
				ExecuteScript(script);
			}
		}

		public Element CreateElement(string tagName)
		{
			if(tagName == null)
				throw new ArgumentNullException("tagName");
			if(tagName == string.Empty)
				throw new ArgumentOutOfRangeException("tagName");

			var invariantTagName = tagName.ToLowerInvariant();

			switch (invariantTagName)
			{
				//todo: fill the list
				case "div":
				case "span":
				case "b": return new HtmlElement(this, tagName);
				case "input": return new HtmlInputElement(this);
				case "script": return new Script(this);
				case "head":return new Head(this);
				case"body":return new Body(this);
			}

			return new Element(this, tagName) { OwnerDocument = this};
		}

		public Attr CreateAttribute(string name)
		{
			return new Attr(name){OwnerDocument = this};
		}

		public Element GetElementById(string id)
		{
			//todo: create index;
			return DocumentElement.Flatten().OfType<Element>().FirstOrDefault(x => x.Id == id);
		}

		public DocumentFragment CreateDocumentFragment()
		{
			return new DocumentFragment(this);
		}

		public Text CreateTextNode(string data)
		{
			return new Text{Data = data, OwnerDocument = this};	
		}

		public Comment CreateComment(string data)
		{
			return new Comment { Data = data, OwnerDocument = this };
		}

		public Element Body
		{
			get { return DocumentElement.GetElementsByTagName("body").FirstOrDefault(); }
		}

		public Head Head
		{
			get { return (Head)DocumentElement.GetElementsByTagName("head").FirstOrDefault(); }
		}

		public Event CreateEvent(string type)
		{
			if (type == null) throw new ArgumentNullException("type");
			if(type == "Event")
				return new Event();

			if(type == "CustomEvent")
				return new CustomEvent();

			if(type == "MutationEvent")
				return new MutationEvent();

			if(type == "UIEvent")
				return new UIEvent();

			if(type == "KeyboardEvent")
				return new KeyboardEvent();

			throw new NotSupportedException("Specified event type is not supported: " + type);
		}

		public event Action<Node> DomNodeInserted;

		internal void HandleNodeAdded(Node node, Node newChild)
		{
			var parent = newChild;
			var praParent = parent;
			while (praParent.ParentNode != null)
				praParent = praParent.ParentNode;

			if (praParent != this)
				return;

			foreach (var script in newChild.Flatten().OfType<Script>())
			{
				var remote = script.HasDelayedContent;
				var async = script.Async && remote || script.Source == NodeSources.Script;
				var defer = script.Defer && remote;

				if (!async && defer && ReadyState != DocumentReadyStates.Complete)
				{
					_unresolvedDelayedResources.AddRange(newChild.Flatten()
						.OfType<IDelayedResource>()
						.Where(delayed => delayed != null && delayed.HasDelayedContent && !delayed.Loaded));
				}
				else
				{
					Task task = null;
					if (remote)
					{
						task = script
							.LoadAsync(_resourceProvider)
							.ContinueWith((t, s) => ExecuteScript((Script) s), script);

						if (!async)
							task.Wait();
					}
					else if (!string.IsNullOrEmpty(script.Text))
					{
						//task = new Task(s => ExecuteScript((Script)s), script);
						//task.Start(TaskScheduler.Default);
						ExecuteScript(script);
					}
				}
			}

			if(newChild.Source != NodeSources.DocumentBuilder)
				RaiseDomNodeInserted(newChild);
		}

		private void RaiseDomNodeInserted(Node newChild)
		{
			if (DomNodeInserted != null)
				DomNodeInserted(newChild);

			var evt = (MutationEvent)CreateEvent("MutationEvent");
			evt.InitMutationEvent("DOMNodeInserted", false, false, newChild.ParentNode, null, null, null, 0);
			evt.Target = newChild;
			DispatchEvent(evt);
		}

		private void ExecuteScript(Script script)
		{
			script.Execute(_scriptExecutor);

			Context.Send(x => RaiseAfterScriptExecute(script), null);
		}

		public event Action<Script> AfterScriptExecute;

		private void RaiseAfterScriptExecute(Script script)
		{
			if (AfterScriptExecute != null)
				AfterScriptExecute(script);

			var evt = CreateEvent("Event");
			evt.InitEvent("AfterScriptExecute",false, false);
			evt.Target = script;
			DispatchEvent(evt);
		}

		internal void HandleNodeEventException(Node node, Exception exception)
		{
			if (OnNodeException != null)
				OnNodeException(node, exception);
		}

		public event Action<Node, Exception> OnNodeException;
	}

	[DomItem]
	public interface IDocument
	{
		Element CreateElement(string tagName);
		Element DocumentElement { get; }
		void Write(string text);
		Event CreateEvent(string type);
		Head Head { get; }
		Element Body { get; }
		Comment CreateComment(string data);
		Text CreateTextNode(string data);
		DocumentFragment CreateDocumentFragment();
		Element GetElementById(string id);
		Attr CreateAttribute(string name);
	}
}
