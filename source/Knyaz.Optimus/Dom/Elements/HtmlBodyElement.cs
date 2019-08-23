using System;
using Knyaz.Optimus.Dom.Events;

namespace Knyaz.Optimus.Dom.Elements
{
	/// <summary>
	/// Represents &lt;BODY&gt; element.
	/// http://www.w3.org/TR/html-markup/body.html#body
	/// </summary>
	public class HtmlBodyElement : HtmlElement
	{
		internal HtmlBodyElement(Document ownerDocument) : base(ownerDocument, TagsNames.Body){}

		/// <summary>
		/// Fired immediately after a page has been loaded.
		/// </summary>
		public Action<Event> OnLoad;

		/// <summary>
		/// Additional handling of 'onload' event.
		/// </summary>
		protected override void CallDirectEventSubscribers(Event evt)
		{
			base.CallDirectEventSubscribers(evt);

			if (evt.Type == "load")
				Handle("onload", OnLoad, evt);
		}
	}
}
