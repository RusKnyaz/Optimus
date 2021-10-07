using Knyaz.Optimus.Dom;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.Dom
{
	[TestFixture]
	public class DomParserTests
	{
		[Test]
		public static void ParseHtml()
		{
			var parser = new DomParser();
			var doc = (HtmlDocument)parser.ParseFromString("<html><title>qwe</title></html>", "text/html");
			Assert.AreEqual("qwe", doc.Title);
		}
	}
}