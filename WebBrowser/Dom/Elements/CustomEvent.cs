using System;
using WebBrowser.ScriptExecuting;

namespace WebBrowser.Dom.Elements
{
	public class CustomEvent : Event, ICustomEvent
	{
		public Object Detail { get; private set; }

		public void InitEvent(string type, bool canBubble, bool canCancel, object detail)
		{
			InitEvent(type, canBubble, canBubble);
			Detail = detail;
		}
	}

	[DomItem]
	public interface ICustomEvent
	{
		void InitEvent(string type, bool canBubble, bool canCancel, object detail);
	}
}
