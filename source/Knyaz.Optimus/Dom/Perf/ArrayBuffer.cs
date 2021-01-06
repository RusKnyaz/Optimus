namespace Knyaz.Optimus.Dom.Perf
{
	/// <summary>
	/// Represents a generic, fixed-length raw binary data buffer.
	/// </summary>
	/// <remarks>
	///  You cannot directly manipulate the contents of an ArrayBuffer; instead, you create one of the typed array objects or a DataView object which represents the buffer in a specific format, and use that to read and write the contents of the buffer.
	///  </remarks>
	public class ArrayBuffer
	{
		readonly internal byte[] Data;
		
		internal ArrayBuffer(byte[] data) => Data = data;

		public ArrayBuffer() :this(0){}
		
		/// <summary>
		/// Creates new instance of <see cref="ArrayBuffer"/>
		/// </summary>
		/// <param name="size">The size, in bytes, of the array buffer to create.</param>
		public ArrayBuffer(int size) => Data = new byte[size];

		/// <summary>
		/// JS compatible constructor.
		/// </summary>
		/// <param name="size">The size, in bytes, of the array buffer to create.</param>
		public ArrayBuffer(double size) : this((int)size) { }

		/// <summary>
		/// Returns <c>true</c> if arg is one of the ArrayBuffer views, such as typed array objects or a DataView. Returns <c>false</c> otherwise.
		/// </summary>
		public static bool IsView(object data)
			=> data != null && data.GetType().IsSubclassOf(typeof(TypedArray<>)) /*|| data is DataView*/;

		/// <summary>
		/// Gets length of buffer in bytes.
		/// </summary>
		public int ByteLength => Data.Length;
	}
}