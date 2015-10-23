using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace WebBrowser.Dom.Elements
{
	internal class WindowTimers
	{
		private readonly SynchronizationContext _context;
		readonly List<Timer> _activeTimers = new List<Timer>();

		public event Action<Exception> OnException;

		private readonly Func<object> _getSyncObj;

		public WindowTimers(Func<Object> getGetSyncObj)
		{
			_getSyncObj = getGetSyncObj;
		}

		public int SetTimeout(Action handler, int timeout)
		{
			var timer = new Timer(state =>
			{
				lock (state)
				{
					try
					{
						handler();
					}
					catch (Exception e)
					{
						if (OnException != null)
							OnException(e);
					}
				}
			}, _getSyncObj(), timeout, Timeout.Infinite);

			return timer.GetHashCode();
		}

		public void ClearTimeout(int handle)
		{
			var timer = _activeTimers.FirstOrDefault(x => x.GetHashCode() == handle);
			if (timer != null)
			{
				timer.Dispose();
			}
		}
	}
}
