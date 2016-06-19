using System;

namespace Knyaz.Optimus.Tools
{
	internal static class UriHelper
	{
		public static bool IsAbsolete(string uri)
		{
			return !uri.StartsWith("/") && (
				uri.StartsWith("http://") || uri.StartsWith("https://") || uri.StartsWith("file://") ||
				   uri.StartsWith("data:"));
		}

		public static Uri GetUri(string root, string uri)
		{
			return IsAbsolete(uri) ? new Uri(uri) : new Uri(new Uri(root), uri);
		}
	}
}
