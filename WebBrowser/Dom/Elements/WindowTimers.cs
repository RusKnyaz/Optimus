using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace WebBrowser.Dom.Elements
{
	internal class WindowTimers
	{
		readonly List<IDisposable> _activeTimers = new List<IDisposable>();

		public event Action<Exception> OnException;

		private readonly Func<object> _getSyncObj;

		public WindowTimers(Func<Object> getGetSyncObj)
		{
			_getSyncObj = getGetSyncObj;
		}

		public int SetTimeout(Action handler, int timeout)
		{
			var timer = new TimeoutTimer(t =>
				{
					handler();
					lock (_activeTimers)
					{
						_activeTimers.Remove(t);
					}
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

		public int SetInterval(Action handler, int timeout)
		{
			var timer = new Timer(state =>
			{
				lock(_getSyncObj()) handler();
			}, null, 0, timeout);

			lock (_activeTimers)
			{
				_activeTimers.Add(timer);
			}

			return timer.GetHashCode();
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
	}
}
