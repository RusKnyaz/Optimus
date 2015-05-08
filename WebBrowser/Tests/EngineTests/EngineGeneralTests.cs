#if NUNIT
using System;
using NUnit.Framework;
using Text = WebBrowser.Dom.Text;

namespace WebBrowser.Tests.EngineTests
{
	[TestFixture]
	public class EngineGeneralTests
	{
		[Test]
		public void EmptyHtml()
		{
			var engine = new Engine();
			engine.Load("<html></html>");
		}
		
		[Test]
		public void GenerateContent()
		{
			var engine = new Engine();
			engine.Load("<html><head><script>var elem = document.getElementById('content');elem.innerHTML = 'Hello';</script></head><body><div id='content'></div></body></html>");
			var contentDiv = engine.Document.GetElementById("content");
			Assert.AreEqual("Hello", contentDiv.InnerHtml);
			Assert.AreEqual(1, contentDiv.ChildNodes.Count);
			var text = contentDiv.ChildNodes[0] as Text;
			Assert.IsNotNull(text);
			Assert.AreEqual("Hello", text.Data);
		}

		[Test]
		public void DomManipulation()
		{
			var engine = new Engine();
			engine.Load("<html><head><script>var c2 = document.getElementById('content2');document.documentElement.getElementsByTagName('body')[0].insertBefore(document.createElement('<div id=\"c3\">'), c2);</script></head><body><div id='content1'></div><div id='content2'></div></body></html>");
			Assert.AreEqual(3, engine.Document.DocumentElement.GetElementsByTagName("body")[0].ChildNodes.Count);
			var elem = engine.Document.GetElementById("c3");
			Assert.IsNotNull(elem);
		}

		[Test]
		public void Text()
		{
			var engine = new Engine();
			ScriptExecuting.ScriptExecutor.Log = o => Console.WriteLine(o);
			engine.Load("<html><head><script>"+
			@"var c2 = document.getElementById('content1').innerHTML = 'Hello';" +
				"</script></head><body><span id='content1'></span></body></html>");
			var elem = engine.Document.GetElementById("content1");
			Assert.AreEqual("Hello", elem.InnerHtml);
		}
	}
}
#endif