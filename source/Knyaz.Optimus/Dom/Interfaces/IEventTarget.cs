using Knyaz.Optimus.Dom.Events;
using Knyaz.Optimus.ScriptExecuting;
using System;

namespace Knyaz.Optimus.Dom.Interfaces
{
	[DomItem]
	public interface IEventTarget
	{
		void AddEventListener(string type, Action<Event> listener, bool useCapture);
		void RemoveEventListener(string type, Action<Event> listener, bool useCapture);
		bool DispatchEvent(Event evt);
	}
}