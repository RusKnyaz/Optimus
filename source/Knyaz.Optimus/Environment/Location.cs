using Knyaz.Optimus.Dom.Interfaces;
using System;
using Knyaz.Optimus.ScriptExecuting;

namespace Knyaz.Optimus.Environment
{
	/// <summary>
	/// Represents the location (URL) of the document.
	/// </summary>
	[DomItem]
	public class Location
	{
		private readonly IEngine _engine;
		private readonly IHistory _history;

		internal Location(IEngine engine, IHistory history)
		{
			_engine = engine;
			_history = history;
		}

		/// <summary>
		/// Gets or sets the entire URL. If changed, the associated document navigates to the new page.
		/// </summary>
		public string Href
		{
			get => _engine.Uri.OriginalString;
			set => _engine.OpenUrl(value);
		}

		/// <summary>
		/// Anchor part of a URL (text which follows '#')
		/// </summary>
		public string Hash
		{
			get => _engine.Uri.Fragment;
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

		/// <summary>
		/// Gets or sets the host, that is the hostname, a ':', and the port of the URL.
		/// </summary>
		public string Host
		{
			get => _engine.Uri.Authority;
			set
			{
				var parts = value.Split(':');
				var builder = new UriBuilder(_engine.Uri){ Host = parts[0], Port = parts.Length > 1 ? int.Parse(parts[1]) : 80};
				_engine.OpenUrl(builder.Uri.ToString());
			}
		}
		
		/// <summary>
		/// Gets or sets the domain of the URL.
		/// </summary>
		public string Hostname
		{
			get => _engine.Uri.Host;
			set => _engine.OpenUrl(new UriBuilder(_engine.Uri) {Host = value}.Uri.ToString());
		}
		
		/// <summary>
		/// Gets or sets the canonical form of the origin of the specific location.
		/// </summary>
		public string Origin
		{
			get => _engine.Uri.GetLeftPart(UriPartial.Authority);
			set => _engine.OpenUrl(new UriBuilder(new Uri(value)){Path = _engine.Uri.PathAndQuery,Fragment = _engine.Uri.Fragment.TrimStart('#')}.ToString());
		}

		/// <summary>
		/// Gets or sets the string ontaining an initial '/' followed by the path of the URL.
		/// </summary>
		public string Pathname
		{
			get => _engine.Uri.PathAndQuery;
			set
			{
				var u = new Uri(new Uri(Origin), value);
				var ub = new UriBuilder(u.ToString()) {Fragment = _engine.Uri.Fragment.TrimStart('#')};
				if (ub.Uri.IsDefaultPort)
					ub.Port = -1;
				_engine.OpenUrl(ub.ToString());
			}
		}

		/// <summary>
		/// Gets or sets the port number of the URL.
		/// </summary>
		public int Port
		{
			get => _engine.Uri.Port;
			set => _engine.OpenUrl(new UriBuilder(_engine.Uri) {Port = value}.Uri.ToString());
		}

		/// <summary>
		/// Gets or sets the protocol scheme of the URL, including the final ':'.
		/// </summary>
		public string Protocol
		{
			get => _engine.Uri.Scheme + ":";
			set => _engine.OpenUrl(new UriBuilder(_engine.Uri) { Scheme = value }.Uri.ToString());
		}

		/// <summary>
		/// Gets or sets a string containing a '?' followed by the parameters or "querystring" of the URL.
		/// </summary>
		public string Search
		{
			get => _engine.Uri.Query;
			set => _engine.OpenUrl(new UriBuilder(_engine.Uri) {Query = value}.Uri.ToString());
		}
		
		/// <summary>
		/// Loads the resource at the URL provided in parameter.
		/// </summary>
		/// <param name="url">The URL of the page to navigate to.</param>
		public void Assign(string url)
		{
			_history.PushState(null, null, url );
			//todo: load the page
		}

		/// <summary>
		/// Replaces the current resource with the one at the provided URL. 
		/// The difference from the assign() method is that after using Replace() the current page will not be saved
		/// in session History, meaning the user won't be able to use the back button to navigate to it.
		/// </summary>
		/// <param name="url"></param>
		public void Replace(string url)
		{
			_history.ReplaceState(null, null, url );
			//todo: reload page
		}

		/// <summary>
		/// Reloads the resource from the current URL.
		/// </summary>
		/// <param name="force">If <c>true</c>, the page to be reloaded from the server. Othervise, the engine may reload the page from its cache.</param>
		public void Reload(bool force = false)
		{
			_engine.OpenUrl(_engine.Uri.ToString());
		}
	}
}
