using System;
using Knyaz.Optimus.Dom.Events;
using Knyaz.Optimus.ScriptExecuting;

namespace Knyaz.Optimus.Dom.Elements
{
	[DomItem]
	public interface IEventTarget
	{
		void AddEventListener(string type, Action<Event> listener, bool useCapture);
		void RemoveEventListener(string type, Action<Event> listener, bool useCapture);
		bool DispatchEvent(Event evt);
	}
}