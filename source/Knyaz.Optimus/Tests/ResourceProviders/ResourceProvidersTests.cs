﻿#if NUNIT
using Knyaz.Optimus.Dom;
using Knyaz.Optimus.ResourceProviders;
using Knyaz.Optimus.Tools;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.ResourceProviders
{
	[TestFixture]
	public class ResourceProvidersTests
	{
		[Test, Ignore("For manual run")]
		public void HttpRequest()
		{
			var provider = new ResourceProvider();
			provider.GetResourceAsync("http://google.com").Wait();
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