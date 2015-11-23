using System;
using System.Runtime.InteropServices;

namespace WebBrowser.Dom.Perf
{
	//todo: write tests
	public class TypedArray<T>
	{
		public TypedArray(ArrayBuffer buffer)
		{
			_data = buffer.Data;
		}

		protected TypedArray(T[] data)
		{
			_data = new byte[data.Length * BytesPerElement];
			Buffer.BlockCopy(data,0, _data, 0, _data.Length);
		} 

		protected byte[] _data;

		private int BytesPerElement {get { return Marshal.SizeOf(typeof(T)); }}

		public ulong Length {get { return (ulong) (_data.Length/BytesPerElement); }}

		
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
		public static int BYTES_PER_ELEMENT = Marshal.SizeOf(typeof(sbyte));

		public Int8Array(ArrayBuffer buffer) : base(buffer) { }

		public Int8Array(sbyte[] data) : base(data) { }
		public Int8Array Subarray(long begin, long? end)
		{
			return new Int8Array(GetSub(begin, end));
		}

		public sbyte this[ulong index]
		{
			get
			{
				checked
				{
					return (sbyte)_data[index];
				}
			}
			set
			{
				checked
				{
					_data[index] = (byte) value;
				}
			}
		}
	}

	public class UInt8Array : TypedArray<byte>
	{
		public UInt8Array(ArrayBuffer buffer) : base(buffer) { }

		protected UInt8Array(byte[] data) : base(data) { }
		public UInt8Array Subarray(long begin, long? end)
		{
			return new UInt8Array(GetSub(begin, end));
		}

		public byte this[ulong index]
		{
			get { return _data[index]; }
			set { _data[index] = value; }
		}
	}

	public class Int16Array : TypedArray<short>
	{
		public Int16Array(ArrayBuffer buffer) : base(buffer) { }

		protected Int16Array(short[] data) : base(data) { }
		public Int16Array Subarray(long begin, long? end)
		{
			return new Int16Array(GetSub(begin, end));
		}

		public unsafe short this[ulong index]
		{
			get
			{
				//todo: check the limits
				fixed (byte* pBuffer = _data)
				{
					return ((short*)pBuffer)[index];
				}
			}
			set
			{
				//todo: check the limits
				fixed (byte* pBuffer = _data)
				{
					((short*)pBuffer)[index] = value;
				}
			}
		}
	}

	public class UInt16Array : TypedArray<ushort>
	{
		public UInt16Array(ArrayBuffer buffer) : base(buffer) { }

		protected UInt16Array(ushort[] data) : base(data) { }
		public UInt16Array Subarray(long begin, long? end)
		{
			return new UInt16Array(GetSub(begin, end));
		}
	}
}
