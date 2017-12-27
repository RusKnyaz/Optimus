using NUnit.Framework;

namespace Knyaz.Optimus.Tests.JsTests
{
	[TestFixture]
	class DocumentJsTests
	{
		[TestCase("ImplementationCreateHtmlDocument")]
		[TestCase("ImplementationCreateDocumentType")]
		[TestCase("ImplementationCreateDocumentWithDocType", Ignore = "To be implemented (#1369)")]
		[TestCase("RemoveDocType")]
		[TestCase("TextContentIsNull")]
		[TestCase("DefaultViewIsWindow")]
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

		[TestCase("Clone")]
		[TestCase("TextContentIsNull")]
		public void DocTypeTests(string testName) => JsTestsRunner.Run("DocTypeTests", testName);

		[TestCase("CallThisIsObj")]
		[TestCase("CallThisIsNull")]
		[TestCase("CallThisIsUndefined")]
		[TestCase("CallThisIsNullStrict")]
		[TestCase("CallThisIsUndefinedStrict")]
		[TestCase("CompareObjectWithWindow")]
		[TestCase("CompareObjectWithWindowInverted")]
		[TestCase("CallAndCompareThisIsUndefined")]
		public void JavaScriptTests(string testName) => JsTestsRunner.Run("JavaScriptTests", testName);
		
		[TestCase("Clone")]
		[TestCase("TextContent")]
		public void CommentTests(string testName)=> JsTestsRunner.Run("CommentTests", testName);
	}
}
