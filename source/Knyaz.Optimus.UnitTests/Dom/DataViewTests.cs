using Knyaz.Optimus.Dom.Perf;
using NUnit.Framework;
using System;

namespace Knyaz.Optimus.Tests.Dom
{
	[TestFixture]
    public class DataViewTests
    {
		[Test]
		public void ConstructWithDefaultOffset() =>
			new DataView(new ArrayBuffer(10)).
			Assert(dataView =>
				dataView.ByteOffset == 0 &&
				dataView.ByteLength == 10);

		[Test]
		public void ConstructWithDefaultLength() =>
			new DataView(new ArrayBuffer(10), 2).
			Assert(dataView =>
				dataView.ByteOffset == 2 &&
				dataView.ByteLength == 8);

		[Test]
		public void Construct() =>
			new DataView(new ArrayBuffer(10), 2, 4).
			Assert(dataView =>
				dataView.ByteOffset == 2 &&
				dataView.ByteLength == 4);

		[Test]
		public void ConstructWithInvalidLength() =>
			Assert.Throws<ArgumentOutOfRangeException>(() => new DataView(new ArrayBuffer(10), 2, 10));

		[Test]
		public void ConstructWithInvalidOffset() =>
			Assert.Throws<ArgumentOutOfRangeException>(() => new DataView(new ArrayBuffer(10), 10));

		[Test]
		public void GetZero() => 
			new DataView(new ArrayBuffer(8)).Assert(dataView =>
				dataView.GetInt8(0) == 0 &&
				dataView.GetInt16(0, false) == 0 &&
				dataView.GetInt32(0, false) == 0 &&
				dataView.GetUint8(0) == 0 &&
				dataView.GetUint16(0, false) == 0 &&
				dataView.GetUint32(0, false) == 0u &&
				dataView.GetFloat64(0, false) == 0d &&
				dataView.GetFloat32(1, false) == 0f &&
				dataView.GetInt16(0, true) == 0 &&
				dataView.GetInt32(0, true) == 0 &&
				dataView.GetUint16(0, true) == 0 &&
				dataView.GetUint32(0, true) == 0u &&
				dataView.GetFloat64(0, true) == 0d &&
				dataView.GetFloat32(1, true) == 0f);


		[Test]
		public void SetFloat32()
		{
			var buffer = new ArrayBuffer(8);
			var dataView = new DataView(buffer);
			dataView.SetFloat32(1, 3f);
			Assert.AreEqual(new byte[] { 0, 64, 64, 0, 0, 0, 0, 0 }, buffer.Data);
		}

		[Test]
		public void SetFloat32Little()
		{
			var buffer = new ArrayBuffer(8);
			var dataView = new DataView(buffer);
			dataView.SetFloat32(1, 3f, true);
			Assert.AreEqual(new byte[] { 0, 0, 0, 64, 64, 0, 0, 0 }, buffer.Data);
		}

		[Test]
		public void SetFloat64()
		{
			var buffer = new ArrayBuffer(10);
			var dataView = new DataView(buffer);
			dataView.SetFloat64(1, 3f);
			Assert.AreEqual(new byte[] { 0, 64, 8, 0, 0, 0, 0, 0, 0, 0 }, buffer.Data);
		}

		[Test]
		public void SetFloat64Little()
		{
			var buffer = new ArrayBuffer(10);
			var dataView = new DataView(buffer);
			dataView.SetFloat64(1, 3f, true);
			Assert.AreEqual(new byte[] { 0, 0, 0, 0, 0, 0, 0, 8, 64, 0 }, buffer.Data);
		}

		[Test]
		public void SetInt16()
		{
			var buffer = new ArrayBuffer(4);
			var dataView = new DataView(buffer);
			dataView.SetInt16(1, 3);
			Assert.AreEqual(3, dataView.GetInt16(1));
			Assert.AreEqual(new byte[] { 0, 0, 3, 0}, buffer.Data);
		}

		[Test]
		public void SetInt16Little()
		{
			var buffer = new ArrayBuffer(4);
			var dataView = new DataView(buffer);
			dataView.SetInt16(1, 3, true);
			Assert.AreEqual(3, dataView.GetInt16(1, true));
			Assert.AreEqual(new byte[] { 0, 3, 0, 0 }, buffer.Data);
		}

		[Test]
		public void SetInt32()
		{
			var buffer = new ArrayBuffer(5);
			var dataView = new DataView(buffer);
			dataView.SetInt32(1, 3);
			Assert.AreEqual(3, dataView.GetInt32(1));
			Assert.AreEqual(new byte[] { 0, 0, 0, 0, 3 }, buffer.Data);
		}

		[Test]
		public void SetInt32Little()
		{
			var buffer = new ArrayBuffer(5);
			var dataView = new DataView(buffer);
			dataView.SetInt32(1, 3, true);
			Assert.AreEqual(3, dataView.GetInt32(1, true));
			Assert.AreEqual(new byte[] { 0, 3, 0, 0, 0 }, buffer.Data);
		}

		[Test]
		public void SetUInt32()
		{
			var buffer = new ArrayBuffer(5);
			var dataView = new DataView(buffer);
			dataView.SetUint32(1, 3);
			Assert.AreEqual(3, dataView.GetInt32(1));
			Assert.AreEqual(new byte[] { 0, 0, 0, 0, 3 }, buffer.Data);
		}

		[Test]
		public void SetUInt32Little()
		{
			var buffer = new ArrayBuffer(5);
			var dataView = new DataView(buffer);
			dataView.SetUint32(1, 3, true);
			Assert.AreEqual(3, dataView.GetUint32(1, true));
			Assert.AreEqual(new byte[] { 0, 3, 0, 0, 0 }, buffer.Data);
		}

		[Test]
		public void SetUInt16()
		{
			var buffer = new ArrayBuffer(4);
			var dataView = new DataView(buffer);
			dataView.SetUint16(1, 3);
			Assert.AreEqual(3, dataView.GetUint16(1));
			Assert.AreEqual(new byte[] { 0, 0, 3, 0 }, buffer.Data);
		}

		[Test]
		public void SetUInt16Little()
		{
			var buffer = new ArrayBuffer(4);
			var dataView = new DataView(buffer);
			dataView.SetUint16(1, 3, true);
			Assert.AreEqual(3, dataView.GetUint16(1, true));
			Assert.AreEqual(new byte[] { 0, 3, 0, 0 }, buffer.Data);
		}

		[Test]
		public void SetInt8()
		{
			var buffer = new ArrayBuffer(4);
			var dataView = new DataView(buffer);
			dataView.SetInt8(1, -3);
			Assert.AreEqual(-3, dataView.GetInt8(1));
			Assert.AreEqual(253, dataView.GetUint8(1));
		}

		[Test]
		public void SetUint8()
		{
			var buffer = new ArrayBuffer(4);
			var dataView = new DataView(buffer);
			dataView.SetUint8(1, 3);
			Assert.AreEqual(3, dataView.GetUint8(1));
		}
	}
}