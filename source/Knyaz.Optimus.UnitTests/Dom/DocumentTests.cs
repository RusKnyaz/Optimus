using System.Drawing;
using System.Linq;
using Knyaz.NUnit.AssertExpressions;
using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.Dom.Interfaces;
using NUnit.Framework;
using Moq;

namespace Knyaz.Optimus.Tests.Dom
{
	[TestFixture]
	public class DocumentTests
	{
		private HtmlDocument _document;

		[SetUp]
		public void SetUp() => _document = new HtmlDocument();

		[Test]
		public void Document() =>
			new HtmlDocument().Assert(document =>
				document.OwnerDocument == null &&
				document.ParentNode == null);

		[Test]
		public void Element()
		{
			var document = new HtmlDocument();
			document.Write("<html><body><span></span></body></html>");
			Assert.IsNotNull(document.DocumentElement);
			Assert.AreEqual(2, document.DocumentElement.ChildNodes.Count);
			Assert.AreEqual("HEAD", ((Element)document.DocumentElement.FirstChild).TagName);
			Assert.AreEqual("BODY", ((Element)document.DocumentElement.FirstChild.NextSibling).TagName);
			Assert.AreEqual("SPAN", ((Element)(document.DocumentElement.FirstChild.NextSibling).FirstChild).TagName);
			Assert.AreEqual(document, document.DocumentElement.ParentNode);
			Assert.AreEqual(document, document.DocumentElement.OwnerDocument);
		}

		[Test]
		public void CreateElement()
		{
			var document = new HtmlDocument();
			var el = document.CreateElement("div");
			Assert.AreEqual("DIV", el.TagName);
			Assert.AreEqual(document, el.OwnerDocument);
		}

		[Test]
		public void Comment()
		{
			var document = new HtmlDocument();
			document.Write("<html><body><!-- hello --></body></html");
			Assert.AreEqual(1, document.Body.ChildNodes.Count);
			Assert.IsInstanceOf<Comment>(document.Body.ChildNodes[0]);
		}

		[Test]
		public void AppendChildRemovesNodeFromOldParrent()
		{
			var document = new HtmlDocument();
			document.Write("<html><body><div id='p1'><span id='s'></span></div><div id='p2'></div></body></html");
			var div1 = document.GetElementById("p1");
			var div2 = document.GetElementById("p2");
			var span = document.GetElementById("s");

			Assert.AreEqual(1, div1.ChildNodes.Count);
			Assert.AreEqual(0, div2.ChildNodes.Count);
			div2.AppendChild(span);
			Assert.AreEqual(0, div1.ChildNodes.Count);
			Assert.AreEqual(1, div2.ChildNodes.Count);
		}

		[Test]
		public void InsertBeforeRemovesNodeFromOldParrent()
		{
			var document = new HtmlDocument();
			document.Write("<html><body><div id='p1'><span id='s'></span></div></body></html>");
			var div1 = document.GetElementById("p1");
			var span = document.GetElementById("s");

			Assert.AreEqual(1, div1.ChildNodes.Count, "Child nodes count before inserting");
			Assert.AreEqual(1, document.Body.ChildNodes.Count);
			document.Body.InsertBefore(span, div1);
			Assert.AreEqual(0, div1.ChildNodes.Count);
			Assert.AreEqual(2, document.Body.ChildNodes.Count);
		}

		[TestCase(true, 1)]
		[TestCase(false, 0)]
		public void CloneNode(bool deep, int expectedChildCount)
		{
			var document = new HtmlDocument();
			document.Write("<html><body><div id='p1'><span id='s'>Span text</span></div></body></html>");
			var span = document.GetElementById("s");

			document.Assert(doc =>
				doc.Body.ChildNodes.Count == 1 &&
				doc.GetElementById("p1").ChildNodes.Count == 1 &&
				doc.GetElementById("s").ChildNodes.Count == 1);

			var clone = span.CloneNode(deep) as HtmlElement;
			Assert.IsNotNull(clone);

			//sate of all old elements should be the same as before
			document.Assert(doc =>
				doc.Body.ChildNodes.Count == 1 &&
				doc.GetElementById("p1").ChildNodes.Count == 1 &&
				doc.GetElementById("s").ChildNodes.Count == 1);

			Assert.AreEqual(expectedChildCount, clone.ChildNodes.Count);
			Assert.IsNotNull(clone.OwnerDocument, "Clone's ownerDocument");
			Assert.AreEqual(span.OwnerDocument, clone.OwnerDocument, "Clone's ownerDocument");
			Assert.IsNull(clone.ParentNode, "Clone's parentNode");
			Assert.AreEqual("s", clone.Id);
		}

		[Test]
		public void CreateComment()
		{
			var document = new HtmlDocument();
			document.CreateComment("Com").Assert(com => com.Data == "Com" && com.OwnerDocument == document);
		}

		[Test]
		public void CreateTextNode()
		{
			var document = new HtmlDocument();
			document.CreateTextNode("X").Assert(text => text.Data == "X" && text.OwnerDocument == document);
		}

		[Test]
		public void CreateAttribute()
		{
			var document = new HtmlDocument();
			document.CreateAttribute("a").Assert(attr => attr.Name == "a" && attr.OwnerDocument == document);
		}

		[Test]
		public void CreateDocumentFragment()
		{
			var document = new HtmlDocument();
			var x = document.CreateDocumentFragment();
			Assert.AreEqual(document, x.OwnerDocument);
			Assert.IsNull(x.ParentNode);
		}

		[Test]
		public void HeadBody()
		{
			var document = new HtmlDocument();
			document.Write("<html><head><meta/></head><body><div></div></body></html>");
			document.Assert(doc => doc.Body.InnerHTML == "<DIV></DIV>" && doc.Head.InnerHTML == "<META></META>");
		}

		[Test]
		public void UnClosedTag()
		{
			var document = new HtmlDocument();
			document.Write("<html><head><meta></head><body><div></div></body></html>");
			document.Assert(doc => doc.Body.InnerHTML == "<DIV></DIV>" && doc.Head.InnerHTML == "<META></META>");
		}

		[Test]
		public void GetElementsByClassName()
		{
			var document = new HtmlDocument();
			document.Write("<html><head><meta></head><body>" +
						   "<div class='a' id='d1'></div>" +
						   "<div class = 'b' id = 'd2'></div>" +
						   "<div class='a b' id='d3'></div></body></html>");

			var result = document.GetElementsByClassName("a");

			Assert.AreEqual(2, result.Count);
			CollectionAssert.AreEqual(new[] { document.GetElementById("d1"), document.GetElementById("d3") }, result);
		}

		[Test]
		public void DocumentElementNodeName()
		{
			var document = new HtmlDocument();
			document.Write("<html><head><meta></head><body>" +
						   "<div class='a' id='d1'></div>" +
						   "<div class = 'b' id = 'd2'></div>" +
						   "<div class='a b' id='d3'></div></body></html>");

			Assert.AreEqual("HTML", document.DocumentElement.NodeName);
		}

		[TestCase("<html></html>", "BackCompat")]
		[TestCase("<!DOCTYPE html><html></html>", "CSS1Compat")]
		public void CompatMode(string html, string expectedMode)
		{
			var document = Document(html);
			Assert.AreEqual(expectedMode, document.CompatMode);
		}

		private HtmlDocument Document(string html)
		{
			var document = new HtmlDocument();
			document.Write(html);
			return document;
		}

		private HtmlElement Div(string innerHtml)
		{
			var div = _document.CreateElement("div");
			div.InnerHTML = innerHtml;
			return (HtmlElement)div;
		}

		[TestCase("")]
		[TestCase("<BODY></BODY>")]
		[TestCase("<BODY></BODY><HEAD></HEAD>")]
		public void HeadBodyAlwaysExists(string html)
		{
			var h = _document.CreateElement("html");
			h.InnerHTML = html;
			h.Assert(x => x.FirstChild.NodeName == "HEAD" && x.LastChild.NodeName == "BODY" && x.FirstChild.NextSibling == x.LastChild);
		}

		[Test]
		//Parsing tests stoled from html5test project
		public void Parsing()
		{
			Div("<div<div>").Assert(e => e.FirstChild.NodeName == "DIV<DIV");
			Div("<div foo<bar=''>").Assert(
				e => ((HtmlElement)e.FirstChild).Attributes[0].NodeName == "foo<bar" &&
				((HtmlElement)e.FirstChild).Attributes[0].Name == "foo<bar");

			Div("<div foo=`bar`>").Assert(e => ((HtmlElement)e.FirstChild).GetAttribute("foo") == "`bar`");

			Div("<div \"foo=''>").Assert(e => ((HtmlElement)e.FirstChild).Attributes[0].NodeName == "\"foo" &&
				((HtmlElement)e.FirstChild).Attributes[0].Name == "\"foo");

			Div("<a href='\nbar'></a>").Assert(e => ((HtmlElement)e.FirstChild).GetAttribute("href") == "\nbar");
			Div("<!DOCTYPE html>").Assert(e => e.FirstChild == null);

			Div("<?import namespace=\"foo\" implementation=\"#bar\">").Assert(e =>
				((CharacterData)e.FirstChild).NodeValue == "?import namespace=\"foo\" implementation=\"#bar\"" &&
				e.FirstChild.NodeType == 8);

			Div("<!--foo--bar-->").Assert(e => e.FirstChild.NodeType == 8 && ((Comment)e.FirstChild).NodeValue == "foo--bar");

			Div("<![CDATA[x]]>").Assert(e => e.FirstChild.NodeType == 8 && ((Comment)e.FirstChild).NodeValue == "[CDATA[x]]");

			Div("<textarea><!--</textarea>--></textarea>").Assert(e => ((CharacterData)e.FirstChild.FirstChild).NodeValue == "<!--");
			Div("<textarea><!--</textarea>-->").Assert(e => ((CharacterData)e.FirstChild.FirstChild).NodeValue == "<!--");
			Div("<style><!--</style>--></style>").Assert(e => ((CharacterData)e.FirstChild.FirstChild).NodeValue == "<!--");
			Div("<style><!--</style>-->").Assert(e => ((CharacterData)e.FirstChild.FirstChild).NodeValue == "<!--");
		}

		[TestCase("\u000D", "\u000A")]
		[TestCase("&lang;&rang;", "\u27E8\u27E9")]
		[TestCase("&apos;", "'")]
		[TestCase("&ImaginaryI;", "\u2148")]
		[TestCase("&Kopf;", "\uD835\uDD42")]
		[TestCase("&notinva;", "\u2209")]
		public void ParsingChars(string innerHtml, string expectedNodeValue)
		{
			Div(innerHtml).Assert(e => ((CharacterData)e.FirstChild).NodeValue == expectedNodeValue);
		}

		[TestCase("<span></span><span></span>", "span", 2)]
		[TestCase("<span><span></span></span>", "span", 2)]
		[TestCase("<span><div><span></span><div></span>", "span", 2)]
		[TestCase("<SpAn></sPaN>", "span", 1)]
		public void GetElementsByTagName(string html, string tagName, int expectedCount)
		{
			var tags = Document(html).GetElementsByTagName("span");
			Assert.AreEqual(expectedCount, tags.Count);
		}

		[TestCase("<table><tbody><tr id=row_0><td></td></tr><tr id=row_1><td></td></tr></tbody></table>", "tr[id^='row_']", "row_0,row_1")]
		[TestCase("<table><tbody><tr id=row_0><td></td></tr><tr id=row_1><td></td></tr></tbody></table>", "tr[id^=row_]", "row_0,row_1")]
		[TestCase("<tr id=row_0><td column-name=Name id=c1></td><td  columne-name=Id id=c2></td></tr>", "[column-name='Name']", "c1")]
		[TestCase("<tr id=row_0><td column-name=Name id=c1></td><td  columne-name=Id id=c2></td></tr>", "[column-name=Name]", "c1")]
		[TestCase("<tr id=row_0><td column-name=Name id=c1></td><td  columne-name=Id id=c2></td></tr>", "#c2", "c2")]
		[TestCase("<div id=sizzle1><div id=in class=blockUI></div></div>", "#sizzle1 >.blockUI", "in")]
		[TestCase("<div id=sizzle1><div id=in class=blockUI></div></div>", "#sizzle1 > .blockUI", "in")]
		[TestCase("<div id=sizzle1><div id=in class=blockUI></div></div>", "#sizzle1>.blockUI", "in")]
		[TestCase("<div id=sizzle1><a><div id=in class=blockUI></div></a></div>", "#sizzle1 >.blockUI", "")]
		[TestCase("<div id=s></div><div id=s></div>", "#s", "s,s")]
		public void QuerySelectorAll(string html, string selector, string expectedIds)
		{
			var doc = new HtmlDocument();
			doc.Write(html);

			var items = doc.QuerySelectorAll(selector).OfType<IElement>().Select(x => x.Id).ToArray();

			CollectionAssert.AreEqual(string.IsNullOrEmpty(expectedIds) ? new string[0] : expectedIds.Split(','), items);
		}

		[TestCase("<span id=span1></span>", "#span1", "<SPAN id=\"span1\"></SPAN>", Description = "Root by id")]
		[TestCase("<div><span id=span1></span></div>", "#span1", "<SPAN id=\"span1\"></SPAN>", Description = "Nested by id")]
		[TestCase("<div><span id=span1></span></div>", "span", "<SPAN id=\"span1\"></SPAN>", Description = "Nested by tag name")]
		[TestCase("<div id=div1><span>1</span></div><DIV id=div2><SPAN>2</SPAN></DIV>", "#div2 span", "<SPAN>2</SPAN>", Description = "By id, than by tag name")]
		[TestCase("<span class='A'></span>", ".A", "<SPAN class=\"A\"></SPAN>", Description = "Simple class selector")]
		[TestCase("<span class='B A'></span>", ".A", "<SPAN class=\"B A\"></SPAN>", Description = "Simple class selector from multiclass defenition")]
		[TestCase("<div class=A><span>1</span></div><DIV><SPAN>2</SPAN></DIV>", ".A span", "<SPAN>1</SPAN>", Description = "By class than by tag name")]
		[TestCase("<div id=a></div><div id=b></div>", "[id=b]", "<DIV id=\"b\"></DIV>")]
		[TestCase("<div><div><label for='OrganizationId'></label></div></div>", "label[for=OrganizationId]", "<LABEL for=\"OrganizationId\"></LABEL>")]
		[TestCase("<div a=ab></div><div a=ac></div><div a=bc></div>", "[a^=a]", "<DIV a=\"ab\"></DIV>,<DIV a=\"ac\"></DIV>")]
		[TestCase("<ul class='left'></ul>", "ul.left", "<UL class=\"left\"></UL>")]
		[TestCase("<select id=d><option selected></option></select>", "#d [selected]", "<OPTION selected=\"\"></OPTION>")]
		public void QuerySelectorAll2(string html, string selector, string expectedResult)
		{
			var doc = new HtmlDocument();
			doc.Write(html);
			Assert.AreEqual(expectedResult, string.Join(",", doc.QuerySelectorAll(selector).OfType<HtmlElement>().Select(x => x.OuterHTML)));
		}

		[Test]
		public void InnerHtml()
		{
			var doc = new HtmlDocument();
			doc.Head.InnerHTML = "<script>var a = 5;</script>";
			Assert.AreEqual("<HEAD><SCRIPT>var a = 5;</SCRIPT></HEAD><BODY></BODY>", doc.DocumentElement.InnerHTML);
		}

		[Test]
		public void AddDocType()
		{
			var impl = new DomImplementation();
			var dt = impl.CreateDocumentType("HTML", "", "");
			var doc = impl.CreateDocument("http://www.w3.org/1999/xhtml", "html", dt);
			Assert.AreEqual(doc, dt.OwnerDocument);
		}

		[Test]
		public void AddChildTwiceDeprecated()
		{
			var doc = DomImplementation.Instance.CreateHtmlDocument();
			Assert.Throws<DOMException>(() => doc.AppendChild(doc.CreateElement("span")));
		}

		[Test]
		public void SetBody()
		{
			var doc = new HtmlDocument();
			var body = doc.CreateElement("body");
			body.InnerHTML = "ABC";
			doc.Body = (HtmlBodyElement)body;
			Assert.AreEqual("<HEAD></HEAD><BODY>ABC</BODY>", doc.DocumentElement.InnerHTML);
		}
		
		[Test]
		public static void GetBoundingClientRect()
		{
			var layoutService = Mock.Of<ILayoutService>(x => 
				x.GetElementBounds(It.IsAny<Element>()) == new[]{new RectangleF(1,3,7,11)});

			var document = DomImplementation.Instance.CreateHtmlDocument("test");
			document.AttachLayoutService(layoutService);
			document.Write("<html><div id=d>abc</div></html>");
			
			var div = document.GetElementById("d");
			div.GetBoundingClientRect().Assert(domRect =>
				domRect.X == 1 &&
				domRect.Y == 3 &&
				domRect.Width == 7 &&
				domRect.Height == 11 &&
				domRect.Left == 1 &&
				domRect.Top == 3 &&
				domRect.Right == 8 &&
				domRect.Bottom == 14 );
		}
		
		[Test]
		public static void GetBoundingClientRectMultiple()
		{
			var layoutService = Mock.Of<ILayoutService>(x => 
				x.GetElementBounds(It.IsAny<Element>()) == new[]
				{
					new RectangleF(1,3,7,11),
					new RectangleF(0,5,1,27)
				});

			var document = DomImplementation.Instance.CreateHtmlDocument("test");
			document.AttachLayoutService(layoutService);
			document.Write("<html><div id=d>abc</div></html>");

			var div = document.GetElementById("d");
			div.GetBoundingClientRect().Assert(domRect =>
				domRect.X == 0 &&
				domRect.Y == 3 &&
				domRect.Width == 8 &&
				domRect.Height == 29 &&
				domRect.Left == 0 &&
				domRect.Top == 3 &&
				domRect.Right == 8 &&
				domRect.Bottom == 32 );
		}
	}
}