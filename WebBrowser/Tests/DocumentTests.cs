﻿#if NUNIT
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
	}
}
#endif