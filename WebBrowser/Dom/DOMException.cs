using System;

namespace WebBrowser.Dom
{
	public class DOMException : Exception
	{
		public enum Codes
		{
			InvalidStateError = 11
		}

		internal DOMException(Codes code)
		{
			
		}
	}
}
