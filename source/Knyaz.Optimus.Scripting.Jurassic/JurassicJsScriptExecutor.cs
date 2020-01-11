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
	/// <summary>
	/// Facade for the script execution engine based on Jint.
	/// </summary>
    internal class JurassicJsScriptExecutor : IJsScriptExecutor
	{
		private readonly IJsEngine _jsEngine;
		
		public JurassicJsScriptExecutor(IWindowEx window, Func<Func<Stream, object>, XmlHttpRequest> createXmlHttpRequest) =>
			_jsEngine = CreateEngine(window, createXmlHttpRequest);

		private static JurassicJsEngine CreateEngine(IWindowEx window, Func<Func<Stream, object>, XmlHttpRequest> createXmlHttpRequest)
		{
			var jsEngine = new JurassicJsEngine();
			jsEngine.Execute("var window = this");
			jsEngine.Execute("var self = window");
			jsEngine.AddGlobalType(typeof(Node));
			jsEngine.AddGlobalType(typeof(Element));
			jsEngine.AddGlobalType(typeof(HtmlBodyElement));
			jsEngine.AddGlobalType(typeof(HtmlButtonElement));
			jsEngine.AddGlobalType(typeof(HtmlBodyElement));
			jsEngine.AddGlobalType(typeof(HtmlButtonElement));
			jsEngine.AddGlobalType(typeof(HtmlBrElement));
			jsEngine.AddGlobalType(typeof(HtmlDivElement));
			jsEngine.AddGlobalType(typeof(HtmlElement));
			jsEngine.AddGlobalType(typeof(HtmlFormElement));
			jsEngine.AddGlobalType(typeof(HtmlIFrameElement));
			jsEngine.AddGlobalType(typeof(HtmlImageElement));
			jsEngine.AddGlobalType(typeof(HtmlInputElement));
			jsEngine.AddGlobalType(typeof(HtmlLabelElement));
			jsEngine.AddGlobalType(typeof(HtmlLinkElement));
			jsEngine.AddGlobalType(typeof(HtmlOptGroupElement));
			jsEngine.AddGlobalType(typeof(HtmlOptionElement));
			jsEngine.AddGlobalType(typeof(HtmlSelectElement));
			jsEngine.AddGlobalType(typeof(HtmlStyleElement));
			jsEngine.AddGlobalType(typeof(HtmlTextAreaElement));
			jsEngine.AddGlobalType(typeof(HtmlTableElement));
			jsEngine.AddGlobalType(typeof(HtmlTableRowElement));
			jsEngine.AddGlobalType(typeof(HtmlTableColElement));
			jsEngine.AddGlobalType(typeof(HtmlTableCellElement));
			jsEngine.AddGlobalType(typeof(HtmlTableCaptionElement));
			jsEngine.AddGlobalType(typeof(HtmlTableSectionElement));
			jsEngine.AddGlobalType(typeof(HtmlUnknownElement));
			jsEngine.AddGlobalType(typeof(HtmlHtmlElement));
			jsEngine.AddGlobalType(typeof(Script));
			jsEngine.AddGlobalType(typeof(Comment));
			jsEngine.AddGlobalType(typeof(Document));
			jsEngine.AddGlobalType(typeof(Text));
			jsEngine.AddGlobalType(typeof(Attr));
			jsEngine.AddGlobalType(typeof(ArrayBuffer));
			jsEngine.AddGlobalType(typeof(Int8Array));
			jsEngine.AddGlobalType(typeof(UInt8Array));
			jsEngine.AddGlobalType(typeof(Int16Array));
			jsEngine.AddGlobalType(typeof(UInt16Array));
			jsEngine.AddGlobalType(typeof(Int32Array));
			jsEngine.AddGlobalType(typeof(UInt32Array));
			jsEngine.AddGlobalType(typeof(Float32Array));
			jsEngine.AddGlobalType(typeof(Float64Array));
			jsEngine.AddGlobalType(typeof(DataView));
			
			jsEngine.SetGlobal(window);


			jsEngine.AddGlobalType(typeof(Event),"Event", new []{typeof(string),typeof(EventInitOptions)}, 
				args => new Event(window.Document, args[0]?.ToString(), args.Length > 1 ? (EventInitOptions)args[1] : null));
			
			jsEngine.AddGlobalType(typeof(Event), "Image", new []{typeof(int), typeof(int)}, args => {
				var img = (HtmlImageElement)window.Document.CreateElement("img");
				
				if (args.Length > 0)
					img.Width = Convert.ToInt32(args[0]);
				
				if(args.Length > 1)
					img.Height = Convert.ToInt32(args[1]);

				return img;
			});
			
			Func<Stream, object> parseJsonFn = s => jsEngine.ParseJson(s.ReadToEnd());

			jsEngine.AddGlobalType("XMLHttpRequest", typeof(XmlHttpRequest), x => createXmlHttpRequest(parseJsonFn));

			return jsEngine;
		}

		public void Execute(string code)
		{
			if (code == null) //if error occurred on script loading.
				return;

			try
			{
				_jsEngine.Execute(code);
			}
			catch (JavaScriptException e)
			{
				throw new ScriptExecutingException(e.Message, e, code);
			}
		}

		public object Evaluate(string code)
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
	}
}