using System;
using Knyaz.Optimus.Tools;

namespace Knyaz.Optimus.ResourceProviders
{
	/// <summary>
	/// Convert relative and other paths to absolute Uri.
	/// </summary>
	public class LinkProvider
	{
		public string Root { get; set; }
		
		public Uri MakeUri(string uri)
		{
			if (uri.Substring(0, 2) == "./")
				uri = uri.Remove(0, 2);

			return UriHelper.IsAbsolete(uri) ? new Uri(uri) : new Uri(new Uri(Root), uri);
		}
	}
}