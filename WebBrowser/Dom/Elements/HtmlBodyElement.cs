using System;

namespace WebBrowser.Dom.Elements
{
	/// <summary>
	/// http://www.w3.org/TR/html-markup/body.html#body
	/// </summary>
	public class HtmlBodyElement : HtmlElement
	{
		public HtmlBodyElement(Document ownerDocument) : base(ownerDocument, "body")
		{
		}

		public Action<Event> OnLoad;


	}
}
