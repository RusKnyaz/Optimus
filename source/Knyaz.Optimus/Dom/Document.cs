using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.Dom.Events;
using Knyaz.Optimus.Environment;
using Knyaz.Optimus.ScriptExecuting;

namespace Knyaz.Optimus.Dom
{
	public static class DocumentReadyStates
	{
		public const string Loading = "loading";
		public const string Interactive = "interactive";
		public const string Complete = "complete";
	}

	/// <summary>
	/// http://www.w3.org/html/wg/drafts/html/master/dom.html#document
	/// http://dev.w3.org/html5/spec-preview/dom.html
	/// all idls http://www.w3.org/TR/REC-DOM-Level-1/idl-definitions.html
	/// </summary>
	public class Document : DocumentFragment, IDocument
	{
		internal Document() :this(null)
		{
			ReadyState = DocumentReadyStates.Loading;
		}

		internal Document(Window window):base(null)
		{
			NodeType = DOCUMENT_NODE;

			DocumentElement = CreateElement(TagsNames.Html);
			ChildNodes.Add(DocumentElement);
			DocumentElement.ParentNode = this;
			DocumentElement.OwnerDocument = this;

			EventTarget = new EventTarget(this, () => window, () => this);
			DefaultView = window;

			ReadyState = DocumentReadyStates.Loading;
		}

		public Window DefaultView { get; private set; }
		public Element DocumentElement { get; private set; }
		public string ReadyState { get; private set; }
		public override string NodeName { get { return "#document"; }}
		[Obsolete("Use window.location")]
		//todo: check is it true for frames
		public Location Location { get { return DefaultView.Location; } }
		public string DocumentURI {get { return DefaultView.Location.Href; }  set { DefaultView.Location.Href = value; }}
		public IEnumerable<HtmlFormElement> Forms { get { return GetElementsByTagName("form").Cast<HtmlFormElement>(); } }
		public IEnumerable<IHtmlScriptElement> Scripts { get { return GetElementsByTagName("script").Cast<IHtmlScriptElement>(); } }

		public void Write(string text)
		{
			using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
			{
				DocumentBuilder.Build(this, stream);
			}
		}

		public event Action<IDocument> DomContentLoaded;

		internal void Complete()
		{
			ReadyState = DocumentReadyStates.Interactive;

			if (DomContentLoaded != null)
				DomContentLoaded (this);

			Trigger("DOMContentLoaded");
			//todo: check is it right
			ReadyState = DocumentReadyStates.Complete;

			//todo: we should fire this event properly
			Trigger("load");
		}

		private void Trigger(string type)
		{
			var evt = CreateEvent("Event");
			evt.InitEvent(type, false, false);
			lock (this)
			{
				//todo: deadlock possible if event raised from js
				DispatchEvent(evt);	
			}
		}

		public Element CreateElement(string tagName)
		{
			if(tagName == null)
				throw new ArgumentNullException("tagName");
			if(tagName == string.Empty)
				throw new ArgumentOutOfRangeException("tagName");

			var invariantTagName = tagName.ToUpperInvariant();
			switch (invariantTagName)
			{
				//todo: fill the list
				case TagsNames.Link: return new HtmlLinkElement(this);
				case TagsNames.Style: return new HtmlStyleElement(this);
				case TagsNames.Select: return new HtmlSelectElement(this);
				case TagsNames.Option: return new HtmlOptionElement(this);
				case "DIV": return new HtmlDivElement(this);
				case "SPAN":
				case TagsNames.Nav:
				case "B": return new HtmlElement(this, invariantTagName);
				case TagsNames.Button: return new HtmlButtonElement(this);
				case TagsNames.Input: return new HtmlInputElement(this);
				case TagsNames.Script: return new Script(this);
				case TagsNames.Head:return new Head(this);
				case TagsNames.Body:return new HtmlBodyElement(this);
				case TagsNames.Textarea: return new HtmlTextAreaElement(this);
				case TagsNames.Form:return new HtmlFormElement(this);
				case TagsNames.IFrame:return new HtmlIFrameElement(this);
				case TagsNames.Html:return new HtmlHtmlElement(this);
			}

			return new HtmlUnknownElement(this, invariantTagName);
		}

		public Attr CreateAttribute(string name)
		{
			return new Attr(name){OwnerDocument = this};
		}

		public Element GetElementById(string id)
		{
			return DocumentElement.Flatten().OfType<Element>().FirstOrDefault(x => x.Id == id);
		}

		public IReadOnlyCollection<Element> GetElementsByName(string name)
		{
			return DocumentElement.Flatten().OfType<Element>().Where(x => x.GetAttribute("name") == name).ToList().AsReadOnly();
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

			/*todo: the event have own logic of propogation. it have not type. i do not know what should i do
			 * if(type == "ErrorEvent")
				return new ErrorEvent();*/

			throw new NotSupportedException("Specified event type is not supported: " + type);
		}

		public event Action<Node> DomNodeInserted;
		public event Action<Node> NodeInserted;
		public event Action<Node, Node> NodeRemoved;

		internal void HandleNodeRemoved(Node parent, Node node)
		{
			if (NodeRemoved!= null)
				NodeRemoved(parent, node);
		}

		internal void HandleNodeAdded(Node newChild)
		{
			if (!newChild.IsInDocument ())
				return;

			if (NodeInserted != null)
				NodeInserted (newChild);

			if (newChild.Source != NodeSources.DocumentBuilder)
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


		internal void HandleNodeEventException(Node node, Exception exception)
		{
			if (OnNodeException != null)
				OnNodeException(node, exception);
		}

		public event Action<Node, Exception> OnNodeException;

		internal void HandleFormSubmit(HtmlFormElement htmlFormElement)
		{
			if (OnFormSubmit != null)
				OnFormSubmit(htmlFormElement);
		}

		internal event Action<HtmlFormElement> OnFormSubmit;

		protected override void RegisterNode(Node node)
		{
			node.ParentNode = this;
			node.OwnerDocument = this;
			HandleNodeAdded(node);
		}

		public string CompatMode
		{
			get { return ChildNodes.OfType<DocType>().Any() ? "CSS1Compat" : "BackCompat"; }
		}

		public string Title { get; set; }

		public object ActiveElement { get; set; }
	}

	[DomItem]
	public interface IDocument : INode
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
		//not a part of public API
		event Action<Node> NodeInserted;
		event Action<IDocument> DomContentLoaded;
		string Title { get; set; }
		object ActiveElement { get; set; }
	}
}
