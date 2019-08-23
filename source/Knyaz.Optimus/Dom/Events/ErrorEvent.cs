namespace Knyaz.Optimus.Dom.Events
{
	/// <summary>
	/// Represents events providing information related to errors in scripts or in files.
	/// </summary>
	public class ErrorEvent : Event
	{
		internal ErrorEvent(Document owner) : base(owner){}
		
		/// <summary>
		/// Gets a human-readable error message describing the problem.
		/// </summary>
		public string Message { get; private set; }
		
		/// <summary>
		/// Gets the name of the script file in which the error occurred.
		/// </summary>
		public string Filename { get; private set; }
		
		/// <summary>
		/// Gets the line number of the script file on which the error occurred.
		/// </summary>
		public ulong Lineno { get; private set;}
		
		/// <summary>
		/// Gets the column number of the script file on which the error occurred.
		/// </summary>
		public ulong Colno { get; private set; }
		
		/// <summary>
		/// Gets the Object that is concerned by the event.
		/// </summary>
		public object Error { get; private set; }

		/// <summary>
		/// Initializes the event with a given values.
		/// </summary>
		internal void ErrorEventInit(string message, string filename, ulong lineno, ulong colno, object error)
		{
			Message = message;
			Filename = filename;
			Lineno = lineno;
			Colno = colno;
			Error = error;
		}
	}
}
