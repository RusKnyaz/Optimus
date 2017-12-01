using Knyaz.Optimus.Dom.Interfaces;

namespace Knyaz.Optimus.Environment
{
	/// <summary>
	/// http://www.w3schools.com/jsref/obj_navigator.asp
	/// </summary>
	public class Navigator : INavigator
	{
		public string AppCodeName { get { return "Optimus Browser"; } }
		public string AppName { get { return "Optimus"; } }
		public string AppVersion{get { return "1.0"; }}
		public bool CookieEnabled{get{ return true; }}
		public string Geolocation{get { return null; /*todo*/ }}
		public bool OnLine{get { return true; }}
		public string Platform{get { return ".NET"; /*todo*/ }}
		public string Product { get { return "Optimus"; } }
		public string UserAgent{get { return "Optimus"; /*todo*/ }}

		public bool JavaEnabled() => true;
	}
}
