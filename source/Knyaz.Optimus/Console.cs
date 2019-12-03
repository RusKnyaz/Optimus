using System;
using Knyaz.Optimus.Dom.Interfaces;
using Knyaz.Optimus.Environment;
using Knyaz.Optimus.ScriptExecuting;

namespace Knyaz.Optimus
{
	/// <summary>
	/// Browser's debugging console.
	/// </summary>
	[Obsolete("Use own implementation of IConsole")]
	public sealed class Console : IConsole
	{
		private readonly IConsole _wrappedConsole;
		
		public Console() : this(new NullConsole()) {}

		internal Console(IConsole console) => _wrappedConsole = console;
		
		/// <summary>
		/// Writes a message to the console.
		/// </summary>
		/// <param name="obj"></param>
		[JsHidden]
		public void Log(object obj)
		{
			if (OnLog != null)
				OnLog(obj);
		}

		/// <summary>
		/// Fired when a new message written to the console.
		/// </summary>
		public event Action<object> OnLog;


		public void Assert(bool assertion, params object[] objs) => _wrappedConsole.Assert(assertion, objs);

		public void Assert(bool assertion, string format, params object[] objs) =>
			_wrappedConsole.Assert(assertion, format, objs);

		public void Clear() => _wrappedConsole.Clear();

		public void Error(params object[] objs) => _wrappedConsole.Error(objs);

		public void Error(string format, params object[] objs) => _wrappedConsole.Error(format, objs);

		public void Group() => _wrappedConsole.Group();

		public void Group(string label) => _wrappedConsole.Group(label);

		public void GroupCollapsed()
		{
			_wrappedConsole.GroupCollapsed();
		}

		public void GroupEnd()
		{
			_wrappedConsole.GroupEnd();
		}

		public void Info(params object[] objs)
		{
			_wrappedConsole.Info(objs);
		}

		public void Info(string format, params object[] objs)
		{
			_wrappedConsole.Info(format, objs);
		}

		public void Log(string format, params object[] objs)
		{
			OnLog?.Invoke(format);

			_wrappedConsole.Log(format, objs);
		}

		public void Log(params object[] objs)
		{
			if(objs!= null && objs.Length > 0)
				OnLog?.Invoke(objs[0]);
			
			_wrappedConsole.Log(objs);
		}

		public void Time(string label)
		{
			_wrappedConsole.Time(label);
		}

		public void TimeEnd(string label)
		{
			_wrappedConsole.TimeEnd(label);
		}

		public void Warn(params object[] objs)
		{
			_wrappedConsole.Warn(objs);
		}

		public void Warn(string format, params object[] objs)
		{
			_wrappedConsole.Warn(format, objs);
		}
	}
}