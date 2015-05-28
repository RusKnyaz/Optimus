using System;
using WebBrowser.ScriptExecuting;

namespace WebBrowser.Dom.Elements
{
	public class Event : IEvent
	{
		public string Type;
		public Node Target;
		public ushort EventPhase;
		public bool Bubbles;
		public bool Cancellable;
		public void StopPropagation()
		{
			//todo: implement
			//throw new NotImplementedException();
		}

		public void PreventDefault()
		{
			//todo: implement
			//throw new NotImplementedException();
		}

		public void InitEvent(string type, bool canBubble, bool canCancel)
		{
			Type = type;
			Cancellable = canCancel;
			Bubbles = canBubble;
		}

		//todo: implement remains properties
	}

	/// <summary>
	/// http://www.w3.org/TR/DOM-Level-2-Events/events.html#Events-Event
	/// </summary>
	[DomItem]
	public interface IEvent
	{
		void InitEvent(string type, bool canBubble, bool canCancel);
	}
}