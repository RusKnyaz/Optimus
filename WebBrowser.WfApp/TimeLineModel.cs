using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using WebBrowser.Dom;
using WebBrowser.Dom.Elements;

namespace WebBrowser.WfApp
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
			var requestHandler = (Action<string>)(s => action(new TimePoint
			{
				DateTime = DateTime.Now,
				EventType = TimeLineEvents.Request,
				ResourceId = s
			}));

			var receivedHandler = (Action<string>)(s => action(new TimePoint
			{
				DateTime = DateTime.Now,
				EventType = TimeLineEvents.Received,
				ResourceId = s
			}));

			var beforeScriptExecute = (Action<Script>)(s =>
			{
				if (!string.IsNullOrEmpty(s.Src))
				action(new TimePoint
				{
					DateTime = DateTime.Now,
					EventType = TimeLineEvents.Executing,
					ResourceId = s.Src
				});
			});

			var afterScriptExecute = (Action<Script>)(s =>
			{
				if(!string.IsNullOrEmpty(s.Src))
				action(new TimePoint
				{
					DateTime = DateTime.Now,
					EventType = TimeLineEvents.Executed,
					ResourceId = s.Src
				});
			});

			DocumentScripting prevScripting = null;

			var documentChanged = (Action) (() =>
			{
				if (prevScripting != null)
				{
					prevScripting.BeforeScriptExecute += beforeScriptExecute;
					prevScripting.AfterScriptExecute += afterScriptExecute;
				}
				prevScripting = engine.Scripting;//todo: bugs possible when document changed quickly
				engine.Scripting.BeforeScriptExecute += beforeScriptExecute;
				engine.Scripting.AfterScriptExecute += afterScriptExecute;
			});

			engine.ResourceProvider.OnRequest += requestHandler;
			engine.ResourceProvider.OnRequest += receivedHandler;
			prevScripting = engine.Scripting;
			engine.DocumentChanged += documentChanged;

			return () =>
			{
				engine.ResourceProvider.OnRequest -= requestHandler;
				engine.ResourceProvider.OnRequest -= receivedHandler;
				engine.DocumentChanged += documentChanged;
			};
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
		Request,
		Received,
		Executing,
		Executed
	}

	internal enum TimeLineResources
	{
		Html,
		Script,
		Other
	}
}
