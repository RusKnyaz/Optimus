using Knyaz.Optimus.ScriptExecuting;

namespace Knyaz.Optimus.Dom.Elements
{
	/// <summary>
	/// Represents DOM node for STYLE element. https://www.w3.org/TR/html5/document-metadata.html
	/// </summary>
	[JsName("HTMLStyleElement")]
	public sealed class HtmlStyleElement : HtmlElement
	{
		class Defaults
		{
			public static string Type = string.Empty;
			public static string Media = string.Empty;
		};

		internal HtmlStyleElement(Document ownerDocument) : base(ownerDocument, TagsNames.Style)
		{
		}

		/// <summary>
		/// Type of embedded resource. Value must be a valid MIME type that designates a styling language.
		/// </summary>
		public string Type
		{
			get => GetAttribute("type", Defaults.Type);
			set => SetAttribute("type", value);
		}


		/// <summary>
		/// Applicable media
		/// </summary>
		public string Media
		{
			get => GetAttribute("media", Defaults.Media);
			set => SetAttribute("media", value);
		}

		/// <summary>
		/// Gets or sets the 'disabled' attribute value.
		/// </summary>
		public bool Disabled
		{
			get => GetAttribute("disabled") != null;
			set => SetAttribute("disabled", value ? "" : null);
		}

	}
}
