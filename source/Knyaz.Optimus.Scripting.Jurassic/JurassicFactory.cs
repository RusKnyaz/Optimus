using Knyaz.Optimus.ResourceProviders;
using Knyaz.Optimus.ScriptExecuting;

namespace Knyaz.Optimus.Scripting.Jurassic
{
    public static class JurassicFactory
	{
		/// <summary>
		/// Creates and initializes script executor based on Jurassic.
		/// </summary>
		/// <param name="ctx"></param>
		/// <returns></returns>
		public static IJsScriptExecutor Create(ScriptExecutionContext ctx)
		{
			var jsEngine = new JurassicJsEngine(ctx.Window);
			jsEngine.Execute("var window = this");
			jsEngine.Execute("var self = window");
			foreach (var type in ScriptingSettings.Default.GlobalTypes)
				jsEngine.AddGlobalType(type);

			return jsEngine;
		}
	}
}