#if NUNIT
using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Elements;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.Dom
{
	[TestFixture]
	public class HtmlSelectElementTests
	{
		private Document _document;
		private HtmlSelectElement _select;

		[SetUp]
		public void SetUp()
		{
			_document = new Document();
			_select = (HtmlSelectElement)_document.CreateElement("select");
		}

		[Test]
		public void RemoveTest()
		{
			_select.AppendChild(_document.CreateElement("option"));
			Assert.AreEqual(1, _select.Length);
			_select.Remove(0);
			Assert.AreEqual(0, _select.Length);
		}

		[Test]
		public void AddingNonOptionsDeprecatedTest()
		{
			_select.AppendChild(_document.CreateElement("div"));
			Assert.AreEqual(0, _select.Length);
			Assert.AreEqual(0, _select.ChildNodes.Count);
		}

		[Test]
		public void AddingOptionTest()
		{
			_select.AppendChild(_document.CreateElement("option"));
			Assert.AreEqual(1, _select.Length);
			Assert.AreEqual(1, _select.ChildNodes.Count);
		}
	}
}
#endif