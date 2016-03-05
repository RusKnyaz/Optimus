#if NUNIT
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
	}
}
#endif