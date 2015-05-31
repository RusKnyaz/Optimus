#if NUNIT
using System.IO;
using System.Text;
using Moq;

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
			Mock.Get(resourceProvider)
				.Setup(x => x.GetResource(url))
				.Returns(() => Mock.Of<IResource>(y => y.Stream == new MemoryStream(Encoding.UTF8.GetBytes(data)) && y.Type == ResourceTypes.Html));
			return resourceProvider;
		}

		public static string Page(string script)
		{
			return "<html><head><script>" + script + "</script></head><body></body></html>";
		}
	}
}
#endif