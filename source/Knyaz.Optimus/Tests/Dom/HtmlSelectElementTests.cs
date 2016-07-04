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
			Assert.AreEqual(1, _select.Options.Length);
		}

		[Test]
		public void OptionsItem()
		{
			var opt = _document.CreateElement("option");
			_select.AppendChild(opt);
			Assert.AreEqual(opt, _select.Options.Item(0));
		}

		[Test]
		public void OptionsIndexerByInt()
		{
			var opt = _document.CreateElement("option");
			_select.AppendChild(opt);
			Assert.AreEqual(opt, _select.Options[0]);
		}

		[Test]
		public void OptionsIndexerByName()
		{
			var opt = _document.CreateElement("option");
			opt.Id = "X";
			_select.AppendChild(opt);
			Assert.AreEqual(opt, _select.Options["X"]);
		}

		[Test]
		public void OptionsNamedItemById()
		{
			var opt = _document.CreateElement("option");
			opt.Id = "X";
			_select.AppendChild(opt);
			Assert.AreEqual(opt, _select.Options.NamedItem("X"));
		}

		[Test]
		public void OptionsNamedItemByName()
		{
			var opt = (HtmlOptionElement)_document.CreateElement("option");
			opt.Name = "X";
			_select.AppendChild(opt);
			Assert.AreEqual(opt, _select.Options.NamedItem("X"));
		}

		[Test]
		public void OptionsSetLengthIncrease()
		{
			_select.Options.Length = 2;
			Assert.AreEqual("<OPTION></OPTION><OPTION></OPTION>", _select.InnerHTML);
		}

		[Test]
		public void OptionsSetLengthDecrease()
		{
			var optX = (HtmlOptionElement)_document.CreateElement("option");
			optX.Name = "X";
			var optY = (HtmlOptionElement)_document.CreateElement("option");
			optY.Name = "Y";
			_select.AppendChild(optX);
			_select.AppendChild(optY);
			_select.Options.Length = 1;
			Assert.AreEqual(1, _select.Options.Length);
			Assert.AreEqual(optX, _select.Options.Item(0));
		}
	}
}
#endif