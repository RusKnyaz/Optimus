using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebBrowser.Dom.Elements;
using WebBrowser.Dom.Events;
using WebBrowser.ScriptExecuting;

namespace WebBrowser.Dom
{
	/// <summary>
	/// Executes scripts for the Document in proper order.
	/// </summary>
	public class DocumentScripting : IDisposable
	{
		private readonly Queue<Tuple<Task, Script>> _unresolvedDelayedResources;
		private readonly IResourceProvider _resourceProvider;
		private readonly IDocument _document;
		private readonly IScriptExecutor _scriptExecutor;

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

			var attr = node as Attr;
			if (attr != null)
			{
				RegisterAttr(attr);

				return;
			}

			foreach (var elt in node.Flatten().OfType<HtmlElement>())
			{
				foreach (var attribute in elt.Attributes)
				{
					RegisterAttr(attribute);
				}

				var script = elt as Script;
				if (script != null)
				{
					var remote = script.HasDelayedContent;
					var async = script.Async && remote || script.Source == NodeSources.Script;
					var defer = script.Defer && remote && !async && script.Source == NodeSources.DocumentBuilder;

					if (defer)
					{
						_unresolvedDelayedResources.Enqueue(new Tuple<Task, Script>(script.LoadAsync(_resourceProvider), script));
					}
					else if (remote)
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

		/// <summary>
		/// Map attribute to event (onclick->click, etc...)
		/// </summary>
		private static IDictionary<string, string> _eventAttr = new Dictionary<string, string>
		{
			{"onclick", "click"}
		};

		private void RegisterAttr(Attr attr)
		{
			string eventName;
			if (_eventAttr.TryGetValue(attr.Name.ToLowerInvariant(), out eventName))
			{
				var parentElement = attr.OwnerElement;

				var fname = eventName + "Handler_" + DateTime.Now.Ticks;
				var funcInit = "function " + fname + "(){" + attr.Value.Trim() + ";}";
				_scriptExecutor.Execute("text/javascript", funcInit);

				var funcCall = fname + "();";

				parentElement.AddEventListener(eventName, e => { _scriptExecutor.Execute("text/javascript", funcCall); }, false);

				//todo: unsubscribe if attribute value changed
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

