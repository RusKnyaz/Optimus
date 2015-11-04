#if NUNIT
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
			return "http://localhost:21449/"+testUrl+"/index.html";
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
		public void FormSubmitTest()
		{
			var engine = Open("form");

			var submitButton = engine.WaitId("submitButton") as HtmlButtonElement;
			Assert.IsNotNull(submitButton, "submitButton");

			submitButton.Click();

			var hi = engine.WaitId("hi");
			Assert.IsNull(hi);
		}
	}

	internal static class EngineExtension
	{
		/// <summary>
		/// Attach System.Console to log engine events;
		/// </summary>
		/// <param name="engine"></param>
		public static Engine AttachConsole(this Engine engine)
		{
			engine.DocumentChanged += () =>
			{
				engine.Scripting.BeforeScriptExecute += script => System.Console.WriteLine(
					"Executing:" + (script.Src ?? script.Id ?? "<script>"));

				engine.Scripting.AfterScriptExecute += script => System.Console.WriteLine(
					"Executed:" + (script.Src ?? script.Id ?? "<script>"));

				engine.Scripting.ScriptExecutionError += (script, exception) => System.Console.WriteLine(
					"Error script execution:" + (script.Src ?? script.Id ?? "<script>") + " " + exception.Message);
			};

			engine.Console.OnLog += o => System.Console.WriteLine(o ?? "<null>");
			return engine;
		}
	}
}
#endif