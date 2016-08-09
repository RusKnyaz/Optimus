using System;
using Jint.Runtime;
using Knyaz.Optimus.Dom.Css;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.Dom.Events;

namespace Knyaz.Optimus.Environment
{
	/// <summary>
	/// http://www.w3.org/TR/html5/browsers.html#window
	/// </summary>
	public class Window : IWindow
	{
		private readonly Engine _engine;
		private EventTarget _eventTarget;

		public WindowTimers Timers { get { return _timers; } }

		public Window(Func<object> getSyncObj, Engine engine)
		{
			_engine = engine;
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
			History = new History(engine);

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
						engine.Console.Log("Unhandled exception in timer handler function: " + exception.ToString());
					}
				};

			_eventTarget = new EventTarget(this, () => null, () => engine.Document);
		}

		public int InnerWidth { get; set; }
		public int InnerHeight { get; set; }

		public Screen Screen { get; private set; }
		public Location Location { get; private set; }
		public Navigator Navigator { get; private set; }
		public History History { get; private set; }

		private readonly WindowTimers _timers;

		public int SetTimeout(Action<object> handler, double? delay, object ctx)
		{
			return _timers.SetTimeout(handler, (int)(delay ?? 1), ctx);
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
			var styling = _engine.Styling;
			if (styling != null)
				return styling.GetComputedStyle(element);

			var htmlElement = element as HtmlElement;
			if (htmlElement != null)
				return htmlElement.Style;
			
			return new CssStyleDeclaration();
		}
	}

	public interface IWindow : IEventTarget
	{
		int InnerWidth { get; set; }
		int InnerHeight { get; set; }

		Screen Screen { get; }
		Location Location { get; }
		Navigator Navigator { get; }
		History History { get;  }
		
		int SetTimeout(Action<object> handler, double? delay, object ctx);
		void ClearTimeout(int handle);
		int SetInterval(Action handler, double? delay);
		void ClearInterval(int handle);

		CssStyleDeclaration GetComputedStyle(Element element);
		CssStyleDeclaration GetComputedStyle(Element element, string pseudoElt);
	}
}
