namespace Knyaz.Optimus.Dom.Events
{
	public class ProgressEvent : Event
	{
		public bool LengthComputable { get; private set; }
		public ulong Loaded { get; private set; }
		public ulong Total { get; private set; }

		public ProgressEvent(string type):base(type)
		{
			
		}

		public void InitProgressEvent(bool lengthComputable, ulong loaded, ulong total)
		{
			LengthComputable = lengthComputable;
			Loaded = loaded;
			Total = total;
		}
	}
}
