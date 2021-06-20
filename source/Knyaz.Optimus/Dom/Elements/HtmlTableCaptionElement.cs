using Knyaz.Optimus.ScriptExecuting;

namespace Knyaz.Optimus.Dom.Elements
{
	/// <summary>
	/// Represents 'caption' tag in table.
	/// </summary>
	[JsName("HTMLTableCaptionElement")]
	public sealed class HtmlTableCaptionElement : HtmlElement
	{
		internal HtmlTableCaptionElement(HtmlDocument ownerDocument) : base(ownerDocument, TagsNames.Caption)
		{
		}

		/// <summary>
		/// Gets or sets 'align' attribute value.
		/// </summary>
		public string Align
		{
			get => GetAttribute("align", string.Empty);
			set => SetAttribute("align", value);
		}
	}
}