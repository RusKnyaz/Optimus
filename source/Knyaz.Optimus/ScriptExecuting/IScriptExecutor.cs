using System;

namespace Knyaz.Optimus.ScriptExecuting
{
	public interface IScriptExecutor
	{
		void Execute(string type, string code);
		object Evaluate(string type, string code);
		event Action<Exception> OnException;
		void Clear();
		object EvalFuncAndCall(string v, object @this, params object[] evt);
	}
}