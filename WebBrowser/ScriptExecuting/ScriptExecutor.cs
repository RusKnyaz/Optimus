using System;
using Jint.Native;
using Jint.Runtime;
using Jint.Runtime.Descriptors;
using Jint.Runtime.Descriptors.Specialized;
using Jint.Runtime.Interop;
using WebBrowser.Dom;
using WebBrowser.Dom.Elements;
using WebBrowser.Dom.Perf;
using WebBrowser.Environment;
using WebBrowser.Properties;

namespace WebBrowser.ScriptExecuting
{
	internal class ScriptExecutor : IScriptExecutor
	{
		private readonly Engine _engine;

		private string _scopeEmbeddingObjectName = "A89A3DC7FB5944849D4DE0781117A595";
		
		private Jint.Engine _jsEngine;
		private DomConverter _typeConverter;

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
			_typeConverter = new DomConverter(() => _jsEngine);

			_jsEngine = new Jint.Engine(o => o.AddObjectConverter(_typeConverter))
				.SetValue(_scopeEmbeddingObjectName, new EngineAdapter(engine));

			_jsEngine.SetValue("console", new {log = (Action<object>) (o => engine.Console.Log(o))});

			_jsEngine.Execute(Resources.clrBridge);

			AddClrType("Node", typeof(Node));
			AddClrType("Element", typeof(Element));
			AddClrType("HTMLBodyElement", typeof(HtmlBodyElement));
			AddClrType("HTMLButtonElement", typeof(HtmlButtonElement));
			AddClrType("HTMLDivElement", typeof(HtmlDivElement));
			AddClrType("HTMLElement", typeof(HtmlElement));
			AddClrType("HTMLIFrameElement", typeof(HtmlIFrameElement));
			AddClrType("HTMLInputElement", typeof(HtmlInputElement));
			AddClrType("HTMLTextAreaElement", typeof(HtmlTextAreaElement));
			AddClrType("HTMLUnknownElement", typeof(HtmlUnknownElement));
			AddClrType("HTMLFormElement", typeof(HtmlFormElement));
			AddClrType("HTMLHtmlElement", typeof(HtmlHtmlElement));
			AddClrType("Script", typeof(Script));
			AddClrType("Comment", typeof(Comment));
			AddClrType("Document", typeof(Document));
			AddClrType("Text", typeof(Text));
			AddClrType("Attr", typeof(Attr));

			//Perf types
			AddClrType("ArrayBuffer", typeof(ArrayBuffer));
			AddClrType("Int8Array", typeof(Int8Array));
			AddClrType("Uint8Array", typeof(UInt8Array));
			AddClrType("Int16Array", typeof(Int16Array));
			AddClrType("Uint16Array", typeof(UInt16Array));

			AddGlobalGetter("document", () => engine.Document);
			AddGlobalGetter("location", () => engine.Window.Location);
			AddGlobalGetter("navigator", () => engine.Window.Navigator);
			AddGlobalGetter("screen", () => engine.Window.Screen);
			AddGlobalGetter("innerWidth", () => engine.Window.InnerWidth);
			AddGlobalGetter("innerHeight", () => engine.Window.InnerHeight);
			
			AddGlobalAct("alert", (_,x) => engine.Window.Alert(x[0].AsString()));
			AddGlobalAct("clearInterval", (_,x) => engine.Window.ClearInterval(x.Length > 0 ? (int)x[0].AsNumber() : -1));
			AddGlobalAct("clearTimeout", (_, x) => engine.Window.ClearTimeout(x.Length > 0 ? (int)x[0].AsNumber() : -1));
			AddGlobalAct("dispatchEvent", (_, x) => engine.Window.DispatchEvent(x.Length > 0 ? (Event)x[0].ToObject() : null));
		}

		private void AddGlobalAct(string name, Action<JsValue, JsValue[]> action)
		{
			var jsFunc = new ClrFunctionInstance(_jsEngine, (jsValue, values) =>
			{
				action(jsValue, values);
				return JsValue.Undefined;
			});

			_jsEngine.Global.DefineOwnProperty(name, new ClrAccessDescriptor(_jsEngine, value => jsFunc), true);
		}

		private void AddGlobalGetter(string name, Func<object> getter)
		{
			_jsEngine.Global.DefineOwnProperty(name, new ClrAccessDescriptor(_jsEngine, value =>
			{
				JsValue res;
				_typeConverter.TryConvert(getter(), out res);
				return res;
			}), true);
		}

		private void AddClrType(string jsName, Type type)
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
