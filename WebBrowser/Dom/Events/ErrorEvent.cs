using WebBrowser.Dom.Elements;

namespace WebBrowser.Dom.Events
{
	public class ErrorEvent : Event
	{
		public string Message { get; private set; }
		public string Filename { get; private set; }
		public ulong Lineno { get; private set;}
		public ulong Colno { get; private set; }
		public object Error { get; private set; }

		public void ErrorEventInit(string message, string filename, ulong lineno, ulong colno, object error)
		{
			Message = message;
			Filename = filename;
			Lineno = lineno;
			Colno = colno;
			Error = error;
		}
	}
}
