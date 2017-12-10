using NUnit.Framework;

namespace Knyaz.Optimus.Tests.JsTests
{
	[TestFixture]
	class DocumentJsTests
	{
		[TestCase("ImplementationCreateHtmlDocument")]
		[TestCase("ImplementationCreateDocumentType")]
		[TestCase("ImplementationCreateDocumentWithDocType")]
		public void Run(string testName)
		{
			JsTestsRunner.Run("DocumentTests", testName);
		}
	}
}
