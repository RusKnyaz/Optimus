using System;
using WebBrowser.Dom;
using WebBrowser.ScriptExecuting;
using WebBrowser.Dom.Elements;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebBrowser.Dom.Events;

namespace WebBrowser
{
	/// <summary>
	/// Executes scripts for the Document in proper order.
	/// </summary>
	public class DocumentScripting : IDisposable
	{
		Queue<Tuple<Task, Script>> _unresolvedDelayedResources;
		private readonly IResourceProvider _resourceProvider;

		IDocument _document;

		IScriptExecutor _scriptExecutor;

		public DocumentScripting (
			IDocument document, 
			IScriptExecutor scriptExecutor,
			IResourceProvider resourceProvider)
		{
			_document = document;
			_scriptExecutor = scriptExecutor;
			_resourceProvider = resourceProvider;
			document.NodeInserted += OnDocumentNodeInserted;
			document.DomContentLoaded += OnDocumentDomContentLoaded;
			_unresolvedDelayedResources = new Queue<Tuple<Task, Script>>();
		}

		void OnDocumentNodeInserted (Node node)
		{
			if (!node.IsInDocument ())
				return;

			foreach (var script in node.Flatten().OfType<Script>())
			{
				var remote = script.HasDelayedContent;
				var async = script.Async && remote || script.Source == NodeSources.Script;
				var defer = script.Defer && remote;

				if (!async && defer && node.OwnerDocument.ReadyState != DocumentReadyStates.Complete)
				{
					_unresolvedDelayedResources.Enqueue(new Tuple<Task, Script>(script.LoadAsync(_resourceProvider), script));
				}
				else
				{
					if (remote)
					{
						var task = script
							.LoadAsync(_resourceProvider)
							.ContinueWith((t, s) => ExecuteScript((Script) s), script);

						if (!async)
							task.Wait();
					}
					else if (!string.IsNullOrEmpty(script.Text))
					{
						ExecuteScript(script);
					}
				}
			}
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

		internal void RunScripts(IEnumerable<Script> scripts)
		{
			//todo: what we should do if some script changes ChildNodes?
			//todo: optimize (create queue of not executed scripts);
			foreach (var script in scripts.ToArray())
			{
				if (script.Executed || string.IsNullOrEmpty(script.Text)) continue;
				ExecuteScript(script);
			}
		}

		private void ExecuteScript(Script script)
		{
			lock (script.OwnerDocument)
			{
				RaiseBeforeScriptExecute(script);

				try
				{
					script.Execute(_scriptExecutor);
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
			if (ScriptExecutionError != null)
				ScriptExecutionError(script, ex);

			var evt = (ErrorEvent)script.OwnerDocument.CreateEvent("ErrorEvent");
			evt.ErrorEventInit(ex.Message, script.Src ?? "...", 0, 0, ex);
			evt.Target = script;
			script.OwnerDocument.DispatchEvent(evt);
		}

		private void RaiseAfterScriptExecute(Script script)
		{
			if (AfterScriptExecute != null)
				AfterScriptExecute(script);

			var evt = script.OwnerDocument.CreateEvent("Event");
			evt.InitEvent("AfterScriptExecute",false, false);
			evt.Target = script;
			script.OwnerDocument.DispatchEvent(evt);
		}

		private void RaiseBeforeScriptExecute(Script script)
		{
			if (BeforeScriptExecute != null)
				BeforeScriptExecute(script);

			var evt = script.OwnerDocument.CreateEvent("Event");
			evt.InitEvent("BeforeScriptExecute", false, false);
			evt.Target = script;
			script.OwnerDocument.DispatchEvent(evt);
		}

		public event Action<Script> BeforeScriptExecute;
		public event Action<Script> AfterScriptExecute;
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

