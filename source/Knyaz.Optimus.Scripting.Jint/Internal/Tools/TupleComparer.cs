using System;
using System.Collections.Generic;

namespace Knyaz.Optimus.Scripting.Jint.Internal.Tools
{
	internal class TupleComparer<T1, T2> : IEqualityComparer<System.Tuple<T1,T2>> 
		where T1 : class 
		where T2 : class
	{
		public bool Equals(System.Tuple<T1, T2> x, Tuple<T1, T2> y) =>
			Equals(x?.Item1, y?.Item1) && Equals(x?.Item2, y?.Item2);

		public int GetHashCode(Tuple<T1, T2> obj) =>
			(obj.Item1?.GetHashCode() ?? 0) ^ (obj.Item2?.GetHashCode() ?? 0);
	}
}