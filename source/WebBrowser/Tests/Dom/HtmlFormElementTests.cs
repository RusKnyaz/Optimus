#if NUNIT
using NUnit.Framework;
using WebBrowser.Dom;
using WebBrowser.Dom.Elements;

namespace WebBrowser.Tests.Dom
{
	[TestFixture]
	public class HtmlFormElementTests
	{
		private Document _document;
		private HtmlFormElement _form;

		[SetUp]
		public void SetUp()
		{
			_document = new Document();
			_form = (HtmlFormElement)_document.CreateElement("form");
		}

		[TestCase("GeT", "get")]
		[TestCase("get", "get")]
		[TestCase("", "get")]
		[TestCase("update", "get")]
		[TestCase("PoSt", "post")]
		public void MethodTest(string setValue, string getValue)
		{
			_form.Method = setValue;
			Assert.AreEqual(setValue, _form.GetAttribute("method"));
			Assert.AreEqual(getValue, _form.Method);
		}
	}
}
#endif