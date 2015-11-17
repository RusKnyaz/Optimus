using System;
using Jint.Native;
using Jint.Runtime;
using WebBrowser.Dom;
using WebBrowser.Dom.Elements;
using WebBrowser.Environment;
using WebBrowser.Properties;

namespace WebBrowser.ScriptExecuting
{
	internal class ScriptExecutor : IScriptExecutor
	{
		private readonly Engine _engine;

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
			public XmlHttpRequest XmlHttpRequest(){ return new XmlHttpRequest(_engine.ResourceProvider, () => Document);}
		}

		public ScriptExecutor(Engine engine)
		{
			_engine = engine;
			CreateEngine(engine);
		}

		private void CreateEngine(Engine engine)
		{
			var typeConverter = new DomConverter(() => _jsEngine);

			_jsEngine = new Jint.Engine(o => o.AddObjectConverter(typeConverter))
				.SetValue(_scopeEmbeddingObjectName, new EngineAdapter(engine));

			_jsEngine.SetValue("console", new {log = (Action<object>) (o => engine.Console.Log(o))});

			_jsEngine.Execute(Resources.clrBridge);

			AddDomType("Node", typeof(Node));
			AddDomType("Element", typeof(Element));
			AddDomType("HTMLBodyElement", typeof(HtmlBodyElement));
			AddDomType("HTMLButtonElement", typeof(HtmlButtonElement));
			AddDomType("HTMLElement", typeof(HtmlElement));
			AddDomType("HTMLIFrameElement", typeof(HtmlIFrameElement));
			AddDomType("HTMLInputElement", typeof(HtmlInputElement));
			AddDomType("HTMLTextAreaElement", typeof(HtmlTextAreaElement));
			AddDomType("HTMLUnknownElement", typeof(HtmlUnknownElement));
			AddDomType("HTMLFormElement", typeof(HtmlFormElement));
			AddDomType("HTMLHtmlElement", typeof(HtmlHtmlElement));
			AddDomType("Script", typeof(Script));
			AddDomType("Comment", typeof(Comment));
			AddDomType("Document", typeof(Document));
			AddDomType("Text", typeof(Text));
			AddDomType("Attr", typeof(Attr));
		}

		private void AddDomType(string jsName, Type type)
		{
			_jsEngine.Global.FastAddProperty(jsName, new JsValue(new ClrPrototype(_jsEngine, type)), false, false, false);
		}

		public void Execute(string type, string code)
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
