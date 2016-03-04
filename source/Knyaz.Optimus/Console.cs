using System;

namespace WebBrowser
{
	public class Console
	{
		public void Log(object obj)
		{
			if (OnLog != null)
				OnLog(obj);
		}

		public event Action<object> OnLog;
	}
}