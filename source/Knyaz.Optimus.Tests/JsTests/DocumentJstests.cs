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
		public void DocumentTests(string testName) => JsTestsRunner.Run("DocumentTests", testName);

		[TestCase("CommentRemove")]
		[TestCase("TextRemove")]
		public void NodeTests(string testName) => JsTestsRunner.Run("NodeTests", testName);

		[TestCase("Clone")]
		public void DocumentFragmentTests(string testName) => JsTestsRunner.Run("DocumentFragmentTests", testName);

		[TestCase("AppendScriptTwice")]
		[TestCase("AppendScriptClone")]
		[TestCase("AppendClonesOfScript")]
		[TestCase("OnloadOnEmbeddedScript")]
		[TestCase("OnloadOnExternalScript")]
		[TestCase("CloneScript")]
		[TestCase("DeepCloneScript")]
		public void ScriptTests(string testName) => JsTestsRunner.Run("ScriptTests", testName);
	}
}
