using System;

namespace Knyaz.Optimus.Dom
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
