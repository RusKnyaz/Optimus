using System;
using Jint.Native;
using Jint.Runtime;
using Jint.Runtime.Descriptors.Specialized;
using Jint.Runtime.Interop;
using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.Dom.Events;
using Knyaz.Optimus.Dom.Perf;
using Knyaz.Optimus.Environment;

namespace Knyaz.Optimus.ScriptExecuting
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

			_jsEngine.Execute("var window = this");

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
			
			AddGlobalAct("addEventListener", (_, x) => engine.Window.AddEventListener(
				x.Length > 0 ? x[0].AsString() : null,
				_typeConverter.ConvertDelegate<Event>(x[1]),
				x.Length > 2 && x[2].AsBoolean()));

			AddGlobalAct("removeEventListener", (_, x) => engine.Window.RemoveEventListener(
				x.Length > 0 ? x[0].AsString() : null,
				_typeConverter.ConvertDelegate<Event>(x[1]),
				x.Length > 2 && x[2].AsBoolean()));

			AddGlobalFunc("setTimeout", (_, x) =>
			{
				if (x.Length == 0)
					return JsValue.Undefined;
				var res= engine.Window.SetTimeout(_typeConverter.ConvertDelegate(x[0]), x.Length > 1 ? x[1].AsNumber() : 1);
				return new JsValue(res);
			});

			AddGlobalFunc("setInterval", (_, x) =>
			{
				if (x.Length == 0)
					return JsValue.Undefined;
				var res = engine.Window.SetInterval(_typeConverter.ConvertDelegate(x[0]), x.Length > 1 ? x[1].AsNumber() : 1);
				return new JsValue(res);
			});

			var jsFunc = new ClrFuncCtor(_jsEngine, (x) =>
			{
				JsValue res;
				_typeConverter.TryConvert(new XmlHttpRequest(_engine.ResourceProvider, () => _engine.Document), out res);
				return res.AsObject();
			});

			jsFunc.FastAddProperty("UNSENT", new JsValue(0), false, false, false);
			jsFunc.FastAddProperty("OPENED", new JsValue(1), false, false, false);
			jsFunc.FastAddProperty("HEADERS_RECEIVED", new JsValue(2), false, false, false);
			jsFunc.FastAddProperty("LOADING", new JsValue(3), false, false, false);
			jsFunc.FastAddProperty("DONE", new JsValue(4),false,false,false );

			_jsEngine.Global.FastAddProperty("XMLHttpRequest", jsFunc, false, false, false);
		}

		private void AddGlobalFunc(string name, Func<JsValue, JsValue[], JsValue> action)
		{
			var jsFunc = new ClrFunctionInstance(_jsEngine, action);
			_jsEngine.Global.DefineOwnProperty(name, new ClrAccessDescriptor(_jsEngine, value => jsFunc), true);
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
