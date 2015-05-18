using System;

namespace WebBrowser.ScriptExecuting
{
	internal interface IScriptExecutor
	{
		void Execute(string type, string code);
		event Action<Exception> OnException;
	}
}