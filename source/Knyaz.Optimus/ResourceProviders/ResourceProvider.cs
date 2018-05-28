using System;
using System.IO;
using System.Net;
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

		private ISpecResourceProvider DataResourceProvider { get; } = new DataResourceProvider();
		protected ISpecResourceProvider FileResourceProvider { get; }
		public ISpecResourceProvider HttpResourceProvider { get; }

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
					return DataResourceProvider;
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
