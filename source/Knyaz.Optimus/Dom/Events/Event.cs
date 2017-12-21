using System;
using Knyaz.Optimus.Dom.Interfaces;

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

		public void StopPropagation() => _stopped = true;

		public bool IsPropagationStopped() => _stopped;

		public void PreventDefault()
		{
			if (Cancellable)
				Cancelled = true;
		}

		public bool IsDefaultPrevented() => Cancelled;

		/// <summary>
		/// Initializes the value of an Event created. If the event has already being dispatched, this method does nothing.
		/// </summary>
		/// <param name="type">Defines the type of event.</param>
		/// <param name="canBubble">Specifies whether the event should bubble up through the event chain or not.</param>
		/// <param name="canCancel">Specifies whether the event can be canceled.</param>
		public void InitEvent(string type, bool canBubble, bool canCancel)
		{
			Type = type;
			Cancellable = canCancel;
			Bubbles = canBubble;
		}

		//todo: implement remains properties
		//todo: do something with const in js
		public const ushort NOT_STARTED = 0;
		public const ushort CAPTURING_PHASE                = 1;
		public const ushort AT_TARGET = 2;
		public const ushort BUBBLING_PHASE = 3;
	}
}