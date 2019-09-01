using System;
using System.Collections.Generic;
using System.IO;
using Jint.Runtime;
using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.Dom.Events;
using Knyaz.Optimus.Dom.Perf;
using Knyaz.Optimus.Dom.Interfaces;
using System.Linq;
using System.Runtime.CompilerServices;
using Knyaz.Optimus.Tools;

namespace Knyaz.Optimus.ScriptExecuting
{
	internal class ScriptExecutor : IScriptExecutor
	{
		private readonly IWindowEx _window;
		private readonly Func<Func<Stream, object>, XmlHttpRequest> _createXmlHttpRequest;

		private IJsEngine _jsEngine;
		
		public ScriptExecutor(IWindowEx window, Func<Func<Stream, object>, XmlHttpRequest> createXmlHttpRequest)
		{
			_window = window ?? throw new ArgumentNullException(nameof(window));
			_createXmlHttpRequest = createXmlHttpRequest;
			CreateEngine(window, createXmlHttpRequest);
		}

		private void CreateEngine(IWindowEx window, Func<Func<Stream, object>, XmlHttpRequest> createXmlHttpRequest)
		{
			_jsEngine = new JintJsEngine();
			_jsEngine.Execute("var window = this");
			_jsEngine.Execute("var self = window");
			_jsEngine.AddGlobalType("Node", typeof(Node));
			_jsEngine.AddGlobalType("Element", typeof(Element));
			_jsEngine.AddGlobalType("HTMLBodyElement", typeof(HtmlBodyElement));
			_jsEngine.AddGlobalType("HTMLButtonElement", typeof(HtmlButtonElement));
			_jsEngine.AddGlobalType("HTMLDivElement", typeof(HtmlDivElement));
			_jsEngine.AddGlobalType("HTMLElement", typeof(HtmlElement));
			_jsEngine.AddGlobalType("HTMLIFrameElement", typeof(HtmlIFrameElement));
			_jsEngine.AddGlobalType("HTMLInputElement", typeof(HtmlInputElement));
			_jsEngine.AddGlobalType("HTMLTextAreaElement", typeof(HtmlTextAreaElement));
			_jsEngine.AddGlobalType("HTMLUnknownElement", typeof(HtmlUnknownElement));
			_jsEngine.AddGlobalType("HTMLFormElement", typeof(HtmlFormElement));
			_jsEngine.AddGlobalType("HTMLHtmlElement", typeof(HtmlHtmlElement));
			_jsEngine.AddGlobalType("Script", typeof(Script));
			_jsEngine.AddGlobalType("Comment", typeof(Comment));
			_jsEngine.AddGlobalType("Document", typeof(Document));
			_jsEngine.AddGlobalType("Text", typeof(Text));
			_jsEngine.AddGlobalType("Attr", typeof(Attr));
			//Perf types
			_jsEngine.AddGlobalType("ArrayBuffer", typeof(ArrayBuffer));
			_jsEngine.AddGlobalType("Int8Array", typeof(Int8Array));
			_jsEngine.AddGlobalType("Uint8Array", typeof(UInt8Array));
			_jsEngine.AddGlobalType("Int16Array", typeof(Int16Array));
			_jsEngine.AddGlobalType("Uint16Array", typeof(UInt16Array));
			_jsEngine.AddGlobalType("Int32Array", typeof(Int32Array));
			_jsEngine.AddGlobalType("Uint32Array", typeof(UInt32Array));
			_jsEngine.AddGlobalType("Float32Array", typeof(Float32Array));
			_jsEngine.AddGlobalType("Float64Array", typeof(Float64Array));
			_jsEngine.AddGlobalType("DataView", typeof(DataView));

			_jsEngine.AddGlobalGetter("console", () => window.Console);
			_jsEngine.AddGlobalGetter("document", () => window.Document);
			_jsEngine.AddGlobalGetter("history", () => window.History);
			_jsEngine.AddGlobalGetter("location", () => window.Location);
			_jsEngine.AddGlobalGetter("sessionStorage", () => window.SessionStorage);
			_jsEngine.AddGlobalGetter("localStorage", () => window.LocalStorage);
			_jsEngine.AddGlobalGetter("navigator", () => window.Navigator);
			_jsEngine.AddGlobalGetter("screen", () => window.Screen);
			_jsEngine.AddGlobalGetter("innerWidth", () => window.InnerWidth);
			_jsEngine.AddGlobalGetter("innerHeight", () => window.InnerHeight);
			
			_jsEngine.AddGlobalAct("alert", args => window.Alert(args[0]?.ToString()));

			_jsEngine.AddGlobalAct("open", values =>
			{
				if (values.Length == 0)
					window.Open();
				else if (values.Length == 1)
					window.Open(values[0]?.ToString());
				else if (values.Length == 2)
					window.Open(values[0]?.ToString(), values[1]?.ToString());
				else
					window.Open(values[0]?.ToString(), values[1]?.ToString(), values[2]?.ToString());
			});
			
			_jsEngine.AddGlobalAct("clearInterval", args => window.ClearInterval(args.Length > 0 ? Convert.ToInt32(args[0]) : -1));
			_jsEngine.AddGlobalAct("clearTimeout", args => window.ClearTimeout(args.Length > 0 ? Convert.ToInt32(args[0]) : -1));
			_jsEngine.AddGlobalAct("dispatchEvent", args => window.DispatchEvent(args.Length > 0 ? (Event)args[0] : null));
			
			
			var listenersHandlers = new ConditionalWeakTable<Action<object[]>, Action<Event>>();

			_jsEngine.AddGlobalAct("addEventListener", args =>
			{
				if (args.Length < 2)
					return;
				
				var jsHandler = (Action<object[]>)args[1];

				Action<Event> handler;
				
				if (jsHandler == null)
				{
					handler = null;
				} 
				else if (!listenersHandlers.TryGetValue(jsHandler, out handler))
				{
					handler = @event => jsHandler(new object[] {@event});
					listenersHandlers.Add(jsHandler, handler);
				}

				if (args.Length <= 2)
				{
					window.AddEventListener(
						args.Length > 0 ? args[0]?.ToString() : null,
						handler,
						false);
					return;
				}
				

				if (args[2] is IDictionary<string, object> options)
				{
					window.AddEventListener(
						args[0]?.ToString(),
						handler,
						new EventListenerOptions {
							Capture = options["capture"] is bool c && c,
							Passive = options["passive"] is bool p && p,
							Once = options["once"] is bool o && o
						});
				}
				else
				{
					window.AddEventListener(
						args[0]?.ToString(),
						handler,
						TryBool(args[2])); 
				}
			});

			_jsEngine.AddGlobalAct("removeEventListener", args =>
			{
				if (args.Length < 2)
					return;
				
				var jsHandler = (Action<object[]>)args[1];

				Action<Event> handler;
				
				if (jsHandler == null)
				{
					handler = null;
				} 
				else if (!listenersHandlers.TryGetValue(jsHandler, out handler))
				{
					handler = @event => jsHandler(new object[] {@event});
					listenersHandlers.Add(jsHandler, handler);
				}
				
				if (args.Length <= 2)
				{
					window.RemoveEventListener(
						args.Length > 0 ? args[0]?.ToString() : null,
						handler,
						false);
					return;
				}
				
				if (args[2] is IDictionary<string, object> options)
				{
					window.RemoveEventListener(
						args[0]?.ToString(),
						handler,
						new EventListenerOptions {
							Capture = options["capture"] is bool c && c,
							Passive = options["passive"] is bool p && p,
							Once = options["once"] is bool o && o
						});
				}
				else
				{
					window.RemoveEventListener(
						args[0]?.ToString(),
						handler,
						TryBool(args[2])); 
				}
			});

			_jsEngine.AddGlobalFunc("matchMedia", args => window.MatchMedia(args[0]?.ToString()));

			_jsEngine.AddGlobalFunc("setTimeout", args =>
			{
				if (args.Length == 0)
					return Undefined.Instance;
				var handler = (Action<object[]>)args[0];
				var timeout = args.Length > 1 ? Convert.ToDouble(args[1]) : 1d;
				var data = args.Length > 2 ? args.Skip(2).ToArray() : null;
				return (object)window.SetTimeout(handler, timeout, data);
			});

			_jsEngine.AddGlobalFunc("setInterval", args =>
			{
				if (args.Length == 0)
					return Undefined.Instance;
				var handler = (Action<object[]>)args[0];
				var interval = args.Length > 1 ? Convert.ToDouble(args[1]) : 1d;
				var data = args.Length > 2 ? args.Skip(2).ToArray() : null;
				return (object)window.SetInterval(handler, interval, data);
			});

			_jsEngine.AddGlobalFunc("getComputedStyle", args => 
				window.GetComputedStyle((IElement)args[0], args.Length > 1 ? args[1]?.ToString() : null));

			_jsEngine.AddGlobalType("Event", args =>
			{
				var evt = _window.Document.CreateEvent("Event");
				var opts = args.Length > 1 ? args[1].AsObject() : null;
				var canCancel = opts != null && !opts.Get("cancelable").IsUndefined() &&
				                opts.Get("cancelable").AsBoolean();
				var canBubble = opts != null && !opts.Get("bubbles").IsUndefined() && opts.Get("bubbles").AsBoolean();
				evt.InitEvent(args[0].AsString(), canBubble, canCancel);
				return evt;
			});
			
			_jsEngine.AddGlobalType("Image", args => {
				var img = (HtmlImageElement)_window.Document.CreateElement("img");
				
				if (args.Length > 0)
					img.Width = (int)args[0].AsNumber();
				
				if(args.Length > 1)
					img.Height = (int)args[1].AsNumber();

				return img;
			});
			
			Func<Stream, object> parseJsonFn = s => _jsEngine.ParseJson(s.ReadToEnd());

			_jsEngine.AddGlobalType("XMLHttpRequest", x => createXmlHttpRequest(parseJsonFn));
			
		}

		private bool TryBool(object o)
		{
			if (o is bool b)
				return b;

			if (o is double d)
				return d > 0;

			return o != null && !(o is Undefined);
		}

		public void Execute(string type, string code)
		{
			if (code == null) //if error occurred on script loading.
				return;

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
					return _jsEngine.Evaluate(code);
				}
				catch (JavaScriptException e)
				{
					return new ScriptExecutingException(e.Error.ToString(), e, code);
				}
			}

			throw new Exception("Unsupported script type: " + type);
		}

		public event Action<Exception> OnException;
		public void Clear() => CreateEngine(_window, _createXmlHttpRequest);

		public object EvalFuncAndCall(string code, object @this, params object[] args)
		{
			var funcCode = "(function(){ return "+code+";})()";

			var func = Evaluate("text/javascript", funcCode) as Func<object, object[], object>;

			try
			{
				return func(@this, args);
			}
			catch (JavaScriptException e)
			{
				OnException?.Invoke(new ScriptExecutingException(e.Error.ToString(), e, code));
			}
			catch (Exception e)
			{
				OnException?.Invoke(e);
			}
			return null;
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
