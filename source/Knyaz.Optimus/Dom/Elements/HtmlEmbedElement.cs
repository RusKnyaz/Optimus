using Knyaz.Optimus.ScriptExecuting;

namespace Knyaz.Optimus.Dom.Elements
{
	[JsName("HTMLEmbedElement")]
	public class HtmlEmbedElement : HtmlElement
	{
		private static class Defaults
		{
			public const string Src = "";
			public const string Width = "";
			public const string Height = "";
			public const string Align = "";
		}
		
		internal HtmlEmbedElement(HtmlDocument ownerDocument) : base(ownerDocument, TagsNames.Embed)
		{
		}
		
		/// <summary>
		/// Gets or sets the 'align' attribute value that specifies the alignment of the frame with respect to the surrounding context.
		/// </summary>
		public string Align
		{
			get => GetAttribute("align", Defaults.Align);
			set => SetAttribute("align", value);
		}
		
		/// <summary>
		/// The src attribute gives the address of a page that the iframe is to contain.
		/// </summary>
		public string Src
		{
			get => GetAttribute("src", Defaults.Src);
			set => SetAttribute("src", value);
		}
		
		public string Type
		{
			get => GetAttribute("type", string.Empty);
			set => SetAttribute("type", value);
		}

		public string Width
		{
			get => GetAttribute("width", Defaults.Width);
			set => SetAttribute("width", value);
		}

		public string Height
		{
			get => GetAttribute("height", Defaults.Height);
			set => SetAttribute("height", value);
		}
	}
}