#if NUNIT
using Moq;
using NUnit.Framework;
using WebBrowser.Dom;
using WebBrowser.Dom.Elements;
using WebBrowser.ScriptExecuting;

namespace WebBrowser.Tests.Dom
{
	[TestFixture]
	public class ScriptTests
	{
		[TestCase("<html><head><script id='s' type='text/html'><span>a</span></script></head></html>", "<span>a</span>")]
		[TestCase("<html><head><script id='s' type='text/javascript'>alert('a');</script></head></html>", "alert('a');")]
		public void EmbeddedScriptInnerHtml(string html, string expectedInnerHtml)
		{
			var document = new Document(null, null, Mock.Of<IScriptExecutor>());
			document.Write(html);
			var s = document.GetElementById("s");
			Assert.AreEqual(expectedInnerHtml, s.InnerHtml);
		}

		[Test]
		public void GetSrc()
		{
			var document = new Document(null, null, Mock.Of<IScriptExecutor>());
			document.Write("<html><head><script type='text/javascript' src='/script.js' id='s'/></head></html>");
			var s = (Script)document.GetElementById("s");
			Assert.AreEqual("/script.js", s.Src);
			Assert.AreEqual("text/javascript", s.Type);
		}
	}
}
#endif