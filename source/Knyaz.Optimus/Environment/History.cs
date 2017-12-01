using System;
using System.Collections.Generic;
using Knyaz.Optimus.Tools;
using Knyaz.Optimus.Dom.Interfaces;

namespace Knyaz.Optimus.Environment
{
	public class History : IHistory
	{
		//todo: history should updated on navigatin using links, and Open method call.
		readonly List<HistoryItem> _history = new List<HistoryItem>();
		private int _currentPosition = -1;

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
			if (_currentPosition == 0)
				return;

			_currentPosition--;
			SetState(_history[_currentPosition]);
		}

		public void Forward()
		{
			if (_currentPosition == _history.Count - 1)
				return;

			_currentPosition++;
			SetState(_history[_currentPosition]);
		}

		public void ReplaceState(object data, string title, string url)
		{
			var item = new HistoryItem { Data = data, Title = title, Url = url };
			_history[_currentPosition] = item;
			SetState(item);
		}

		public void PushState(object data, string title, string url)
		{
			//todo: raise events
			var item = new HistoryItem {Data = data, Title = title, Url = url};
			_history.Add(item);
			_currentPosition++;
			SetState(item);
		}

		private void SetState(HistoryItem item)
		{
			var url = item.Url;
			_engine.Uri = UriHelper.IsAbsolete(url) ? new Uri(url) : new Uri(new Uri(_engine.Uri.GetLeftPart(UriPartial.Authority)), url);
			if (item.Title != null)
				_engine.Document.Title = item.Title;
		}
	}
}