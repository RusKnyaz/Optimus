using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Knyaz.Optimus.Dom.Css;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.Dom.Events;
using Knyaz.Optimus.Environment;
using Knyaz.Optimus.Dom.Interfaces;
using Knyaz.Optimus.Tools;
using Knyaz.Optimus.ScriptExecuting;

namespace Knyaz.Optimus.Dom
{
	/// <summary>
	/// Available values of the <see cref="Document.ReadyState"/> property.
	/// </summary>
	public static class DocumentReadyStates
	{
		/// <summary>
		/// The document is still loading.
		/// </summary>
		public const string Loading = "loading";
		/// <summary>
		/// The document has finished loading and the document has been parsed but sub-resources such as images, stylesheets and frames are still loading.
		/// </summary>
		public const string Interactive = "interactive";
		/// <summary>
		/// The document and all sub-resources have finished loading. The state indicates that the load event is about to fire.
		/// </summary>
		public const string Complete = "complete";
	}

	/// <summary>
	/// http://www.w3.org/html/wg/drafts/html/master/dom.html#document
	/// http://dev.w3.org/html5/spec-preview/dom.html
	/// all idls http://www.w3.org/TR/REC-DOM-Level-1/idl-definitions.html
	/// </summary>
	public class Document : Element, IDocument
	{
		internal DateTime CreatedOn = DateTime.UtcNow;
		
		private Element _body;

		internal Document() : this(null)
		{
			ReadyState = DocumentReadyStates.Loading;
		}

		/// <summary>
		/// Get or set the cookies associated with the current document. 
		/// </summary>
		public string Cookie
		{
			get
			{
				if (CookieContainer == null)
					return "";

				return CookieParser.ToString(CookieContainer.GetCookies(new Uri(Location.Origin)));
			}
			set
			{
				if (CookieContainer == null)
					return;

				var cookie = CookieParser.FromString(value);
				cookie.Domain = Location.Host;
				
				CookieContainer.Add(cookie);
			}
		}

		/// <summary>
		/// Creates new <sse cref="Document"/> instance.
		/// </summary>
		/// <param name="window">The Window object ot be associated with the document. Can be null.</param>
		public Document(IWindow window) : base(null)
		{
			Implementation = new DomImplementation();

			StyleSheets = new StyleSheetsList();
			NodeType = DOCUMENT_NODE;

			DocumentElement = CreateElement(TagsNames.Html);
			DocumentElement.AppendChild(Head = (Head)CreateElement(TagsNames.Head));
			DocumentElement.AppendChild(Body = (HtmlBodyElement)CreateElement(TagsNames.Body));

			AppendChild(DocumentElement);

			EventTarget = new EventTarget(this, () => window, () => this);
			DefaultView = window;

			ReadyState = DocumentReadyStates.Loading;
		}

		/// <summary>
		/// Is always null.
		/// </summary>
		public override string TextContent
		{
			get => null;
			set { }
		}

		/// <summary>
		/// Returns first DocType element in document.
		/// </summary>
		[JsName("doctype")]
		public DocType DocType => ChildNodes.OfType<DocType>().FirstOrDefault();

		/// <summary>
		/// Return this document's DOMimplementation object.
		/// </summary>
		public DomImplementation Implementation { get; }

		/// <summary>
		/// Returns the window object associated with a document, or null if none is available.
		/// </summary>
		public IWindow DefaultView { get; }

		/// <summary>
		/// Gets the Document Element of the document (the &lt;html&gt; element)
		/// </summary>
		public Element DocumentElement { get; }

		/// <summary>
		/// Gets the (loading) status of the document.
		/// </summary>
		public string ReadyState { get; private set; }

		/// <summary>
		/// This is always #document.
		/// </summary>
		public override string NodeName => "#document";

		/// <summary>
		/// Returns a <see cref="Location"/> object, which contains information about the URL of the document 
		/// and provides methods for changing that URL and loading another URL.
		/// </summary>
		public Location Location => DefaultView.Location;

		/// <summary>
		/// Sets or gets the location of the document
		/// </summary>
		public string DocumentURI { get => DefaultView.Location.Href; set => DefaultView.Location.Href = value;}

		/// <summary>
		/// Returns a collection of all &lt;form&gt; elements in the document.
		/// </summary>
		public IEnumerable<HtmlFormElement> Forms => GetElementsByTagName("form").Cast<HtmlFormElement>();

		/// <summary>
		/// Returns a collection of &lt;script&gt; elements in the document.
		/// </summary>
		public IEnumerable<Script> Scripts => GetElementsByTagName("script").Cast<Script>();

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

		/// <summary>
		/// Fired when the initial HTML document has been completely loaded and parsed, without waiting for stylesheets, images, and subframes to finish loading.
		/// </summary>
		public event Action<IDocument> DomContentLoaded;

		/// <summary>
		/// Returns a StyleSheetList of CSSStyleSheet objects for stylesheets explicitly linked into or embedded in a document.
		/// </summary>
		public StyleSheetsList StyleSheets { get; }

		internal void Complete()
		{
			ReadyState = DocumentReadyStates.Interactive;

			DomContentLoaded?.Invoke(this);

			Trigger("DOMContentLoaded");
			//todo: check is it right
			ReadyState = DocumentReadyStates.Complete;

			//onload event
			var evt = CreateEvent("Event");
			evt.InitEvent("load", false, false);
			lock (this)
			{
				//todo: deadlock possible if event raised from js
				Body.DispatchEvent(evt);
			}
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
				throw new ArgumentNullException(nameof(tagName));
			if(tagName == string.Empty)
				throw new ArgumentOutOfRangeException(nameof(tagName));

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
				case TagsNames.Bold: return new HtmlElement(this, invariantTagName);
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
				case TagsNames.Label: return new HtmlLabelElement(this);
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
		/// Sets or gets the document's body (the &lt;body&gt; element)
		/// </summary>
		public Element Body
		{
			get => _body;
			set
			{
				if (value == null)
					throw new ArgumentNullException(nameof(value));
				
				if(!(value is HtmlBodyElement /*|| value is HtmlFramesetElement*/))
					throw new ArgumentOutOfRangeException("Should be BODY or FRAMESET element");

				DocumentElement.RemoveChild(_body);
				_body = value;
				DocumentElement.AppendChild(_body);
			}
		}

		/// <summary>
		/// Returns the &lt;head&gt; element of the current document. If there are more than one &lt;head&gt; elements, the first one is returned.
		/// </summary>
		public Head Head { get; private set; }

		
		/// <summary>
		/// Creates an event of the type specified.
		/// </summary>
		/// <remarks>
		/// The returned object should be first initialized and can then be passed to <see cref="Node.DispatchEvent"/>.
		/// </remarks>
		/// <param name="type">The string that represents the type of event to be created. 
		/// Possible event types include: "UIEvents", "MouseEvents", 
		/// "MutationEvents", "HTMLEvents", "KeyboardEvents".</param>
		/// <returns>Created <see cref="Event"/> object.</returns>
		public Event CreateEvent(string type)
		{
			if (type == null) throw new ArgumentNullException("type");

			type = type.ToLowerInvariant();
			
			
			switch (type)
			{
				case "event":
				case "events":
					return new Event(this);
				case "customevent":
				case "customevents":
					return new CustomEvent(this);
				case "mutationevent":
				case "mutationevents":
					return new MutationEvent(this);
				case "uievent":
				case "uievents":
					return new UIEvent(this);
				case "keyboardevent":
				case "keyboardevents":
					return new KeyboardEvent(this);
				case "errorevent":
				case "errorevents":
					return new ErrorEvent(this);
			}


			throw new NotSupportedException("Specified event type is not supported: " + type);
		}

		/// <summary>
		/// Faired when new element inserted into Document.
		/// </summary>
		public event Action<Node> DomNodeInserted;
		public event Action<Node> NodeInserted;
		public event Action<Node, Node> NodeRemoved;
		public event Action<Node, Exception> OnNodeException;
		internal event Action<HtmlFormElement, HtmlElement> OnFormSubmit;

		internal void HandleNodeRemoved(Node parent, Node node) => NodeRemoved?.Invoke(parent, node);

		internal void HandleNodeAdded(Node newChild)
		{
			if (!newChild.IsInDocument ())
				return;

			NodeInserted?.Invoke(newChild);
		}

		protected override void CallDirectEventSubscribers(Event obj)
		{
			base.CallDirectEventSubscribers(obj);

			if (obj.Type == "DOMNodeInserted")
			{
				DomNodeInserted?.Invoke((Node)obj.Target);	
			}
		}

		
		internal void HandleNodeEventException(Node node, Exception exception) => 
			OnNodeException?.Invoke(node, exception);

		internal void HandleFormSubmit(HtmlFormElement htmlFormElement, HtmlElement submitElement) => 
			OnFormSubmit?.Invoke(htmlFormElement, submitElement);

		public string CompatMode => ChildNodes.OfType<DocType>().Any() ? "CSS1Compat" : "BackCompat";

		/// <summary>
		/// Sets or gets the title of the document.
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// Gets the currently focused element in the document.
		/// </summary>
		public object ActiveElement { get; set; }

		internal CookieContainer CookieContainer { get; set; }

		/// <summary>
		/// Gets the first element that matches a specified CSS selector(s) in the document.
		/// </summary>
		/// <param name="query">The CSS selector.</param>
		/// <returns>Found element or <c>null</c>.</returns>
		public override IElement QuerySelector(string query) => new CssSelector(query).Select(DocumentElement).FirstOrDefault();

		/// <summary>
		/// Gets a collection containing all elements that matches a specified CSS selector(s) in the document
		/// </summary>
		/// <param name="query">The CSS selector.</param>
		/// <returns>The Readonly collection of found elements.</returns>
		public override IReadOnlyList<IElement> QuerySelectorAll(string query) => new CssSelector(query).Select(DocumentElement).ToList().AsReadOnly();

		/// <summary>
		/// Document nodes call this method when execution of some event handler script required. 
		/// For example when clicked on the node with attribute onclick='callFunc()'.
		/// </summary>
		/// <param name="evt"></param>
		/// <param name="code"></param>
		internal void HandleNodeScript(Event evt, string code) => OnHandleNodeScript?.Invoke(evt, code);

		/// <summary>
		/// Called when execution of some code inside attribute event handler requred.
		/// </summary>
		internal event Action<Event, string> OnHandleNodeScript;
	}
}
