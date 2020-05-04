using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Elements;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.Dom
{
	[TestFixture]
	public class NodeTests
	{
		private Element CreateElement(string html)
		{
			var document = new Document();
			var tmp = document.CreateElement("div");
			tmp.InnerHTML = html;
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
		public void GetNoStyle()
		{
			var element = (HtmlElement)CreateElement("<div></div>");
			Assert.AreEqual("", element.Style[0]);
		}

		[TestCase("<div id='n1'></div><div id='n2'></div>", ExpectedResult = 4)]
		[TestCase("<div id='n2'></div><div id='n1'></div>", ExpectedResult = 2)]
		[TestCase("<div id='n2'><div id='n1'></div></div>", ExpectedResult = 10)]
		[TestCase("<div id='n1'><div id='n2'></div></div>", ExpectedResult = 20)]
		[TestCase("<div id='n2'><div><div id='n1'></div></div></div>", ExpectedResult = 10)]
		[TestCase("<div id='n1'><div><div id='n2'></div></div></div>", ExpectedResult = 20)]
		[TestCase("<div id='n2'></div><div><div id='n1'></div></div>", ExpectedResult = 4)]
		[TestCase("<div id='n1'></div><div><div id='n2'></div></div>", ExpectedResult = 2)]
		[TestCase("<div><div id='n2'></div></div><div><div id='n1'></div></div>", ExpectedResult = 4)]
		[TestCase("<div><div id='n1'></div></div><div><div id='n2'></div></div>", ExpectedResult = 2)]
		public int CompareDocumentPosition_NodeNode(string html)
		{
			var doc = new Document();
			doc.Write("<html><body>"+html+"</doby></html>");
			return doc.GetElementById("n1").CompareDocumentPosition(doc.GetElementById("n2"));
		}

		[TestCase("<div id='n1'></div>", "n1", ExpectedResult = 20)]
		[TestCase("<div id='n1'><div id='n2'></div></div>", "n2", ExpectedResult = 20)]
		[TestCase("<div id='n2'><div id='n1'></div></div>", "n2", ExpectedResult = 2)]
		[TestCase("<div id='n2'></div><div id='n1'></div>", "n2", ExpectedResult = 2)]
		[TestCase("<div id='n1'></div><div id='n2'></div>", "n2", ExpectedResult = 4)]
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

		
		[Test]
		public void SetInnerHtml()
		{
			var doc = new Document();
			var d = doc.CreateElement("<div>");
			d.InnerHTML = "<strong></strong>";
			Assert.AreEqual(doc, d.ChildNodes[0].OwnerDocument);
		}

		[Test]
		public void SetAttributeFromId()
		{
			var doc = new Document();
			var d = doc.CreateElement("div") as HtmlDivElement;
			d.SetAttribute("Id", "d");
			Assert.AreEqual("d", d.Id);
		}

		[Test]
		public void TextParentTest()
		{
			var doc = new Document();
			var div = doc.CreateElement("div");
			div.InnerHTML = "text";
			Assert.AreEqual(div, div.ChildNodes[0].ParentNode);
		}
	}
}