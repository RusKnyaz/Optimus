using System;
using Jint.Runtime;
using WebBrowser.Dom;
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
				.SetValue(_scopeEmbeddingObjectName, new
					{
						_engine.Document, 
						_engine.Window,
						XmlHttpRequest = (Func<XmlHttpRequest>)(() => new XmlHttpRequest(engine.ResourceProvider.HttpResourceProvider)) 
					})
				.SetValue("console", new {log = (Action<object>)(o => engine.Console.Log(o))});

			InitializeScope();
		}
		
		public void Execute(string type, string code)
		{
			if (type.ToLowerInvariant() == "text/javascript")
			{
				try
				{
					_jsEngine.Execute(code);
				}
				catch (JavaScriptException e)
				{
					if (OnException != null)
						OnException(e);
					throw;
				}
			}
		}

		public event Action<Exception> OnException;
		
		private void InitializeScope()
		{
			_jsEngine.Execute(Resources.clrBridge);
		}
	}
}
