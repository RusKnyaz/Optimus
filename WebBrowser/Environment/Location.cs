using WebBrowser.ScriptExecuting;

namespace WebBrowser.Environment
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
		string Port { get; set; }
		string Protocol { get; set; }
		string Search { get; set; }
	}

	public class Location : ILocation
	{
		public string Href { get; set; }
		public string Hash { get; set; }
		public string Host { get; set; }
		public string Hostname { get; set; }
		public string Origin { get; private set; }
		public string Pathname { get; set; }
		public string Port { get; set; }
		public string Protocol { get; set; }
		public string Search { get; set; }
	}
}
