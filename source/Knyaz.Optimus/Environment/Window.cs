using System;
using Jint.Runtime;
using Knyaz.Optimus.Dom.Css;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.Dom.Events;
using Knyaz.Optimus.Dom.Interfaces;

namespace Knyaz.Optimus.Environment
{
	/// <summary>
	/// http://www.w3.org/TR/html5/browsers.html#window
	/// </summary>
	public class Window : IWindow
	{
		private readonly Engine _engine;
		private readonly EventTarget _eventTarget;

		internal WindowTimers Timers => _timers;

		internal Window(Func<object> getSyncObj, Engine engine)
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
			Navigator = new Navigator();
			History = new History(engine);
			Location = new Location(engine, History);

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

		/// <summary>
		/// Width (in pixels) of the browser window viewport including, if rendered, the vertical scrollbar.
		/// </summary>
		public int InnerWidth { get; set; }

		/// <summary>
		/// Height (in pixels) of the browser window viewport including, if rendered, the horizontal scrollbar.
		/// </summary>
		public int InnerHeight { get; set; }
		
		/// <summary>
		/// Returns a reference to the <see cref="Screen"/> object associated with the window
		/// </summary>
		public IScreen Screen { get; private set; }
		
		/// <summary>
		/// Represents the location (URL) of the object it is linked to. Changes done on it are reflected on the object it relates to. 
		/// </summary>
		public Location Location { get; private set; }
		
		/// <summary>
		/// Gets a reference to the Navigator object, which can be queried for information about the application running the script.
		/// </summary>
		public INavigator Navigator { get; private set; }
		
		/// <summary>
		/// Gets a reference to the History object, which provides an interface for manipulating the browser session history (pages visited in the tab or frame that the current page is loaded in).
		/// </summary>
		public IHistory History { get; private set; }

		private readonly WindowTimers _timers;

		/// <summary>
		/// Sets a timer which executes a function or specified piece of code once after the timer expires.
		/// </summary>
		/// <param name="handler"></param>
		/// <param name="delay"></param>
		/// <param name="ctx"></param>
		/// <returns></returns>
		public int SetTimeout(Action<object> handler, double? delay, object ctx) =>
			_timers.SetTimeout(handler, (int)(delay ?? 1), ctx);

		/// <summary>
		/// Cancels a timeout previously established by calling <see cref="SetTimeout"/>.
		/// </summary>
		/// <param name="handle"></param>
		public void ClearTimeout(int handle) => _timers.ClearTimeout(handle);

		/// <summary>
		/// Repeatedly calls a function or executes a code snippet, with a fixed time delay between each call.
		/// </summary>
		/// <returns>It returns an interval ID which uniquely identifies the interval, so you can remove it later by calling <see cref="ClearInterval"/></returns>
		public int SetInterval(Action handler, double? delay) =>
			_timers.SetInterval(handler, (int)(delay ?? 1));

		/// <summary>
		/// Cancels a timed, repeating action which was previously established by a call to <see cref="Window.SetInterval"/>.
		/// </summary>
		/// <param name="handle">The interval ID to be cancelled.</param>
		public void ClearInterval(int handle) => _timers.ClearTimeout(handle);

		/// <summary>
		/// Ddds the specified EventListener-compatible object to the list of event listeners for the specified event type on the EventTarget on which it is called. 
		/// </summary>
		/// <param name="type">A string representing the event type to listen for.</param>
		/// <param name="listener">Event handler.</param>
		/// <param name="useCapture">indicating whether events of this type will be dispatched to the registered listener before being dispatched to any EventTarget beneath it in the DOM tree.</param>
		public void AddEventListener(string type, Action<Event> listener, bool useCapture) =>
			_eventTarget.AddEventListener(type, listener, useCapture);

		/// <summary>
		/// Removes from the EventTarget an event listener previously registered with <see cref="AddEventListener"/>
		/// </summary>
		public void RemoveEventListener(string type, Action<Event> listener, bool useCapture) =>
			_eventTarget.RemoveEventListener(type, listener, useCapture);

		/// <summary>
		/// Dispatches an Event at the specified EventTarget, invoking the affected EventListeners in the appropriate order.
		/// </summary>
		/// <param name="evt"></param>
		/// <returns></returns>
		public bool DispatchEvent(Event evt) => _eventTarget.DispatchEvent(evt);

		/// <summary>
		/// Displays an alert dialog with the optional specified content and an OK button.
		/// </summary>
		/// <param name="message"></param>
		public void Alert(string message) => OnAlert?.Invoke(message);

		/// <summary>
		/// Callback to attach the handler of 'alert' method calls.
		/// </summary>
		public event Action<string> OnAlert;

		/// <summary>
		/// Gives the values of all the CSS properties of an element after applying the active stylesheets and resolving any basic computation those values may contain.
		/// </summary>
		public ICssStyleDeclaration GetComputedStyle(IElement element) => GetComputedStyle(element, null);

		/// <summary>
		/// Gives the values of all the CSS properties of an element after applying the active stylesheets and resolving any basic computation those values may contain. 
		/// </summary>
		/// <param name="element">The <see cref="Element"/> for which to get the computed style.</param>
		/// <param name="pseudoElt">A string specifying the pseudo-element to match. Must be omitted (or null) for regular elements.</param>
		public ICssStyleDeclaration GetComputedStyle(IElement element, string pseudoElt)
		{
			var styling = _engine.Styling;
			if (styling != null)
				return styling.GetComputedStyle(element);

			if (element is HtmlElement htmlElement)
				return htmlElement.Style;
			
			return new CssStyleDeclaration();
		}

		/// <summary>
		/// Returns a new MediaQueryList object representing the parsed results of the specified media query string.
		/// </summary>
		public MediaQueryList MatchMedia(string media)
			=> new MediaQueryList(media, () => _engine.CurrentMedia);

		/// <summary>
		/// Disposes the window object.
		/// </summary>
		public void Dispose() => _timers.Dispose();
	}
}
