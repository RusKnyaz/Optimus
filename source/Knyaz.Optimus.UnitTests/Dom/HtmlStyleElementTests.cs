using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Elements;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.Dom
{
	[TestFixture]
	public class HtmlStyleElementTests
	{
		private Document _document;
		private HtmlStyleElement _style;

		[SetUp]
		public void SetUp()
		{
			_document = new Document();
			_style = (HtmlStyleElement)_document.CreateElement("style");
		}

		[Test]
		public void Defaults()
		{
			_style.Assert(x => 
				x.Media == ""
				&& x.Type == ""
				&& x.Disabled == false);
		}

		[Test]
		public void FillValuesFromAttributes()
		{
			_style.SetAttribute("type", "text/css");
			_style.SetAttribute("media", "all");

			_style.Assert(style => style.Media == "all" && style.Type == "text/css");
		}
	}
}