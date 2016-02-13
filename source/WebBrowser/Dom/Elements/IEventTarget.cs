using System;
using WebBrowser.ScriptExecuting;

namespace WebBrowser.Dom.Elements
{
	[DomItem]
	public interface IEventTarget
	{
		void AddEventListener(string type, Action<Event> listener, bool useCapture);
		void RemoveEventListener(string type, Action<Event> listener, bool useCapture);
		bool DispatchEvent(Event evt);
	}
}