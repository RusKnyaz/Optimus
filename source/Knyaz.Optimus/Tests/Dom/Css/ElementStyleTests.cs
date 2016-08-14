#if NUNIT
using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Css;
using Knyaz.Optimus.Dom.Elements;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.Dom.Css
{
	[TestFixture]
	public class ElementStyleTests
	{
		private Document _document;
		HtmlDivElement _div;

		[SetUp]
		public void SetUp()
		{
			_document = new Document();
			_div = (HtmlDivElement) _document.CreateElement("DIV");
		}

		[Test]
		public void CssTextFromStyle()
		{
			_div.SetAttribute("style", "background-color:Red;");
			_div.Style.Assert(style => 
				style.CssText == "background-color:Red;"
				&& style.Length == 1);
		}

		[Test]
		public void DefaultValues()
		{
			_div.Style.Assert(style => 
				style.CssText == "" 
				&& style.ParentRule == null
				&& style.Length == 0);
		}

		[Test]
		public void DefaultComputedStyle()
		{
			_document.Body.AppendChild(_div);
			var styling = new DocumentStyling(_document, null);
			styling.LoadDefaultStyles();

			styling.GetComputedStyle(_div).Assert(style =>
				style.GetPropertyValue("display") == "block");
		}
	}
}
#endif