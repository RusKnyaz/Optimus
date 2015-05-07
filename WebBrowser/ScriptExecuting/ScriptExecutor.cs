using System;
using Jint.Runtime;
using WebBrowser.Properties;

namespace WebBrowser.ScriptExecuting
{
	internal class ScriptExecutor : IScriptExecutor
	{
		private string _scopeEmbeddingObjectName = "A89A3DC7FB5944849D4DE0781117A595";
		
		private readonly Engine _engine;
		private Jint.Engine _jsEngine;

		public ScriptExecutor(Engine engine)
		{
			_engine = engine;
			_jsEngine = new Jint.Engine()
				.SetValue(_scopeEmbeddingObjectName, new {_engine.Document})
				.SetValue("console", new{log = (Action<object>)OnLog});

			InitializeScope();
		}
		
		public void Execute(string type, string code)
		{
			if (type == "text/JavaScript")
			{
				try
				{
					_jsEngine.Execute(code);
				}
				catch (JavaScriptException e)
				{
					var er = _jsEngine.GetLastSyntaxNode();
					throw;
				}
			}
		}
		
		private void InitializeScope()
		{
			_jsEngine.Execute(Resources.clrBridge);
		}


		//todo: remove. it's for testing
		public static Action<object> Log;

		private static void OnLog(object obj)
		{
			if (Log != null)
				Log(obj);
		}
	}
}
