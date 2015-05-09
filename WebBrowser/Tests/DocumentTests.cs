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
		public void Element()
		{
			var document = new Document(null);
			document.Write("<html><body><span></span></body></html>");
			Assert.IsNotNull(document.DocumentElement);
			Assert.AreEqual(1, document.DocumentElement.ChildNodes.Count);
			Assert.AreEqual("body", ((Element)document.DocumentElement.FirstChild).TagName);
			Assert.AreEqual("span", ((Element)((Element)document.DocumentElement.FirstChild).FirstChild).TagName);
		}

		[Test]
		public void DocumentCreateElement()
		{
			var document = new Document(null);
			var el = document.CreateElement("div");
			Assert.AreEqual("div", el.TagName);
		}

		[Test]
		public void InputChecked()
		{
			var document = new Document(null);
			document.Write("<html><body><input id='i' type='checkbox' checked='true'></input></body></html>");
			var el = (HtmlInputElement)document.GetElementById("i");
			Assert.IsTrue(el.Checked);
		}

		[Test]
		public void Comment()
		{
			var document = new Document(null);
			document.Write("<html><body><!-- hello --></body></html");
			Assert.AreEqual(1, document.Body.ChildNodes.Count);
			Assert.IsInstanceOf<Comment>(document.Body.ChildNodes[0]);
		}

		[Test]
		public void RemoveNodeFromParrentWhenAppendToOtherNode()
		{
			var document = new Document(null);
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
	}
}
#endif