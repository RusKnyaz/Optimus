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
			
			if (uri.StartsWith("data:"))
			{
				var data = uri.Substring(5);
				var type = new string(data.TakeWhile(c => c != ',').ToArray());
				var content = data.Substring(type.Length);
				return new Task<IResource>(() => new Response(ResourceTypes.Html /*todo: fix type*/, new MemoryStream(Encoding.UTF8.GetBytes(content))));
			}

			var u = Uri.IsWellFormedUriString(uri, UriKind.Absolute) ? new Uri(uri) : new Uri(new Uri(Root), uri);

			var scheme = u.GetLeftPart(UriPartial.Scheme).ToLowerInvariant();
			if (scheme == "http://" || scheme == "https://")
			{
				var httpRequest = CreateRequest<HttpRequest>(u.OriginalString);
				return GetResourceAsync(httpRequest);
			}
			
			throw new Exception("Unsupported scheme: " + scheme);
		}

		public T CreateRequest<T>(string url) where T : class, IRequest
		{
			if (typeof (T) == typeof (HttpRequest))
			{
				return new HttpRequest("GET", url) as T;
			}

			throw new Exception("Unknown request type specified");
		}

		public Task<IResource> GetResourceAsync(IRequest req)
		{
			if (OnRequest != null)
				OnRequest(req.Url);

			var httpRequest = req as HttpRequest;
			if(req != null)
				return HttpResourceProvider.SendRequestAsync(httpRequest).ContinueWith(t =>
					{
						if (Received != null)
							Received(req.Url);
						return (IResource) t.Result;
					});

			throw new Exception("Invalid request type");
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
