#if NUNIT
using NUnit.Framework;
using WebBrowser.Dom;
using WebBrowser.Dom.Elements;

namespace WebBrowser.Tests
{
	[TestFixture]
	public class DocumentTests
	{
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
			Assert.AreEqual("body", ((Element)document.DocumentElement.FirstChild).TagName);
			Assert.AreEqual("span", ((Element)((Element)document.DocumentElement.FirstChild).FirstChild).TagName);
			Assert.AreEqual(document, document.DocumentElement.ParentNode);
			Assert.AreEqual(document, document.DocumentElement.OwnerDocument);
		}

		[Test]
		public void CreateElement()
		{
			var document = new Document();
			var el = document.CreateElement("div");
			Assert.AreEqual("div", el.TagName);
			Assert.AreEqual(document, el.OwnerDocument);
		}

		[Test]
		public void InputChecked()
		{
			var document = new Document();
			document.Write("<html><body><input id='i' type='checkbox' checked='true'></input></body></html>");
			var el = (HtmlInputElement)document.GetElementById("i");
			Assert.IsTrue(el.Checked);
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
			var div1 = document.GetElementById("p1");
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
	}
}
#endif