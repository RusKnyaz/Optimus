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

		public WindowTimers(SynchronizationContext context)
		{
			_context = context;
		}

		public int SetTimeout(Action handler, int timeout)
		{
			//todo: handle exceptions;
			
			var timer = new Timer(state =>
			{
				_context.Send(o => handler(), null);
			}, null, timeout, Timeout.Infinite);

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
