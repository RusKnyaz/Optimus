#if NUNIT
using System;
using NUnit.Framework;
using WebBrowser.ResourceProviders;

namespace WebBrowser.Tests
{
	[TestFixture]
	public class ResourceProvidersTests
	{
		[Test, Ignore]
		public void HttpRequest()
		{
			var provider = new ResourceProvider();
			var res = provider.GetResource("http://google.com");
		}
	}
}
#endif