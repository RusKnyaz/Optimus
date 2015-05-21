using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using WebBrowser.Dom.Elements;
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

	public class Document : DocumentFragment
	{
		private readonly IResourceProvider _resourceProvider;
		private readonly SynchronizationContext _context;

		internal Document()
			:this(null, new StubSynchronizationContext())
		{
			
		}

		internal Document(IResourceProvider resourceProvider, SynchronizationContext context)
		{
			_resourceProvider = resourceProvider;
			_context = context;
			ChildNodes = new List<INode> {new Element("html"){Parent = this}};
			_unresolvedDelayedResources = new List<IDelayedResource>();
			NodeType = DOCUMENT_NODE;
		}

		public Element DocumentElement { get; private set; }

		public override string NodeName { get { return "#document"; }}

		private readonly List<IDelayedResource> _unresolvedDelayedResources;

		public void Write(string text)
		{
			Load(DocumentBuilder.Build(this, text));
		}

		internal void Load(IEnumerable<INode> elements)
		{
			ChildNodes.Clear();
			
			foreach(var element in elements)
			{
				element.Parent = this;
				ChildNodes.Add(element);

				var delayed = element as IDelayedResource;
				if (delayed != null)
					_unresolvedDelayedResources.Add(delayed);

				RegisterDelayed(element.ChildNodes);
			}

			if (ChildNodes.Count > 1)
			{
				var rootElem = ChildNodes[0] as Element;
				if (rootElem == null || rootElem.TagName != "html")
				{
					rootElem = new Element("html"){Parent = this};
					foreach (var element in ChildNodes)
					{
						element.Parent = rootElem;
						rootElem.ChildNodes.Add(element);	
						ChildNodes.Clear();
						ChildNodes.Add(rootElem);
					}
				}
			}

			DocumentElement = ChildNodes.FirstOrDefault() as Element ?? new Element("html");

			foreach (var childNode in ChildNodes)
			{
				childNode.Parent = this;
			}

			foreach (var childNode in DocumentElement.Flatten())
			{
				childNode.OwnerDocument = this;
			}

			Trigger("DOMContentLoaded");
			
		}

		private void Trigger(string type)
		{
			var evt = CreateEvent("Event");
			evt.InitEvent(type, false, false);
			_context.Post(state => DispatchEvent(evt), null);
		}

		private void RegisterDelayed(IEnumerable<INode> elements)
		{
			foreach (var documentElement in elements)
			{
				var delayed = documentElement as IDelayedResource;
				if (delayed != null)
					_unresolvedDelayedResources.Add(delayed);

				RegisterDelayed(documentElement.ChildNodes);
			}
		}

		public void ResolveDelayedContent()
		{
			foreach (var delayedResource in _unresolvedDelayedResources)
			{
				try
				{
					delayedResource.Load(_resourceProvider);
				}
				catch
				{
					//todo: handle error;
				}
			}
			_unresolvedDelayedResources.Clear();
			
			Trigger("load");
		}

		internal void RunScripts(IScriptExecutor executor)
		{
			//todo: what we should do if some script changes ChildNodes?
			foreach (var documentElement in ChildNodes.SelectMany(x => x.Flatten()).OfType<IScript>().ToArray())
			{
				executor.Execute(documentElement.Type, documentElement.Text);
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
				case "b":
					return new HtmlElement(tagName) {OwnerDocument = this};
				case "input":
					return new HtmlInputElement {OwnerDocument = this};
			}

			return new Element(tagName) { OwnerDocument = this};
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
			return new DocumentFragment{ OwnerDocument = this};
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

		public Event CreateEvent(string type)
		{
			if (type == null) throw new ArgumentNullException("type");
			if(type == "Event")
				return new Event();

			//todo: UIEvent

			throw new NotSupportedException("Specified event type is not supported: " + type);
		}
	}
}
