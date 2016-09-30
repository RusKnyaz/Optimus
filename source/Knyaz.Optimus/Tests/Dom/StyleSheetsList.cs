using System;
using System.Collections;
using System.Collections.Generic;
using Knyaz.Optimus.Dom.Css;

namespace Knyaz.Optimus.Tests.Dom
{
	public class StyleSheetsList  : IList<CssStyleSheet>
	{
		List<CssStyleSheet> _innerList = new List<CssStyleSheet>();

		internal event Action Changed;

		public IEnumerator<CssStyleSheet> GetEnumerator()
		{
			return _innerList.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable) _innerList).GetEnumerator();
		}

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

		public bool Contains(CssStyleSheet item)
		{
			return _innerList.Contains(item);
		}

		public void CopyTo(CssStyleSheet[] array, int arrayIndex)
		{
			_innerList.CopyTo(array, arrayIndex);
		}

		public bool Remove(CssStyleSheet item)
		{
			var res = _innerList.Remove(item);
			Unregister(item);
			return res;
		}

		public int Count
		{
			get { return _innerList.Count; }
		}

		public bool IsReadOnly
		{
			get { return ((ICollection<CssStyleDeclaration>) _innerList).IsReadOnly; }
		}

		public int IndexOf(CssStyleSheet item)
		{
			return _innerList.IndexOf(item);
		}

		public void Insert(int index, CssStyleSheet item)
		{
			_innerList.Insert(index, item);
			Register(item);
		}

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

		public CssStyleSheet this[int index]
		{
			get { return _innerList[index]; }
			set
			{
				Unregister(_innerList[index]);
				_innerList[index] = value;
				Register(value);
			}
		}
	}
}
