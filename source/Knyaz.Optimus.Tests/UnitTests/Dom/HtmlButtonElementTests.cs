#if NUNIT
using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Elements;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.Dom
{
	[TestFixture]
	public class HtmlButtonElementTests
	{
		private Document _document;
		private HtmlButtonElement _button;

		[SetUp]
		public void SetUp()
		{
			_document = new Document();
			_button = (HtmlButtonElement)_document.CreateElement("button");
		}


		[TestCase(null, "submit")]
		[TestCase("asd", "submit")]
		[TestCase("button", "button")]
		[TestCase("reset", "reset")]
		public void SetGetType(string setType, string getType)
		{
			_button.Type = setType;
			Assert.AreEqual(setType, _button.GetAttribute("type"));
			Assert.AreEqual(getType, _button.Type);
		}

		[Test]
		public void Defaults()
		{
			_button.Assert(b => b.Disabled == false);
		}
	}
}
#endif