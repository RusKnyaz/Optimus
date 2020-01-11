using Knyaz.Optimus.ScriptExecuting;

namespace Knyaz.Optimus.Dom.Elements
{
	/// <summary>
	/// Represents &lt;optgroup&gt; html element.
	/// </summary>
	[JsName("HTMLOptGroupElement")]
	public class HtmlOptGroupElement : HtmlElement
	{
		internal HtmlOptGroupElement(Document ownerDocument) : base(ownerDocument, TagsNames.OptGroup){}
	}
}