using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Knyaz.Optimus.Tools
{
	class CachedEnumerable<T> : IEnumerable<T>
	{
		private readonly IEnumerable<T> _enumerable;
		private IEnumerator<T> _currentEnumerator;
		private List<T> _cache;

		public CachedEnumerable(IEnumerable<T> enumerable)
		{
			_enumerable = enumerable;
		}

		public IEnumerator<T> GetEnumerator()
		{
			if (_currentEnumerator == null)
			{
				_currentEnumerator = _enumerable.GetEnumerator();
				_cache = new List<T>();
			}

			foreach (var val in _cache)
			{
				yield return val;
			}

			while (_currentEnumerator.MoveNext())
			{
				var val = _currentEnumerator.Current;
				_cache.Add(val);
				yield return val;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Reset()
		{
			if (_currentEnumerator != null)
			{
				_currentEnumerator.Dispose();
				_currentEnumerator = null;
			}
			_cache = null;
		}
	}
}
