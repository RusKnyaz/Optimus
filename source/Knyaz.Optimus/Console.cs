using System;
using Knyaz.Optimus.ScriptExecuting;

namespace Knyaz.Optimus
{
	[DomItem]
	public sealed class Console
	{
		public void Log(object obj)
		{
			if (OnLog != null)
				OnLog(obj);
		}

		public event Action<object> OnLog;
	}
}