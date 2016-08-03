namespace Knyaz.Optimus.Dom.Elements
{
	/// <summary>
	/// Represents DOM node for STYLE element. https://www.w3.org/TR/html5/document-metadata.html
	/// </summary>
	public class HtmlStyleElement : HtmlElement
	{
		class Defaults
		{
			public static string Type = string.Empty;
			public static string Media = string.Empty;
		};

		public HtmlStyleElement(Document ownerDocument) : base(ownerDocument, TagsNames.Style)
		{
		}

		/// <summary>
		/// Type of embedded resource. Value must be a valid MIME type that designates a styling language.
		/// </summary>
		public string Type
		{
			get { return GetAttribute("type", Defaults.Type); }
			set { SetAttribute("type", value); }
		}


		/// <summary>
		/// Applicable media
		/// </summary>
		public string Media
		{
			get { return GetAttribute("media", Defaults.Media); }
			set { SetAttribute("media", value); }
		}


		public bool Disabled
		{
			get { return GetAttribute("disabled") != null; }
			set { SetAttribute("disabled", value ? "" : null); }
		}

	}
}
