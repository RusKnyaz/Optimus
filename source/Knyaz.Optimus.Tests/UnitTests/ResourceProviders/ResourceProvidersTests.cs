using System;
using Knyaz.Optimus.ResourceProviders;
using Knyaz.Optimus.Tools;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.UnitTests.ResourceProviders
{
	[TestFixture]
	public class ResourceProvidersTests
	{
		[Test, Ignore("For manual run")]
		public void HttpRequest() =>
			new ResourceProvider().GetResourceAsync(new Uri("http://google.com")).Wait();

		[Test]
		public void DataResource()
		{
			var t = new ResourceProvider().GetResourceAsync(new Uri("data:text/javascript;charset=utf8,window"));
			t.Wait();
			Assert.AreEqual(",window", t.Result.Stream.ReadToEnd());
		}
	}
}