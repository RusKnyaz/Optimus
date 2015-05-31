using System;
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
		int Port { get; set; }
		string Protocol { get; }
		string Search { get; set; }
		void Assign(string uri);
		void Replace();
		void Reload(string uri);
	}

	public class Location : ILocation
	{
		private readonly Engine _engine;

		public Location(Engine engine)
		{
			_engine = engine;
		}

		public string Href
		{
			get
			{
				return _engine.Uri.OriginalString;
			} 
			set
			{
				_engine.OpenUrl(value);	
			}
		}

		public string Hash
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public string Host
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}
		public string Hostname
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}
		public string Origin
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}
		public string Pathname
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public int Port
		{
			get { return _engine.Uri.Port; }
			set
			{
				throw new NotImplementedException();
			}
		}

		public string Protocol
		{
			get { return _engine.Uri.Scheme + ":"; }
			set
			{
				throw new NotImplementedException();
			}
		}

		public string Search
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}
		public void Assign(string uri)
		{
			throw new NotImplementedException();
		}

		public void Replace()
		{
			throw new NotImplementedException();
		}

		public void Reload(string uri)
		{
			throw new NotImplementedException();
		}
	}
}
