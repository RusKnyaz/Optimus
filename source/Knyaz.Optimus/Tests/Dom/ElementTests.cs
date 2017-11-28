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

		[Test]
		public void ClassListReflectsClassNameChanges()
		{
			var document = new Document();
			var div = document.CreateElement("DIV");
			div.ClassName = "a b c";
			var classList = div.ClassList;
			Assert.IsTrue(classList.Contains("a"));
			Assert.IsFalse(classList.Contains("d"));
			//change class
			div.ClassName = "d e f";
			classList.Assert(x =>
				x.Contains("a") == false &&
				x.Contains("d") == true);
		}

		[Test]
		public void ClassListReflectsClassAttributeChanges()
		{
			var document = new Document();
			var div = document.CreateElement("DIV");
			div.ClassName = "a b c";
			var classList = div.ClassList;
			Assert.IsTrue(classList.Contains("a"));
			Assert.IsFalse(classList.Contains("d"));
			//change class
			div.SetAttribute("class", "d e f");
			classList.Assert(x =>
				x.Contains("a") == false &&
				x.Contains("d") == true);
		}

		[Test]
		public void ClassListReflectsClassAttributeChanges2()
		{
			var document = new Document();
			var div = document.CreateElement("DIV");
			div.ClassName = "a b c";
			var classList = div.ClassList;
			Assert.IsTrue(classList.Contains("a"));
			Assert.IsFalse(classList.Contains("d"));
			//change class
			div.Attributes["class"].Value = "d e f";
			classList.Assert(x =>
				x.Contains("a") == false &&
				x.Contains("d") == true);
		}

		[Test]
		public void Contains()
		{
			var doc = new Document();
			doc.Write("<div id=d1><div id=d2><div id=d3></div></div></div>");
			var d1 = doc.GetElementById("d1");
			var d2 = doc.GetElementById("d2");
			var d3 = doc.GetElementById("d3");
			Assert.IsTrue(d1.Contains(d2), "Contains child (d2)");
			Assert.IsTrue(d1.Contains(d3), "Contains descendant (d3)");
		}

		[Test]
		public void OwnerDocumentCanNotBeSetted()
		{
			var document = new Document();
			var div = document.CreateElement("DIV");
			var doc2 = new Document();
			div.OwnerDocument = doc2;
			Assert.AreNotEqual(doc2, div.OwnerDocument);
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

		[TestCase("<div id=parent><div id=ref></div><div id=new></div></div>", "<DIV id=\"new\"></DIV><DIV id=\"ref\"></DIV>", TestName = "InsertBefore_BothChildOfOne")]
		[TestCase("<div id=parent><div id=ref></div></div><div id=new></div>", "<DIV id=\"new\"></DIV><DIV id=\"ref\"></DIV>", TestName = "InsertBefore_DifferentParents")]
		[TestCase("<div id=parent></div><div id=new></div>", "<DIV id=\"new\"></DIV>", TestName = "InsertBefore_ParentIsEmpty")]
		[TestCase("<div id=parent><div>child</div></div><div id=new></div>", "<DIV>child</DIV><DIV id=\"new\"></DIV>", TestName = "InsertBefore_RefIsNull_AddsToEnd")]
		public void InsertBefore(string sourceHtml, string resultHtml)
		{
			var doc = new Document();
			doc.Write(sourceHtml);
			var dnew = doc.GetElementById("new");
			var dref = doc.GetElementById("ref");
			var parent = doc.GetElementById("parent");
			parent.InsertBefore(dnew, dref);
			Assert.AreEqual(resultHtml, parent.InnerHTML);
		}

		[TestCase("<DIV id=parent><DIV id=ref></DIV><DIV id=new></DIV></DIV>", "<DIV id=\"parent\"><DIV id=\"new\"></DIV></DIV>")]
		[TestCase("<DIV id=parent><DIV id=ref></DIV></DIV><DIV id=new></DIV>", "<DIV id=\"parent\"><DIV id=\"new\"></DIV></DIV>")]
		public void ReplaceChild(string sourceHtml, string resultHtml)
		{
			var doc = new Document();
			doc.Write(sourceHtml);
			var dnew = doc.GetElementById("new");
			var dref = doc.GetElementById("ref");
			var parent = doc.GetElementById("parent");
			parent.ReplaceChild(dnew, dref);
			Assert.AreEqual(resultHtml, doc.Body.InnerHTML);
		}

		[Test]
		public void ChildNodesLive()
		{
			var doc = new Document();
			doc.Write("<div id=d1></div>");
			var childNodes = doc.Body.ChildNodes;
			doc.Body.InnerHTML = "<div></div><span></span>";
			Assert.AreEqual(2, childNodes.Count);
		}
	}
}
#endif