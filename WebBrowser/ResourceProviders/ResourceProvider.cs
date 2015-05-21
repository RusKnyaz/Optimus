using System;
using System.IO;
using System.Net;

namespace WebBrowser.ResourceProviders
{
	internal class ResourceProvider : IResourceProvider
	{
		private readonly CookieContainer _cookies;

		public ResourceProvider()
		{
			_cookies = new CookieContainer();
			HttpResourceProvider = new HttpResourceProvider(_cookies);
		}

		public string Root { get; set; }

		public IResource GetResource(string uri)
		{
			if(string.IsNullOrEmpty(uri))
				throw new ArgumentOutOfRangeException("uri");

			var u = uri[0] == '/' ? new Uri(new Uri(Root), uri) : new Uri(uri);

			var scheme = u.GetLeftPart(UriPartial.Scheme).ToLowerInvariant();
			if (scheme == "http://" || scheme == "https://")
			{
				//todo: use HttpResourceProvider
				return GetHttpResource(u);
			}

			throw new Exception("Unsupported scheme: " + scheme);
		}

		public IHttpResourceProvider HttpResourceProvider { get; private set; }

		private IResource GetHttpResource(Uri uri)
		{
			var request = WebRequest.CreateHttp(uri);
			var response = request.GetResponse();
			
			//todo: copy resource
			return new Response(ResourceTypes.Html, response.GetResponseStream());
		}
	}

	public class Response : IResource
	{
		public Response(ResourceTypes type, Stream stream)
		{
			Stream = stream;
			Type = type;
		}

		public ResourceTypes Type { get; private set; }
		public Stream Stream { get; private set; }
	}
}
