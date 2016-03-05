namespace Knyaz.Optimus.Dom.Elements
{
	public sealed class HtmlUnknownElement : HtmlElement
	{
		public HtmlUnknownElement(Document ownerDocument, string tagName) : base(ownerDocument, tagName.ToUpperInvariant())
		{
		}
	}
}
