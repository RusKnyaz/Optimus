#if NUNIT
using System.IO;
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