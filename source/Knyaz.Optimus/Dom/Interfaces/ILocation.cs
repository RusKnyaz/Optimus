using Knyaz.Optimus.ScriptExecuting;

namespace Knyaz.Optimus.Dom.Interfaces
{
	[DomItem]
	public interface ILocation
	{
		string Href { get; set; }
		string Hash { get; set; }
		string Host { get; set; }
		string Hostname { get; set; }
		string Origin { get; }
		string Pathname { get; set; }
		int Port { get; set; }
		string Protocol { get; }
		string Search { get; set; }
		void Assign(string uri);
		void Replace(string uri);
		void Reload(bool force);
	}
}
