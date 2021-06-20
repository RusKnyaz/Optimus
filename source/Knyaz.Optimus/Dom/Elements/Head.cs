namespace Knyaz.Optimus.Dom.Elements
{
	/// <summary>
	/// Represents &lt;HEAD&gt; element.
	/// </summary>
	public sealed class Head : HtmlElement
	{
		internal Head(HtmlDocument ownerDocument) : base(ownerDocument, TagsNames.Head){}
	}
}
