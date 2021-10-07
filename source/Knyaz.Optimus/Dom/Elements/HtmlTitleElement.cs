using Knyaz.Optimus.ScriptExecuting;

namespace Knyaz.Optimus.Dom.Elements
{
	[JsName("HTMLTitleElement")]
	public class HtmlTitleElement : HtmlElement
	{
		internal HtmlTitleElement(HtmlDocument ownerDocument) : base(ownerDocument, TagsNames.Title)
		{
		}
	}
}