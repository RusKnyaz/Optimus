using System.Collections.Generic;
using System.Linq;

namespace WebBrowser.Tools
{
	internal static class ListExtension
	{
		public static IEnumerable<T> Last<T>(this List<T> list, int count)
		{
			return list.Count <= count ? list : list.Skip(list.Count - count);
		}

	}
}
