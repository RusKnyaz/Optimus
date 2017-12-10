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
			JsTestsRunner.Run("DataViewTests", testName);
		}
    }
}
