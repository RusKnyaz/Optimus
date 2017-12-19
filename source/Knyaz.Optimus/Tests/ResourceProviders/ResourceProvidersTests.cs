﻿#if NUNIT
using Knyaz.Optimus.ResourceProviders;
using Knyaz.Optimus.Tools;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.ResourceProviders
{
	[TestFixture]
	public class ResourceProvidersTests
	{
		[Test, Ignore("For manual run")]
		public void HttpRequest() =>
			new ResourceProvider().GetResourceAsync("http://google.com").Wait();

		[Test]
		public void DataResource()
		{
			var t = new ResourceProvider().GetResourceAsync("data:text/javascript;charset=utf8,window");
			t.Wait();
			Assert.AreEqual(",window", t.Result.Stream.ReadToEnd());
		}

		[TestCase("http://chromium.github.io/octane", "http://chromium.github.io/octane/js/jquery.js")]
		[TestCase("file:///var/www/site/subdir", "file:///var/www/site/subdir/js/jquery.js")]
		public void CreateRequestRelativePath(string root, string result) =>
			new ResourceProvider {Root = root}
				.CreateRequest("js/jquery.js")
				.Assert(request => request.Url == result);
	}
}
#endif