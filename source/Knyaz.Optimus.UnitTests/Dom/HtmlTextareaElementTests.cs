using Knyaz.NUnit.AssertExpressions;
using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Elements;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.Dom
{
	[TestFixture]
	public class HtmlTextareaElementTests
	{
		private HtmlDocument _document;
		private HtmlTextAreaElement _textArea;

		[SetUp]
		public void SetUp()
		{
			_document = new HtmlDocument();
			_textArea = (HtmlTextAreaElement)_document.CreateElement("textarea");
		}

		[Test]
		public void Defaults()
		{
			Assert.AreEqual(null, _textArea.GetAttribute("rows"));
			Assert.AreEqual(2, _textArea.Rows);
			Assert.AreEqual(20, _textArea.Cols);
			Assert.AreEqual(false, _textArea.Autofocus);
			Assert.AreEqual("", _textArea.Placeholder);
			Assert.AreEqual(false, _textArea.Required);
			Assert.AreEqual(false, _textArea.ReadOnly);
		}

		[Test]
		public void SetRows()
		{
			_textArea.Rows = 3;
			Assert.AreEqual("3", _textArea.GetAttribute("rows"));
			Assert.AreEqual(3, _textArea.Rows);
		}

		[Test]
		public void SetAttribute()
		{
			_textArea.SetAttribute("rows", "3");
			Assert.AreEqual("3", _textArea.GetAttribute("rows"));
			Assert.AreEqual(3, _textArea.Rows);
		}

		[Test]
		public void ResetAttribute()
		{
			_textArea.SetAttribute("rows", null);
			Assert.AreEqual(null, _textArea.GetAttribute("rows"));
			Assert.AreEqual(2, _textArea.Rows);
		}

		[Test]
		public void AncestorOwnerForm()
		{
			var form = _document.CreateElement("form");
			form.AppendChild(_textArea);
			Assert.AreEqual(form, _textArea.Form);
		}

		[Test]
		public void NeighbourOwnerForm()
		{
			var form = _document.CreateElement("form");
			form.Id = "myForm";
			_textArea.SetAttribute("form", "myForm");
			_document.DocumentElement.AppendChild(_textArea);
			_document.DocumentElement.AppendChild(form);
			Assert.AreEqual(form, _textArea.Form);
		}

		[Test]
		public void ToStringTest()
		{
			Assert.AreEqual("[object HTMLTextAreaElement]", _textArea.ToString());
		}

		[TestCase("hello")]
		[TestCase("<div>ABC</div>")]
		[TestCase("\r\n")]
		public void SetValue(string val)
		{
			var text = (HtmlTextAreaElement)_document.CreateElement("textarea");
			text.Value = val;
			Assert.AreEqual(val, text.Value);
		}

		[TestCase("","","")]
		[TestCase("ABC","ABC","ABC")]
		[TestCase("<div>ABC!@#$</dIv>","&lt;div&gt;ABC!@#$&lt;/dIv&gt;","<div>ABC!@#$</dIv>")]
		public void SetInnerHtml(string html, string expectedHtml, string expectedValue)
		{
			var text = (HtmlTextAreaElement)_document.CreateElement("textarea");
			text.InnerHTML = html;
			text.Assert(t => t.InnerHTML == expectedHtml && t.Value == expectedValue);
		}
	}
}