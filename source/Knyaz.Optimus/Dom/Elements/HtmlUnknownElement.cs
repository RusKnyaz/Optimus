namespace Knyaz.Optimus.Dom.Elements
{
	/// <summary>
	/// Represents custom tag in html document.
	/// </summary>
	public sealed class HtmlUnknownElement : HtmlElement
	{
		internal HtmlUnknownElement(Document ownerDocument, string tagName) 
			: base(ownerDocument, tagName.ToUpperInvariant()){}
	}
}
