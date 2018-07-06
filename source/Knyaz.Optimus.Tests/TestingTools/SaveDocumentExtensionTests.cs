using System.Threading.Tasks;
using Knyaz.Optimus.Dom;
using Knyaz.Optimus.TestingTools;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.TestingTools
{
	[TestFixture]
	public static class SaveDocumentExtensionTests
	{
		[TestCase("<html><body>Hello</body></html>",
			"<!DOCTYPE html><HTML><HEAD></HEAD><BODY>Hello</BODY></HTML>", TestName = "SimpleHtml")]
		[TestCase("<html><head><script>console.log('a')</script></head><body>Hello</body></html>",
			"<!DOCTYPE html><HTML><HEAD></HEAD><BODY>Hello</BODY></HTML>", TestName = "SkipScript")]
		[TestCase("<html><body width=100px></body></html>", "<!DOCTYPE html><HTML><HEAD></HEAD><BODY width=\"100px\"></BODY></HTML>", TestName = "Attribute")]
		[TestCase("<html><body onload='onLoad(10)'></body></html>", "<!DOCTYPE html><HTML><HEAD></HEAD><BODY></BODY></HTML>", TestName = "SkipFuncAttribute")]
		[TestCase("<html><body myatt='myVal'></body></html>", "<!DOCTYPE html><HTML><HEAD></HEAD><BODY myatt=\"myVal\"></BODY></HTML>", TestName = "CustomAttribute")]
		[TestCase("<html><body hidden></body></html>", "<!DOCTYPE html><HTML><HEAD></HEAD><BODY hidden=\"\"></BODY></HTML>")]
		public static void ReadWrite(string srcHtml, string expectedHtml)
		{
			var doc = DomImplementation.Instance.CreateHtmlDocument();
			doc.Write(srcHtml);
			Assert.AreEqual(expectedHtml, doc.Save());
		}

		[TestCase("<html><head><style>body{background-color:red}</style></head><body>hi</body></html>", 
			"<HTML><HEAD><STYLE>body{background-color:red}</STYLE></HEAD><BODY>hi</BODY></HTML>", TestName = "EmbeddedStyle")]
		[TestCase("<html><link type='text/css' rel='stylesheet' href='http://localhost/main.css'><body>hi</body></html>",
			"<HTML><HEAD><STYLE>body{background-color:red}</STYLE></HEAD><BODY>hi</BODY></HTML>", TestName = "External style in head")]
		[TestCase("<html><head><link type='text/css' rel='stylesheet' href='http://localhost/main.css'></head><body>hi</body></html>",
			"<HTML><HEAD><STYLE>body{background-color:red}</STYLE></HEAD><BODY>hi</BODY></HTML>", TestName = "External style out of head")]
		public static async Task WriteStyle(string srcHtml, string expectedHtml)
		{
			var resourceProvider = Mocks
				.ResourceProvider("http://localhost/",srcHtml)
				.Resource("http://localhost/main.css", "body{background-color:red}");

			var engine = new Engine(resourceProvider) {ComputedStylesEnabled = true};
			var page = await engine.OpenUrl("http://localhost");
			Assert.AreEqual(expectedHtml, page.Document.Save());
		}
	}
}