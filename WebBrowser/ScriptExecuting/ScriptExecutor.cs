using System;
using System.Threading;
using Jint.Native;
using Jint.Runtime;
using Jint.Runtime.Interop;
using WebBrowser.Dom;
using WebBrowser.Dom.Elements;
using WebBrowser.Environment;
using WebBrowser.Properties;

namespace WebBrowser.ScriptExecuting
{
	internal class ScriptExecutor : IScriptExecutor
	{
		private readonly Engine _engine;
		private SynchronizationContext _context;

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
			public XmlHttpRequest XmlHttpRequest(){ return new XmlHttpRequest(_engine.ResourceProvider, _engine.Context);}
		}

		public ScriptExecutor(Engine engine)
		{
			_engine = engine;
			_context = engine.Context;
			CreateEngine(engine);
		}

		private void CreateEngine(Engine engine)
		{
			var typeConverter = new DomConverter(() => _jsEngine);

			_jsEngine = new Jint.Engine(o => o.AddObjectConverter(typeConverter))
				.SetValue(_scopeEmbeddingObjectName, new EngineAdapter(engine));

			_jsEngine.SetValue("console", new {log = (Action<object>) (o => engine.Console.Log(o))});

			_jsEngine.Execute(Resources.clrBridge);

			_jsEngine.Global.FastAddProperty("HTMLElement", new JsValue(TypeReference.CreateTypeReference(_jsEngine, typeof(HtmlElement))), false, false, false);
			_jsEngine.Global.FastAddProperty("Element", new JsValue(TypeReference.CreateTypeReference(_jsEngine, typeof(Element))), false, false, false);
		}

		public void Execute(string type, string code)
		{
			_context.Send(_ =>
			{
				if (string.IsNullOrEmpty(type) || type.ToLowerInvariant() == "text/javascript")
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
					}
					catch (Exception e)
					{
						if (OnException != null)
							OnException(e);
					}
				}
			}, null);
		}

		public event Action<Exception> OnException;
		public void Clear()
		{
			CreateEngine(_engine);
		}
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
			: base(message ?? "Script executing error.", inner)
		{
			Code = code;
		}

		public string Code { get; private set; }
	}
}
