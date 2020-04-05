using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.TestingTools;
using Knyaz.Optimus.Tests.TestingTools;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests
{
	[TestFixture, Ignore("For manual run")]
	public class TestPages
	{
		private string GetTestUrl(string testUrl)
		{
			return "http://localhost:21449/" + (testUrl.Contains("/") ? (testUrl+".html") : (testUrl + "/index.html"));
		}

		private Engine Open(string testUrl)
		{
			var engine = TestingEngine.BuildJint().AttachConsole();
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

		[TestCase("b", "greeting", Description = "Test .on(evt, selector)")]
		[TestCase("b2", "greeting2", Description = "Test .bind")]
		[TestCase("b3", "greeting3", Description = "Test .on(evt, selector, handler)")]
		public void JQueryOn(string buttonId, string greetingId)
		{
			var engine = Open("jquery") ;
			Assert.IsNotNull(engine.WaitId("ready"), "ready");
			var b = engine.WaitId(buttonId) as HtmlButtonElement;
			Assert.IsNotNull(b, "button");
			b.Click();
			Thread.Sleep(100);
			var g = engine.Document.GetElementById(greetingId);

			Assert.IsNotNull(g, "greeting");
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

		[Test]
		public void Ajax()
		{
			var engine = Open("ajax");
			var button = engine.WaitId("t");

			var log = new List<object>();
			engine.Console.OnLog+= o =>{log.Add(o);};
			Assert.IsNotNull(button);

			Thread.Sleep(1000000);
			Assert.AreNotEqual(0, log.Count);
			Assert.AreEqual("X-SourceFiles: =?UTF-8?B?QzpccHJvamVjdHNcV2ViQnJvd3NlclxUZXN0UGFnZXNcYWpheFxkYXRhLmh0bWw=",
				button.InnerHTML);
		}

		[Test]
		public void KnockoutClick()
		{
			var engine = Open("knockout");
			engine.WaitDocumentLoad();
			var button = engine.Document.GetElementById("b") as HtmlButtonElement;

			button.Assert(b => b.InnerHTML == "Click me");
			button.Click();

			var text = engine.Document.GetElementById("p") as HtmlElement;
			Assert.IsNotNull(text, "text");
			Assert.AreEqual("HI", text.InnerHTML);
			Assert.IsTrue(button.Disabled);
		}
	}
}
