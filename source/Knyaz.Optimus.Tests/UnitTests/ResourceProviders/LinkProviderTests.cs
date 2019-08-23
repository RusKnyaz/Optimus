using Knyaz.Optimus.ResourceProviders;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.UnitTests.ResourceProviders
{
	[TestFixture]
	public class LinkProviderTests
	{
		[TestCase("http://todosoft.ru/index.html", "knockout.js", "http://todosoft.ru/knockout.js")]
		[TestCase("http://todosoft.ru/test/", "./js/script.js", "http://todosoft.ru/test/js/script.js")]
		[TestCase("http://todosoft.ru/test/", "/js/script.js", "http://todosoft.ru/js/script.js")]
		[TestCase("http://chromium.github.io/octane/", "js/jquery.js", "http://chromium.github.io/octane/js/jquery.js")]
		[TestCase("file:///var/www/site/subdir/", "js/jquery.js", "file:///var/www/site/subdir/js/jquery.js")]
		public void CreateRequestRelativePath(string root, string rel, string result) =>
			new LinkProvider {Root = root}
				.MakeUri(rel)
				.Assert(request => request.ToString() == result);
	}
}