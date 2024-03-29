﻿using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
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
	/// http://www.w3.org/html/wg/drafts/html/master/dom.html#document
	/// http://dev.w3.org/html5/spec-preview/dom.html
	/// all idls http://www.w3.org/TR/REC-DOM-Level-1/idl-definitions.html
	/// </summary>
	[JsName("Document")]
	public class Document : Element
	{
		//used for event's timestamp.
		internal DateTime CreatedOn = DateTime.UtcNow;

		//Used in HtmlImageElement for image loading.
		//todo: remove it from document or initialize from ctor.
		internal Func<string, Task<IImage>> GetImage;

		internal Func<Element, RectangleF[]> GetElementBounds;
		
		private HtmlBodyElement _body;

		internal Document(IWindow window = null) : this("http://www.w3.org/1999/xhtml","html", null, window)
		{
			ReadyState = DocumentReadyStates.Loading;
		}

		/// <summary> Get or set the cookies associated with the current document. </summary>
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
		
		/// <summary> Creates new <sse cref="HtmlDocument"/> instance. </summary>
		internal Document(string namespaceUri, string qualifiedNameStr, DocType docType, IWindow window) : base(null)
		{
			Implementation = new DomImplementation();

			StyleSheets = new StyleSheetsList();
			NodeType = DOCUMENT_NODE;
			
			if(docType != null)
				AppendChild(docType);
			
			var root = CreateElementNs(namespaceUri, qualifiedNameStr);
			AppendChild(root);
			
			Initialize();
			
			Scripts = new HtmlCollection(() => GetElementsByTagName(TagsNames.Script));
			Forms = new HtmlCollection(() => GetElementsByTagName(TagsNames.Form));
			Images = new HtmlCollection(() => GetElementsByTagName(TagsNames.Img));
			Links = new HtmlCollection(() => 
				GetElementsByTagName(TagsNames.A).Where(x => !string.IsNullOrEmpty(((HtmlAnchorElement)x).Href))
					.Concat(GetElementsByTagName(TagsNames.Textarea).Where(x => !string.IsNullOrEmpty(((HtmlAreaElement)x).Href))));
			Embeds = new HtmlCollection(() => GetElementsByTagName(TagsNames.Embed));

			DefaultView = window;

			ReadyState = DocumentReadyStates.Loading;
		}

		private protected virtual void Initialize()
		{
			
		}
		

		public override Node AppendChild(Node node)
		{
			if (ChildNodes.Count == 0)
			{
				if (node is DocType docType)
				{
					DocType = docType;
					return base.AppendChild(docType);
				}
				else
				{
					DocumentElement = (Element) node;
					return base.AppendChild(node);
				}
			}
			else if (ChildNodes.Count == 1)
			{
				if (DocType == null)
				{
					throw new DOMException(DOMException.Codes.HierarchyRequestError,
						"Only one child node allowed for the document.");
				}
				else 
				{
					DocumentElement = (Element) node;
					return base.AppendChild(node);
				}
			}
			else
			{
				throw new DOMException(DOMException.Codes.HierarchyRequestError,
					"Only one child node allowed for the document.");
			}
		}

		public override Node RemoveChild(Node node)
		{
			if (node == DocType)
				DocType = null;

			if (node == DocumentElement)
				DocumentElement = null;
			
			return base.RemoveChild(node);
		}

		/// <summary>
		/// Is always null.
		/// </summary>
		public override string TextContent
		{
			get => null;
			set { }
		}

		/// <summary> Returns the character encoding of the document that it's currently rendered with. </summary>
		public string CharacterSet => "UTF-8";

		/// <summary> Returns the MIME type that the document is being rendered as. </summary>
		public string ContentType { get; internal set; } = "text/html";

		/// <summary>
		/// The directionality of the text of the document. Possible values are 'rtl', right to left, and 'ltr', left to right.
		/// </summary>
		public string Dir { get; set; } = string.Empty;
		
		public HtmlCollection Embeds { get; }

		/// <summary>
		/// Returns first DocType element in document.
		/// </summary>
		[JsName("doctype")]
		public DocType DocType { get; set; }

		/// <summary>
		/// Return this document's DOM implementation object.
		/// </summary>
		public DomImplementation Implementation { get; }

		/// <summary>
		/// Returns the window object associated with a document, or null if none is available.
		/// </summary>
		public IWindow DefaultView { get; }

		/// <summary> Gets the Document Element of the document (the &lt;html&gt; element) </summary>
		public Element DocumentElement { get; private set; }

		/// <summary> Gets the (loading) status of the document. </summary>
		public string ReadyState { get; private set; }

		/// <summary> This is always #document. </summary>
		public override string NodeName => "#document";

		/// <summary>
		/// Returns a <see cref="Location"/> object, which contains information about the URL of the document 
		/// and provides methods for changing that URL and loading another URL.
		/// </summary>
		public Location Location => DefaultView.Location;

		/// <summary> Sets or gets the location of the document </summary>
		public string DocumentURI { get => DefaultView.Location.Href; set => DefaultView.Location.Href = value;}

		/// <summary> Returns a collection of all &lt;form&gt; elements in the document. </summary>
		public HtmlCollection Forms { get; }
		
		/// <summary> Returns collection of the images in the current HTML document. </summary>
		public HtmlCollection Images { get; }
		
		/// <summary>
		/// Returns a collection of all &lt;area&gt; elements and &lt;a&gt; elements in a document with a value for the href attribute.
		/// </summary>
		public HtmlCollection Links { get; }

		/// <summary> Returns a collection of &lt;script&gt; elements in the document. </summary>
		public HtmlCollection Scripts { get; }
		
		/// <summary> Run-time property. Reflects currently executing script element. </summary>
		public HtmlScriptElement CurrentScript { get; internal set; }

		/// <summary> Writes HTML expressions or JavaScript code to a document. </summary>
		/// <param name="text"></param>
		public void Write(string text)
		{
			using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
			{
				DocumentBuilder.Build(this, stream, NodeSources.Script);
			}
		}

		/// <summary> Same as write(), but adds a newline character after each statement </summary>
		public void WriteLn(string text) => Write(text + System.Environment.NewLine);

		/// <summary>
		/// Fired when the initial HTML document has been completely loaded and parsed, without waiting for stylesheets, images, and subframes to finish loading.
		/// </summary>
		internal event Action<Document> DomContentLoaded;

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

		public Element CreateElementNs(string namespaceUri, string qualifiedName)
		{
			var elt = CreateElement(qualifiedName);
			elt.NamespaceUri = namespaceUri;
			return elt;
		}
		

		/// <summary> Creates an Element node. </summary>
		/// <param name="tagName">The tag name of element to be created.</param>
		public virtual Element CreateElement(string tagName)
		{
			if(tagName == null)
				throw new ArgumentNullException(nameof(tagName));
			if(tagName == string.Empty)
				throw new ArgumentOutOfRangeException(nameof(tagName));

			return new Element(this, tagName);
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

		/// <summary> Returns the first element of the document that has the ID attribute with the specified value. </summary>
		public Element GetElementById(string id)
		{
			return DocumentElement.Flatten().OfType<Element>().FirstOrDefault(x => x.Id == id);
		}

		/// <summary> Returns a collection containing all elements of the document with a specified name. </summary>
		/// <param name="name">Value of the name attribute of the element(s) to be found.</param>
		/// <returns>Live <see cref="NodeList"/> Collection which it automatically updated as the document changed.</returns>
		public NodeList GetElementsByName(string name) => 
			new NodeList(() => DocumentElement.Flatten().OfType<Element>().Where(x => x.GetAttribute("name") == name));

		/// <summary> Creates an empty DocumentFragment node. </summary>
		public DocumentFragment CreateDocumentFragment()
		{
			return new DocumentFragment(this);
		}

		/// <summary> Creates a Text node. </summary>
		public Text CreateTextNode(string data) => new Text(this) {Data = data};

		/// <summary> Creates a Comment node with the specified text. </summary>
		public Comment CreateComment(string data) => new Comment(this) { Data = data };

		/// <summary>
		/// Sets or gets the document's body (the &lt;body&gt; element)
		/// </summary>
		public HtmlBodyElement Body
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
		public Head Head { get; protected set; }

		
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
					return new UiEvent(this);
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

		
		private HtmlTitleElement GetTitleElement() => (HtmlTitleElement)GetElementsByTagName(TagsNames.Title).FirstOrDefault();
		
		/// <summary>
		/// Sets or gets the title of the document.
		/// </summary>
		public string Title
		{
			get => GetTitleElement()?.TextContent ?? string.Empty;
			set
			{
				var elt = GetTitleElement();
				if (elt == null && !string.IsNullOrEmpty(value))
				{
					elt = (HtmlTitleElement)CreateElement("title");
					Head.AppendChild(elt);
				}

				if(elt != null)
					elt.TextContent = value;
			}
		}

		/// <summary> Gets the currently focused element in the document. </summary>
		public Element ActiveElement { get; set; }

		internal CookieContainer CookieContainer { get; set; }

		/// <summary>
		/// Gets the first element that matches a specified CSS selector(s) in the document.
		/// </summary>
		/// <param name="query">The CSS selector.</param>
		/// <returns>Found element or <c>null</c>.</returns>
		public override Element QuerySelector(string query) => new CssSelector(query).Select(DocumentElement).FirstOrDefault();

		/// <summary> Gets a collection containing all elements that matches a specified CSS selector(s) in the document. </summary>
		/// <param name="query">The CSS selector.</param>
		/// <returns>The Readonly collection of found elements.</returns>
		public override NodeList QuerySelectorAll(string query)
		{
			var result = new CssSelector(query).Select(DocumentElement).OfType<HtmlElement>().ToList();
			return new NodeList(() => result);
		}

		/// <summary>
		/// Document nodes call this method when execution of some event handler script required. 
		/// For example when clicked on the node with attribute onclick='callFunc()'.
		/// </summary>
		/// <param name="evt"></param>
		/// <param name="code"></param>
		internal void HandleNodeScript(Event evt, string code)
		{
			OnHandleNodeScript?.Invoke(evt, code);
		}

		/// <summary> Called when execution of some code inside attribute event handler required. </summary>
		internal event Action<Event, string> OnHandleNodeScript;
	}
	
	/// <summary> Available values of the <see cref="HtmlDocument.ReadyState"/> property. </summary>
	public static class DocumentReadyStates
	{
		/// <summary> The document is still loading. </summary>
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
}
