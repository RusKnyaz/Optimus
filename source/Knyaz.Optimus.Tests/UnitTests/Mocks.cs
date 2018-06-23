using System;
using System.Collections.Generic;
using System.Diagnostics;
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
		public static IResourceProvider ResourceProvider(string path, byte[] data)
		{
			var url = new Uri(path, UriKind.RelativeOrAbsolute);
			return Mock.Of<IResourceProvider>().Resource(url, data);
		}
		
		public static IResourceProvider ResourceProvider(string path, string data)
		{
			var url = new Uri(path, UriKind.RelativeOrAbsolute);
			return Mock.Of<IResourceProvider>().Resource(url, data);
		}
		
		public static IResourceProvider ResourceProvider(Uri url, string data) => 
			Mock.Of<IResourceProvider>().Resource(url, data);

		public static IResourceProvider Resource(this IResourceProvider resourceProvider, string url, string data) => 
			resourceProvider.Resource(new Uri(url, UriKind.RelativeOrAbsolute), data);

		public static IResourceProvider Resource(this IResourceProvider resourceProvider, Uri url, byte[] data)
		{
			var resource= (IResource)new HttpResponse(HttpStatusCode.OK, new MemoryStream(data), null);

			var rpMock = new Mock<IResourceProvider>();
			rpMock.Setup(x => x.SendRequestAsync(It.IsAny<Request>()))
				.Returns<Request>(x => x.Url == url ? Task.Run(() => resource) : resourceProvider.SendRequestAsync(x));
			return rpMock.Object;
		}

		public static IResourceProvider Resource(this IResourceProvider resourceProvider, Uri url, string data) =>
			resourceProvider.Resource(url, Encoding.UTF8.GetBytes(data));

		public static SpecResourceProvider HttpResourceProvider() => new SpecResourceProvider();

		public class SpecResourceProvider : IResourceProvider
		{
			Dictionary<Uri, IResource> _resources = new Dictionary<Uri, IResource>();
			
			private HttpResponse _response404 = new HttpResponse(HttpStatusCode.NotFound, Stream.Null, "");

			public Task<IResource> SendRequestAsync(Request request)
			{
				History.Add(request);
				return Task.Run(() => 
					_resources.ContainsKey(request.Url) ? _resources[request.Url] 
					: _response404); 
			}

			public SpecResourceProvider Resource(string url, string data, string resourceType = "text/html")
			{
				var uri = new Uri(url, UriKind.RelativeOrAbsolute); 
				
				_resources[uri] =  new HttpResponse(
					HttpStatusCode.OK, 
					new MemoryStream(Encoding.UTF8.GetBytes(data)), 
					null, 
					resourceType, 
					uri);

				return this;
			}

			public List<Request> History = new List<Request>();
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