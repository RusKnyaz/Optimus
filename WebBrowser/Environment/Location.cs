namespace WebBrowser.Environment
{
	public class Location
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
