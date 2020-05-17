using Knyaz.NUnit.AssertExpressions;
using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Elements;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.Dom
{
	[TestFixture]
	public class HtmlBrElementTests
	{
		private Document _document;

		[SetUp]
		public void SetUp()
		{
			_document = new Document();
		}

		[Test]
		public void CreateElement() =>
			_document.CreateElement("br").Assert(brElement => brElement is HtmlBrElement);

		[Test]
		public void Defaults() => 
			((HtmlBrElement) _document.CreateElement("br")).Assert(br => br.Clear == "");

		[Test]
		public void SetClearUsingAttribute()
		{
			var br = (HtmlBrElement)_document.CreateElement("br");
			br.SetAttribute("clear", "right");
			Assert.AreEqual("right", br.Clear);
		}

		[Test]
		public void SetClear()
		{
			var br = (HtmlBrElement)_document.CreateElement("br");
			br.Clear = "all";
			Assert.AreEqual("all", br.GetAttribute("clear"));
		}
	}
}