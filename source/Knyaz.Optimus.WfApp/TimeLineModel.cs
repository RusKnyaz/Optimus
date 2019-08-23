using System;
using System.Collections.Generic;
using System.Linq;
using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.ResourceProviders;

namespace Knyaz.Optimus.WfApp
{
	internal static class EngineTimeLineExtension
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="engine"></param>
		/// <param name="action">Dispose action</param>
		/// <returns></returns>
		public static Action SubscribeTimeLine(this Engine engine, Action<TimePoint> action)
		{
			var requestHandler = (Action<Request>)(s => action(new TimePoint
			{
				DateTime = DateTime.Now,
				EventType = TimeLineEvents.Request,
				ResourceId = s.Url.ToString()
			}));

			var receivedHandler = (Action<ReceivedEventArguments>)(s => action(new TimePoint
			{
				DateTime = DateTime.Now,
				EventType = TimeLineEvents.Received,
				ResourceId = s.Request.Url.ToString()
			}));

			var beforeScriptExecute = (Action<Script>)(s =>
			{
				var scriptName = GetScriptName(s);

				action(new TimePoint
				{
					DateTime = DateTime.Now,
					EventType = TimeLineEvents.Executing,
					ResourceId = scriptName
				});
			});

			var afterScriptExecute = (Action<Script>)(s =>
			{
				var scriptName = GetScriptName(s);

				action(new TimePoint
				{
					DateTime = DateTime.Now,
					EventType = TimeLineEvents.Executed,
					ResourceId = scriptName
				});
			});

			var scriptError = (Action<Script, Exception>) ((s, r) =>
			{
				var scriptName = GetScriptName(s);

				action(new TimePoint
				{
					DateTime = DateTime.Now,
					EventType = TimeLineEvents.ExecutionFailed,
					ResourceId = scriptName
				});
			});

			DocumentScripting curScripting = null;

			var documentChanged = (Action) (() =>
			{
				if (curScripting != null)
				{
					curScripting.BeforeScriptExecute -= beforeScriptExecute;
					curScripting.AfterScriptExecute -= afterScriptExecute;
					curScripting.ScriptExecutionError -= scriptError;
				}
				curScripting = engine.Scripting;//todo: bugs possible when document changed quickly
				curScripting.BeforeScriptExecute += beforeScriptExecute;
				curScripting.AfterScriptExecute += afterScriptExecute;
				curScripting.ScriptExecutionError += scriptError;
			});

			var timerExecuting = (Action) (() =>
			{
				action(new TimePoint
				{
					DateTime = DateTime.Now,
					EventType = TimeLineEvents.Executing,
					ResourceId = "[Timer]"
				});
			});

			var timerExecuted = (Action) (() =>
			{
				action(new TimePoint
				{
					DateTime = DateTime.Now,
					EventType = TimeLineEvents.Executed,
					ResourceId = "[Timer]"
				});
			});

			var timerFailed = (Action<Exception>) (_ =>
			{
				action(new TimePoint
				{
					DateTime = DateTime.Now,
					EventType = TimeLineEvents.ExecutionFailed,
					ResourceId = "[Timer]"
				});
			});

			engine.OnRequest += requestHandler;
			engine.OnResponse += receivedHandler;
			curScripting = engine.Scripting;
			engine.DocumentChanged += documentChanged;

			engine.Window.Timers.OnExecuting += timerExecuting;
			engine.Window.Timers.OnExecuted += timerExecuted;
			engine.Window.Timers.OnException += timerFailed;

			return () =>
			{
				engine.OnRequest -= requestHandler;
				engine.OnResponse -= receivedHandler;
				engine.DocumentChanged -= documentChanged;
				engine.Window.Timers.OnExecuting -= timerExecuting;
				engine.Window.Timers.OnExecuted -= timerExecuted;
				engine.Window.Timers.OnException -= timerFailed;
			};
		}

		private static string GetScriptName(Script s)
		{
			var scriptName = !string.IsNullOrEmpty(s.Src)
				? s.Src
				: s.Text.Substring(0, Math.Min(30, s.Text.Length)).Replace("\r", "").Replace("\n","");

			if (string.IsNullOrEmpty(scriptName))
				scriptName = "Script_" + s.GetHashCode();

			return scriptName;
		}
	}

	/// <summary>
	/// Save engine events and times
	/// </summary>
	internal class TimeLineModel : IDisposable
	{
		List<List<TimePoint>> _lines = new List<List<TimePoint>>();
		private readonly Action _unsubscribe;

		public List<List<TimePoint>> Lines { get { return _lines;} }

		public TimeLineModel(Engine engine)
		{
			_unsubscribe = engine.SubscribeTimeLine(OnEvent);
		}

		private void OnEvent(TimePoint timePoint)
		{
			var targetList = _lines.FirstOrDefault(x => x[0].ResourceId == timePoint.ResourceId);
			if (targetList == null)
			{
				targetList = new List<TimePoint>();
				_lines.Add(targetList);
			}
			targetList.Add(timePoint);

			OnChanged();
		}

		public event Action Changed;

		protected virtual void OnChanged()
		{
			var handler = Changed;
			if (handler != null) handler();
		}

		public void Dispose()
		{
			_unsubscribe();
		}
	}

	internal struct TimePoint
	{
		public DateTime DateTime;
		public TimeLineEvents EventType;
		public string ResourceId;
	}

	internal enum TimeLineEvents
	{
		Unknown,
		Request,
		Received,
		Executing,
		Executed,
		ExecutionFailed
	}

	internal enum TimeLineResources
	{
		Html,
		Script,
		Other
	}
}
