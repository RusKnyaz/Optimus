﻿using System;
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

		public IRequest CreateRequest(string uri)
		{
			return GetResourceProvider(UriHelper.GetUri(Root, uri)).CreateRequest(uri);
		}

		public Task<IResource> GetResourceAsync(IRequest req)
		{
			if (OnRequest != null)
				OnRequest(req.Url);
			
			var u = UriHelper.GetUri(Root, req.Url);

			var provider = GetResourceProvider(u);

			return provider.SendRequestAsync(req).ContinueWith(t =>
					{
						try
						{
							if (Received != null)
								Received(this, new ReceivedEventArguments(req, t.Result));
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
