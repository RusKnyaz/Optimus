using System;
using System.Collections;
using System.Collections.Generic;

namespace Knyaz.Optimus.Dom.Css
{
	/// <summary>
	/// Collection of style sheets.
	/// </summary>
	public class StyleSheetsList  : IList<CssStyleSheet>
	{
		List<CssStyleSheet> _innerList = new List<CssStyleSheet>();

		internal event Action Changed;

		public IEnumerator<CssStyleSheet> GetEnumerator() => _innerList.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable) _innerList).GetEnumerator();

		public void Add(CssStyleSheet item)
		{
			_innerList.Add(item);
			Register(item);
		}

		public void Clear()
		{
			var prev = _innerList;
			_innerList = new List<CssStyleSheet>();
			if (Changed != null)
			{
				foreach (var cssStyleSheet in prev)
				{
					Unregister(cssStyleSheet);
				}
			}
			prev.Clear();
		}

		public bool Contains(CssStyleSheet item) => _innerList.Contains(item);

		public void CopyTo(CssStyleSheet[] array, int arrayIndex) => 
			_innerList.CopyTo(array, arrayIndex);

		public bool Remove(CssStyleSheet item)
		{
			var res = _innerList.Remove(item);
			Unregister(item);
			return res;
		}

		public int Count => _innerList.Count;

		/// <summary>
		/// Always <c>false</c>.
		/// </summary>
		public bool IsReadOnly => false;

		public int IndexOf(CssStyleSheet item) => _innerList.IndexOf(item);

		public void Insert(int index, CssStyleSheet item)
		{
			_innerList.Insert(index, item);
			Register(item);
		}

		/// <summary>
		/// Removes stylesheet at the specified postion.
		/// </summary>
		/// <param name="index">Index of the stylesheet to be removed.</param>
		public void RemoveAt(int index)
		{
			Unregister(_innerList[index]);
			_innerList.RemoveAt(index);
		}

		private void OnChanged()
		{
			if (Changed != null)
				Changed();
		}

		private void Register(CssStyleSheet cssStyleSheet)
		{
			//todo: subscribe for changes in CssStyleSheet
			cssStyleSheet.Changed += OnChanged;
			OnChanged();
		}

		private void Unregister(CssStyleSheet cssStyleSheet)
		{
			cssStyleSheet.Changed -= OnChanged;
			OnChanged();
		}

		/// <summary>
		/// Gets the stylesheet at the specified index.
		/// </summary>
		/// <param name="index">The index of stylesheet to get.</param>
		public CssStyleSheet this[int index]
		{
			get => _innerList[index];
			set
			{
				Unregister(_innerList[index]);
				_innerList[index] = value;
				Register(value);
			}
		}
	}
}
