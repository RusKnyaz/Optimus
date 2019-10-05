using System;
using System.Collections.Generic;

namespace Knyaz.Optimus.Tools
{
	internal class TupleComparer<T1, T2> : IEqualityComparer<Tuple<T1,T2>> 
		where T1 : class 
		where T2 : class
	{
		public bool Equals(Tuple<T1, T2> x, Tuple<T1, T2> y) =>
			Equals(x?.Item1, y?.Item1) && Equals(x?.Item2, y?.Item2);

		public int GetHashCode(Tuple<T1, T2> obj) =>
			obj.Item1.GetHashCode() ^ obj.Item2.GetHashCode();
	}
}