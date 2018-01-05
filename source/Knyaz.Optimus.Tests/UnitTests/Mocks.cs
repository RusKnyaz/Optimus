using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Knyaz.Optimus.ResourceProviders;
using Moq;

namespace Knyaz.Optimus.Tests
{
	internal static class Mocks
	{
		public static IResourceProvider ResourceProvider(string url, string data)
		{
			return Mock.Of<IResourceProvider>().Resource(url, data);
		}

		public static IResourceProvider Resource(this IResourceProvider resourceProvider, string url, string data)
		{
			var request = new HttpRequest("GET", url);

			Mock.Get(resourceProvider)
				.Setup(x => x.CreateRequest(url)).Returns(request);
			Mock.Get(resourceProvider)
				.Setup(x => x.SendRequestAsync(request))
				.Returns(() =>Task.Run(() => (IResource)new HttpResponse(HttpStatusCode.OK, new MemoryStream(Encoding.UTF8.GetBytes(data)), null)));
			return resourceProvider;
		}

		public static SpecResourceProvider HttpResourceProvider() => new SpecResourceProvider();

		public class SpecResourceProvider : ISpecResourceProvider
		{
			public IRequest CreateRequest(string url) => new HttpRequest("GET", url);
			
			Dictionary<string, HttpResponse> _resources = new Dictionary<string, HttpResponse>();

			public Task<IResource> SendRequestAsync(IRequest request)
			{
				History.Add(request as HttpRequest);
				return Task.Run(() => (IResource)_resources[request.Url]);
			}

			public SpecResourceProvider Resource(string url, string data, string resourceType = "text/html")
			{
				_resources[url] =  new HttpResponse(
					HttpStatusCode.OK, 
					new MemoryStream(Encoding.UTF8.GetBytes(data)), 
					null, 
					resourceType, 
					new Uri(url, UriKind.RelativeOrAbsolute));

				return this;
			}

			public List<HttpRequest> History = new List<HttpRequest>();
		}

		public static string Page(string script)
		{
			return "<html><head></head><body></body><script>" + script + "</script></html>";
		}
		
		public static string Page(string script, string body)
		{
			return "<html><head></head><body>" + body + "</body><script>" + script + "</script></html>";
		}
	}
}