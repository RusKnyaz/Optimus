using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace WebBrowser.ResourceProviders
{
	internal class ResourceProvider : IResourceProvider
	{
		public event Action<string> OnRequest;

		private readonly CookieContainer _cookies;
		private string _root;

		public ResourceProvider()
		{
			_cookies = new CookieContainer();
			HttpResourceProvider = new HttpResourceProvider(_cookies);
		}

		public string Root
		{
			get { return _root; }
			set
			{
				HttpResourceProvider.Root = value;
				_root = value;
			}
		}

		public Task<IResource> GetResourceAsync(string uri)
		{
			if (string.IsNullOrEmpty(uri))
				throw new ArgumentOutOfRangeException("uri");

			if (OnRequest != null)
				OnRequest(uri);

			var u = uri[0] == '/' ? new Uri(new Uri(Root), uri) : new Uri(uri);

			var scheme = u.GetLeftPart(UriPartial.Scheme).ToLowerInvariant();
			if (scheme == "http://" || scheme == "https://")
			{
				var httpRequest = new HttpRequest("GET", u.OriginalString);
				return HttpResourceProvider.SendRequestAsync(httpRequest).ContinueWith(t => (IResource)t.Result);
			}
			throw new Exception("Unsupported scheme: " + scheme);
		}

		public IHttpResourceProvider HttpResourceProvider { get; private set; }
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
