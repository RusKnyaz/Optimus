using System;
using System.Collections.Generic;
using Knyaz.Optimus.Dom.Interfaces;

namespace Knyaz.Optimus.Tests.TestingTools
{
	public class TestingConsole : IConsole
	{
		public readonly List<object> LogHistory = new List<object>();
		public event Action<object> OnLog;
		
		public void Assert(bool assertion, params object[] objs)
		{
		}

		public void Assert(bool assertion, string format, params object[] objs)
		{
		}

		public void Clear()
		{
		}

		public void Error(params object[] objs)
		{
		}

		public void Error(string format, params object[] objs)
		{
		}

		public void Group()
		{
		}

		public void Group(string label)
		{
		}

		public void GroupCollapsed()
		{
		}

		public void GroupEnd()
		{
		}

		public void Info(params object[] objs)
		{
		}

		public void Info(string format, params object[] objs)
		{
		}

		public void Log(string format, params object[] objs) => Log((object)format);

		public void Log(params object[] objs)
		{
			if (objs == null)
			{
				LogHistory.Add(null);
				OnLog?.Invoke(null);
			}
			else if (objs.Length == 1)
			{
				LogHistory.Add(objs[0]);
				OnLog?.Invoke(objs[0]);
			}
			else
			{
				LogHistory.Add(objs);
				OnLog?.Invoke(objs);
			}
		}

		public void Time(string label)
		{
		}

		public void TimeEnd(string label)
		{
		}

		public void Warn(params object[] objs)
		{
		}

		public void Warn(string format, params object[] objs)
		{
		}
	}
}