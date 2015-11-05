using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WebBrowser.Dom.Elements;
using WebBrowser.Dom.Events;
using WebBrowser.Environment;
using WebBrowser.ScriptExecuting;

namespace WebBrowser.Dom
{
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
		internal Document() :this(null)
		{
			ReadyState = DocumentReadyStates.Loading;
		}

		internal Document(Window window):base(null)
		{
			NodeType = DOCUMENT_NODE;

			DocumentElement = new Element(this, "html"){ParentNode = this};
			ChildNodes = new List<Node> { DocumentElement };
			EventTarget = new EventTarget(this, () => window);
			DefaultView = window;
		}

		public Window DefaultView { get; private set; }
		public Element DocumentElement { get; private set; }
		public DocumentReadyStates ReadyState { get; private set; }
		public override string NodeName { get { return "#document"; }}

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

			var invariantTagName = tagName.ToLowerInvariant();
			switch (invariantTagName)
			{
				//todo: fill the list
				case "div":
				case "span":
				case "b": return new HtmlElement(this, tagName);
				case "button": return new HtmlButtonElement(this);
				case "input": return new HtmlInputElement(this);
				case "script": return new Script(this);
				case "head":return new Head(this);
				case "body":return new Body(this);
				case "textarea": return new HtmlTextAreaElement(this);
				case "form":return new HtmlFormElement(this);
			}

			return new HtmlUnknownElement(this, tagName);
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

			if(type == "Error")
				return new ErrorEvent();

			throw new NotSupportedException("Specified event type is not supported: " + type);
		}

		public event Action<Node> DomNodeInserted;
		public event Action<Node> NodeInserted;


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
	}
}
