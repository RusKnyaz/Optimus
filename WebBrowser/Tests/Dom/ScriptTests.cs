#if NUNIT
using Moq;
using NUnit.Framework;
using WebBrowser.Dom;

namespace WebBrowser.Tests.Dom
{
	[TestFixture]
	public class ScriptTests
	{
		[TestCase("<html><head><script id='s' type='text/html'><span>a</span></script></head></html>", "<span>a</span>")]
		[TestCase("<html><head><script id='s' type='text/javascript'>alert('a');</script></head></html>", "alert('a');")]
		public void EmbeddedScriptInnerHtml(string html, string expectedInnerHtml)
		{
			var document = new Document(null, null);
			document.Write(html);
			var s = document.GetElementById("s");
			Assert.AreEqual(expectedInnerHtml, s.InnerHTML);
		}
	}
}
#endif