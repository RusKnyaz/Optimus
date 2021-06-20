using Knyaz.Optimus.ScriptExecuting;

namespace Knyaz.Optimus.Dom.Elements
{
	/// <summary>
	/// Represents &lt;DIV&gt; HTML element.
	/// </summary>
	[JsName("HTMLDivElement")]
	public sealed class HtmlDivElement : HtmlElement
	{
		internal HtmlDivElement(HtmlDocument ownerDocument) : base(ownerDocument, TagsNames.Div) { }
	}
}
