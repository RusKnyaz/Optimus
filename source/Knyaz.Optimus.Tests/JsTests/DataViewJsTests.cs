using System.Diagnostics;
using Knyaz.Optimus.Tests.Resources;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.JsTests
{
	/// <summary>
	/// This stuff runs JS tests from Resources/JsTests/DataViewTetst.js file.
	/// </summary>
	[TestFixture]
    class DataViewJsTests
    {
		[TestCase("Construct")]
		[TestCase("ConstructWithDefaultLength")]
		[TestCase("ConstructWithDefaultOffset")]
		[TestCase("SetInt32DefaultEndian")]
		[TestCase("SetInt32")]
		public void Run(string testName)
		{
			var engine = new Engine();
			engine.Load("<html></html>");
			engine.Document.Head.InnerHTML = "<script>" + R.JsTestsBase + "</script>" + "<script>" + R.JsTestsDataViewTests + "</script>";

			engine.Console.OnLog += o => {
				Debug.WriteLine(o.ToString());
			};
			var res = engine.ScriptExecutor.Evaluate("text/javascript", @"Run('DataViewJsTests', '" + testName+"');");

			if (res != null)
				Assert.Fail(res.ToString());
		}
    }
}
