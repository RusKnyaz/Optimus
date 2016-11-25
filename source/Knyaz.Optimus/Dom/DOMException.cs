using System;

namespace Knyaz.Optimus.Dom
{
	public class DOMException : Exception
	{
		private readonly Codes _code;

		public enum Codes
		{
			NoModificationAllowedError = 7,
			InvalidStateError = 11
		}

		internal DOMException(Codes code)
		{
			_code = code;
			Name = Enum.GetName(typeof(Codes), code);
		}

		public int Code { get { return (int)_code; } }
		public string Name { get; private set; }
	}
}
