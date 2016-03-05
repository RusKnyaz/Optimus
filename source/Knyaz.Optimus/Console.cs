using System;

namespace Knyaz.Optimus
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