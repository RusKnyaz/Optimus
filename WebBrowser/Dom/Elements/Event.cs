using System;

namespace WebBrowser.Dom.Elements
{
	public class Event
	{
		public string Type;
		public Node Target;
		public ushort EventPhase;
		public bool Bubbles;
		public bool Cancellable;
		public void StopPropagation()
		{
			throw new NotImplementedException();
		}

		public void PreventDefault()
		{
			throw new NotImplementedException();
		}

		public void InitEvent(string type, bool canBubble, bool canCancel)
		{
			Type = type;
			Cancellable = canCancel;
			Bubbles = canBubble;
		}

		//todo: implement remains properties
	}
}