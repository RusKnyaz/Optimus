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
		private readonly IHistory _history;
		private readonly Func<Uri> _getUri;
		private readonly Action<string> _openUri;

		private Uri Uri => _getUri();

		internal Location(IHistory history, Func<Uri> getUri, Action<string> openUri)
		{
			_history = history;
			_getUri = getUri;
			_openUri = openUri;
		}

		/// <summary>
		/// Gets or sets the entire URL. If changed, the associated document navigates to the new page.
		/// </summary>
		public string Href
		{
			get => Uri.OriginalString;
			set => _openUri(value);
		}

		/// <summary>
		/// Anchor part of a URL (text which follows '#')
		/// </summary>
		public string Hash
		{
			get => Uri.Fragment;
			set
			{
				var hash = string.IsNullOrEmpty(value)
					? string.Empty
					: value.StartsWith("#") ? value : "#" + value;

				var splt = _getUri().OriginalString.Split('#');
				//todo: do not reopen page, modify history
				_openUri(splt[0] + hash);
			}
		}

		/// <summary>
		/// Gets or sets the host, that is the hostname, a ':', and the port of the URL.
		/// </summary>
		public string Host
		{
			get => Uri.Authority;
			set
			{
				var parts = value.Split(':');
				var builder = new UriBuilder(Uri){ Host = parts[0], Port = parts.Length > 1 ? int.Parse(parts[1]) : 80};
				_openUri(builder.Uri.ToString());
			}
		}
		
		/// <summary>
		/// Gets or sets the domain of the URL.
		/// </summary>
		public string Hostname
		{
			get => Uri.Host;
			set => _openUri(new UriBuilder(Uri) {Host = value}.Uri.ToString());
		}
		
		/// <summary>
		/// Gets or sets the canonical form of the origin of the specific location.
		/// </summary>
		public string Origin
		{
			get => Uri.GetLeftPart(UriPartial.Authority);
			set => _openUri(new UriBuilder(new Uri(value)){Path = Uri.PathAndQuery,Fragment = Uri.Fragment.TrimStart('#')}.ToString());
		}

		/// <summary>
		/// Gets or sets the string ontaining an initial '/' followed by the path of the URL.
		/// </summary>
		public string Pathname
		{
			get => Uri.PathAndQuery;
			set
			{
				var u = new Uri(new Uri(Origin), value);
				var ub = new UriBuilder(u.ToString()) {Fragment = Uri.Fragment.TrimStart('#')};
				if (ub.Uri.IsDefaultPort)
					ub.Port = -1;
				_openUri(ub.ToString());
			}
		}

		/// <summary>
		/// Gets or sets the port number of the URL.
		/// </summary>
		public int Port
		{
			get => Uri.Port;
			set => _openUri(new UriBuilder(Uri) {Port = value}.Uri.ToString());
		}

		/// <summary>
		/// Gets or sets the protocol scheme of the URL, including the final ':'.
		/// </summary>
		public string Protocol
		{
			get => Uri.Scheme + ":";
			set => _openUri(new UriBuilder(Uri) { Scheme = value }.Uri.ToString());
		}

		/// <summary>
		/// Gets or sets a string containing a '?' followed by the parameters or "querystring" of the URL.
		/// </summary>
		public string Search
		{
			get => Uri.Query;
			set => _openUri(new UriBuilder(Uri) {Query = value}.Uri.ToString());
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
		public void Reload(bool force = false) => _openUri(Uri.ToString());
	}
}
