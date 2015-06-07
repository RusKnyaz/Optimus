#if NUNIT
using NUnit.Framework;
using WebBrowser.ResourceProviders;

namespace WebBrowser.Tests
{
	[TestFixture]
	public class ResourceProvidersTests
	{
		[Test, Ignore]
		public async void HttpRequest()
		{
			var provider = new ResourceProvider();
			await provider.GetResourceAsync("http://google.com");
		}
	}
}
#endif