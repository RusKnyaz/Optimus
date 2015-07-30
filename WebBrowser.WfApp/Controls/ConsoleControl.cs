using System;
using System.Windows.Forms;
using WebBrowser.Dom;
using WebBrowser.Dom.Elements;

namespace WebBrowser.WfApp.Controls
{
	public partial class ConsoleControl : UserControl
	{
		private Engine _engine;

		public ConsoleControl()
		{
			InitializeComponent();
		}

		public Engine Engine
		{
			get
			{
				return _engine;
			}
			set
			{
				if (_engine != null)
				{
					_engine.Console.OnLog -= ConsoleOnOnLog;
					_engine.ResourceProvider.OnRequest -= ResourceProviderOnOnRequest;
					_engine.ResourceProvider.OnRequest -= ResourceProviderOnReceived;
					_engine.DocumentChanged-=EngineOnDocumentChanged;
				}

				_engine = value;

				if (_engine != null)
				{
					_engine.Console.OnLog += ConsoleOnOnLog;
					_engine.ResourceProvider.OnRequest += ResourceProviderOnReceived;
					_engine.DocumentChanged += EngineOnDocumentChanged;
				}
				else
				{
					EngineOnDocumentChanged();
				}
			}
		}

		private Document _document = null;

		private void EngineOnDocumentChanged()
		{
			if (_document != null)
			{
				_document.RemoveEventListener("AfterScriptExecute", OnScriptExecuted, false);
			}
			if(_engine == null)
				return;
			
			_document = _engine.Document;
			if (_document != null)
			{
				_document.AddEventListener("AfterScriptExecute", OnScriptExecuted, false);
			}
		}

		private void OnScriptExecuted(Event @event)
		{
			var script = (Script) @event.Target;
			Log("Execute: " + (script.Src ?? script.Id ?? "..."));
		}

		private void ResourceProviderOnOnRequest(string s)
		{
			Log("Request: " + s);
		}

		private void ResourceProviderOnReceived(string s)
		{
			Log("Received: " + s);
		}

		private void ConsoleOnOnLog(object o)
		{
			Log(o.ToString());
		}

		void Log(string text)
		{
			this.SafeBeginInvoke(() => textBoxLog.AppendText(text + "\r\n"));
		}
	}

	public static class ControlExtension
	{
		public static void SafeBeginInvoke(this Control obj, Action action)
		{
			if (obj.InvokeRequired)
			{
				var args = new object[0];
				obj.BeginInvoke(action, args);
			}
			else
			{
				action();
			}
		}

		public static void SafeInvoke(this Control obj, Action action)
		{
			if (obj.InvokeRequired)
			{
				var args = new object[0];
				obj.Invoke(action, args);
			}
			else
			{
				action();
			}
		}
	}
}
