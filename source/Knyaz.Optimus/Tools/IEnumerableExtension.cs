using System;
using System.Collections.Generic;

namespace Knyaz.Optimus.Tools
{
	public static class IEnumerableExtension
	{
		public static IEnumerable<T> Flat<T>(this IEnumerable<T> e, Func<T, IEnumerable<T>> children)
		{
			foreach (var item in e)
			{
				yield return item;
				foreach (var child in children(item).Flat(children))
				{
					yield return child;
				}
			}
		}

		public static IEnumerable<T> GetRecursive<T>(this T item, Func<T, T> getNext) where T : class 
		{
			var cur = item;
			while (cur != null)
			{
				yield return cur;
				cur = getNext(cur);
			}
		}

		public static int IndexOf<T>(this IEnumerable<T> e, T item)
		{
			var idx = 0;
			foreach (var currentItem in e)
			{
				if (Equals(currentItem, item))
					return idx;
				idx++;
			}
			return -1;
		}

		public static int IndexOf<T>(this IEnumerable<T> e, Func<T, bool> condition)
		{
			var idx = 0;
			foreach (var currentItem in e)
			{
				if (condition(currentItem))
					return idx;
				idx++;
			}
			return -1;
		}
	}
}