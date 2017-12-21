using System;
using System.Collections.Generic;
using Knyaz.Optimus.Dom.Events;
using Knyaz.Optimus.Dom.Interfaces;

namespace Knyaz.Optimus.Dom.Elements
{
	public class EventTarget : IEventTarget
	{
		private readonly IEventTarget _element;
		private readonly Func<IEventTarget> _getParent;
		readonly Dictionary<string, List<Action<Event>>> _bubblingListeners = new Dictionary<string, List<Action<Event>>>();
		readonly Dictionary<string, List<Action<Event>>> _capturingListeners = new Dictionary<string, List<Action<Event>>>();

		public Func<object> _getLockObject;

		public EventTarget(IEventTarget originalTarget, Func<IEventTarget> getParent)
			: this(originalTarget, getParent, () => new object())
		{
			
		}

		public EventTarget(IEventTarget originalTarget, Func<IEventTarget> getParent, Func<object> getLockObject)
		{
			_element = originalTarget;
			_getParent = getParent;
			_getLockObject = getLockObject;
		}

		List<Action<Event>> GetBubblingListeners(string type) =>
			_bubblingListeners.ContainsKey(type) ? _bubblingListeners[type] : (_bubblingListeners[type] = new List<Action<Event>>());

		List<Action<Event>> GetCapturingListeners(string type) =>
			_capturingListeners.ContainsKey(type) ? _capturingListeners[type] : (_capturingListeners[type] = new List<Action<Event>>());

		public void AddEventListener(string type, Action<Event> listener) => AddEventListener(type, listener, false);

		public void AddEventListener(string type, Action<Event> listener, bool useCapture) =>
			(useCapture ? GetCapturingListeners(type) : GetBubblingListeners(type)).Add(listener);

		public void RemoveEventListener(string type, Action<Event> listener) =>
			RemoveEventListener(type, listener, false);

		public void RemoveEventListener(string type, Action<Event> listener, bool useCapture) =>
			(useCapture ? GetCapturingListeners(type) : GetBubblingListeners(type)).Remove(listener);

		public event Action<Exception> HandlerException;

		public event Action<Event> BeforeEventDispatch;

		public virtual bool DispatchEvent(Event evt)
		{
			bool isOriginalTarget = evt.Target == null;

			if (evt.Target == null)
			{
				evt.Target = _element;
				evt.EventPhase = Event.CAPTURING_PHASE;
			}

			var parentTarget = _getParent();

			if (evt.EventPhase == Event.CAPTURING_PHASE)
			{
				if (parentTarget != null)
				{
					parentTarget.DispatchEvent(evt);

					if (evt.IsPropagationStopped())
						return !evt.Cancelled;
				}

				if (!isOriginalTarget)
				{
					NotifyListeners(evt, GetCapturingListeners);

					if (evt.IsPropagationStopped())
						return !evt.Cancelled;
				}
			}

			if (isOriginalTarget)
			{
				evt.EventPhase = Event.AT_TARGET;
			}
			else if(evt.EventPhase == Event.CAPTURING_PHASE)
			{
				return !evt.Cancelled;
			}

			if (evt.EventPhase == Event.AT_TARGET || evt.EventPhase == Event.BUBBLING_PHASE)
			{
				//direct subscribed event handlers (from attributes or element properties like div.onclick
				evt.CurrentTarget = _element;
				BeforeEventDispatch?.Invoke(evt);
				if (evt.IsPropagationStopped())
					return !evt.Cancelled;

				NotifyListeners(evt, GetBubblingListeners);
				if (evt.IsPropagationStopped())
					return !evt.Cancelled;
			}

			if (isOriginalTarget)
			{
				NotifyListeners(evt, GetCapturingListeners);
				if (evt.IsPropagationStopped())
					return !evt.Cancelled;
			}

			if (evt.Bubbles && !evt.IsPropagationStopped() && parentTarget != null && evt.Bubbles)
			{
				evt.EventPhase = Event.BUBBLING_PHASE;
				parentTarget.DispatchEvent(evt);
			}

			return !evt.Cancelled;
		}

		private void NotifyListeners(Event evt, Func<string, IList<Action<Event>>> listeners)
		{
			evt.CurrentTarget = _element;
			lock (_getLockObject())
			{
				foreach (var listener in listeners(evt.Type))
				{
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