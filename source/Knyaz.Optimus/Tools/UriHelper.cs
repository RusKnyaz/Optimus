using System;
using System.Text;

namespace Knyaz.Optimus.Tools
{
	internal static class UriHelper
	{
		public static bool IsAbsolete(string uri) =>
			!uri.StartsWith("/") && (
				uri.StartsWith("http://") || uri.StartsWith("https://") || uri.StartsWith("file://") ||
				   uri.StartsWith("data:"));

		public static Uri GetUri(string root, string uri) =>
			 IsAbsolete(uri) ? new Uri(uri) : new Uri(new Uri(root), uri);
		
		public static UriData GetUriData(this Uri uri)
		{
			var uriStr = uri.ToString();
			var data = uriStr.Substring(5);

			var dataSplitIndex = data.IndexOf(',');
			var type = data.Substring(0, dataSplitIndex).Split(';');
			var content = data.Substring(dataSplitIndex + 1);

			if (string.IsNullOrEmpty(type[0]))
				type[0] = "text/html";

			var encodedContent = type.Length > 1 && type[1] == "base64"
				? Convert.FromBase64String(content)
				: Encoding.UTF8.GetBytes(content);
			
			return  new UriData{Type = type[0], Data = encodedContent};
		}
	}
	
	public class UriData
	{
		public string Type;
		public byte[] Data;
	}
}
