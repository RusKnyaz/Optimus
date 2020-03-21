using System;
using System.IO;
using Jint.Runtime;
using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.Dom.Events;
using Knyaz.Optimus.Dom.Interfaces;
using Knyaz.Optimus.Tools;

namespace Knyaz.Optimus.ScriptExecuting.Jint
{
	/// <summary>
	/// Facade for the script execution engine based on Jint.
	/// </summary>
	internal class JintJsScriptExecutor : IJsScriptExecutor
	{
		private readonly IWindowEx _window;

		private JintJsEngine _jsEngine;
		
		public JintJsScriptExecutor(IWindowEx window, Func<Func<Stream, object>, XmlHttpRequest> createXmlHttpRequest)
		{
			_window = window ?? throw new ArgumentNullException(nameof(window));
			CreateEngine(window, createXmlHttpRequest);
		}

		private void CreateEngine(IWindowEx window, Func<Func<Stream, object>, XmlHttpRequest> createXmlHttpRequest)
		{
			_jsEngine = new JintJsEngine(window);
			
			_jsEngine.Execute("var window = this");
			_jsEngine.Execute("var self = window");

			foreach (var type in ScriptingSettings.Default.GlobalTypes)
			{
				_jsEngine.AddGlobalType(type);
			}
			
			_jsEngine.AddGlobalType<Event>(new []{typeof(string),typeof(EventInitOptions)}, 
				args => new Event(window.Document, args[0]?.ToString(), args.Length > 1 ? (EventInitOptions)args[1] : null));
			
			_jsEngine.AddGlobalType<Image>(new []{typeof(int), typeof(int)}, args => {
				var img = (HtmlImageElement)_window.Document.CreateElement("img");
				
				if (args.Length > 0)
					img.Width = Convert.ToInt32(args[0]);
				
				if(args.Length > 1)
					img.Height = Convert.ToInt32(args[1]);

				return img;
			});
			
			Func<Stream, object> parseJsonFn = s => _jsEngine.ParseJson(s.ReadToEnd());

			_jsEngine.AddGlobalType("XMLHttpRequest", x => createXmlHttpRequest(parseJsonFn));
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
				throw new ScriptExecutingException(e.Error.ToString(), e, code);
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
				return new ScriptExecutingException(e.Error.ToString(), e, code);
			}
		}
	}
}