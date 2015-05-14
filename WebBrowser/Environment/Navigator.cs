namespace WebBrowser.Environment
{
	/// <summary>
	/// http://www.w3schools.com/jsref/obj_navigator.asp
	/// </summary>
	public class Navigator
	{
		public string AppCodeName{get { return "Tinto Browser"; }}
		public string AppName{get { return "Tinto"; }}
		public string AppVersion{get { return "1.0"; }}
		public bool CookieEnabled{get{ return true; }}
		public string Geolocation{get { return null; /*todo*/ }}
		public bool OnLine{get { return true; }}
		public string Platform{get { return ".NET"; /*todo*/ }}
		public string Product{get { return "Tinto"; }}
		public string UserAgent{get { return "Tinto"; /*todo*/ }}

		public bool JavaEnabled()
		{
			return true;
		}

	}
}
