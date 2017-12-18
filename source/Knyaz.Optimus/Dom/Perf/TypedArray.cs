using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace Knyaz.Optimus.Dom.Perf
{
	/// <summary>
	/// Base class for typed arrays.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class TypedArray<T>
	{
		protected TypedArray(ArrayBuffer buffer)
		{
			if (buffer.Data.Length % _bytesPerElement != 0)
				throw new ArgumentOutOfRangeException("byte length of "+GetType().Name+" should be a multiple of " + _bytesPerElement);

			_data = buffer.Data;
		}

		protected TypedArray(T[] data)
		{
			_data = new byte[data.Length * _bytesPerElement];
			Buffer.BlockCopy(data,0, _data, 0, _data.Length);
		} 

		protected byte[] _data;

		protected int _bytesPerElement = Marshal.SizeOf(typeof(T));

		/// <summary>
		/// Returns the length of the typed array from the start of its ArrayBuffer.
		/// </summary>
		public ulong Length => (ulong) (_data.Length/ _bytesPerElement);

		/// <summary>
		/// Stores multiple values in the typed array.
		/// </summary>
		/// <remarks>
		/// Two arrays may share the same underlying ArrayBuffer; the browser will intelligently copy the source range of the buffer to the destination range.
		/// </remarks>
		/// <param name="array">The array from which to copy values. </param>
		/// <param name="offset">The offset into the target array at which to begin writing values from the source array. If you omit this value, 0 is assumed (that is, the source array will overwrite values in the target array starting at index 0).</param>
		public void Set(TypedArray<T> array, int offset)
		{
			//todo: write tests for the case when two arrays share the same ArrayBuffer.

			Buffer.BlockCopy(array._data, 0, _data, offset * _bytesPerElement, array._data.Length);
		}

		/// <summary>
		/// Stores multiple values in the typed array.
		/// </summary>
		/// <remarks>
		/// All values from the source array are copied into the target array, unless the length of the source array plus the offset exceeds the length of the target array, in which case an exception is thrown.
		/// </remarks>
		/// <param name="array">The array from which to copy values. </param>
		/// <param name="offset">The offset into the target array at which to begin writing values from the source array. If you omit this value, 0 is assumed (that is, the source array will overwrite values in the target array starting at index 0).</param>
		public void Set(T[] array, int offset)
		{
			Buffer.BlockCopy(array, 0, _data, offset * _bytesPerElement, array.Length * _bytesPerElement);
		}

		protected T[] GetSub(long begin, long? end)
		{
			var cnt = (end ?? (_data.Length - 1)) - begin;
			var result = new T[cnt];
			Array.Copy(_data, begin, result, 0, cnt);
			return result;
		}

		/// <summary>
		/// Gets or sets the value from/to specified position.
		/// </summary>
		/// <param name="index"></param>
		public T this[ulong index]
		{
			get
			{
				if (index < 0 || index >= Length)
					return default(T);

				return GetData((int)index * _bytesPerElement);
			}
			set
			{
				if (index < 0 || index >= Length)
					return;

				var bytes = GetBytes(value);
				Array.Copy(bytes, 0, _data, (int)index * _bytesPerElement, bytes.Length);
			}
		}

		protected abstract T GetData(int index);
		protected abstract byte[] GetBytes(T val);
	}

	/// <summary>
	/// 8-bit two's complement signed integer array.
	/// </summary>
	public class Int8Array : TypedArray<sbyte>
	{
		public Int8Array(ArrayBuffer buffer) : base(buffer) { }
		public Int8Array(sbyte[] data) : base(data) { }
		public Int8Array(object[] data) : base(data.Select(FromObject).ToArray()) { }
		public Int8Array Subarray(long begin, long? end = null)	=> new Int8Array(GetSub(begin, end));
		protected override sbyte GetData(int index)	=> (sbyte)_data[index];
		protected override byte[] GetBytes(sbyte val) => new byte[] { (byte)val };
		public static string Name => "Int8Array";

		static sbyte FromObject(object val)
		{
			if (val is double d)
				return (sbyte)d;
			else if (val is sbyte u)
				return u;

			return Convert.ToSByte(val);
		}
	}

	/// <summary>
	/// 8-bit unsigned integer array.
	/// </summary>
	public class UInt8Array : TypedArray<byte>
	{
		public UInt8Array(ArrayBuffer buffer) : base(buffer) { }
		public UInt8Array(object[] data) : base(data.Select(FromObject).ToArray()) { }
		public UInt8Array(byte[] data) : base(data) { }

		public UInt8Array Subarray(long begin, long? end) => new UInt8Array(GetSub(begin, end));

		protected override byte GetData(int index) => _data[index];
		protected override byte[] GetBytes(byte val) => new[] { val };
		public static string Name => "Uint8Array";

		static byte FromObject(object val)
		{
			if (val is double d)
				return (byte)d;
			else if (val is byte u)
				return u;

			return Convert.ToByte(val);
		}
	}

	/// <summary>
	/// 16-bit two's complement signed integer array.
	/// </summary>
	public class Int16Array : TypedArray<short>
	{
		public Int16Array(ArrayBuffer buffer) : base(buffer) { }
		public Int16Array(object[] data) : base(data.Select(FromObject).ToArray()) { }
		public Int16Array(short[] data) : base(data) { }
		public Int16Array Subarray(long begin, long? end) => new Int16Array(GetSub(begin, end));
		protected override short GetData(int index)	=> BitConverter.ToInt16(_data, index);
		protected override byte[] GetBytes(short val) => BitConverter.GetBytes(val);
		public static string Name => "Int8Array";

		static short FromObject(object val)
		{
			if (val is double d)
				return (short)d;
			else if (val is short u)
				return u;

			return Convert.ToInt16(val);
		}
	}

	/// <summary>
	/// 16-bit unsigned integer
	/// </summary>
	public class UInt16Array : TypedArray<ushort>
	{
		public UInt16Array(ArrayBuffer buffer) : base(buffer) { }
		public UInt16Array(object[] data) : base(data.Select(FromObject).ToArray()) { }
		public UInt16Array(ushort[] data) : base(data) { }
		public UInt16Array Subarray(long begin, long? end) => new UInt16Array(GetSub(begin, end));
		protected override ushort GetData(int index) => BitConverter.ToUInt16(_data, index);
		protected override byte[] GetBytes(ushort val) => BitConverter.GetBytes(val);
		public static string Name => "Uint16Array";

		static ushort FromObject(object val)
		{
			if(val is double d)
				return (ushort)d;
			else if(val is ushort u)
				return u;

			return Convert.ToUInt16(val);
		}
	}
}
