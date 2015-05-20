#if NUNIT
using System.Linq;
using NUnit.Framework;
using WebBrowser.Dom;
using WebBrowser.Dom.Elements;

namespace WebBrowser.Tests.Dom
{
	[TestFixture]
	public class NodeTests
	{
		[Test]
		public void CloneNodeAttributeWithQuotes()
		{
			var elem = (Element)DocumentBuilder.Build("<div data-bind='template:\"itemTemplate\"'></div>").Single();
			var clone = (Element)elem.CloneNode();
			Assert.AreEqual("template:\"itemTemplate\"", elem.GetAttribute("data-bind"));
			Assert.AreEqual("template:\"itemTemplate\"", clone.GetAttribute("data-bind"));
		}

		[Test]
		public void GetStyleTest()
		{
			var elem = (HtmlElement)DocumentBuilder.Build("<div style='width:100pt'></div>").Single();
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
		public int CompareDocumentPosition(string html)
		{
			var doc = new Document(null);
			doc.Write("<html><body>"+html+"</doby></html>");
			return doc.GetElementById("n1").CompareDocumentPosition(doc.GetElementById("n2"));
		}
	}
}
#endif