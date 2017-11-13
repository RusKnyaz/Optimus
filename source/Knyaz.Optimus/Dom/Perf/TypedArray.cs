using System;
using System.Runtime.InteropServices;

namespace Knyaz.Optimus.Dom.Perf
{
	public class TypedArray<T>
	{
		public TypedArray(ArrayBuffer buffer)
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

		public ulong Length {get { return (ulong) (_data.Length/ _bytesPerElement); }}

		public void Set(TypedArray<T> array, int offset)
		{
			Buffer.BlockCopy(array._data, 0, _data, offset * _bytesPerElement, array._data.Length);
		}
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
    
	}

	public class Int8Array : TypedArray<sbyte>
	{
		public static int BYTES_PER_ELEMENT = Marshal.SizeOf(typeof(sbyte));

		public Int8Array(ArrayBuffer buffer) : base(buffer) { }

		public Int8Array(sbyte[] data) : base(data) { }
		public Int8Array Subarray(long begin, long? end = null)
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

		public short this[ulong index]
		{
			get
			{
				if (index < 0 || index >= Length)
					return 0;
				
				return BitConverter.ToInt16(_data, (int)index * _bytesPerElement);
			}
			set
			{
				if (index < 0 || index >= Length)
					return;
				
				var bytes = BitConverter.GetBytes(value);
				Array.Copy(bytes, 0, _data, (int)index * _bytesPerElement, bytes.Length);
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

		public ushort this[ulong index]
		{
			get
			{
				if (index < 0 || index >= Length)
					return 0;
				
				return BitConverter.ToUInt16(_data, (int)index * _bytesPerElement);
			}
			set
			{
				if (index < 0 || index >= Length)
					return;
				
				var bytes = BitConverter.GetBytes(value);
				Array.Copy(bytes, 0, _data, (int)index*_bytesPerElement, bytes.Length);
			}
		}
	}
}
