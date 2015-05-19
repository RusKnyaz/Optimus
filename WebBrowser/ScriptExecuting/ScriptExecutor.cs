using System;
using Jint;
using Jint.Runtime;
using WebBrowser.Dom;
using WebBrowser.Environment;
using WebBrowser.Properties;

namespace WebBrowser.ScriptExecuting
{
	internal class ScriptExecutor : IScriptExecutor
	{
		private string _scopeEmbeddingObjectName = "A89A3DC7FB5944849D4DE0781117A595";
		
		private Jint.Engine _jsEngine;

		class EngineAdapter
		{
			private readonly Engine _engine;

			public EngineAdapter(Engine engine)
			{
				_engine = engine;
			}

			public Document Document { get { return _engine.Document; } }
			public Window Window { get { return _engine.Window; } }
			public XmlHttpRequest XmlHttpRequest(){ return new XmlHttpRequest(_engine.ResourceProvider.HttpResourceProvider);}
		}

		public ScriptExecutor(Engine engine)
		{
			_jsEngine = new Jint.Engine(o => o.AllowDebuggerStatement(true))
				.SetValue(_scopeEmbeddingObjectName, new EngineAdapter(engine))
				.SetValue("console", new {log = (Action<object>)(o => engine.Console.Log(o))});

			_jsEngine.Execute(Resources.clrBridge);
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
					var ex = new ScriptExecutingException(e.Error.ToString(), e, code);
					if (OnException != null)
						OnException(ex);
					throw ex;
				}
			}
		}

		public event Action<Exception> OnException;
	}

	[Serializable]
	public class ScriptExecutingException : Exception
	{
		public ScriptExecutingException()
		{
		}

		public ScriptExecutingException(string message) : base(message)
		{
		}

		public ScriptExecutingException(string message, Exception inner) : base(message, inner)
		{
		}

		public ScriptExecutingException(string message, Exception inner, string code)
			: base("Script executing error.", inner)
		{
			Code = code;
		}

		public string Code { get; private set; }
	}
}
