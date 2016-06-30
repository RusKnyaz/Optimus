#if NUNIT
using Knyaz.Optimus.Dom;
using Knyaz.Optimus.ResourceProviders;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests
{
	[TestFixture]
	public class ResourceProvidersTests
	{
		[Test, Ignore("For manual run")]
		public async void HttpRequest()
		{
			var provider = new ResourceProvider();
			await provider.GetResourceAsync("http://google.com");
		}

		[Test]
		public void DataResource()
		{
			var provider =new ResourceProvider();
			var t = provider.GetResourceAsync("data:text/javascript;charset=utf8,window");
			t.Wait();
			Assert.AreEqual(",window", t.Result.Stream.ReadToEnd());
		}
	}
}
#endif