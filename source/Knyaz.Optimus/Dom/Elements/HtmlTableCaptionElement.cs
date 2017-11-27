namespace Knyaz.Optimus.Dom.Elements
{
	/// <summary>
	/// Represents 'caption' tag in table.
	/// </summary>
	public sealed class HtmlTableCaptionElement : HtmlElement
	{
		public HtmlTableCaptionElement(Document ownerDocument) : base(ownerDocument, TagsNames.Caption)
		{
		}

		public string Align
		{
			get { return GetAttribute("align", string.Empty); }
			set { SetAttribute("align", value); }
		}
	}
}