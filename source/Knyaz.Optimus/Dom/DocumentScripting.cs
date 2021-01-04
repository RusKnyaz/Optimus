using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.Dom.Events;
using Knyaz.Optimus.ResourceProviders;
using Knyaz.Optimus.ScriptExecuting;
using Knyaz.Optimus.Dom.Interfaces;
using Knyaz.Optimus.Tools;
using System.IO;

namespace Knyaz.Optimus.Dom
{
	/// <summary>
	/// Executes scripts for the Document in proper order.
	/// </summary>
	public class DocumentScripting : IDisposable
	{
		private readonly Queue<Tuple<Task<string>, HtmlScriptElement>> _deferredScripts = 
			new Queue<Tuple<Task<string>, HtmlScriptElement>>();
		private readonly IDocument _document;
		private readonly IScriptExecutor _scriptExecutor;
		private readonly Func<string, Task<IResource>> _getResourceAsyncFn;

		internal DocumentScripting (
			Document document, 
			IScriptExecutor scriptExecutor,
			Func<string,Task<IResource>> getResourceAsyncFn)
		{
			_document = document;
			_scriptExecutor = scriptExecutor;
			_getResourceAsyncFn = getResourceAsyncFn;
			document.NodeInserted += OnDocumentNodeInserted;
			document.DomContentLoaded += OnDocumentDomContentLoaded;
			document.OnHandleNodeScript += OnHandleNodeScript;
		}
		
		private void OnHandleNodeScript(Event evt, string handlerCode)
		{
			//the code in the 'href' attribute of the anchor element have to be executed asynchronously
			var async = evt.Type == "click" && evt.CurrentTarget is HtmlAnchorElement anchor &&
				string.IsNullOrEmpty(anchor.GetAttribute("onclick"));

			handlerCode = handlerCode.Trim();
			if (handlerCode.Last() != ';')
				handlerCode += ";";

			if (async)
			{
				Task.Run(
					() =>
					{
						lock(_document)
							return _scriptExecutor.EvalFuncAndCall("function (event){" + handlerCode + "}", evt.Target, evt);
					});
			}
			else
			{
				_scriptExecutor.EvalFuncAndCall("function (event){" + handlerCode + "}", evt.Target, evt);	
			}
		}

		void OnDocumentNodeInserted (Node node)
		{
			if (!node.IsInDocument ())
				return;

			if (node is Attr)
				return;

			//Prevent 'Collection was modified' exception.
			var tmpChildNodes = node.Flatten().OfType<HtmlElement>().ToArray();
			
			foreach (var script in tmpChildNodes.OfType<HtmlScriptElement>())
			{
				var remote = IsExternalScript(script);
				var async = script.Async && remote || script.Source == NodeSources.Script;
				var defer = script.Defer && remote && !async && script.Source == NodeSources.DocumentBuilder;

				if (defer)
				{
					_deferredScripts.Enqueue(new Tuple<Task<string>, HtmlScriptElement>(LoadAsync(script, _getResourceAsyncFn), script));
				}
				else if (remote) //script that have to be loaded
				{
					var task = 
						LoadAsync(script, _getResourceAsyncFn)
						.ContinueWith(t => ExecuteScript(new ScriptInfo(script, t.Result)));

					if (!async)
						task.Wait();
				}
				else if (!string.IsNullOrEmpty(script.Text) && script.Type == "text/javascript" || string.IsNullOrEmpty(script.Type))
				{
					ExecuteScript(new ScriptInfo(script, script.Text));
				}
			}
		}

		//todo: revise it. it shouldn't be here.
		internal static async Task<string> LoadAsync(HtmlScriptElement script, Func<string, Task<IResource>> getResourceAsyncFn)
		{
			if (string.IsNullOrEmpty(script.Src))
				throw new InvalidOperationException("Src not set.");

			var resource = await getResourceAsyncFn(script.Src);

			try
			{
				using (var reader = new StreamReader(resource.Stream))
					return reader.ReadToEnd();
			}
			catch
			{
				lock (script.OwnerDocument)
				{
					script.RaiseEvent("error", false, false);
				}
			}

			return null;
		}

		void OnDocumentDomContentLoaded (IDocument document)
		{
			//Execute deferred scripts
			while (_deferredScripts.Count > 0)
			{
				var scriptTask = _deferredScripts.Dequeue();
				ExecuteScript(new ScriptInfo(scriptTask.Item2, scriptTask.Item1.Result));
			}
		}

		private void ExecuteScript(ScriptInfo script)
		{
			if (script.Node.Executed)
				return;

			lock (script.Node.OwnerDocument)
			{
				RaiseBeforeScriptExecute(script.Node);

				try
				{
					_scriptExecutor.Execute(
						script.Node.Type ?? "text/javascript", 
						script.Code);
					script.Node.Executed = true;
					if (IsExternalScript(script.Node))
						script.Node.RaiseEvent("load", true, false);
				}
				catch (Exception ex)
				{
					RaiseScriptExecutionError(script.Node, ex);
				}

				RaiseAfterScriptExecute(script.Node);
			}
		}

		private void RaiseScriptExecutionError(HtmlScriptElement script, Exception ex)
		{
			ScriptExecutionError?.Invoke(script, ex);

			var evt = (ErrorEvent)script.OwnerDocument.CreateEvent("ErrorEvent");
			evt.ErrorEventInit(ex.Message, script.Src ?? "...", 0, 0, ex);
			evt.Target = script;
			script.OwnerDocument.DispatchEvent(evt);
		}

		private void RaiseAfterScriptExecute(HtmlScriptElement script)
		{
			AfterScriptExecute?.Invoke(script);

			var evt = script.OwnerDocument.CreateEvent("Event");
			evt.InitEvent("AfterScriptExecute",true, false);
			script.DispatchEvent(evt);
		}

		private void RaiseBeforeScriptExecute(HtmlScriptElement script)
		{
			BeforeScriptExecute?.Invoke(script);

			var evt = script.OwnerDocument.CreateEvent("Event");
			evt.InitEvent("BeforeScriptExecute", true, false);
			script.DispatchEvent(evt);
		}

		/// <summary>
		/// Raised before running the script.
		/// </summary>
		public event Action<HtmlScriptElement> BeforeScriptExecute;
		
		/// <summary>
		/// Raised after running the script.
		/// </summary>
		public event Action<HtmlScriptElement> AfterScriptExecute;
		
		/// <summary>
		/// Raised on script execution error.
		/// </summary>
		public event Action<HtmlScriptElement, Exception> ScriptExecutionError;

		#region IDisposable implementation

		public void Dispose ()
		{
			_document.NodeInserted -= OnDocumentNodeInserted;
			_document.DomContentLoaded -= OnDocumentDomContentLoaded;
		}

		#endregion

		class ScriptInfo
		{
			public readonly HtmlScriptElement Node;
			public readonly string Code;

			public ScriptInfo(HtmlScriptElement node, string code)
			{
				Node = node;
				Code = code;
			}
		}
		
		private static bool IsExternalScript(HtmlScriptElement script) => !string.IsNullOrEmpty(script.Src);
	}
}

