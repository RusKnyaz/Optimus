namespace WebBrowser.Dom.Perf
{
	public class ArrayBuffer
	{
		readonly internal byte[] Data;

		public ArrayBuffer(int size)
		{
			Data = new byte[size];
		}
	}
}