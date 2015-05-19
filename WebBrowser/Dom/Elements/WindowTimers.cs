using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace WebBrowser.Dom.Elements
{
	internal class WindowTimers
	{
		readonly List<Timer> _activeTimers = new List<Timer>();

		public int SetTimeout(Action handler, int timeout)
		{
			var timer = new Timer(state => {handler();}, null, timeout, Timeout.Infinite);

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
