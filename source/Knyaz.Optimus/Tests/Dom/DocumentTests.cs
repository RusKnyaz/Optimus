#if NUNIT
using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Elements;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.Dom
{
	[TestFixture]
	public class DocumentTests
	{
		private Document _document;

		[SetUp]
		public void SetUp()
		{
			_document = new Document();
		}

		[Test]
		public void Document()
		{
			var document = new Document();
			Assert.IsNull(document.OwnerDocument);
			Assert.IsNull(document.ParentNode);
		}

		[Test]
		public void Element()
		{
			var document = new Document();
			document.Write("<html><body><span></span></body></html>");
			Assert.IsNotNull(document.DocumentElement);
			Assert.AreEqual(1, document.DocumentElement.ChildNodes.Count);
			Assert.AreEqual("BODY", ((Element)document.DocumentElement.FirstChild).TagName);
			Assert.AreEqual("SPAN", ((Element)(document.DocumentElement.FirstChild).FirstChild).TagName);
			Assert.AreEqual(document, document.DocumentElement.ParentNode);
			Assert.AreEqual(document, document.DocumentElement.OwnerDocument);
		}

		[Test]
		public void CreateElement()
		{
			var document = new Document();
			var el = document.CreateElement("div");
			Assert.AreEqual("DIV", el.TagName);
			Assert.AreEqual(document, el.OwnerDocument);
		}

		[Test]
		public void Comment()
		{
			var document = new Document();
			document.Write("<html><body><!-- hello --></body></html");
			Assert.AreEqual(1, document.Body.ChildNodes.Count);
			Assert.IsInstanceOf<Comment>(document.Body.ChildNodes[0]);
		}

		[Test]
		public void AppendChildRemovesNodeFromOldParrent()
		{
			var document = new Document();
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
			var document = new Document();
			document.Write("<html><body><div id='p1'><span id='s'></span></body></html");
			var div1 = document.GetElementById("p1");
			var span = document.GetElementById("s");
			
			Assert.AreEqual(1, div1.ChildNodes.Count);
			Assert.AreEqual(1, document.Body.ChildNodes.Count);
			document.Body.InsertBefore(span, div1);
			Assert.AreEqual(0, div1.ChildNodes.Count);
			Assert.AreEqual(2, document.Body.ChildNodes.Count);
		}

		[TestCase(true, 1)]
		[TestCase(false, 0)]
		public void CloneNode(bool deep, int expectedChildCount)
		{
			var document = new Document();
			document.Write("<html><body><div id='p1'><span id='s'>Span text</span></body></html");
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
			var document = new Document();
			var com = document.CreateComment("Com");
			Assert.AreEqual("Com", com.Data);
			Assert.AreEqual(document, com.OwnerDocument);
		}

		[Test]
		public void CreateTextNode()
		{
			var document = new Document();
			var x = document.CreateTextNode("X");
			Assert.AreEqual("X", x.Data);
			Assert.AreEqual(document, x.OwnerDocument);
		}

		[Test]
		public void CreateDocumentFragment()
		{
			var document = new Document();
			var x = document.CreateDocumentFragment();
			Assert.AreEqual(document, x.OwnerDocument);
		}

		[Test]
		public void HeadBody()
		{
			var document = new Document();
			document.Write("<html><head><meta/></head><body><div></div></body></html>");
			document.Assert(doc => doc.Body.InnerHTML == "<DIV></DIV>" && doc.Head.InnerHTML == "<META></META>");
		}

		[Test]
		public void UnClosedTag()
		{
			var document = new Document();
			document.Write("<html><head><meta></head><body><div></div></body></html>");
			document.Assert(doc => doc.Body.InnerHTML == "<DIV></DIV>" && doc.Head.InnerHTML == "<META></META>");
		}

		[Test]
		public void GetElementsByClassName()
		{
			var document = new Document();
			document.Write("<html><head><meta></head><body>" +
			               "<div class='a' id='d1'></div>" +
			               "<div class = 'b' id = 'd2'></div>" +
			               "<div class='a b' id='d3'></div></body></html>");

			var result = document.GetElementsByClassName("a");

			Assert.AreEqual(2, result.Length);
			CollectionAssert.AreEqual(new[] { document.GetElementById("d1"), document.GetElementById("d3") },  result);
		}

		[Test]
		public void DocumentElementNodeName()
		{
			var document = new Document();
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

		private Document Document(string html)
		{
			var document = new Document();
			document.Write(html);
			return document;
		}

		private HtmlElement Div(string innerHtml)
		{
			var div = _document.CreateElement("div");
			div.InnerHTML = innerHtml;
			return (HtmlElement)div;
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
		[TestCase("&lang;&rang;", "〈〉")]
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
			Assert.AreEqual(expectedCount, tags.Length);
		}
	}
}
#endif