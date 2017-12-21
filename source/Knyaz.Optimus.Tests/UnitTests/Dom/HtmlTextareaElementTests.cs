using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Elements;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.Dom
{
	[TestFixture]
	public class HtmlTextareaElementTests
	{
		private Document _document;
		private HtmlTextAreaElement _textArea;

		[SetUp]
		public void SetUp()
		{
			_document = new Document();
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
		public void NeightbourOwnerForm()
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
	}
}