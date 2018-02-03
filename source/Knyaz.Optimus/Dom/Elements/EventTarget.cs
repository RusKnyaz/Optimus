using System;
using System.Collections.Generic;
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
		readonly Dictionary<string, List<Action<Event>>> _bubblingListeners = new Dictionary<string, List<Action<Event>>>();
		readonly Dictionary<string, List<Action<Event>>> _capturingListeners = new Dictionary<string, List<Action<Event>>>();

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

		List<Action<Event>> GetBubblingListeners(string type) =>
			_bubblingListeners.ContainsKey(type) ? _bubblingListeners[type] : (_bubblingListeners[type] = new List<Action<Event>>());

		List<Action<Event>> GetCapturingListeners(string type) =>
			_capturingListeners.ContainsKey(type) ? _capturingListeners[type] : (_capturingListeners[type] = new List<Action<Event>>());

		/// <summary>
		/// Registers new event handler.
		/// </summary>
		/// <param name="type">The type name of the event.</param>
		/// <param name="listener">The event handler.</param>
		/// <param name="useCapture">If <c>true</c> the handler invoked in 'capturing' order, 
		/// othervise in the handler invoked in 'bubbling' order.</param>
		public void AddEventListener(string type, Action<Event> listener, bool useCapture = false)
		{
			if (listener == null)
				return;
			
			(useCapture ? GetCapturingListeners(type) : GetBubblingListeners(type)).Add(listener);
		}

		/// <summary>
		/// Removes previously registered event handler.
		/// </summary>
		/// <param name="type">The type name of event.</param>
		/// <param name="listener">The handler to be removed.</param>
		/// <param name="useCapture">The invocation order to be handler removed from.</param>
		public void RemoveEventListener(string type, Action<Event> listener, bool useCapture = false)
		{
			if (listener == null)
				return;
			
			(useCapture ? GetCapturingListeners(type) : GetBubblingListeners(type)).Remove(listener);	
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

		private void NotifyListeners(Event evt, Func<string, IList<Action<Event>>> listenersFn)
		{
			evt.CurrentTarget = _target;
			lock (_getLockObject())
			{
				var listeners = listenersFn(evt.Type);
				// ReSharper disable once ForCanBeConvertedToForeach, to avoid 'collection was modified' exception
				for(var i = 0;i< listeners.Count;i++)
				{
					var listener = listeners[i];
					try
					{
						listener(evt);
					}
					catch (Exception e)
					{
						HandlerException?.Invoke(e);
					}
				}
			}
		}
	}
}