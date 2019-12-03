using Knyaz.Optimus.Dom.Interfaces;

namespace Knyaz.Optimus.Environment
{
	/// <summary>
	/// The console implementation that doing nothing.
	/// </summary>
	internal class NullConsole : IConsole
	{
		public void Assert(bool assertion, params object[] objs) {}

		public void Assert(bool assertion, string format, params object[] objs) {}

		public void Clear() {}

		public void Error(params object[] objs) {}

		public void Error(string format, params object[] objs) {}

		public void Group() {}

		public void Group(string label) {}

		public void GroupCollapsed() {}

		public void GroupEnd() {}

		public void Info(params object[] objs) {}

		public void Info(string format, params object[] objs) {}

		public void Log(string format, params object[] objs) {}

		public void Log(params object[] objs) {}

		public void Time(string label) {}

		public void TimeEnd(string label) {}

		public void Warn(params object[] objs) {}

		public void Warn(string format, params object[] objs) {}
	}
}