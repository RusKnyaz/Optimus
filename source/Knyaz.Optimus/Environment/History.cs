using System;
using System.Collections.Generic;
using Knyaz.Optimus.ScriptExecuting;
using Knyaz.Optimus.Tools;

namespace Knyaz.Optimus.Environment
{
	public class History : IHistory
	{
		//todo: history should updated on navigatin using links, and Open method call.
		readonly List<HistoryItem> _history = new List<HistoryItem>();
		private int _currentPosition = 0;

		class HistoryItem
		{
			public string Url;
			public string Title;
			public object Data;
		}

		private readonly Engine _engine;

		public History(Engine engine)
		{
			_engine = engine;
		}

		public long Lenght { get { return _history.Count; } }

		public void Go(long delta = 0)
		{
			throw new NotImplementedException();
		}

		public void Back()
		{
			throw new NotImplementedException();
		}

		public void Forward()
		{
			throw new NotImplementedException();
		}

		public void ReplaceState(object data, string title, string url)
		{
			throw new NotImplementedException();
		}

		public void PushState(object data, string title, string url)
		{
			//todo: raise events
			_history.Add(new HistoryItem{Data = data, Title = title, Url = url});
			_currentPosition++;
			_engine.Uri = UriHelper.IsAbsolete(url) ? new Uri(url) : new Uri(new Uri(_engine.Uri.GetLeftPart(UriPartial.Authority)), url);
		}
	}

	[DomItem]
	public interface IHistory
	{
		long Lenght { get; }
		void Go(long delta = 0);
		void Back();
		void Forward();
		void ReplaceState(object data, string title, string url);
		void PushState(object data, string title, string url);
	}
}