using System;
using System.Threading;
using Jint.Runtime;
using WebBrowser.Dom.Elements;

namespace WebBrowser.Environment
{
	/// <summary>
	/// http://www.w3.org/TR/html5/browsers.html#window
	/// </summary>
	public class Window : IEventTarget
	{
		private EventTarget _eventTarget;

		public Window(SynchronizationContext context, Engine engine)
		{
			Screen = new Screen
				{
					Width = 1024,
					Height = 768,
					AvailWidth = 1024,
					AvailHeight = 768,
					ColorDepth = 24,
					PixelDepth = 24
				};

			InnerWidth = 1024;
			InnerHeight = 768;
			Location = new Location(engine);//todo: remove the stub href value
			Navigator = new Navigator();

			_timers = new WindowTimers(context);
			_timers.OnException += exception =>
				{
					var jsEx = exception as JavaScriptException;
					if (jsEx != null)
					{
						engine.Console.Log("Unhandled exception in timer handler function: " + jsEx.Error.ToString());
					}
					else
					{
						engine.Console.Log("Unhandled exception in timer handler function: " + exception.Message);
					}
				};

			_eventTarget = new EventTarget(this, () => null);
		}

		public int InnerWidth { get; set; }
		public int InnerHeight { get; set; }

		public Screen Screen { get; private set; }
		public Location Location { get; private set; }
		public Navigator Navigator { get; private set; }

		private WindowTimers _timers;

		public int SetTimeout(Action handler, int delay)
		{
			return _timers.SetTimeout(handler, delay);
		}

		public void ClearTimeout(int handle)
		{
			_timers.ClearTimeout(handle);
		}

		public int SetInterval(Action handler, int delay)
		{
			throw new NotImplementedException();
		}

		public void ClearInterval(int handle)
		{
			throw new NotImplementedException();
		}

		public void AddEventListener(string type, Action<Event> listener, bool useCapture)
		{
			_eventTarget.AddEventListener(type, listener, useCapture);
		}

		public void RemoveEventListener(string type, Action<Event> listener, bool useCapture)
		{
			_eventTarget.RemoveEventListener(type, listener, useCapture);
		}

		public bool DispatchEvent(Event evt)
		{
			return _eventTarget.DispatchEvent(evt);
		}

		public void Alert(string message)
		{
			if (OnAlert != null)
				OnAlert(message);
		}

		public event Action<string> OnAlert;
	}
}
