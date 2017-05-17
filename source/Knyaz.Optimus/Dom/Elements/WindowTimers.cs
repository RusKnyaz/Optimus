using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Knyaz.Optimus.Tools;

namespace Knyaz.Optimus.Dom.Elements
{
	public class WindowTimers
	{
		readonly List<IDisposable> _activeTimers = new List<IDisposable>();

		public event Action<Exception> OnException;
		public event Action OnExecuting;
		public event Action OnExecuted;

		private readonly Func<object> _getSyncObj;

		public WindowTimers(Func<Object> getGetSyncObj)
		{
			_getSyncObj = getGetSyncObj;
		}

		/// <summary>
		/// Call handler once
		/// </summary>
		/// <param name="handler"></param>
		/// <param name="timeout"></param>
		/// <param name="ctx">The data object passed to callback function.</param>
		/// <returns></returns>
		public int SetTimeout(Action<object> handler, int timeout, object ctx)
		{
			var timer = new TimeoutTimer(t =>
				{
					RaiseOnExecuting();
					handler(ctx);
					lock (_activeTimers)
					{
						_activeTimers.Remove(t);
					}
					RaiseOnExecuted();
				}, exception =>
					{
						if (OnException != null)
							OnException(exception);
					}, timeout, _getSyncObj);

			lock (_activeTimers)
			{
				_activeTimers.Add(timer);	
			}

			timer.Start();
			
			return timer.GetHashCode();
		}

		/// <summary>
		/// Call handler periodically
		/// </summary>
		/// <param name="handler"></param>
		/// <param name="timeout"></param>
		/// <returns></returns>
		public int SetInterval(Action handler, int timeout)
		{
			bool stopped = false;
			var container = new Timer[1];
			Disposable disp;
			lock (_activeTimers)
			{
				disp = new Disposable(() => stopped = true);
				_activeTimers.Add(disp);
			}

			var timer = new Timer(state =>
			{
				lock (_getSyncObj())
				{
					try
					{
						if (!stopped)
						{
							handler();
						}
						else if (container[0] != null)
						{
							container[0].Dispose();
							container[0] = null;
						}
					}
					catch (Exception e)
					{
					}
				}
			}, null, 0, timeout);

			container[0] = timer;

			return disp.GetHashCode();
		}

		class TimeoutTimer : IDisposable
		{
			private readonly Action<TimeoutTimer> _handler;
			private readonly Action<Exception> _errorHandler;
			private readonly int _timeout;
			private readonly Func<object> _getSync;
			private Timer _timer;

			public TimeoutTimer(Action<TimeoutTimer> handler, Action<Exception> errorHandler, int timeout, Func<Object> getSync)
			{
				_handler = handler;
				_errorHandler = errorHandler;
				_timeout = timeout;
				_getSync = getSync;
			}

			private void Callback(object state)
			{
				lock (_getSync())
				{
					try
					{
						_handler(this);
					}
					catch (Exception e)
					{
						if (_errorHandler != null)
							_errorHandler(e);
					}
				}
			}
			
			public void Dispose()
			{
				lock (this)
				{
					if(_timer != null)
						_timer.Dispose();
				}
			}

			public void Start()
			{
				_timer = new Timer(Callback, null, _timeout, Timeout.Infinite);
			}
		}

		public void ClearTimeout(int handle)
		{
			lock (_activeTimers)
			{
				var timer = _activeTimers.FirstOrDefault(x => x.GetHashCode() == handle);
				if (timer != null)
				{
					timer.Dispose();
					_activeTimers.Remove(timer);
				}
			}
		}

		protected virtual void RaiseOnExecuting()
		{
			var handler = OnExecuting;
			if (handler != null) handler();
		}

		protected virtual void RaiseOnExecuted()
		{
			var handler = OnExecuted;
			if (handler != null) handler();
		}
	}


}
