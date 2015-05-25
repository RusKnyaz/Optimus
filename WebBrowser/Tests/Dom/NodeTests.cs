#if NUNIT
using NUnit.Framework;
using WebBrowser.Dom;
using WebBrowser.Dom.Elements;

namespace WebBrowser.Tests.Dom
{
	[TestFixture]
	public class NodeTests
	{
		private Element CreateElement(string html)
		{
			var document = new Document();
			var tmp = document.CreateElement("div");
			tmp.InnerHtml = html;
			return (Element)tmp.FirstChild;
		}

		[Test]
		public void CloneNodeAttributeWithQuotes()
		{
			var elem =CreateElement("<div data-bind='template:\"itemTemplate\"'></div>");
			var clone = (Element)elem.CloneNode();
			Assert.AreEqual("template:\"itemTemplate\"", elem.GetAttribute("data-bind"));
			Assert.AreEqual("template:\"itemTemplate\"", clone.GetAttribute("data-bind"));
		}

		[Test]
		public void GetStyleTest()
		{
			var elem = (HtmlElement)CreateElement("<div style='width:100pt'></div>");
			var style = elem.Style;

			Assert.AreEqual(1, style.Properties.Count);
			Assert.AreEqual("100pt", style["width"], "style[\"width\"]");
			Assert.AreEqual("100pt", style.GetPropertyValue("width"));
			Assert.AreEqual("width", style[0]);
		}

		[TestCase("<div id='n1'></div><div id='n2'></div>", Result = 4)]
		[TestCase("<div id='n2'></div><div id='n1'></div>", Result = 2)]
		[TestCase("<div id='n2'><div id='n1'></div></div>", Result = 10)]
		[TestCase("<div id='n1'><div id='n2'></div></div>", Result = 20)]
		[TestCase("<div id='n2'><div><div id='n1'></div></div></div>", Result = 10)]
		[TestCase("<div id='n1'><div><div id='n2'></div></div></div>", Result = 20)]
		[TestCase("<div id='n2'></div><div><div id='n1'></div></div>", Result = 4)]
		[TestCase("<div id='n1'></div><div><div id='n2'></div></div>", Result = 2)]
		[TestCase("<div><div id='n2'></div></div><div><div id='n1'></div></div>", Result = 4)]
		[TestCase("<div><div id='n1'></div></div><div><div id='n2'></div></div>", Result = 2)]
		public int CompareDocumentPosition_NodeNode(string html)
		{
			var doc = new Document();
			doc.Write("<html><body>"+html+"</doby></html>");
			return doc.GetElementById("n1").CompareDocumentPosition(doc.GetElementById("n2"));
		}

		[TestCase("<div id='n1'></div>", "n1", Result = 20)]
		[TestCase("<div id='n1'><div id='n2'></div></div>", "n2", Result = 20)]
		[TestCase("<div id='n2'><div id='n1'></div></div>", "n2", Result = 2)]
		[TestCase("<div id='n2'></div><div id='n1'></div>", "n2", Result = 2)]
		[TestCase("<div id='n1'></div><div id='n2'></div>", "n2", Result = 4)]
		public int CompareDocumentPosition_NodeAttr(string html, string attrElemId)
		{
			var doc = new Document();
			doc.Write("<html><body>" + html + "</doby></html>");
			return doc.GetElementById("n1").CompareDocumentPosition(doc.GetElementById(attrElemId).GetAttributeNode("id"));
		}

		[Test]
		public void AttributeNode()
		{
			var doc = new Document();
			doc.Write("<html><body><div id='x'></div></doby></html>");
			var div = doc.GetElementById("x");
			var attr = div.GetAttributeNode("id");
			Assert.AreEqual(doc, attr.OwnerDocument);
			Assert.AreEqual(div, attr.OwnerElement);
			Assert.AreEqual("x", attr.Value);
		}

		[Test]
		public void AttributeNodeOfAddedElement()
		{
			var doc = new Document();
			doc.Write("<html><body><div id='x'></div></doby></html>");

			var span = doc.CreateElement("span");
			span.SetAttribute("id", "s");

			var attrNode = span.GetAttributeNode("id");
			Assert.IsNotNull(attrNode);
			Assert.AreEqual(doc, attrNode.OwnerDocument);
		}

		[Test]
		public void SetAttributeNode()
		{
			var doc = new Document();

			var span = doc.CreateElement("span");
			var attr = doc.CreateAttribute("name");
			Assert.AreEqual(doc, attr.OwnerDocument, "OwnerDocument of free attr");

			span.SetAttributeNode(attr);

			Assert.AreEqual(doc, attr.OwnerDocument, "OwnerDocument");
			Assert.AreEqual(span , attr.OwnerElement, "OwnerElement");
		}

		[Test]
		public void CompareDocumentPositionOfFreeNodes()
		{
			var doc = new Document();
			var n1 = doc.CreateElement("div");
			var n2 = doc.CreateElement("div");
			Assert.AreEqual(1, n1.CompareDocumentPosition(n2));
			Assert.AreEqual(1, n2.CompareDocumentPosition(n1));
		}

		[Test]
		public void RemoveAttribute()
		{
			var doc = new Document();
			var div = doc.CreateElement("div");
			var attr = doc.CreateAttribute("name");
			attr.Value = "D";
			div.SetAttributeNode(attr);
			Assert.AreEqual("D", div.GetAttribute("name"));

			div.RemoveAttribute("name");
			Assert.AreEqual(null, attr.OwnerElement);
		}

		[Test]
		public void RemoveAttributeNode()
		{
			var doc = new Document();
			var div = doc.CreateElement("div");
			var attr = doc.CreateAttribute("name");
			attr.Value = "D";
			div.SetAttributeNode(attr);
			Assert.AreEqual("D", div.GetAttribute("name"));

			div.RemoveAttributeNode(attr);
			Assert.AreEqual(null, attr.OwnerElement);
		}

		[TestCase("<div id='a'></div>", Result = 0)]
		[TestCase("<div id='a'><strong></strong></div>", Result = 1)]
		[TestCase("<div id='a'><strong><strong></strong></strong></div>", Result = 2)]
		[TestCase("<div id='a'><div><strong></strong></div></div>", Result = 1)]
		public int GetElementsByTagName(string html)
		{
			var doc = new Document();
			doc.Write("<html><body>" + html + "</body></html>");

			return doc.GetElementById("a").GetElementsByTagName("strong").Length;
		}

		[Test]
		public void SetInnerHtml()
		{
			var doc = new Document();
			var d = doc.CreateElement("<div>");
			d.InnerHtml = "<strong></strong>";
			Assert.AreEqual(doc, d.ChildNodes[0].OwnerDocument);
		}

		[Test]
		public void SetAttributeFromId()
		{
			var doc = new Document();
			var d = doc.CreateElement("<div>");
			d.SetAttribute("Id", "d");
			Assert.AreEqual("d", d.Id);
		}
	}
}
#endif