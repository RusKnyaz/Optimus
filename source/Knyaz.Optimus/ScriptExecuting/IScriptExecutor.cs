using System;

namespace Knyaz.Optimus.ScriptExecuting
{
	public interface IScriptExecutor
	{
		void Execute(string type, string code);
		event Action<Exception> OnException;
		void Clear();
	}
}