#if NUNIT
using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Elements;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.Dom
{
	[TestFixture]
	public class HtmlLinkElementTests
	{
		private Document _document;
		private HtmlLinkElement _link;

		[SetUp]
		public void SetUp()
		{
			_document = new Document();
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
	}
}
#endif
