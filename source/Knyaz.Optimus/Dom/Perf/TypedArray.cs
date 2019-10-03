using System;
using System.Linq;
using System.Runtime.InteropServices;
using Knyaz.Optimus.ScriptExecuting;

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
				if (index >= Length)
					return default(T);

				return GetData((int)index * _bytesPerElement);
			}
			set
			{
				if (index >= Length)
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
		protected override byte[] GetBytes(sbyte val) => new [] { (byte)val };
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
	[JsName("Uint8Array")]
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
	/// 32-bit integer array.
	/// </summary>
	public class Int32Array : TypedArray<int>
	{
		public Int32Array(ArrayBuffer buffer) : base(buffer) { }
		public Int32Array(object[] data) : base(data.Select(FromObject).ToArray()) { }
		public Int32Array(int[] data) : base(data) { }
		public Int32Array Subarray(long begin, long? end) => new Int32Array(GetSub(begin, end));
		protected override int GetData(int index)	=> BitConverter.ToInt32(_data, index);
		protected override byte[] GetBytes(int val) => BitConverter.GetBytes(val);
		public static string Name => "Int8Array";

		static int FromObject(object val)
		{
			if (val is double d)
				return (int)d;
			else if (val is int u)
				return u;

			return Convert.ToInt32(val);
		}
	}
	
	/// <summary>
	/// 32-bit integer array.
	/// </summary>
	public class UInt32Array : TypedArray<uint>
	{
		public UInt32Array(ArrayBuffer buffer) : base(buffer) { }
		public UInt32Array(object[] data) : base(data.Select(FromObject).ToArray()) { }
		public UInt32Array(uint[] data) : base(data) { }
		public UInt32Array Subarray(long begin, long? end) => new UInt32Array(GetSub(begin, end));
		protected override uint GetData(int index)	=> BitConverter.ToUInt32(_data, index);
		protected override byte[] GetBytes(uint val) => BitConverter.GetBytes(val);
		public static string Name => "Int8Array";

		static uint FromObject(object val)
		{
			if (val is double d)
				return (uint)d;
			else if (val is uint u)
				return u;

			return Convert.ToUInt32(val);
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

	/// <summary>
	/// float 32 (single) array.
	/// </summary>
	public class Float32Array : TypedArray<float>
	{
		public Float32Array(ArrayBuffer buffer) : base(buffer) { }
		public Float32Array(object[] data) : base(data.Select(FromObject).ToArray()) { }
		public Float32Array(float[] data) : base(data) { }
		public Float32Array Subarray(long begin, long? end) => new Float32Array(GetSub(begin, end));
		protected override float GetData(int index) => BitConverter.ToSingle(_data, index);
		protected override byte[] GetBytes(float val) => BitConverter.GetBytes(val);
		public static string Name => "Float32Array";

		static float FromObject(object val)
		{
			switch (val)
			{
				case double d: return (float)d;
				case float u: return u;
				default: return Convert.ToSingle(val);
			}
		}
	}
	
	/// <summary>
	/// float 64 (double) array.
	/// </summary>
	public class Float64Array : TypedArray<double>
	{
		public Float64Array(ArrayBuffer buffer) : base(buffer) { }
		public Float64Array(object[] data) : base(data.Select(FromObject).ToArray()) { }
		public Float64Array(double[] data) : base(data) { }
		public Float64Array Subarray(long begin, long? end) => new Float64Array(GetSub(begin, end));
		protected override double GetData(int index) => BitConverter.ToDouble(_data, index);
		protected override byte[] GetBytes(double val) => BitConverter.GetBytes(val);
		public static string Name => "Float64Array";

		static double FromObject(object val)
		{
			switch (val)
			{
				case double d: return d;
				case float u: return u;
				default: return Convert.ToSingle(val);
			}
		}
	}
}
