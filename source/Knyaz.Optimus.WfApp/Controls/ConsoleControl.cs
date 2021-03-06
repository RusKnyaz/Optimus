﻿using System;
using System.Windows.Forms;
using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.Dom.Events;
using Knyaz.Optimus.ResourceProviders;

namespace Knyaz.Optimus.WfApp.Controls
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
					_engine.OnRequest -= ResourceProviderOnOnRequest;
					_engine.OnResponse -= ResourceProviderOnReceived;
					_engine.Window.OnAlert -= OnAlert;

					_engine.Scripting.ScriptExecutionError -= DocumentOnScriptExecutionError;
				}

				_engine = value;

				if (_engine != null)
				{
					_engine.Console.OnLog += ConsoleOnOnLog;
					_engine.OnRequest += ResourceProviderOnOnRequest;
					_engine.OnResponse += ResourceProviderOnReceived;
					_engine.Window.OnAlert += OnAlert;
					Document = _engine.Document;
					_engine.DocumentChanged += OnDocumentChanged;
				}
			}
		}

		private void OnDocumentChanged()
		{
			Document = _engine.Document;
		}

		private void OnAlert(string obj)
		{
			Log("Alert: " + obj);
		}

		private Document _document = null;

		private Document Document
		{
			set
			{
				if (_document != null)
				{
					_document.RemoveEventListener("AfterScriptExecute", OnScriptExecuted, false);
					_document.RemoveEventListener("BeforeScriptExecute", OnScriptExecuting, false);
					_engine.Scripting.ScriptExecutionError -= DocumentOnScriptExecutionError;
				}

				_document = value;
				if (_document != null)
				{
					_document.AddEventListener("AfterScriptExecute", OnScriptExecuted, false);
					_document.AddEventListener("BeforeScriptExecute", OnScriptExecuting, false);
					_engine.Scripting.ScriptExecutionError += DocumentOnScriptExecutionError;
				}
			}
		}

		public bool LogInfo = false;

		private void DocumentOnScriptExecutionError(Script script, Exception exception)
		{
			Log("Script execution error: " + (script.Src ?? script.Id ?? "...") + " Ex: " + exception.Message);
		}

		private void OnScriptExecuted(Event @event)
		{
			if (!LogInfo)
				return;
			var script = (Script) @event.Target;
			Log("Executed: " + (script.Src ?? script.Id ?? "..."));
		}

		private void OnScriptExecuting(Event @event)
		{
			if (!LogInfo)
				return;
			var script = (Script)@event.Target;
			Log("Executing: " + (script.Src ?? script.Id ?? "..."));
		}

		private void ResourceProviderOnOnRequest(Request s)
		{
			if (!LogInfo)
				return;
			Log($"Request: {s.Url}");
		}

		private void ResourceProviderOnReceived(ReceivedEventArguments args)
		{
			if (!LogInfo)
				return;
			Log($"Received: {args.Request.Url}");
		}

		private void ConsoleOnOnLog(object o)
		{
			Log((o ?? "<null>").ToString());
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
