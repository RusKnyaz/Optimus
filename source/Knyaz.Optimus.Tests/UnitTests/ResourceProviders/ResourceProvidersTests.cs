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

		[TestCase("http://todosoft.ru/index.html", "knockout.js", "http://todosoft.ru/knockout.js")]
		[TestCase("http://todosoft.ru/test/", "./js/script.js", "http://todosoft.ru/test/js/script.js")]
		[TestCase("http://todosoft.ru/test/", "/js/script.js", "http://todosoft.ru/js/script.js")]
		[TestCase("http://chromium.github.io/octane/", "js/jquery.js", "http://chromium.github.io/octane/js/jquery.js")]
		[TestCase("file:///var/www/site/subdir/", "js/jquery.js", "file:///var/www/site/subdir/js/jquery.js")]
		public void CreateRequestRelativePath(string root, string rel, string result) =>
			new ResourceProvider {Root = root}
				.CreateRequest(rel)
				.Assert(request => request.Url == result);
	}
}
#endif