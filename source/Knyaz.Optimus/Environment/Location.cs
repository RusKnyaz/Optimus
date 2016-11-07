using System;
using Knyaz.Optimus.ScriptExecuting;

namespace Knyaz.Optimus.Environment
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
		void Reload(string uri);
	}

	public class Location : ILocation
	{
		private readonly IEngine _engine;

		public Location(IEngine engine)
		{
			_engine = engine;
		}

		public string Href
		{
			get { return _engine.Uri.OriginalString; } 
			set { _engine.OpenUrl(value);	}
		}

		/// <summary>
		/// Anchor part of a URL (text which follows '#')
		/// </summary>
		public string Hash
		{
			get { return _engine.Uri.Fragment;}
			set
			{
				var hash = string.IsNullOrEmpty(value)
					? string.Empty
					: value.StartsWith("#") ? value : "#" + value;

				var splt = _engine.Uri.OriginalString.Split('#');
				//todo: do not reopen page, modify history
				_engine.OpenUrl(splt[0] + hash);
			}
		}

		public string Host
		{
			get
			{
				return _engine.Uri.Authority;
			}
			set
			{
				var parts = value.Split(':');
				var builder = new UriBuilder(_engine.Uri){ Host = parts[0], Port = parts.Length > 1 ? int.Parse(parts[1]) : 80};
				_engine.OpenUrl(builder.Uri.ToString());
			}
		}
		public string Hostname
		{
			get { return _engine.Uri.Host;}
			set
			{
				_engine.OpenUrl(new UriBuilder(_engine.Uri) {Host = value}.Uri.ToString());
			}
		}
		public string Origin
		{
			get
			{
				return _engine.Uri.GetLeftPart(UriPartial.Authority);
			}
			set
			{
				var b = new UriBuilder(new Uri(value)){Path = _engine.Uri.PathAndQuery,Fragment = _engine.Uri.Fragment.TrimStart('#')};
				_engine.OpenUrl(b.ToString());
			}
		}

		//The relative path with query but without the hash
		public string Pathname
		{
			get { return _engine.Uri.PathAndQuery; }
			set
			{
				var u = new Uri(new Uri(Origin), value);
				var ub = new UriBuilder(u.ToString()) {Fragment = _engine.Uri.Fragment.TrimStart('#')};
				if (ub.Uri.IsDefaultPort)
					ub.Port = -1;
				_engine.OpenUrl(ub.ToString());
			}
		}

		public int Port
		{
			get { return _engine.Uri.Port; }
			set
			{
				_engine.OpenUrl(new UriBuilder(_engine.Uri) {Port = value}.Uri.ToString());
			}
		}

		public string Protocol
		{
			get { return _engine.Uri.Scheme + ":"; }
			set
			{
				_engine.OpenUrl(new UriBuilder(_engine.Uri) { Scheme = value }.Uri.ToString());
			}
		}

		public string Search
		{
			get
			{
				return _engine.Uri.Query;
			}
			set
			{
				var b = new UriBuilder(_engine.Uri) {Query = value};
				_engine.OpenUrl(b.Uri.ToString());
			}
		}
		public void Assign(string uri)
		{
			throw new NotImplementedException();
		}

		public void Replace(string uri)
		{
			throw new NotImplementedException();
		}

		public void Reload(string uri)
		{
			throw new NotImplementedException();
		}
	}
}
