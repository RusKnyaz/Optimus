using Knyaz.NUnit.AssertExpressions;
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
			var document = new HtmlDocument(null);
			document.Write(html);
			document.GetElementById("s").Assert(script => script.InnerHTML == expectedInnerHtml); 
		}

		[Test]
		public void ParseFromHtml()
		{
			var document = new HtmlDocument(null);
			var div = document.CreateElement("div");
			div.InnerHTML = "<script>var x = 5;</script>";
			div.Assert(d=> 
				d.ChildNodes.Count == 1 &&
				d.FirstChild.ChildNodes.Count == 1 &&
				d.FirstChild.FirstChild.NodeName == "#text" &&
				((Text)d.FirstChild.FirstChild).NodeValue == "var x = 5;");
		}

		[Test]
		public void CloneTest()
		{
			var document = new HtmlDocument();
			document.Write("<script id=d>ABC</script>");
			var script = document.GetElementById("d");
			var clone = script.CloneNode(true) as HtmlScriptElement;
			clone.Assert(x => x.Text == "ABC");
		}
	}
}