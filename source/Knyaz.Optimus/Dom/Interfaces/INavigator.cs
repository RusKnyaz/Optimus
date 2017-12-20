using Knyaz.Optimus.ScriptExecuting;

namespace Knyaz.Optimus.Dom.Interfaces
{
	[DomItem]
	public interface INavigator
	{
		string AppCodeName { get; }
		string AppName { get; }
		string AppVersion { get; }
		bool CookieEnabled { get; }
		string Geolocation { get; }
		bool OnLine { get; }
		string Platform { get; }
		string Product { get; }
		string UserAgent { get; }
		bool JavaEnabled();
		string Language { get; }
	}
}
