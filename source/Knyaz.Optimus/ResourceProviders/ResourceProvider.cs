using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Knyaz.Optimus.ResourceProviders
{
	/// <summary>
	/// Redirects requests to resource provider that connected with specified protocol. 
	/// </summary>
	internal class ResourceProvider : IResourceProvider
	{
		public event Action<Uri> OnRequest;
		public event EventHandler<ReceivedEventArguments> Received;
		
		public CookieContainer CookieContainer { get; } 

		public ResourceProvider(IResourceProvider httpResourceProvider,
			IResourceProvider fileResourceProvider)
		{
			HttpResourceProvider = httpResourceProvider;
			CookieContainer = (httpResourceProvider as HttpResourceProvider)?.CookieContainer;
			FileResourceProvider = fileResourceProvider;
		}

		private IResourceProvider DataResourceProvider { get; } = new DataResourceProvider();
		protected IResourceProvider FileResourceProvider { get; }
		public IResourceProvider HttpResourceProvider { get; }

		private IResourceProvider GetResourceProvider(Uri u)
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

		public Task<IResource> SendRequestAsync(Request req)
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
