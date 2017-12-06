using NUnit.Framework;
using System.Collections.Generic;

namespace Knyaz.Optimus.Tests.JsTests
{
	[TestFixture]
	class TypedArraysJsTests
	{
		private object[] Execute(string script)
		{
			var log = new List<object>();
			var engine = new Engine();
			engine.Console.OnLog += log.Add;
			engine.Load("<html><head><script> function out(x){console.log(x);}"+script+"</script></head></html>");
			return log.ToArray();
		}

		[Test]
		public void NewArrayBuffer()
		{
			var result = Execute("var buffer= new ArrayBuffer(2);out(buffer.byteLength);");
			CollectionAssert.AreEqual(new object[] { 2 }, result);
		}

		[Test]
		public void Int16FromArrayBuffer()
		{
			var result = Execute(
				"var buffer= new ArrayBuffer(2);" +
				"var arr = new Int16Array(buffer);" +
				"out(arr.length);"
			);
			CollectionAssert.AreEqual(new[] { 1 }, result);
		}

		[Test]
		public void Int16FromArray()
		{
			var result = Execute(
				"var arr = new Int16Array([1, 2, -3]);" +
				"out(arr[0]);" +
				"out(arr[1]);" +
				"out(arr[2]);" +
				"out(arr.length);"
			);
			CollectionAssert.AreEqual(new[] { 1, 2, -3, 3 }, result);
		}

		[Test]
		public void Int16FromArrayWithFloats()
		{
			var result = Execute(
				"var arr = new Int16Array([1.5, 2.7]);" +
				"out(arr[0]);" +
				"out(arr[1]);" +
				"out(arr.length);"
			);
			CollectionAssert.AreEqual(new[] { 1, 2, 2 }, result);
		}

		[Test]
		public void Uint16InstatiatedFromArray()
		{
			var result = Execute(
				"var arr = new Uint16Array([1, 2, 3]);" +
				"out(arr[0]);" +
				"out(arr[1]);" +
				"out(arr[2]);" +
				"out(arr.length);"
			);
			CollectionAssert.AreEqual(new[] { 1, 2, 3, 3 }, result);
		}

		[Test]
		public void Uint16InstatiatedFromSignedArray()
		{
			var result = Execute(
				"var arr = new Uint16Array([1, 2, -3]); " +
				"out(arr[2]);");

			CollectionAssert.AreEqual(new[] { 65533 }, result);
		}
	}
}
