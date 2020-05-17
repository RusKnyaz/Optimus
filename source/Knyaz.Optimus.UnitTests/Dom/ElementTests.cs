using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Elements;
using NUnit.Framework;
using System.Collections.Generic;
using Knyaz.NUnit.AssertExpressions;

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

		[Test]
		public void SetAttributeNodeNew()
		{
			var doc = new Document();
			var div = doc.CreateElement("div");
			var attr1 = doc.CreateAttribute("attr1");
			attr1.Value = "1";
			
			var result = div.SetAttributeNode(attr1);
			Assert.IsNull(result);
			Assert.AreEqual("1", div.GetAttribute("attr1"));
		}

		[Test]
		public void SetAttributeNodeExists()
		{
			var doc = new Document();
			var div = doc.CreateElement("div");
			var attr1 = doc.CreateAttribute("attr");
			attr1.Value = "1";
			var attr2 = doc.CreateAttribute("attr");
			attr2.Value = "2";
			
			div.SetAttributeNode(attr1);
			var result = div.SetAttributeNode(attr2);
			
			Assert.AreEqual(attr1, result);
			Assert.IsNull(attr1.OwnerElement, "attr1.OwnerElement");
			Assert.AreEqual("2", div.GetAttribute("attr"));
		}
		
		[Test]
		public void SetAttributeNodeTwice()
		{
			var doc = new Document();
			var div = doc.CreateElement("div");
			var attr1 = doc.CreateAttribute("attr");
			attr1.Value = "1";
			
			div.SetAttributeNode(attr1);
			var result = div.SetAttributeNode(attr1);
			
			Assert.AreEqual(attr1, result);
			Assert.AreEqual(div, attr1.OwnerElement, "attr1.OwnerElement");
			Assert.AreEqual("1", div.GetAttribute("attr"));
		}

		[TestCase("a c", ExpectedResult = 1)]
		[TestCase("a   c  ", ExpectedResult = 1)]
		[TestCase("a b c", ExpectedResult = 1)]
		[TestCase("a b c d", ExpectedResult = 0)]
		[TestCase("", ExpectedResult = 0)]
		public int GetElementsByClassName(string selector)
		{
			var doc = new Document();
			doc.Write("<div class='a b c'></div>");
			var res = doc.GetElementsByClassName(selector);
			return res.Count;
		}

		[TestCase("span", ExpectedResult = 1)]
		[TestCase("div", ExpectedResult = 2)]
		[TestCase("*", ExpectedResult = 3)]
		public int GetElementsByTagName(string tagName)
		{
			var doc = new Document();
			doc.Write("<div><div><span></span></div></div>");
			return doc.Body.GetElementsByTagName(tagName).Count;
		}
		
		[TestCase("<div id='a'></div>", ExpectedResult = 0)]
		[TestCase("<div id='a'><strong></strong></div>", ExpectedResult = 1)]
		[TestCase("<div id='a'><strong><strong></strong></strong></div>", ExpectedResult = 2)]
		[TestCase("<div id='a'><div><strong></strong></div></div>", ExpectedResult = 1)]
		public int GetElementsByTagNameStrong(string html)
		{
			var doc = new Document();
			doc.Write("<html><body>" + html + "</body></html>");

			return doc.GetElementById("a").GetElementsByTagName("strong").Count;
		}

		[Test]
		public void Remove()
		{
			var doc = new Document();
			doc.Write("<div id='d'></div>");
			var div = doc.GetElementById("d");
			div.Remove();
			Assert.IsNull(doc.GetElementById("d"));
		}

		[Test]
		public void EventHandlingOrder()
		{
			var sequence = new List<string>();
			var document = new Document();
			var d1 = document.CreateElement("div") as HtmlElement;
			d1.Id = "A";
			var d2 = document.CreateElement("div") as HtmlElement;
			d2.Id = "B";
			d1.AppendChild(d2);
			d1.OnClick += e =>
			{
				sequence.Add("d1 attr - " + e.EventPhase + ((HtmlElement) e.CurrentTarget).Id);
				return null;
			};
			d1.AddEventListener("click", e => sequence.Add("d1 bubbling - " + e.EventPhase+((HtmlElement)e.CurrentTarget).Id), false);
			d1.AddEventListener("click", e => sequence.Add("d1 capture - " + e.EventPhase + ((HtmlElement)e.CurrentTarget).Id), true);
			d2.OnClick += e =>
			{
				sequence.Add("d2 attr - " + e.EventPhase + ((HtmlElement) e.CurrentTarget).Id);
				return null;
			};
			d2.AddEventListener("click", e => sequence.Add("d2 bubbling - " + e.EventPhase + ((HtmlElement)e.CurrentTarget).Id), false);
			d2.AddEventListener("click", e => sequence.Add("d2 capture - " + e.EventPhase + ((HtmlElement)e.CurrentTarget).Id), true);

			d2.Click();

			Assert.AreEqual("d1 capture - 1A,d2 attr - 2B,d2 bubbling - 2B,d2 capture - 2B,d1 attr - 3A,d1 bubbling - 3A", 
				string.Join(",", sequence));
		}

		[Test]
		public void EventHandlingOrderNotBubblable()
		{
			var sequence = new List<string>();
			var document = new Document();
			var d1 = document.CreateElement("div") as HtmlElement;
			d1.Id = "A";
			var d2 = document.CreateElement("div") as HtmlElement;
			d2.Id = "B";
			d1.AppendChild(d2);
			d1.OnClick += e =>
			{
				sequence.Add("d1 attr - " + e.EventPhase + ((HtmlElement) e.CurrentTarget).Id);
				return false;
			};
			d1.AddEventListener("click", e => sequence.Add("d1 bubbling - " + e.EventPhase + ((HtmlElement)e.CurrentTarget).Id), false);
			d1.AddEventListener("click", e => sequence.Add("d1 capture - " + e.EventPhase + ((HtmlElement)e.CurrentTarget).Id), true);
			d2.OnClick += e =>
			{
				sequence.Add("d2 attr - " + e.EventPhase + ((HtmlElement) e.CurrentTarget).Id);
				return false;
			};
			d2.AddEventListener("click", e => sequence.Add("d2 bubbling - " + e.EventPhase + ((HtmlElement)e.CurrentTarget).Id), false);
			d2.AddEventListener("click", e => sequence.Add("d2 capture - " + e.EventPhase + ((HtmlElement)e.CurrentTarget).Id), true);

			var evt = document.CreateEvent("Event");
			evt.InitEvent("click", false, true);
			d2.DispatchEvent(evt);

			Assert.AreEqual("d1 capture - 1A,d2 attr - 2B,d2 bubbling - 2B,d2 capture - 2B",
				string.Join(",", sequence));
		}
	}
}