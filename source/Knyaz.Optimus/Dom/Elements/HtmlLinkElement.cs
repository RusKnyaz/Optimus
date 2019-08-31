using System;
using Knyaz.Optimus.Dom.Events;

namespace Knyaz.Optimus.Dom.Elements
{
	/// <summary>
	/// The HTMLLinkElement interface represents reference information for external resources and the relationship of those resources to a document and vice-versa.
    /// This object inherits all of the properties and methods of the HTMLElement interface.
	/// https://www.w3.org/TR/html5/document-metadata.html#the-link-element
	/// </summary>
	public sealed class HtmlLinkElement : HtmlElement
	{
		private class Defaults
		{
			public static string Href = string.Empty;
			public static string Type = string.Empty;
			public static string Media = string.Empty;
			public static string Hreflang = string.Empty;
			public static string Charset = string.Empty;
			public static string Rel = string.Empty;
			public static string Rev = string.Empty;
			public static string Target = String.Empty;
		}

		internal HtmlLinkElement(Document ownerDocument) : base(ownerDocument, TagsNames.Link){}

		/// <summary>
		/// Gets or sets a list of one or more media formats to which the resource applies.
		/// </summary>
		public string Media
		{
			get { return GetAttribute("media", Defaults.Media); }
			set { SetAttribute("media", value); }
		}

		/// <summary>
		/// Gets or sets the URI for the target resource.
		/// </summary>
		public string Href
		{
			get { return GetAttribute("href", Defaults.Href); }
			set { SetAttribute("href", value); }
		}
		
		/// <summary>
		/// Gets or sets the MIME type of the linked resource
		/// </summary>
		public string Type
		{
			get { return GetAttribute("type", Defaults.Type); }
			set { SetAttribute("type", value); }
		}

		/// <summary>
		/// Gets or sets whether the link is disabled; currently only used with style sheet links.
		/// </summary>
		public bool Disabled
		{
			get { return GetAttribute("disabled") != null; }
			set { SetAttribute("disabled", value ? "" : null); }
		}

		/// <summary>
		/// Gets or sets the language code for the linked resource.
		/// </summary>
		public string Hreflang
		{
			get { return GetAttribute("hreflang", Defaults.Hreflang); }
			set { SetAttribute("hreflang", value); }
		}


		/// <summary>
		/// Gets or sets the character encoding for the target resource.
		/// </summary>
		public string Charset
		{
			get { return GetAttribute("charset", Defaults.Charset); }
			set { SetAttribute("charset", value); }
		}

		/// <summary>
		/// Gets or sets the forward relationship of the linked resource from the document to the resource.
		/// </summary>
		public string Rel
		{
			get { return GetAttribute("rel", Defaults.Rel); }
			set { SetAttribute("rel", value); }
		}

		/// <summary>
		/// Gets or sets the reverse relationship of the linked resource from the resource to the document.
		/// </summary>
		public string Rev
		{
			get { return GetAttribute("rev", Defaults.Rev); }
			set { SetAttribute("rev", value); }
		}


		/// <summary>
		/// Gets or sets the name of the target frame to which the resource applies.
		/// </summary>
		public string Target
		{
			get { return GetAttribute("target", Defaults.Target); }
			set { SetAttribute("target", value); }
		}

		/*
		 //Is a DOMTokenList that reflects the rel HTML attribute, as a list of tokens.
		 HTMLLinkElement.relList Read only

HTMLLinkElement.sizes Read only
Is a DOMSettableTokenList that reflects the sizes HTML attribute, as a list of tokens.
LinkStyle.sheet Read only
Returns the StyleSheet object associated with the given element, or null if there is none.
*/

		public event Action<Event> OnLoad;
		public event Action<Event> OnError;
		
		protected override void CallDirectEventSubscribers(Event evt)
		{
			base.CallDirectEventSubscribers(evt);

			switch (evt.Type)
			{
				case "load":Handle("onload", OnLoad, evt);break;
				case "error":Handle("onerror", OnError, evt);break;
			}
		}
	}
}
