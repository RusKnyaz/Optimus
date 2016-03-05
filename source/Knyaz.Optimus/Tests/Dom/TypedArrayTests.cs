#if NUNIT
using Knyaz.Optimus.Dom.Perf;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.Dom
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

		[Test]
		public void InitByZero()
		{
			new ArrayBuffer(16).Assert(buf =>
					new Int8Array(buf)[0] == 0 &&
					new UInt8Array(buf)[0] == 0 &&
					new Int16Array(buf)[0] == 0 &&
					new UInt16Array(buf)[0] == 0
				);
		}

		[Test]
		public void Int8SetArray()
		{
			var buffer = new ArrayBuffer(4);
			var i8 = new Int8Array(buffer);
			i8.Set(new sbyte[]{1,2,3}, 1);
			CollectionAssert.AreEqual(new[] { 0, 1, 2, 3 }, buffer.Data);
		}

		[Test]
		public void Int16SetArray()
		{
			var buffer = new ArrayBuffer(4);
			var i16 = new Int16Array(buffer);
			i16.Set(new short[] { 1 }, 1);
			CollectionAssert.AreEqual(new[] { 0, 0, 1, 0 }, buffer.Data);
		}

		[Test]
		public void Int16SetTypedArray()
		{
			var buffer = new ArrayBuffer(4);
			var buffer2 = new ArrayBuffer(2);
			var arr1 = new Int16Array(buffer);
			var arr2 = new Int16Array(buffer2);
			arr2[0] = 1;
			arr1.Set(arr2, 1);
			CollectionAssert.AreEqual(new[] { 0, 0, 1, 0 }, buffer.Data);
		}
	}
}
#endif