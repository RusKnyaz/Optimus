using Knyaz.Optimus.ScriptExecuting;

namespace Knyaz.Optimus.Dom.Elements
{
	/// <summary>
	/// Represents custom tag in html document.
	/// </summary>
	[JsName("HTMLUnknownElement")]
	public sealed class HtmlUnknownElement : HtmlElement
	{
		internal HtmlUnknownElement(HtmlDocument ownerDocument, string tagName) 
			: base(ownerDocument, tagName.ToUpperInvariant()){}
	}
}
