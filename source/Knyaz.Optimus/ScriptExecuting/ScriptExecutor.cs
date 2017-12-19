using System;
using Jint.Native;
using Jint.Runtime;
using Jint.Runtime.Descriptors.Specialized;
using Jint.Runtime.Interop;
using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.Dom.Events;
using Knyaz.Optimus.Dom.Perf;
using Knyaz.Optimus.Dom.Interfaces;

namespace Knyaz.Optimus.ScriptExecuting
{
	internal class ScriptExecutor : IScriptExecutor
	{
		private readonly Engine _engine;

		private Jint.Engine _jsEngine;
		private DomConverter _typeConverter;

		public ScriptExecutor(Engine engine)
		{
			_engine = engine;
			CreateEngine(engine);
		}

		private void CreateEngine(Engine engine)
		{
			_typeConverter = new DomConverter(() => _jsEngine);

			_jsEngine = new Jint.Engine(o => o.AddObjectConverter(_typeConverter));

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

			_jsEngine.Execute("var window = this");
			_jsEngine.Execute("var self = window");

			//Perf types
			AddClrType("ArrayBuffer", typeof(ArrayBuffer));
			AddClrType("Int8Array", typeof(Int8Array));
			AddClrType("Uint8Array", typeof(UInt8Array));
			AddClrType("Int16Array", typeof(Int16Array));
			AddClrType("Uint16Array", typeof(UInt16Array));
			AddClrType("Int32Array", typeof(Int32Array));
			AddClrType("Uint32Array", typeof(UInt32Array));
			AddClrType("Float32Array", typeof(Float32Array));
			AddClrType("Float64Array", typeof(Float64Array));
			AddClrType("DataView", typeof(DataView));

			AddGlobalGetter("console", () => engine.Console);
			AddGlobalGetter("document", () => engine.Document);
			AddGlobalGetter("history", () => engine.Window.History);
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
				x.Length > 2 && ToBoolean(x[2])));

			AddGlobalAct("removeEventListener", (_, x) => engine.Window.RemoveEventListener(
				x.Length > 0 ? x[0].AsString() : null,
				_typeConverter.ConvertDelegate<Event>(x[1]),
				x.Length > 2 && ToBoolean(x[2])));

			AddGlobalFunc("matchMedia", (value, values) =>
			{
				var res = engine.Window.MatchMedia(values[0].AsString());
				return new JsValue(new ClrObject(_jsEngine, res));
			});

			AddGlobalFunc("setTimeout", (_, x) =>
			{
				if (x.Length == 0)
					return JsValue.Undefined;
				var res= engine.Window.SetTimeout(_typeConverter.ConvertDelegate<object>(x[0]), x.Length > 1 ? x[1].AsNumber() : 1, x.Length > 2 ? x[2].ToObject() : null);
				return new JsValue(res);
			});

			AddGlobalFunc("setInterval", (_, x) =>
			{
				if (x.Length == 0)
					return JsValue.Undefined;
				var res = engine.Window.SetInterval(_typeConverter.ConvertDelegate(x[0]), x.Length > 1 ? x[1].AsNumber() : 1);
				return new JsValue(res);
			});

			AddGlobalFunc("getComputedStyle", (value, values) =>
			{
				var elt = (ClrObject) values[0].AsObject();
				var res = engine.Window.GetComputedStyle((IElement)elt.Target, values.Length > 1 ? values[1].TryCast<string>() : null);
				return new JsValue(new ClrObject(_jsEngine, res));
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

		private bool ToBoolean(JsValue x)
		{
			if (x.Type == Types.Boolean)
				return x.AsBoolean();

			if (x.Type == Types.Number)
				return x.AsNumber() != 0;

			return x.AsObject() != null;
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

		public object Evaluate(string type, string code)
		{
			if (string.IsNullOrEmpty(type) || type.ToLowerInvariant() == "text/javascript")
			{
				try
				{
					return _jsEngine.Execute(code).GetCompletionValue().ToObject();
				}
				catch (JavaScriptException e)
				{
					return new ScriptExecutingException(e.Error.ToString(), e, code);
				}
			}

			throw new Exception("Unsupported script type: " + type);
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
