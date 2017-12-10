using Knyaz.Optimus.ScriptExecuting;
using System;

namespace Knyaz.Optimus.Dom.Interfaces
{
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
