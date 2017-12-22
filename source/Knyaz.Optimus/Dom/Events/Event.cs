using System;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.Dom.Interfaces;
using Knyaz.Optimus.ScriptExecuting;

namespace Knyaz.Optimus.Dom.Events
{
	/// <summary>
	/// Represents the any event occured in the DOM.
	/// </summary>
	[DomItem]
	public class Event
	{
		/// <summary>
		/// The name of the event (case-insensitive).
		/// </summary>
		public string Type { get; private set; }
		
		/// <summary>
		/// A reference to the target to which the event was originally dispatched.
		/// </summary>
		public object Target { get; internal set; }
		
		/// <summary>
		/// A reference to the currently registered target for the event.
		/// </summary>
		public object CurrentTarget { get; internal set; }
		
		/// <summary>
		/// The phase of the event flow is being processed. 
		/// Possible values: <see cref="AT_TARGET"/>, <see cref="BUBBLING_PHASE"/> and <see cref="CAPTURING_PHASE"/>.
		/// </summary>
		public ushort EventPhase { get; internal set; }
		
		/// <summary>
		/// Indicates whether the event bubbles up through the DOM or not.
		/// </summary>
		public bool Bubbles { get; private set; }
		
		/// <summary>
		/// Indicates whether the event is cancelable.
		/// </summary>
		public bool Cancellable { get; private set; }
		
		/// <summary>
		/// The time at which the event was created (in milliseconds, elapsed from the beginning of the current document's lifetime).
		/// </summary>
		//todo: fix the the value
		public DateTime TimeStamp { get; private set; }

		internal bool _stopped;
		internal bool Cancelled { get; private set; }

		public Event()
		{
			TimeStamp = DateTime.Now;
		}

		/// <summary>
		/// Creates the new event with the specified type.
		/// </summary>
		/// <param name="type"></param>
		public Event(string type):this()
		{
			Type = type;
		}

		/// <summary>
		/// Prevents further propagation of the current event in the capturing and bubbling phases.
		/// </summary>
		public void StopPropagation() => _stopped = true;

		internal bool IsPropagationStopped() => _stopped;

		/// <summary>
		/// Cancels the event (if it is cancelable).
		/// </summary>
		public void PreventDefault()
		{
			if (Cancellable)
				Cancelled = true;
		}

		internal bool IsDefaultPrevented() => Cancelled;

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
		/// <summary>
		/// 1
		/// </summary>
		public const ushort CAPTURING_PHASE                = 1;
		/// <summary>
		/// 2
		/// </summary>
		public const ushort AT_TARGET = 2;
		/// <summary>
		/// 3
		/// </summary>
		public const ushort BUBBLING_PHASE = 3;
	}
}