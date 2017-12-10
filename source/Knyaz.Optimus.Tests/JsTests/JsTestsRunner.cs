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

			var engine = new Engine();
			engine.Load("<html></html>");
			engine.Document.Head.InnerHTML = "<script>" + R.JsTestsBase + "</script>" + "<script>" + testJs + "</script>";

			engine.Console.OnLog += o => {
				Debug.WriteLine(o.ToString());
			};
			var res = engine.ScriptExecutor.Evaluate("text/javascript", @"Run('"+fixture+"', '" + testName + "');");

			if (res != null)
				Assert.Fail(res.ToString());
		}
    }
}
