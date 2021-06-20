using System;

namespace Knyaz.Optimus.Dom.Events
{
	/// <summary>
	/// Represents events initialized by an application for any purpose.
	/// </summary>
	public class CustomEvent : Event
	{
		internal CustomEvent(HtmlDocument owner) : base(owner){}
		
		/// <summary>
		/// Any data passed when initializing the event.
		/// </summary>
		public Object Detail { get; private set; }

		/// <summary>
		/// Initializes a CustomEvent object. If the event has already being dispatched, this method does nothing.
		/// </summary>
		/// <param name="type">The name of the event.</param>
		/// <param name="canBubble">Specifies whether the event bubbles up through the DOM or not.</param>
		/// <param name="canCancel">Specifies whether the event is cancelable.</param>
		/// <param name="detail">The data to be accessible by <see cref="Detail"/> property.</param>
		public void InitCustomEvent(string type, bool canBubble, bool canCancel, object detail)
		{
			InitEvent(type, canBubble, canBubble);
			Detail = detail;
		}
	}
}
