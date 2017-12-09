using Knyaz.Optimus.Tests.Resources;
using NUnit.Framework;
using System.Diagnostics;

namespace Knyaz.Optimus.Tests.JsTests
{
	/// <summary>
	/// This stuff runs JS tests from Resources/JsTests/ElementTetst.js file.
	/// </summary>
	[TestFixture]
	class ElementJsTests
    {
		[TestCase("SetParent")]
		[TestCase("SetOwnerDocument")]
		public void Run(string testName)
		{
			var engine = new Engine();
			engine.Load("<html></html>");
			engine.Document.Head.InnerHTML = "<script>" + R.JsTestsBase + "</script>" + "<script>" + R.JsTestsElementTests + "</script>";

			engine.Console.OnLog += o => {	Debug.WriteLine(o.ToString());	};
			var res = engine.ScriptExecutor.Evaluate("text/javascript", @"Run('ElementJsTests', '" + testName + "');");

			if (res != null)
				Assert.Fail(res.ToString());
		}
	}
}
