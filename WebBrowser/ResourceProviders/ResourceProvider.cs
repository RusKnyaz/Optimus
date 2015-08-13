using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WebBrowser.ResourceProviders
{
	internal class ResourceProvider : IResourceProvider
	{
		public event Action<string> OnRequest;
		public event Action<string> Received;

		private readonly CookieContainer _cookies;
		private string _root;

		public ResourceProvider()
		{
			_cookies = new CookieContainer();
			HttpResourceProvider = new HttpResourceProvider(_cookies);
			FileResourceProvider = new FileResourceProvider();
		}

		protected FileResourceProvider FileResourceProvider { get; private set; }
		public IHttpResourceProvider HttpResourceProvider { get; private set; }

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
			
			if (uri.StartsWith("data:"))
			{
				var data = uri.Substring(5);
				var type = new string(data.TakeWhile(c => c != ',').ToArray());
				var content = data.Substring(type.Length);
				return new Task<IResource>(() => new Response(ResourceTypes.Html /*todo: fix type*/, new MemoryStream(Encoding.UTF8.GetBytes(content))));
			}

			var u = GetUri(uri);

			var provider = GetResourceProvider(u);
			var req = provider.CreateRequest(u.OriginalString);
			return GetResourceAsync(req);
		}

		private Uri GetUri(string uri)
		{
			return Uri.IsWellFormedUriString(uri, UriKind.Absolute) ? new Uri(uri) : new Uri(new Uri(Root), uri);
		}

		private ISpecResourceProvider GetResourceProvider(Uri u)
		{
			var scheme = u.GetLeftPart(UriPartial.Scheme).ToLowerInvariant();

			switch (scheme)
			{
				case "http://":
				case "https://":
					return HttpResourceProvider;
				case "file://":
					return FileResourceProvider;
				default:
					throw new Exception("Unsupported scheme: " + scheme);
			}
		}

		public Task<IResource> GetResourceAsync(IRequest req)
		{
			if (OnRequest != null)
				OnRequest(req.Url);
			
			var u = GetUri(req.Url);

			var provider = GetResourceProvider(u);

			return provider.SendRequestAsync(req).ContinueWith(t =>
					{
						if (Received != null)
							Received(req.Url);
						return t.Result;
					});
		}
	}

	public class Response : IResource
	{
		public Response(string type, Stream stream)
		{
			Stream = stream;
			Type = type;
		}

		public string Type { get; private set; }
		public Stream Stream { get; private set; }
	}
}
