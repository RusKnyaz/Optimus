#if NUNIT
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Moq;
using WebBrowser.ResourceProviders;

namespace WebBrowser.Tests
{
	public static class Mocks
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
				.Setup(x => x.GetResourceAsync(request))
				.Returns(() =>Task.Run(() => (IResource)new HttpResponse(HttpStatusCode.OK, new MemoryStream(Encoding.UTF8.GetBytes(data)), null)));
			return resourceProvider;
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
#endif