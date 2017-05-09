using System;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.ScriptExecuting;

namespace Knyaz.Optimus.Dom.Events
{
	public class Event : IEvent
	{
		public string Type { get; private set; }
		public IEventTarget Target { get; internal set; }
		public IEventTarget CurrentTarget { get; internal set; }
		public ushort EventPhase { get; internal set; }
		public bool Bubbles { get; private set; }
		public bool Cancellable { get; private set; }
		public DateTime TimeStamp { get; private set; }

		internal bool _stopped;
		internal bool Cancelled { get; private set; }

		public Event()
		{
			TimeStamp = DateTime.Now;
		}

		public Event(string type):this()
		{
			Type = type;
		}

		public void StopPropagation()
		{
			_stopped = true;
		}

		public bool IsPropagationStopped()
		{
			return _stopped;
		}

		public void PreventDefault()
		{
			if (Cancellable)
				Cancelled = true;
		}

		public bool IsDefaultPrevented()
		{
			return Cancelled;
		}

		public void InitEvent(string type, bool canBubble, bool canCancel)
		{
			Type = type;
			Cancellable = canCancel;
			Bubbles = canBubble;
		}

		//todo: implement remains properties
		//todo: do something with const in js
		public const ushort CAPTURING_PHASE                = 1;
		public const ushort AT_TARGET = 2;
		public const ushort BUBBLING_PHASE = 3;
	}

	/// <summary>
	/// http://www.w3.org/TR/DOM-Level-2-Events/events.html#Events-Event
	/// </summary>
	[DomItem]
	public interface IEvent
	{
		void InitEvent(string type, bool canBubble, bool canCancel);
		void StopPropagation();
		void PreventDefault();

		IEventTarget Target { get; }
		IEventTarget CurrentTarget { get; }
		ushort EventPhase { get; }
		bool Bubbles { get; }
		bool Cancellable { get; }
		DateTime TimeStamp { get; }
		string Type { get; }
	}
}