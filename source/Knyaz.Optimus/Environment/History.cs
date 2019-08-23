using System;
using System.Collections.Generic;
using Knyaz.Optimus.Tools;
using Knyaz.Optimus.Dom.Interfaces;

namespace Knyaz.Optimus.Environment
{
	/// <summary>
	/// Part of WEB API that allows manipulation of the session history.
	/// </summary>
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

		internal History(Engine engine) => _engine = engine;

		/// <summary>
		/// Gets the number of elements in the session history.
		/// </summary>
		public long Lenght => _history.Count;

		/// <summary>
		/// Loads a page from the session history, identified by its relative location to the current page.
		/// </summary>
		/// <param name="delta">Relative page location in history stack (-1 - previous, 1 - next, etc).</param>
		public void Go(long delta = 0)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Goes to the previous page in session history. Equivalent to History.Go(-1)
		/// </summary>
		public void Back()
		{
			if (_currentPosition == 0)
				return;

			_currentPosition--;
			SetState(_history[_currentPosition]);
		}

		/// <summary>
		/// Goes to the next page in session history; this is equivalent to History.Go(1).
		/// </summary>
		public void Forward()
		{
			if (_currentPosition == _history.Count - 1)
				return;

			_currentPosition++;
			SetState(_history[_currentPosition]);
		}

		/// <summary>
		/// Updates the most recent entry on the history stack to have the specified data, title, and, if provided, URL.
		/// </summary>
		/// <param name="data">The object which is associated with the new history entry created by pushState(). 
		/// This object passed in to the popstate event handler.</param>
		/// <param name="title">Some title.</param>
		/// <param name="url">The new history entry's URL is given by this parameter. Note that the new page won't be loaded.</param>
		public void ReplaceState(object data, string title, string url)
		{
			var item = new HistoryItem { Data = data, Title = title, Url = url };
			_history[_currentPosition] = item;
			SetState(item);
		}

		/// <summary>
		/// Adds new history entry.
		/// </summary>
		/// <param name="data">The object which is associated with the new history entry created by pushState(). 
		/// This object passed in to the popstate event handler.</param>
		/// <param name="title">Some title.</param>
		/// <param name="url">The new history entry's URL is given by this parameter. Note that the new page won't be loaded.</param>
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