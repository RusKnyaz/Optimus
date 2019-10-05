using System;
using System.IO;
using Jurassic;
using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.Dom.Events;
using Knyaz.Optimus.Dom.Interfaces;
using Knyaz.Optimus.Dom.Perf;
using Knyaz.Optimus.ScriptExecuting;
using Knyaz.Optimus.Tools;

namespace Knyaz.Optimus.Scripting.Jurassic
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
			_jsEngine = new JurassicJsEngine();
			_jsEngine.Execute("var window = this");
			_jsEngine.Execute("var self = window");
			_jsEngine.AddGlobalType(typeof(Node));
			_jsEngine.AddGlobalType(typeof(Element));
			_jsEngine.AddGlobalType(typeof(HtmlBodyElement));
			_jsEngine.AddGlobalType(typeof(HtmlButtonElement));
			_jsEngine.AddGlobalType(typeof(HtmlDivElement));
			_jsEngine.AddGlobalType(typeof(HtmlElement));
			_jsEngine.AddGlobalType(typeof(HtmlIFrameElement));
			_jsEngine.AddGlobalType(typeof(HtmlInputElement));
			_jsEngine.AddGlobalType(typeof(HtmlTextAreaElement));
			_jsEngine.AddGlobalType(typeof(HtmlUnknownElement));
			_jsEngine.AddGlobalType(typeof(HtmlFormElement));
			_jsEngine.AddGlobalType(typeof(HtmlHtmlElement));
			_jsEngine.AddGlobalType(typeof(Script));
			_jsEngine.AddGlobalType(typeof(Comment));
			_jsEngine.AddGlobalType(typeof(Document));
			_jsEngine.AddGlobalType(typeof(Text));
			_jsEngine.AddGlobalType(typeof(Attr));
			_jsEngine.AddGlobalType(typeof(ArrayBuffer));
			_jsEngine.AddGlobalType(typeof(Int8Array));
			_jsEngine.AddGlobalType(typeof(UInt8Array));
			_jsEngine.AddGlobalType(typeof(Int16Array));
			_jsEngine.AddGlobalType(typeof(UInt16Array));
			_jsEngine.AddGlobalType(typeof(Int32Array));
			_jsEngine.AddGlobalType(typeof(UInt32Array));
			_jsEngine.AddGlobalType(typeof(Float32Array));
			_jsEngine.AddGlobalType(typeof(Float64Array));
			_jsEngine.AddGlobalType(typeof(DataView));
			
			_jsEngine.SetGlobal(window);


			_jsEngine.AddGlobalType(typeof(Event),"Event", new []{typeof(string),typeof(EventInitOptions)}, 
				args => new Event(window.Document, args[0]?.ToString(), args.Length > 1 ? (EventInitOptions)args[1] : null));
			
			_jsEngine.AddGlobalType(typeof(Event), "Image", new []{typeof(int), typeof(int)}, args => {
				var img = (HtmlImageElement)_window.Document.CreateElement("img");
				
				if (args.Length > 0)
					img.Width = Convert.ToInt32(args[0]);
				
				if(args.Length > 1)
					img.Height = Convert.ToInt32(args[1]);

				return img;
			});
			
			Func<Stream, object> parseJsonFn = s => _jsEngine.ParseJson(s.ReadToEnd());

			_jsEngine.AddGlobalType("XMLHttpRequest", typeof(XmlHttpRequest), x => createXmlHttpRequest(parseJsonFn));
			
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
					var ex = new ScriptExecutingException(e.Message, e, code);
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
					return new ScriptExecutingException(e.Message, e, code);
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
				OnException?.Invoke(new ScriptExecutingException(e.Message, e, code));
			}
			catch (Exception e)
			{
				OnException?.Invoke(e);
			}
			return null;
		}
	}
}