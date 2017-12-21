using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Elements;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.Dom
{
	[TestFixture]
	public class ScriptTests
	{
		[TestCase("<html><head><script id='s' type='text/html'><span>a</span></script></head></html>", "<span>a</span>")]
		[TestCase("<html><head><script id='s' type='text/javascript'>alert('a');</script></head></html>", "alert('a');")]
		public void EmbeddedScriptInnerHtml(string html, string expectedInnerHtml)
		{
			var document = new Document(null);
			document.Write(html);
			var s = document.GetElementById("s");
			Assert.AreEqual(expectedInnerHtml, s.InnerHTML);
		}

		[Test]
		public void CloneTest()
		{
			var document = new Document();
			document.Write("<script id=d>ABC</script>");
			var script = document.GetElementById("d");
			var clone = script.CloneNode() as Script;
			clone.Assert(x => x.Text == "ABC");
		}
	}
}