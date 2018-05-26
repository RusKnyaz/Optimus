using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Knyaz.Optimus.ResourceProviders
{
	internal class ResourceProvider : IResourceProvider
	{
		public event Action<Uri> OnRequest;
		public event EventHandler<ReceivedEventArguments> Received;
		public CookieContainer CookieContainer { get; } = new CookieContainer();

		public ResourceProvider()
		{
			HttpResourceProvider = new HttpResourceProvider(CookieContainer);
			FileResourceProvider = new FileResourceProvider();
		}

		public ResourceProvider(ISpecResourceProvider httpResourceProvider,
			ISpecResourceProvider fileResourceProvider)
		{
			HttpResourceProvider = httpResourceProvider;
			FileResourceProvider = fileResourceProvider;
		}

		protected ISpecResourceProvider FileResourceProvider { get; private set; }
		public ISpecResourceProvider HttpResourceProvider { get; private set; }

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
				case "data://": //mono
				case "data:":
					return new DataResourceProvider();
				default:
					throw new Exception("Unsupported scheme: " + scheme);
			}
		}

		public IRequest CreateRequest(Uri uri)
		{
			var resourceProvider = GetResourceProvider(uri);
			return resourceProvider.CreateRequest(uri);
		}
		

		public Task<IResource> SendRequestAsync(IRequest req)
		{
			OnRequest?.Invoke(req.Url);

			var provider = GetResourceProvider(req.Url);

			return provider.SendRequestAsync(req).ContinueWith(t =>
					{
						try
						{
							Received?.Invoke(this, new ReceivedEventArguments(req, t.Result));
						}
						catch { }
						return t.Result;
					});
		}

		class DataResourceProvider : ISpecResourceProvider
		{
			public IRequest CreateRequest(Uri url) => new DataRequest(url);

			public Task<IResource> SendRequestAsync(IRequest request)
			{
				var uri = request.Url.ToString();
				var data = uri.Substring(5);
				var type = new string(data.TakeWhile(c => c != ',').ToArray());
				var content = data.Substring(type.Length);
				return
					Task.Run(
						() => (IResource)new Response(ResourceTypes.Html /*todo: fix type*/, new MemoryStream(Encoding.UTF8.GetBytes(content))));
			}

			class DataRequest : IRequest
			{
				public DataRequest(Uri url) => Url = url;
				public Uri Url { get; }
			}
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
