using System;
using System.Collections.Generic;

namespace Knyaz.Optimus.WfApp.Tools
{
	public static class ListThreadSafeExtensions
	{
		public static IList<T> CopyListThreadSafe<T>(this IList<T> list)
		{
			var result = new List<T>(list.Count);
			for (var i = 0; i < list.Count; i++)
			{
				try
				{
					result.Add(list[i]);
				}
				catch (ArgumentOutOfRangeException)
				{
					return result;
				}
			}

			return result;
		}	
	}
}