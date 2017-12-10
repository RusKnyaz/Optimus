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
			JsTestsRunner.Run("ElementTests", testName);
		}
	}
}
