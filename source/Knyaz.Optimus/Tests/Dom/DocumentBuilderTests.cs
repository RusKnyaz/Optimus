﻿#if NUNIT
using System.IO;
using System.Linq;
using System.Text;
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
				((HtmlElement)doc.Body.ChildNodes[2]).InnerHTML == "<TBODY><TR></TR></TBODY>");
		}

		private Document Build(string txt)
		{
			var d = new Document();
			using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(txt)))
			{
				DocumentBuilder.Build(d, stream);	
			}
			return d;
		}
	}
}
#endif