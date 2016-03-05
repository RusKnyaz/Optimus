using System;
using System.Collections.Generic;
using Knyaz.Optimus.Dom.Events;

namespace Knyaz.Optimus.Dom.Elements
{
	public class EventTarget : IEventTarget
	{
		private readonly IEventTarget _originalTarget;
		private readonly Func<IEventTarget> _getParent;
		readonly Dictionary<string, List<Action<Event>>> _listeners = new Dictionary<string, List<Action<Event>>>();
		readonly Dictionary<string, List<Action<Event>>> _capturingListeners = new Dictionary<string, List<Action<Event>>>();

		public Func<object> _getLockObject;

		public EventTarget(IEventTarget originalTarget, Func<IEventTarget> getParent)
			: this(originalTarget, getParent, () => new object())
		{
			
		}

		public EventTarget(IEventTarget originalTarget, Func<IEventTarget> getParent, Func<object> getLockObject)
		{
			_originalTarget = originalTarget;
			_getParent = getParent;
			_getLockObject = getLockObject;
		}

		List<Action<Event>> GetListeners(string type)
		{
			return _listeners.ContainsKey(type) ? _listeners[type] : (_listeners[type] = new List<Action<Event>>());
		}

		List<Action<Event>> GetCapturingListeners(string type)
		{
			return _capturingListeners.ContainsKey(type) ? _capturingListeners[type] : (_capturingListeners[type] = new List<Action<Event>>());
		}

		public void AddEventListener(string type, Action<Event> listener, bool useCapture)
		{
			(useCapture ? GetCapturingListeners(type) : GetListeners(type)).Add(listener);
		}

		public void RemoveEventListener(string type, Action<Event> listener, bool useCapture)
		{
			//todo: test it
			(useCapture ? GetCapturingListeners(type) : GetListeners(type)).Remove(listener);
		}

		public event Action<Exception> HandlerException;

		public virtual bool DispatchEvent(Event evt)
		{
			bool isOriginalTarget = evt.Target == null;

			if (evt.Target == null)
			{
				evt.Target = _originalTarget;
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

					evt.CurrentTarget = _originalTarget;
					NotifyListeners(evt, GetCapturingListeners);
				}
				else if(isOriginalTarget)
				{
					NotifyListeners(evt, GetCapturingListeners);
				}

				if (evt.IsPropagationStopped() || !isOriginalTarget)
					return !evt.Cancelled;
			}

			evt.EventPhase = Event.AT_TARGET;
			evt.CurrentTarget = _originalTarget;

			NotifyListeners(evt, GetListeners);
			
			evt.EventPhase = Event.BUBBLING_PHASE;
			if (evt.Bubbles && !evt.IsPropagationStopped() && parentTarget != null)
				parentTarget.DispatchEvent(evt);

			//todo: handle default action;

			return !evt.Cancelled;
		}

		private void NotifyListeners(Event evt, Func<string, IList<Action<Event>>> listeners)
		{
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
						if (HandlerException != null)
							HandlerException(e);
					}
				}
			}
		}
	}
}