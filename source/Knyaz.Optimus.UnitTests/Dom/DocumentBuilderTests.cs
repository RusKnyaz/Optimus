using System.IO;
using System.Text;
using Knyaz.NUnit.AssertExpressions;
using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Elements;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.Dom
{
	[TestFixture]
	public class DocumentBuilderTests
	{
		[Test]
		public void HtmlSkipNewLines()
		{
			Build("<html>\r\n\t<head></head></html>").Assert(doc => 
				doc.DocumentElement.ChildNodes.Count == 2 &&
				doc.DocumentElement.FirstChild.NodeName == "HEAD");
		}

		[Test]
		public void ZeroLevelNodes()
		{
			Build("<!DOCTYPE html><html><head></head><body></body></html>").Assert(doc =>
				doc.ChildNodes.Count == 2 &&
					doc.ChildNodes[0].NodeType == Node.DOCUMENT_TYPE_NODE &&
					doc.ChildNodes[1].NodeType == Node.ELEMENT_NODE &&
					((HtmlElement) doc.ChildNodes[1]).TagName == "HTML" &&
					doc.DocumentElement.TagName == "HTML");
		}

		[Test]
		public void TableNotAcceptDivs()
		{
			var document = Build("<html><body><table><div></div><span></span><tbody></tbody><tr></tr></table></body></html>");
			document.Assert(doc =>
				doc.Body.ChildNodes.Count == 3 &&
				((HtmlElement)doc.Body.ChildNodes[0]).TagName == "DIV" &&
				((HtmlElement) doc.Body.ChildNodes[1]).TagName == "SPAN" &&
				((HtmlElement) doc.Body.ChildNodes[2]).TagName == "TABLE" &&
				((HtmlElement)doc.Body.ChildNodes[2]).InnerHTML == "<TBODY></TBODY><TBODY><TR></TR></TBODY>");
		}

		[Test]
		public void CreateColGroupForCol()
		{
			var document = Build("<html><body><table><colgroup></colgroup><col></col></table></body></html>");
			document.Assert(doc => 
				doc.Body.ChildNodes[0].ChildNodes.Count == 2 &&
				((HtmlElement)doc.Body.ChildNodes[0].ChildNodes[0]).TagName == "COLGROUP" &&
				((HtmlElement)doc.Body.ChildNodes[0].ChildNodes[1]).TagName == "COLGROUP");
		}

		[Test]
		public void TableInnerHtml()
		{
			var document = Build("<html></html>");
			var table = document.CreateElement("table");
			table.InnerHTML = "<div></div>";
			table.Assert(tbl => tbl.ChildNodes.Count == 1 && 
				((HtmlElement)tbl.FirstChild).TagName == "DIV");
		}

		[Test]
		public void NoMainTags()
		{
			Build("Text<p>para</p>").Assert(doc => 
				doc.InnerHTML == "<HTML><HEAD></HEAD><BODY>Text<P>para</P></BODY></HTML>");
		}

		[Test]
		public void DomBuildOrder()
		{
			var document = new HtmlDocument();
			var res = "";
            
			var div = document.CreateElement("div");
			div.AddEventListener("DOMNodeInserted", x => {
				res = ((HtmlElement)x.Target).OuterHTML;
			});

			div.InnerHTML = "<div><span></span></div>";
			Assert.AreEqual("<DIV><SPAN></SPAN></DIV>", res);
		}

		private HtmlDocument Build(string txt)
		{
			var d = new HtmlDocument();
			using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(txt)))
			{
				DocumentBuilder.Build(d, stream);	
			}
			return d;
		}
	}
}