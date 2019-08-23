using System;
using System.Collections.Generic;
using System.Linq;
using Knyaz.Optimus.Dom.Events;
using Knyaz.Optimus.Dom.Interfaces;

namespace Knyaz.Optimus.Dom.Elements
{
	/// <summary>
	/// Implements logic for event propogation and handling.
	/// </summary>
	public sealed class EventTarget : IEventTarget
	{
		private readonly object _target;
		private readonly Func<IEventTarget> _getParent;
		readonly Dictionary<string, List<Listener>> _bubblingListeners = new Dictionary<string, List<Listener>>();
		readonly Dictionary<string, List<Listener>> _capturingListeners = new Dictionary<string, List<Listener>>();

		private Func<object> _getLockObject;

		/// <summary>
		/// Creates new EventTarget.
		/// </summary>
		/// <param name="target">The html node or Window object this event target attached to.</param>
		/// <param name="getParent">The function to get event target of parent element.</param>
		public EventTarget(object target, Func<IEventTarget> getParent)
			: this(target, getParent, () => new object()){}

		/// <summary>
		/// Creates new EventTarget.
		/// </summary>
		/// <param name="target">The html node or Window object this event target attached to.</param>
		/// <param name="getParent">The function to get event target of parent element.</param>
		/// <param name="getLockObject">The function to get sync object.</param>
		public EventTarget(object target, Func<IEventTarget> getParent, Func<object> getLockObject)
		{
			_target = target;
			_getParent = getParent;
			_getLockObject = getLockObject;
		}

		List<Listener> GetBubblingListeners(string type) =>
			_bubblingListeners.ContainsKey(type) ? _bubblingListeners[type] : (_bubblingListeners[type] = new List<Listener>());

		List<Listener> GetCapturingListeners(string type) =>
			_capturingListeners.ContainsKey(type) ? _capturingListeners[type] : (_capturingListeners[type] = new List<Listener>());

		/// <summary>
		/// Registers new event handler.
		/// </summary>
		/// <param name="type">The type name of the event.</param>
		/// <param name="handler">The event handler.</param>
		/// <param name="options">An options object that specifies characteristics about the event listener. </param>
		public void AddEventListener(string type, Action<Event> handler, EventListenerOptions options)
		{
			if (handler == null)
				return;
			var listenersList = options.Capture ? GetCapturingListeners(type) : GetBubblingListeners(type);
			if (listenersList.Any(x => x.Handler == handler))//do not add listener twice
				return;
			listenersList.Add(new Listener {Handler = handler, Options = options});
		}

		public void RemoveEventListener(string type, Action<Event> listener, EventListenerOptions options) =>
			RemoveEventListener(type, listener, options.Capture);

		private static EventListenerOptions CaptureOptions = new EventListenerOptions {Capture = true};
		private static EventListenerOptions BubbleOptions = new EventListenerOptions();
		
		/// <summary>
		/// Registers new event handler.
		/// </summary>
		/// <param name="type">The type name of the event.</param>
		/// <param name="handler">The event handler.</param>
		/// <param name="useCapture">If <c>true</c> the handler invoked in 'capturing' order, 
		/// othervise in the handler invoked in 'bubbling' order.</param>
		public void AddEventListener(string type, Action<Event> handler, bool useCapture = false) =>
			AddEventListener(type, handler, useCapture ? CaptureOptions : BubbleOptions);

		/// <summary>
		/// Removes previously registered event handler.
		/// </summary>
		/// <param name="type">The type name of event.</param>
		/// <param name="handler">The handler to be removed.</param>
		/// <param name="useCapture">The invocation order to be handler removed from.</param>
		public void RemoveEventListener(string type, Action<Event> handler, bool useCapture = false)
		{
			if (handler == null)
				return;

			var list = useCapture ? GetCapturingListeners(type) : GetBubblingListeners(type);

			list.RemoveAll(x => x.Handler == handler);
		}
		

		/// <summary>
		/// Called when exception in handler occured.
		/// </summary>
		public event Action<Exception> HandlerException;

		/// <summary>
		/// Called before the event dispatched.
		/// </summary>
		public event Action<Event> BeforeEventDispatch;
		
		/// <summary>
		/// Call element's event handlers attached using attribute or property (like 'onclick' and etc.).
		/// </summary>
		public event Action<Event> CallDirectEventSubscribers;

		/// <summary>
		/// Called after the event dispatched.
		/// </summary>
		public event Action<Event> AfterEventDispatch;

		/// <summary>
		/// Dispatches an event.
		/// </summary>
		/// <param name="evt">The event to be dispatched.</param>
		/// <returns><c>False</c> if event was cancelled.</returns>
		public bool DispatchEvent(Event evt)
		{
			bool isOriginalTarget = evt.Target == null;

			if (evt.Target == null)
			{
				evt.Target = _target;
				evt.EventPhase = Event.CAPTURING_PHASE;
			}

			var parentTarget = _getParent();

			BeforeEventDispatch?.Invoke(evt);

			if (evt.EventPhase == Event.CAPTURING_PHASE)
			{
				if (parentTarget != null)
				{
					parentTarget.DispatchEvent(evt);

					if (evt.IsPropagationStopped())
						return !evt.Canceled;
				}

				if (!isOriginalTarget)
				{
					NotifyListeners(evt, GetCapturingListeners);

					if (evt.IsPropagationStopped())
						return !evt.Canceled;
				}
			}

			if (isOriginalTarget)
			{
				evt.EventPhase = Event.AT_TARGET;
			}
			else if(evt.EventPhase == Event.CAPTURING_PHASE)
			{
				return !evt.Canceled;
			}

			if (evt.EventPhase == Event.AT_TARGET || evt.EventPhase == Event.BUBBLING_PHASE)
			{
				//direct subscribed event handlers (from attributes or element properties like div.onclick
				evt.CurrentTarget = _target;
				CallDirectEventSubscribers?.Invoke(evt);
				if (evt.IsPropagationStopped())
					return !evt.Canceled;

				NotifyListeners(evt, GetBubblingListeners);
				if (evt.IsPropagationStopped())
					return !evt.Canceled;
			}

			if (isOriginalTarget)
			{
				NotifyListeners(evt, GetCapturingListeners);
				if (evt.IsPropagationStopped())
					return !evt.Canceled;
			}

			if (evt.Bubbles && !evt.IsPropagationStopped() && parentTarget != null && evt.Bubbles)
			{
				evt.EventPhase = Event.BUBBLING_PHASE;
				parentTarget.DispatchEvent(evt);
			}
			
			if(evt.Target == _target)
				AfterEventDispatch?.Invoke(evt);

			return !evt.Canceled;
		}

		private void NotifyListeners(Event evt, Func<string, IList<Listener>> listenersFn)
		{
			evt.CurrentTarget = _target;
			lock (_getLockObject())
			{
				var listeners = listenersFn(evt.Type);
				// ReSharper disable once ForCanBeConvertedToForeach, to avoid 'collection was modified' exception
				for(var i = 0;i< listeners.Count;i++)
				{
					var listener = listeners[i];
					if (listener.Options != null && listener.Options.Passive)
						evt.PreventDefaultDeprecated = true;

					try
					{
						listener.Handler(evt);
					}
					catch (Exception e)
					{
						HandlerException?.Invoke(e);
					}
					finally
					{
						evt.PreventDefaultDeprecated = false;
					}

					if (listener.Options != null && listener.Options.Once)
						RemoveEventListener(evt.Type, listener.Handler, listener.Options);
				}
			}
		}

		class Listener
		{
			public Action<Event> Handler;
			public EventListenerOptions Options;
		}
	}
}