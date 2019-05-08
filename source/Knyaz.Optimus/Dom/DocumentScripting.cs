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
		private readonly Queue<Tuple<Task, Script>> _unresolvedDelayedResources;
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
			_unresolvedDelayedResources = new Queue<Tuple<Task, Script>>();
		}

		private void OnHandleNodeScript(Event evt, string handlerCode)
		{
			handlerCode = handlerCode.Trim();
			if (handlerCode.Last() != ';')
				handlerCode += ";";
			
			_scriptExecutor.EvalFuncAndCall("function (event){" + handlerCode + "}", evt.Target, evt);
		}

		void OnDocumentNodeInserted (Node node)
		{
			if (!node.IsInDocument ())
				return;

			if (node is Attr)
				return;

			//Prevent 'Collection was modified' exception.
			var tmpChildNodes = node.Flatten().OfType<HtmlElement>().ToArray();
			
			foreach (var elt in tmpChildNodes)
			{
				if (elt is Script script)
				{
					var remote = script.IsExternalScript;
					var async = script.Async && remote || script.Source == NodeSources.Script;
					var defer = script.Defer && remote && !async && script.Source == NodeSources.DocumentBuilder;

					if (defer)
					{
						_unresolvedDelayedResources.Enqueue(new Tuple<Task, Script>(LoadAsync(script, _getResourceAsyncFn), script));
					}
					else if (remote)
					{
						var task = 
							LoadAsync(script, _getResourceAsyncFn)
							.ContinueWith((t, s) => ExecuteScript((Script) s), script);

						if (!async)
							task.Wait();
					}
					else if (!string.IsNullOrEmpty(script.Text) && script.Type == "text/javascript" || string.IsNullOrEmpty(script.Type))
					{
						ExecuteScript(script);
					}
				}
			}
		}

		//todo: revise it. it shouldn't be here.
		internal static Task LoadAsync(Script script, Func<string, Task<IResource>> getResourceAsyncFn)
		{
			if (string.IsNullOrEmpty(script.Src))
				throw new InvalidOperationException("Src not set.");

			return getResourceAsyncFn(script.Src).ContinueWith(
				resource =>
				{
					try
					{
						using (var reader = new StreamReader(resource.Result.Stream))
						{
							script.Code = reader.ReadToEnd();//wrong.
						}
					}
					catch
					{
						lock (script.OwnerDocument)
						{
							script.RaiseEvent("error", false, false);
						}
					}
				});
		}

		void OnDocumentDomContentLoaded (IDocument document)
		{
			while (_unresolvedDelayedResources.Count > 0)
			{
				var scriptTask = _unresolvedDelayedResources.Dequeue();
				scriptTask.Item1.Wait();
				ExecuteScript(scriptTask.Item2);
			}
		}

		private void ExecuteScript(Script script)
		{
			if (script.Executed)
				return;

			lock (script.OwnerDocument)
			{
				RaiseBeforeScriptExecute(script);

				try
				{
					_scriptExecutor.Execute(script.Type ?? "text/javascript", 
						script.IsExternalScript ? script.Code : script.Text);
					script.Executed = true;
					if (script.IsExternalScript)
						script.RaiseEvent("load", true, false);
				}
				catch (Exception ex)
				{
					RaiseScriptExecutionError(script, ex);
				}

				RaiseAfterScriptExecute(script);
			}
		}

		private void RaiseScriptExecutionError(Script script, Exception ex)
		{
			ScriptExecutionError?.Invoke(script, ex);

			var evt = (ErrorEvent)script.OwnerDocument.CreateEvent("ErrorEvent");
			evt.ErrorEventInit(ex.Message, script.Src ?? "...", 0, 0, ex);
			evt.Target = script;
			script.OwnerDocument.DispatchEvent(evt);
		}

		private void RaiseAfterScriptExecute(Script script)
		{
			AfterScriptExecute?.Invoke(script);

			var evt = script.OwnerDocument.CreateEvent("Event");
			evt.InitEvent("AfterScriptExecute",true, false);
			script.DispatchEvent(evt);
		}

		private void RaiseBeforeScriptExecute(Script script)
		{
			BeforeScriptExecute?.Invoke(script);

			var evt = script.OwnerDocument.CreateEvent("Event");
			evt.InitEvent("BeforeScriptExecute", true, false);
			script.DispatchEvent(evt);
		}

		/// <summary>
		/// Raised before running the script.
		/// </summary>
		public event Action<Script> BeforeScriptExecute;
		
		/// <summary>
		/// Raised after running the script.
		/// </summary>
		public event Action<Script> AfterScriptExecute;
		
		/// <summary>
		/// Raised on script execution error.
		/// </summary>
		public event Action<Script, Exception> ScriptExecutionError;

		#region IDisposable implementation

		public void Dispose ()
		{
			_document.NodeInserted -= OnDocumentNodeInserted;
			_document.DomContentLoaded -= OnDocumentDomContentLoaded;
		}

		#endregion
	}
}

