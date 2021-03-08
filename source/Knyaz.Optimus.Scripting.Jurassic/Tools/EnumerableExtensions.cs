using System;
using System.Collections.Generic;

namespace Knyaz.Optimus.Scripting.Jurassic.Tools
{
	internal static class EnumerableExtensions
	{
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