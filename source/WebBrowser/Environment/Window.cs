using System;
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

		public WindowTimers Timers { get { return _timers; } }

		public Window(Func<object> getSyncObj, Engine engine)
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

			_timers = new WindowTimers(getSyncObj);
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

			_eventTarget = new EventTarget(this, () => null, () => engine.Document);
		}

		public int InnerWidth { get; set; }
		public int InnerHeight { get; set; }

		public Screen Screen { get; private set; }
		public Location Location { get; private set; }
		public Navigator Navigator { get; private set; }

		private WindowTimers _timers;

		public int SetTimeout(Action handler, double? delay)
		{
			return _timers.SetTimeout(handler, (int)(delay ?? 1));
		}

		public void ClearTimeout(int handle)
		{
			_timers.ClearTimeout(handle);
		}

		public int SetInterval(Action handler, double? delay)
		{
			return _timers.SetInterval(handler, (int)(delay ?? 1));
		}

		public void ClearInterval(int handle)
		{
			_timers.ClearTimeout(handle);
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

		public CssStyleDeclaration GetComputedStyle(Element element)
		{
			return GetComputedStyle(element, null);
		}
		
		public CssStyleDeclaration GetComputedStyle(Element element, string pseudoElt)
		{
			//throw new NotImplementedException();
			//todo: implement style computing
			
			return new CssStyleDeclaration();
		}
	}
}
