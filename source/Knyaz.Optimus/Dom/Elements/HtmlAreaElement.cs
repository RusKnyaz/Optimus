using Knyaz.Optimus.ScriptExecuting;

namespace Knyaz.Optimus.Dom.Elements
{
	/// <summary> Describes an area inside an image map that has predefined clickable areas. </summary>
	[JsName("HTMLAreaElement")]
	public class HtmlAreaElement : HtmlElement
	{
		internal HtmlAreaElement(HtmlDocument ownerDocument) : base(ownerDocument, TagsNames.Area)
		{
		}
		
		/// <summary> Gets or sets address of the hyperlink </summary>
		public string Href
		{
			get => GetAttribute("href", string.Empty);
			set => SetAttribute("href", value);
		}
		
		/// <summary> Reflects 'alt' attribute value. </summary>
		public string Alt
		{
			get => GetAttribute("alt", string.Empty);
			set => SetAttribute("alt", value);
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
		
		public string Coords
		{
			get => GetAttribute("coords", string.Empty);
			set => SetAttribute("coords", value);
		}
		
		/// <summary> Language of the linked resource </summary>
		public string Hreflang
		{
			get => GetAttribute("hreflang", string.Empty);
			set => SetAttribute("hreflang", value);
		}
	}
}