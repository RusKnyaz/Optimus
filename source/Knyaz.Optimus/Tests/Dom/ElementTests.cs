#if NUNIT
using Knyaz.Optimus.Dom;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.Dom
{
	[TestFixture]
	public class ElementTests
	{
		[Test]
		public void GetOuterHtml()
		{
			var document = new Document();
			document.Write("<html><div id=a CustomAttr=abc><span>123</span></div></html>");
			var div = document.GetElementById("a");
			Assert.AreEqual("<DIV id=\"a\" customattr=\"abc\"><SPAN>123</SPAN></DIV>", div.OuterHTML);
		}

		[Test]
		public void SetOuterHtml()
		{
			var document = new Document();
			document.Write("<html><body><div id=a CustomAttr=abc><span>123</span></div></body></html>");
			var div = document.GetElementById("a");
			div.OuterHTML = "<span>123</span><span>qwe</span>";
			Assert.AreEqual("<SPAN>123</SPAN><SPAN>qwe</SPAN>", document.Body.InnerHTML);
		}

		[Test]
		public void SetOuterHtmlWithoutParent()
		{
			var document = new Document();
			var div = document.CreateElement("div");
			div.InnerHTML = "ABC";
			Assert.AreEqual("<DIV>ABC</DIV>", div.OuterHTML);
			Assert.Throws<DOMException>(() => div.OuterHTML = "<SPAN>123</SPAN>");
		}

		[TestCase("beforebegin", "A<DIV>B</DIV>C<DIV id=\"a\"><SPAN>123</SPAN></DIV>")]
		[TestCase("afterbegin", "<DIV id=\"a\">A<DIV>B</DIV>C<SPAN>123</SPAN></DIV>")]
		[TestCase("beforeend", "<DIV id=\"a\"><SPAN>123</SPAN>A<DIV>B</DIV>C</DIV>")]
		[TestCase("afterend", "<DIV id=\"a\"><SPAN>123</SPAN></DIV>A<DIV>B</DIV>C")]
		public void InsertAdjacentHtml(string position, string expectedHtml)
		{
			var document = new Document();
			document.Write("<html><body><div id=a><span>123</span></div></body></html>");
			var div = document.GetElementById("a");
			div.InsertAdjacentHTML(position, "A<div>B</div>C");
			Assert.AreEqual(expectedHtml, document.Body.InnerHTML);
		}

		[Test]
		public void ClassListAdd()
		{
			var document = new Document();
			var div = document.CreateElement("DIV");
			div.ClassName = "a b c";
			div.ClassList.Add("f");
			Assert.AreEqual("a b c f", div.ClassName);
			Assert.AreEqual("a b c f", div.GetAttribute("class"));
		}

		[TestCase("<div><div id=start name=target></div></div>", "div")]
		[TestCase("<div name=target><span id=start></span></div>", "div")]
		[TestCase("<div id=d1 name=target><div id=start></div></div>", "#d1")]
		public void Closest(string html, string query)
		{
			var document = new Document();
			document.Write(html);
			var start = document.GetElementById("start");
			start.Closest(query).Assert(elt => elt.GetAttribute("name") == "target");
		}
	}
}
#endif