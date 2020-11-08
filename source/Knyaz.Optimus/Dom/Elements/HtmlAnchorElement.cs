using Knyaz.Optimus.Dom.Events;
using Knyaz.Optimus.ScriptExecuting;

namespace Knyaz.Optimus.Dom.Elements
{
	/// <summary>
	/// Anchor element implementation (tag: a).
	/// <seealso href="https://www.w3.org/TR/2012/WD-html-markup-20121025/a.html#a"/>
	/// <seealso href="https://html.spec.whatwg.org/multipage/text-level-semantics.html#the-a-element"/>
	/// </summary>
	[JsName("HTMLAnchorElement")]
	public class HtmlAnchorElement : HtmlElement
	{
		private readonly TokenList _relList;
		
		internal HtmlAnchorElement(Document ownerDocument) : base(ownerDocument, TagsNames.A)
		{
			_relList = new TokenList(() => Rel);
			_relList.Changed += () => {
				Rel = string.Join(" ", _relList);
			};
		}
		
		protected override void CallDirectEventSubscribers(Event evt)
		{
			base.CallDirectEventSubscribers(evt);

			if (evt.Type == "click" && !evt.IsDefaultPrevented() && evt.Bubbles)
			{
				var href = Href.Trim();
				if (!href.StartsWith("JavaScript"))
					return;

				var idx = href.IndexOf(':');
				if (idx < 0 || idx == href.Length-1)
					return;

				var code = href.Substring(idx + 1);
				
				
				//todo: call async
				OwnerDocument.HandleNodeScript(evt, code, true);
			}
		}
		
		/// <summary>
		/// Gets or sets address of the hyperlink
		/// </summary>
		public string Href
		{
			get => GetAttribute("href", string.Empty);
			set => SetAttribute("href", value);
		}

		/// <summary>
		/// Browsing context for hyperlink navigation
		/// </summary>
		public string Target
		{
			get => GetAttribute("target", string.Empty);
			set => SetAttribute("target", value);
		}
		
		/// <summary>
		/// Whether to download the resource instead of navigating to it, and its file name if so
		/// </summary>
		public string Download
		{
			get => GetAttribute("download", string.Empty);
			set => SetAttribute("download", value);
		}
		
		/// <summary>
		/// URLs to ping
		/// </summary>
		public string Ping
		{
			get => GetAttribute("ping", string.Empty);
			set => SetAttribute("ping", value);
		}
		
		/// <summary>
		/// Relationship between the location in the document containing the hyperlink and the destination resource
		/// <seealso href="https://wiki.whatwg.org/wiki/RelExtensions"/>
		/// </summary>
		public string Rel
		{
			get => GetAttribute("rel", string.Empty);
			set => SetAttribute("rel", value);
		}
		
		/// <summary>
		/// Language of the linked resource
		/// </summary>
		public string Hreflang
		{
			get => GetAttribute("hreflang", string.Empty);
			set => SetAttribute("hreflang", value);
		}
		
		/// <summary>
		///Hint for the type of the referenced resource 
		/// </summary>
		public string Type
		{
			get => GetAttribute("type", string.Empty);
			set => SetAttribute("type", value);
		}

		public string Text
		{
			get => TextContent;
			set => TextContent = value;
		}

		public ITokenList RelList => _relList;
		
		/*
		attribute DOMString media;


		// URL decomposition IDL attributes
		attribute DOMString protocol;
		attribute DOMString host;
		attribute DOMString hostname;
		attribute DOMString port;
		attribute DOMString pathname;
		attribute DOMString search;
		attribute DOMString hash;*/
	}
}
