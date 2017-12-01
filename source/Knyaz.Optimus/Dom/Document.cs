using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Knyaz.Optimus.Dom.Css;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.Dom.Events;
using Knyaz.Optimus.Environment;
using Knyaz.Optimus.Tests.Dom;
using Knyaz.Optimus.Dom.Interfaces;
using Knyaz.Optimus.Tools;

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
	public class Document : DocumentFragment, IDocument, IElementSelector
	{
		internal Document() : this(null)
		{
			ReadyState = DocumentReadyStates.Loading;
		}

		public Document(IWindow window) : base(null)
		{
			StyleSheets = new StyleSheetsList();
			NodeType = DOCUMENT_NODE;

			DocumentElement = CreateElement(TagsNames.Html);
			DocumentElement.AppendChild(Head = (Head)CreateElement(TagsNames.Head));
			DocumentElement.AppendChild(Body = (HtmlBodyElement)CreateElement(TagsNames.Body));
			ChildNodes.Add(DocumentElement);
			DocumentElement.ParentNode = this;
			DocumentElement.SetOwner(this);

			EventTarget = new EventTarget(this, () => window, () => this);
			DefaultView = window;

			ReadyState = DocumentReadyStates.Loading;
		}

		public IWindow DefaultView { get; private set; }

		/// <summary>
		/// Gets the Document Element of the document (the &lt;html&gt; element)
		/// </summary>
		public Element DocumentElement { get; private set; }

		/// <summary>
		/// Gets the (loading) status of the document.
		/// </summary>
		public string ReadyState { get; private set; }

		/// <summary>
		/// This is always #document.
		/// </summary>
		public override string NodeName { get { return "#document"; } }

		[Obsolete("Use window.location")]
		//todo: check is it true for frames
		public ILocation Location => DefaultView.Location;

		/// <summary>
		/// Sets or gets the location of the document
		/// </summary>
		public string DocumentURI { get { return DefaultView.Location.Href; } set { DefaultView.Location.Href = value; } }

		/// <summary>
		/// Returns a collection of all <form> elements in the document.
		/// </summary>
		public IEnumerable<HtmlFormElement> Forms { get { return GetElementsByTagName("form").Cast<HtmlFormElement>(); } }

		/// <summary>
		/// Returns a collection of <script> elements in the document.
		/// </summary>
		public IEnumerable<IHtmlScriptElement> Scripts { get { return GetElementsByTagName("script").Cast<IHtmlScriptElement>(); } }

		/// <summary>
		/// Writes HTML expressions or JavaScript code to a document.
		/// </summary>
		/// <param name="text"></param>
		public void Write(string text)
		{
			using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
			{
				DocumentBuilder.Build(this, stream);
			}
		}

		/// <summary>
		/// Same as write(), but adds a newline character after each statement
		/// </summary>
		public void WriteLn(string text)
		{
			throw new NotImplementedException("Please use write insted.");
		}

		public event Action<IDocument> DomContentLoaded;

		public StyleSheetsList StyleSheets { get; private set; }

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

		/// <summary>
		/// Creates an Element node.
		/// </summary>
		/// <param name="tagName">The tag name of element to be created.</param>
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
				case TagsNames.Br: return new HtmlBrElement(this);
				case TagsNames.TFoot:
				case TagsNames.THead:
				case TagsNames.TBody:
					return new HtmlTableSectionElement(this, invariantTagName);
				case TagsNames.Td:
				case TagsNames.Th:
					return new HtmlTableCellElement(this, invariantTagName);
				case TagsNames.Caption: return new HtmlTableCaptionElement(this);
				case TagsNames.Table: return new HtmlTableElement(this);
				case TagsNames.Tr: return new HtmlTableRowElement(this);
				case TagsNames.Link: return new HtmlLinkElement(this);
				case TagsNames.Style: return new HtmlStyleElement(this);
				case TagsNames.Select: return new HtmlSelectElement(this);
				case TagsNames.Option: return new HtmlOptionElement(this);
				case TagsNames.Div: return new HtmlDivElement(this);
				case TagsNames.Span:
				case TagsNames.Nav:
				case TagsNames.B: return new HtmlElement(this, invariantTagName);
				case TagsNames.Button: return new HtmlButtonElement(this);
				case TagsNames.Input: return new HtmlInputElement(this);
				case TagsNames.Script: return new Script(this);
				case TagsNames.Head:return new Head(this);
				case TagsNames.Body:return new HtmlBodyElement(this);
				case TagsNames.Textarea: return new HtmlTextAreaElement(this);
				case TagsNames.Form:return new HtmlFormElement(this);
				case TagsNames.IFrame:return new HtmlIFrameElement(this);
				case TagsNames.Html:return new HtmlHtmlElement(this);
				case TagsNames.Col: return new HtmlTableColElement(this);
			}

			return new HtmlUnknownElement(this, invariantTagName);
		}

		/// <summary>
		/// Creates an attribute with the specified name, and returns the attribute as an <see cref="Attr"/> object.
		/// </summary>
		/// <param name="name">The name of attribute.</param>
		/// <returns>Created attribute.</returns>
		public Attr CreateAttribute(string name)
		{
			return new Attr(name, this);
		}

		/// <summary>
		/// Returns the first element of the document that has the ID attribute with the specified value.
		/// </summary>
		public Element GetElementById(string id)
		{
			return DocumentElement.Flatten().OfType<Element>().FirstOrDefault(x => x.Id == id);
		}

		/// <summary>
		/// Returns a collection containing all elements of the document with a specified name.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public IReadOnlyCollection<Element> GetElementsByName(string name)
		{
			return DocumentElement.Flatten().OfType<Element>().Where(x => x.GetAttribute("name") == name).ToList().AsReadOnly();
		}

		/// <summary>
		/// Creates an empty DocumentFragment node.
		/// </summary>
		public DocumentFragment CreateDocumentFragment()
		{
			return new DocumentFragment(this);
		}

		/// <summary>
		/// Creates a Text node.
		/// </summary>
		public Text CreateTextNode(string data)
		{
			return new Text(this) {Data = data};	
		}

		/// <summary>
		/// Creates a Comment node with the specified text.
		/// </summary>
		public Comment CreateComment(string data)
		{
			return new Comment(this) { Data = data };
		}

		/// <summary>
		/// Sets or gets the document's body (the <body> element)
		/// </summary>
		public HtmlBodyElement Body { get; private set; }

		public Head Head { get; private set; }

		
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

			if(type == "ErrorEvent")
				return new ErrorEvent();

			throw new NotSupportedException("Specified event type is not supported: " + type);
		}

		public event Action<Node> DomNodeInserted;
		public event Action<Node> NodeInserted;
		public event Action<Node, Node> NodeRemoved;
		public event Action<Node, Exception> OnNodeException;
		internal event Action<HtmlFormElement> OnFormSubmit;

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

		internal void HandleFormSubmit(HtmlFormElement htmlFormElement)
		{
			if (OnFormSubmit != null)
				OnFormSubmit(htmlFormElement);
		}

		protected override void RegisterNode(Node node)
		{
			node.ParentNode = this;
			node.SetOwner(this);
			HandleNodeAdded(node);
		}

		public string CompatMode
		{
			get { return ChildNodes.OfType<DocType>().Any() ? "CSS1Compat" : "BackCompat"; }
		}

		/// <summary>
		/// Sets or gets the title of the document.
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// Gets the currently focused element in the document.
		/// </summary>
		public object ActiveElement { get; set; }

		/// <summary>
		/// Gets the first element that matches a specified CSS selector(s) in the document.
		/// </summary>
		/// <param name="query">The CSS selector.</param>
		/// <returns>Found element or <c>null</c>.</returns>
		override public IElement QuerySelector(string query) => new CssSelector(query).Select(DocumentElement).FirstOrDefault();

		/// <summary>
		/// Gets a collection containing all elements that matches a specified CSS selector(s) in the document
		/// </summary>
		/// <param name="query">The CSS selector.</param>
		/// <returns>The Readonly collection of found elements.</returns>
		override public IReadOnlyList<IElement> QuerySelectorAll(string query) => new CssSelector(query).Select(DocumentElement).ToList().AsReadOnly();
	}
}
