#if NUNIT
using System.Threading;
using NUnit.Framework;

namespace WebBrowser.Tests
{
	[TestFixture]
	public class TestPages
	{
		private string GetTestUrl(string testUrl)
		{
			return "http://localhost:21449/"+testUrl+"/index.html";
		}

		[TestCase("jq_rq_knock")]
		[TestCase("jq_rq")]
		public void OpenPageTest(string subUrl)
		{
			var engine = new Engine().AttachConsole();
			engine.OpenUrl(GetTestUrl(subUrl));
			Thread.Sleep(2000);

			var greeting = engine.Document.GetElementById("greeting");
			Assert.IsNotNull(greeting);
			Assert.AreEqual("Hello, Browser!", greeting.InnerHTML);
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