using Knyaz.Optimus.Tests.Resources;
using NUnit.Framework;
using System.Diagnostics;

namespace Knyaz.Optimus.Tests.JsTests
{
    class JsTestsRunner
    {
		public static void Run(string fixture, string testName)
		{
			var testJs = R.GetString("Knyaz.Optimus.Tests.Resources.JsTests."+fixture+".js");

			var engine = new Engine(
				Mocks.ResourceProvider("http://test", "<html><script>" + R.JsTestsBase + "</script>" + "<script>" + testJs + "</script></html>")
				.Resource("sample.js", R.GetString("Knyaz.Optimus.Tests.Resources.JsTests.sample.js")));

			engine.OpenUrl("http://test").Wait();

			engine.Console.OnLog += o => {
				Debug.WriteLine(o.ToString());
			};
			var res = engine.ScriptExecutor.Evaluate("text/javascript", @"Run('"+fixture+"', '" + testName + "');");

			if (res != null)
				Assert.Fail(res.ToString());
		}
    }
}
