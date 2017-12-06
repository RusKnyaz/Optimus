using System;

namespace Knyaz.Optimus.Dom.Perf
{
	/// <summary>
	/// Provides a low-level interface for reading and writing multiple number types in an ArrayBuffer irrespective of the platform's endianness.
	/// </summary>
	public class DataView
	{
		/// <summary>
		/// Creates new DataView instance matches entire buffer.
		/// </summary>
		public DataView(ArrayBuffer buffer) : this(buffer, 0){}

		/// <summary>
		/// Creates new DataView instance matches given buffer from specified offset to the end.
		/// </summary>
		public DataView(ArrayBuffer buffer, int byteOffset) : this(buffer, byteOffset, buffer.ByteLength - byteOffset){}

		/// <summary>
		/// Creates new DataView instance matches specifeid bytes count of buffer from specified offset.
		/// </summary>
		public DataView(ArrayBuffer buffer, int byteOffset, int byteLength)
		{
			if (byteOffset >= buffer.ByteLength)
				throw new ArgumentOutOfRangeException("byteOffset");

			if (byteOffset + byteLength > buffer.ByteLength)
				throw new ArgumentOutOfRangeException("byteLength");
			Buffer = buffer;
			ByteOffset = byteOffset;
			ByteLength = byteLength;
		}

		/// <summary>
		/// Gets the ArrayBuffer or SharedArrayBuffer referenced by the DataView.
		/// </summary>
		public ArrayBuffer Buffer { get; }

		/// <summary>
		/// Gets the offset (in bytes) of this view from the start of its ArrayBuffer.
		/// </summary>
		public int ByteOffset { get; }

		/// <summary>
		/// Gets the length (in bytes) of this view from the start of its ArrayBuffer.
		/// </summary>
		public int ByteLength { get; }

		/// <summary>
		/// Gets a signed 32-bit float (float) at the specified byte offset from the start of the DataView
		/// </summary>
		/// <param name="byteOffset">The offset, in byte, from the start of the view where to read the data.</param>
		/// <param name="littleEndian">Indicates whether the 32-bit float is stored in little- or big-endian format. If false, a big-endian value is read.</param>
		public float GetFloat32(int byteOffset, bool littleEndian = false)
			=>	BitConverter.IsLittleEndian == littleEndian
				? BitConverter.ToSingle(Buffer.Data, byteOffset)
				: BitConverter.ToSingle(GetReversed(byteOffset, sizeof(float)), 0);


		/// <summary>
		/// Gets a signed 64-bit float (double) at the specified byte offset from the start of the DataView
		/// </summary>
		/// <param name="byteOffset">The offset, in byte, from the start of the view where to read the data.</param>
		/// <param name="littleEndian">Indicates whether the 64-bit float is stored in little- or big-endian format. If false, a big-endian value is read.</param>
		public double GetFloat64(int byteOffset, bool littleEndian = false)
			=> BitConverter.IsLittleEndian == littleEndian
				? BitConverter.ToDouble(Buffer.Data, byteOffset)
				: BitConverter.ToDouble(GetReversed(byteOffset, sizeof(double)), 0);

		/// <summary>
		/// Gets a signed 32-bit integer (int) at the specified byte offset from the start of the DataView
		/// </summary>
		/// <param name="byteOffset">The offset, in byte, from the start of the view where to read the data.</param>
		/// <param name="littleEndian">Indicates whether the 32-bit int is stored in little- or big-endian format. If false, a big-endian value is read.</param>
		public int GetInt32(int byteOffset, bool littleEndian = false)
			=> BitConverter.IsLittleEndian == littleEndian
				? BitConverter.ToInt32(Buffer.Data, byteOffset)
				: BitConverter.ToInt32(GetReversed(byteOffset, sizeof(int)), 0);

		/// <summary>
		/// Gets a unsigned 32-bit integer (uint) at the specified byte offset from the start of the DataView
		/// </summary>
		/// <param name="byteOffset">The offset, in byte, from the start of the view where to read the data.</param>
		/// <param name="littleEndian">Indicates whether the 32-bit int is stored in little- or big-endian format. If false, a big-endian value is read.</param>
		public uint GetUint32(int byteOffset, bool littleEndian = false)
			=> BitConverter.IsLittleEndian == littleEndian
				? BitConverter.ToUInt32(Buffer.Data, byteOffset)
				: BitConverter.ToUInt32(GetReversed(byteOffset, sizeof(uint)), 0);

		/// <summary>
		/// Gets a signed 16-bit integer (short) at the specified byte offset from the start of the DataView
		/// </summary>
		/// <param name="byteOffset">The offset, in byte, from the start of the view where to read the data.</param>
		/// <param name="littleEndian">Indicates whether the 16-bit int is stored in little- or big-endian format. If false, a big-endian value is read.</param>
		public short GetInt16(int byteOffset, bool littleEndian = false)
			=> BitConverter.IsLittleEndian == littleEndian
				? BitConverter.ToInt16(Buffer.Data, byteOffset)
				: BitConverter.ToInt16(GetReversed(byteOffset, sizeof(short)), 0);

		/// <summary>
		/// Gets a unsigned 16-bit integer (ushort) at the specified byte offset from the start of the DataView
		/// </summary>
		/// <param name="byteOffset">The offset, in byte, from the start of the view where to read the data.</param>
		/// <param name="littleEndian">Indicates whether the 16-bit int is stored in little- or big-endian format. If false, a big-endian value is read.</param>
		public ushort GetUint16(int byteOffset, bool littleEndian = false)
			=> BitConverter.IsLittleEndian == littleEndian
				? BitConverter.ToUInt16(Buffer.Data, byteOffset)
				: BitConverter.ToUInt16(GetReversed(byteOffset, sizeof(ushort)), 0);

		/// <summary>
		/// Gets a unsigned 8-bit integer (byte) at the specified byte offset from the start of the DataView
		/// </summary>
		/// <param name="byteOffset">The offset, in byte, from the start of the view where to read the data.</param>
		public byte GetUint8(int byteOffset) => Buffer.Data[byteOffset];

		/// <summary>
		/// Gets a signed 8-bit integer (sbyte) at the specified byte offset from the start of the DataView
		/// </summary>
		/// <param name="byteOffset">The offset, in byte, from the start of the view where to read the data.</param>
		public sbyte GetInt8(int byteOffset) => (sbyte)Buffer.Data[byteOffset];


		byte[] GetReversed(int byteOffset, int count)
		{
			var result = new byte[count];
			Array.Copy(Buffer.Data, byteOffset, result, 0, count);
			Array.Reverse(result);
			return result;
		}

		/// <summary>
		/// Stores a signed 32-bit float (float) value at the specified byte offset from the start of the DataView.
		/// </summary>
		/// <param name="byteOffset">The offset, in byte, from the start of the view where to store the data.</param>
		/// <param name="value">The value to set.</param>
		/// <param name="littleEndian">Indicates whether the 32-bit float is stored in little- or big-endian format.</param>
		public void SetFloat32(int byteOffset, float value, bool littleEndian = false)
		{
			var bytes = BitConverter.GetBytes(value);
			if (BitConverter.IsLittleEndian ^ littleEndian)
				Array.Reverse(bytes);
			Array.Copy(bytes, 0, Buffer.Data, byteOffset, bytes.Length);
		}

		/// <summary>
		/// Stores a signed 64-bit float (double) value at the specified byte offset from the start of the DataView.
		/// </summary>
		/// <param name="byteOffset">The offset, in byte, from the start of the view where to store the data.</param>
		/// <param name="value">The value to set.</param>
		/// <param name="littleEndian">Indicates whether the 64-bit float is stored in little- or big-endian format.</param>
		public void SetFloat64(int byteOffset, double value, bool littleEndian = false)
		{
			var bytes = BitConverter.GetBytes(value);
			if (BitConverter.IsLittleEndian ^ littleEndian)
				Array.Reverse(bytes);
			Array.Copy(bytes, 0, Buffer.Data, byteOffset, bytes.Length);
		}

		/// <summary>
		/// Stores a signed 16-bit integer (short) value at the specified byte offset from the start of the DataView.
		/// </summary>
		/// <param name="byteOffset">The offset, in byte, from the start of the view where to store the data.</param>
		/// <param name="value">The value to set.</param>
		/// <param name="littleEndian">Indicates whether the 16-bit int is stored in little- or big-endian format.</param>
		public void SetInt16(int byteOffset, short value, bool littleEndian = false)
		{
			var bytes = BitConverter.GetBytes(value);
			if (BitConverter.IsLittleEndian ^ littleEndian)
				Array.Reverse(bytes);
			Array.Copy(bytes, 0, Buffer.Data, byteOffset, bytes.Length);
		}

		/// <summary>
		/// Stores a signed 32-bit integer (short) value at the specified byte offset from the start of the DataView.
		/// </summary>
		/// <param name="byteOffset">The offset, in byte, from the start of the view where to store the data.</param>
		/// <param name="value">The value to set.</param>
		/// <param name="littleEndian">Indicates whether the 32-bit int is stored in little- or big-endian format.</param>
		public void SetInt32(int byteOffset, int value, bool littleEndian = false)
		{
			var bytes = BitConverter.GetBytes(value);
			if (BitConverter.IsLittleEndian ^ littleEndian)
				Array.Reverse(bytes);
			Array.Copy(bytes, 0, Buffer.Data, byteOffset, bytes.Length);
		}

		/// <summary>
		/// Stores a unsigned 16-bit integer (short) value at the specified byte offset from the start of the DataView.
		/// </summary>
		/// <param name="byteOffset">The offset, in byte, from the start of the view where to store the data.</param>
		/// <param name="value">The value to set.</param>
		/// <param name="littleEndian">Indicates whether the 16-bit int is stored in little- or big-endian format.</param>
		public void SetUint16(int byteOffset, short value, bool littleEndian = false)
		{
			var bytes = BitConverter.GetBytes(value);
			if (BitConverter.IsLittleEndian ^ littleEndian)
				Array.Reverse(bytes);
			Array.Copy(bytes, 0, Buffer.Data, byteOffset, bytes.Length);
		}

		/// <summary>
		/// Stores a unsigned 32-bit integer (short) value at the specified byte offset from the start of the DataView.
		/// </summary>
		/// <param name="byteOffset">The offset, in byte, from the start of the view where to store the data.</param>
		/// <param name="value">The value to set.</param>
		/// <param name="littleEndian">Indicates whether the 32-bit int is stored in little- or big-endian format.</param>
		public void SetUint32(int byteOffset, int value, bool littleEndian = false)
		{
			var bytes = BitConverter.GetBytes(value);
			if (BitConverter.IsLittleEndian ^ littleEndian)
				Array.Reverse(bytes);
			Array.Copy(bytes, 0, Buffer.Data, byteOffset, bytes.Length);
		}

		/// <summary>
		/// Stores a signed 8-bit integer (byte) value at the specified byte offset from the start of the DataView.
		/// </summary>
		/// <param name="byteOffset">The offset, in byte, from the start of the view where to store the data.</param>
		/// <param name="value">The value to set.</param>
		public void SetInt8(int byteOffset, sbyte value, bool littleEndian = false)
			=>	Buffer.Data[byteOffset] = (byte)value;

		/// <summary>
		/// Stores a signed 8-bit integer (byte) value at the specified byte offset from the start of the DataView.
		/// </summary>
		/// <param name="byteOffset">The offset, in byte, from the start of the view where to store the data.</param>
		/// <param name="value">The value to set.</param>
		public void SetUint8(int byteOffset, byte value, bool littleEndian = false)
			=> Buffer.Data[byteOffset] = value;
	}
}
