#if NUNIT
using System.Linq;
using System.Threading;
using NUnit.Framework;
using WebBrowser.Dom.Elements;
using WebBrowser.Tools;

namespace WebBrowser.Tests
{
	[TestFixture]
	public class TestPages
	{
		private string GetTestUrl(string testUrl)
		{
			return "http://localhost:21449/" + (testUrl.Contains("/") ? (testUrl+".html") : (testUrl + "/index.html"));
		}

		private Engine Open(string testUrl)
		{
			var engine = new Engine().AttachConsole();
			engine.OpenUrl(GetTestUrl(testUrl));
			engine.WaitDocumentLoad();
			return engine;
		}

		[TestCase("jq_rq_knock")]
		[TestCase("jq_rq")]
		public void OpenHelloPageTest(string subUrl)
		{
			var engine = Open(subUrl);
			Thread.Sleep(1000);

			var greeting = engine.Document.GetElementById("greeting");
			Assert.IsNotNull(greeting);
			Assert.AreEqual("Hello, Browser!", greeting.InnerHTML);
		}

		[TestCase("but1", "hi1")]
		[TestCase("but2", "hi2")]
		[TestCase("but3", "hi3")]
		public void ClickTest(string buttonId, string divId)
		{
			var engine = Open("click");
			Thread.Sleep(1000);

			engine.FirstElement("#" + buttonId).Click();
			Assert.AreEqual("HI", engine.FirstElement("#" + divId).InnerHTML);
		}

		[Test]
		public void FormToFrameTest()
		{
			var engine = Open("form/frametarget");

			var submitButton = engine.WaitId("submitButton") as HtmlButtonElement;
			Assert.IsNotNull(submitButton, "submitButton");

			submitButton.Click();
			Thread.Sleep(1000);


			var frame = engine.Document.GetElementsByName("t").FirstOrDefault() as HtmlIFrameElement;
			Assert.IsNotNull(frame, "iframe");

			
			Assert.IsNotNull(frame);
			Assert.IsNotNull(frame.ContentDocument);
			var hi = frame.ContentDocument.WaitId("hi");
			Assert.IsNotNull(hi, "hi");
			Assert.AreEqual("HI", hi.InnerHTML);
		}

		[Test]
		public void FormSubmitTest()
		{
			var engine = Open("form");

			var submitButton = engine.WaitId("submitButton") as HtmlButtonElement;
			Assert.IsNotNull(submitButton, "submitButton");

			submitButton.Click();

			var hi = engine.WaitId("hi");
			Assert.IsNotNull(hi);
			Assert.AreEqual("HI", hi.InnerHTML);
		}

		[Test]
		public void JQueryOn()
		{
			var engine = Open("jquery") ;
			Assert.IsNotNull(engine.WaitId("ready"), "ready");
			var b = engine.WaitId("b") as HtmlButtonElement;
			Assert.IsNotNull(b);
			b.Click();
			Thread.Sleep(100);
			var g = engine.Document.GetElementById("greeting");
			Assert.IsNotNull(g);
			Assert.AreEqual("HI", g.InnerHTML);
		}

		[Test]
		public void JQueryBind()
		{
			var engine = Open("jquery");
			Assert.IsNotNull(engine.WaitId("ready"), "ready");
			var b = engine.WaitId("b2") as HtmlButtonElement;
			Assert.IsNotNull(b);
			b.Click();
			Thread.Sleep(100);
			var g = engine.Document.GetElementById("greeting2");
			Assert.IsNotNull(g);
			Assert.AreEqual("HI", g.InnerHTML);
		}

		[Test]
		public void JQueryForm()
		{
			var engine = Open("jquery/jqueryForm");
			Assert.IsNotNull(engine.WaitId("ready"), "ready");
			var button = engine.WaitId("b") as HtmlButtonElement;
			Assert.IsNotNull(button);
			button.Click();

			Thread.Sleep(1000);
			var g = engine.FirstElement("#g");
			Assert.IsNotNull(g);
			Assert.AreEqual("HI", g.InnerHTML);
		}

		

		[Test, Ignore]
		public void Ajax()
		{
			var engine = Open("ajax");
			var button = engine.WaitId("t");
			Assert.IsNotNull(button);

			Thread.Sleep(1000);
			Assert.AreEqual("HI", button.InnerHTML);
		}

		
	}
}
#endif