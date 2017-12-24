﻿using System;
 using System.Diagnostics;
 using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Knyaz.Optimus.Tools;

namespace Knyaz.Optimus.ResourceProviders
{
	internal class ResourceProvider : IResourceProvider
	{
		public event Action<string> OnRequest;
		public event EventHandler<ReceivedEventArguments> Received;

		public ResourceProvider() 
		:this(new HttpResourceProvider(
				new CookieContainer()), new FileResourceProvider())
		{}

		public ResourceProvider(ISpecResourceProvider httpResourceProvider,
			ISpecResourceProvider fileResourceProvider)
		{
			HttpResourceProvider = httpResourceProvider;
			FileResourceProvider = fileResourceProvider;
		}

		protected ISpecResourceProvider FileResourceProvider { get; private set; }
		public ISpecResourceProvider HttpResourceProvider { get; private set; }

		public string Root { get; set; }

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

		public IRequest CreateRequest(string path)
		{
			var uri = MakeUri(path);
			var resourceProvider = GetResourceProvider(uri);

			if (resourceProvider is DataResourceProvider data)
			{
				return data.CreateRequest(path);
			}
			
			return resourceProvider.CreateRequest(uri.ToString());
		}
		
		private Uri MakeUri(string uri)
		{
			if (uri.Substring(0, 2) == "./")
				uri = uri.Remove(0, 2);

			return UriHelper.IsAbsolete(uri) ? new Uri(uri) : new Uri(new Uri(Root), uri);
		}

		public Task<IResource> SendRequestAsync(IRequest req)
		{
			OnRequest?.Invoke(req.Url);

			var u = UriHelper.GetUri(Root, req.Url);

			var provider = GetResourceProvider(u);

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
			public IRequest CreateRequest(string url)
			{
				return new DataRequest(url);
			}

			public Task<IResource> SendRequestAsync(IRequest request)
			{
				var uri = request.Url;
				var data = uri.Substring(5);
				var type = new string(data.TakeWhile(c => c != ',').ToArray());
				var content = data.Substring(type.Length);
				return
					Task.Run(
						() => (IResource)new Response(ResourceTypes.Html /*todo: fix type*/, new MemoryStream(Encoding.UTF8.GetBytes(content))));
			}

			class DataRequest : IRequest
			{
				public DataRequest(string url)
				{
					Url = url;
				}

				public string Url { get; private set; }
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
