namespace Knyaz.Optimus.Dom.Events
{
	/// <summary>
	/// Represents events measuring progress of an underlying process, like an HTTP request 
	/// (for an XMLHttpRequest, or the loading of the underlying resource of an &lt;img&gt;, &lt;audio&gt; and other).
	/// </summary>
	public class ProgressEvent : Event
	{
		/// <summary>
		/// Indicates if the progress is measurable or not.
		/// </summary>
		public bool LengthComputable { get; private set; }
		
		/// <summary>
		/// The amount of work already performed by the underlying process. 
		/// </summary>
		public ulong Loaded { get; private set; }
		
		/// <summary>
		/// The total amount of work that the underlying process is in the progress of performing.
		/// </summary>
		public ulong Total { get; private set; }

		/// <summary>
		/// Creates a new ProgressEvent with a given type.
		/// </summary>
		public ProgressEvent(string type, Document owner):base(type, owner) {}

		/// <summary>
		/// Initializes the vent.
		/// </summary>
		public void InitProgressEvent(bool lengthComputable, ulong loaded, ulong total)
		{
			LengthComputable = lengthComputable;
			Loaded = loaded;
			Total = total;
		}
	}
}
