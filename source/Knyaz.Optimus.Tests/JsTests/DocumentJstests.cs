using NUnit.Framework;

namespace Knyaz.Optimus.Tests.JsTests
{
	[TestFixture]
	class DocumentJsTests
	{
		[TestCase("ImplementationCreateHtmlDocument")]
		[TestCase("ImplementationCreateDocumentType")]
		[TestCase("ImplementationCreateDocumentWithDocType")]
		[TestCase("RemoveDocType")]
		public void Run(string testName) => JsTestsRunner.Run("DocumentTests", testName);

		[TestCase("CommentRemove")]
		[TestCase("TextRemove")]
		public void NodeTests(string testName) => JsTestsRunner.Run("NodeTests", testName);

		[TestCase("Clone")]
		public void DocumentFragmentTests(string testName) => JsTestsRunner.Run("DocumentFragmentTests", testName);
	}
}
