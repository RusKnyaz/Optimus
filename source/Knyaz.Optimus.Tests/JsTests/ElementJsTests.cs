using NUnit.Framework;

namespace Knyaz.Optimus.Tests.JsTests
{
	/// <summary>
	/// This stuff runs JS tests from Resources/JsTests/ElementTetst.js file.
	/// </summary>
	[TestFixture]
	public class ElementJsTests
    {
		[TestCase("SetParent")]
		[TestCase("SetOwnerDocument")]
		[TestCase("Remove")]
		[TestCase("AttributesLength")]
		[TestCase("AttributesGetByName")]
		[TestCase("SetOnClickAttribute")]
		[TestCase("SetOnClickAttributePropogation")]
		public void Run(string testName)
		{
			JsTestsRunner.Run("ElementTests", testName);
		}
	}
}
