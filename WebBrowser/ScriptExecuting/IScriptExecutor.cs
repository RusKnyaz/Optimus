using System;

namespace WebBrowser.ScriptExecuting
{
	public interface IScriptExecutor
	{
		void Execute(string type, string code);
		event Action<Exception> OnException;
	}
}