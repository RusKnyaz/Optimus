#if NUNIT
using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using WebBrowser.Dom;

namespace WebBrowser.Tests.Dom
{
	[TestFixture]
	public class DocumentBuilderTests
	{
		[Test]
		public void HtmlSkipNewLines()
		{
			Build("<html>\r\n\t<head></head></html>").Assert(doc => 
				doc.DocumentElement.ChildNodes.Count == 1 &&
				doc.DocumentElement.FirstChild.NodeName == "HEAD");
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