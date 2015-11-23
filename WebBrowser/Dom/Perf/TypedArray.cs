using System;
using System.Runtime.InteropServices;

namespace WebBrowser.Dom.Perf
{
	//todo: write tests
	public class TypedArray<T>
	{
		public TypedArray(ulong size)
		{
			_data = new T[size];
		}

		protected TypedArray(T[] data)
		{
			_data = data;
		} 

		protected T[] _data;

		public ulong BYTES_PER_ELEMENT {get { return (ulong) Marshal.SizeOf(typeof(T)); }}

		public ulong Length {get { return (ulong) _data.Length; }}

		public T this[ulong index]
		{
			get { return _data[index]; }
			set { _data[index] = value; }
		}

		public void Set(TypedArray<T> array, ulong offset)
		{
			array._data.CopyTo(_data, (long)offset);
		}
		public void Set(T[] array, ulong offset)
		{
			array.CopyTo(_data, (long)offset);
		}

		protected T[] GetSub(long begin, long? end)
		{
			var cnt = (end ?? (_data.Length - 1)) - begin;
			var result = new T[cnt];
			Array.Copy(_data, begin, result, 0, cnt);
			return result;
		}
    
	}

	public class Int8Array : TypedArray<sbyte>
	{
		public Int8Array(ulong size) : base(size){}

		protected Int8Array(sbyte[] data) : base(data) { }
		public Int8Array Subarray(long begin, long? end)
		{
			return new Int8Array(GetSub(begin, end));
		}
	}

	public class UInt8Array : TypedArray<byte>
	{
		public UInt8Array(ulong size) : base(size){}

		protected UInt8Array(byte[] data) : base(data) { }
		public UInt8Array Subarray(long begin, long? end)
		{
			return new UInt8Array(GetSub(begin, end));
		}
	}

	public class Int16Array : TypedArray<short>
	{
		public Int16Array(ulong size) : base(size) { }

		protected Int16Array(short[] data) : base(data) { }
		public Int16Array Subarray(long begin, long? end)
		{
			return new Int16Array(GetSub(begin, end));
		}
	}

	public class UInt16Array : TypedArray<ushort>
	{
		public UInt16Array(ulong size) : base(size) { }

		protected UInt16Array(ushort[] data) : base(data) { }
		public UInt16Array Subarray(long begin, long? end)
		{
			return new UInt16Array(GetSub(begin, end));
		}
	}
}
