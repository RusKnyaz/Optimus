using Knyaz.Optimus.Dom.Interfaces;
using Knyaz.Optimus.ScriptExecuting;

namespace Knyaz.Optimus
{
	public class ScriptExecutionContext
	{
		public ScriptExecutionContext(IWindowEx window)
		{
			Window = window;
		}

		public IWindowEx Window { get; }

		public ScriptingSettings Settings => ScriptingSettings.Default;
	}
}