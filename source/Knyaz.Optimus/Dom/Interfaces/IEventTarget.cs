using Knyaz.Optimus.Dom.Events;
using Knyaz.Optimus.ScriptExecuting;
using System;

namespace Knyaz.Optimus.Dom.Interfaces
{
	[DomItem]
	public interface IEventTarget
	{
		void AddEventListener(string type, Action<Event> listener, EventListenerOptions options);
		void AddEventListener(string type, Action<Event> listener, bool useCapture);
		void RemoveEventListener(string type, Action<Event> listener, EventListenerOptions options);
		void RemoveEventListener(string type, Action<Event> listener, bool useCapture);
		bool DispatchEvent(Event evt);
	}

	
	/// <summary>
	/// Specifies characteristics about the event listener. 
	/// </summary>
	public class EventListenerOptions
	{
		/// <summary>
		/// Indicates that events of this type will be dispatched to the registered listener before being dispatched to any EventTarget beneath it in the DOM tree.
		/// </summary>
		public bool Capture;
		
		/// <summary>
		/// If true, indicates that the function specified by listener will never call preventDefault(). If a passive listener does call preventDefault(), the user agent will do nothing other than generate a console warning.
		/// </summary>
		public bool Passive;
		
		/// <summary>
		/// Indicates that the listener should be invoked at most once after being added. If true, the listener would be automatically removed when invoked.
		/// </summary>
		public bool Once;
	}

}