using Knyaz.NUnit.AssertExpressions;
using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Elements;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.Dom
{
	[TestFixture]
	public class HtmlLinkElementTests
	{
		private HtmlDocument _document;
		private HtmlLinkElement _link;

		[SetUp]
		public void SetUp()
		{
			_document = new HtmlDocument();
			_link = (HtmlLinkElement)_document.CreateElement("link");
		}

		[Test]
		public void Defaults()
		{
			_link.Assert(x =>
				x.Href == ""
				&& x.Type == ""
				&& x.Disabled == false);
		}

		[Test]
		public void FillValuesFromAttributes()
		{
			_link.SetAttribute("type", "text/css");
			_link.SetAttribute("href", "file://some.css");

			_link.Assert(style => style.Href == "file://some.css" && style.Type == "text/css");
		}
		
		[Test]
		public static void RelList()
		{
			var doc = DomImplementation.Instance.CreateHtmlDocument();
			var anchor = new HtmlLinkElement(doc);
			Assert.AreEqual(0, anchor.RelList.Count);
			anchor.Rel = "hello world";
			Assert.AreEqual(new[] {"hello", "world"}, anchor.RelList);
			anchor.RelList.Remove("hello");
			Assert.AreEqual("world", anchor.Rel);
		}

	}
}