#if NUNIT
using System;
using NUnit.Framework;
using WebBrowser.Dom.Perf;

namespace WebBrowser.Tests.Dom
{
	[TestFixture]
	public class TypedArrayTests
	{
		[Test]
		public void InitAndGet()
		{
			var arr = new Int8Array(new sbyte[]{ 4, 3, 2});
			arr.Assert(array => array [0] == 4 && array [1] == 3 && array [2] == 2 && array.Length == 3);
		}

		[Test]
		public void Sharing()
		{
			var buf = new ArrayBuffer (2);
			var i8 = new Int8Array (buf);
			var i16 = new Int16Array (buf);
			i8 [1] = 1;
			Assert.AreEqual (256, i16 [0]);
		}
	}
}
#endif