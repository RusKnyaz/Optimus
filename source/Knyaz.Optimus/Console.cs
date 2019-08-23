using System;
using Knyaz.Optimus.ScriptExecuting;

namespace Knyaz.Optimus
{
	/// <summary>
	/// Browser's debugging console.
	/// </summary>
	[DomItem]
	public sealed class Console
	{
		/// <summary>
		/// Writes a message to the console.
		/// </summary>
		/// <param name="obj"></param>
		public void Log(object obj)
		{
			if (OnLog != null)
				OnLog(obj);
		}

		/// <summary>
		/// Fired when a new message written to the console.
		/// </summary>
		public event Action<object> OnLog;
	}
}