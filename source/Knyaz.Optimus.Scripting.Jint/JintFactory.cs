using System.Reflection;
using Knyaz.Optimus.Scripting.Jint.Internal;

namespace Knyaz.Optimus.ScriptExecuting.Jint
{
	public static class JintFactory
	{
		public static IJsScriptExecutor Create(ScriptExecutionContext ctx)
		{
			var jsEngine = new JintJsEngine(ctx.Window);

			jsEngine.Execute("var window = this");
			jsEngine.Execute("var self = window");

			foreach (var type in ctx.Settings.GlobalTypes)
			{
				var name = type.GetCustomAttribute<JsNameAttribute>()?.Name ?? type.Name;
				jsEngine.AddGlobalType(type, name);
			}

			return jsEngine;
		}
	}
}