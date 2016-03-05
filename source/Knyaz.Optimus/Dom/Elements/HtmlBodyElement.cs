using System;
using Knyaz.Optimus.Dom.Events;

namespace Knyaz.Optimus.Dom.Elements
{
	/// <summary>
	/// http://www.w3.org/TR/html-markup/body.html#body
	/// </summary>
	public class HtmlBodyElement : HtmlElement
	{
		public HtmlBodyElement(Document ownerDocument) : base(ownerDocument, TagsNames.Body)
		{
		}

		public Action<Event> OnLoad;
	}
}
