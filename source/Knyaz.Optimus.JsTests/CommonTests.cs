using NUnit.Framework;

namespace Knyaz.Optimus.Tests.JsTests
{
	public enum JsEngines { Jint, Jurassic}
	
	/// <summary>
	/// This stuff runs JS tests from Resources/JsTests/*.js files.
	/// </summary>
	[TestFixture(JsEngines.Jint)]
	[TestFixture(JsEngines.Jurassic)]
	public class CommonTests
	{
		private readonly JsEngines _jsEngine;

		public CommonTests(JsEngines jsEngine) => _jsEngine = jsEngine;
	    
		[TestCase("DocumentElement")]
	    [TestCase("ImplementationCreateHtmlDocument")]
		[TestCase("ImplementationCreateDocumentType")]
		[TestCase("ImplementationCreateDocumentWithDocTypeSvg")]
		[TestCase("ImplementationCreateDocumentWithoutDocType")]
		[TestCase("RemoveDocType")]
		[TestCase("TextContentIsNull")]
		[TestCase("DefaultViewIsWindow")]
		[TestCase("DomBuildOrder")]
		[TestCase("SetBody")]
		[TestCase("SetBodyDiv")]
		[TestCase("SetBodyNull")]
	    [TestCase("GetElementsByTagName")]
		[TestCase("GetElementsByTagNameReturnsHTMLCollection")]
		[TestCase("GetElementsByTagNameByIndex")]
		[TestCase("GetElementsByTagNameIsLive")]
		[TestCase("GetElementsByTagNameNamedItem")]
		[TestCase("GetElementsByTagNameNamedItemIndexer")]

		[TestCase("GetElementsByClassName")]
		[TestCase("GetElementsByClassNameAndSlice")]
		[TestCase("GetElementsByClassNameReturnsHTMLCollection")]
		[TestCase("GetElementsByClassNameIsLive")]
		
		[TestCase("GetElementsByNameReturnsLiveCollection")]
		[TestCase("GetElementsByNameReturnsNodeList")]
	    [TestCase("InstanceOf")]
	    [TestCase("Prototype")]
	    [TestCase("Properties")]
		[TestCase("QuerySelectorAllReturnsNodeList")]
		[TestCase("QuerySelectorAllReturnsStaticCollection")]
		public void DocumentTests(string testName) => JsTestsRunner.Run(_jsEngine, testName);

		[TestCase("CommentRemove")]
		[TestCase("TextRemove")]
		public void NodeTests(string testName) => JsTestsRunner.Run(_jsEngine, testName);

		[TestCase("Clone")]
		public void DocumentFragmentTests(string testName) => JsTestsRunner.Run(_jsEngine, testName);

		[TestCase("AppendScriptTwice")]
		[TestCase("AppendScriptClone")]
		[TestCase("AppendClonesOfScript")]
		[TestCase("OnloadOnEmbeddedScript")]
		[TestCase("OnloadOnExternalScript")]
		[TestCase("CloneScript")]
		[TestCase("DeepCloneScript")]
		[TestCase("TextContent")]
		[TestCase("ParseFromHtml")]
		[TestCase("SetInnerHtml")]
		[TestCase("TextOfExternal")]
		public void ScriptTests(string testName) => JsTestsRunner.Run(_jsEngine, testName);

		[TestCase("Clone")]
		[TestCase("TextContentIsNull")]
		public void DocTypeTests(string testName) => JsTestsRunner.Run(_jsEngine, testName);

		[TestCase("CallThisIsObj")]
		[TestCase("CallThisIsNull")]
		[TestCase("CallThisIsUndefined")]
		[TestCase("CallThisIsNullStrict")]
		[TestCase("CallThisIsUndefinedStrict")]
		[TestCase("CompareObjectWithWindow")]
		[TestCase("CompareObjectWithWindowInverted")]
		[TestCase("CallAndCompareThisIsUndefined")]
		[TestCase("Misc")]
		[TestCase("Splice")]
		[TestCase("PushApply")]
		[TestCase("ArrayPush")]
		[TestCase("SliceCall")]
		[TestCase("ChildNodesSlice")]
		[TestCase("ResizeArray")]
		[TestCase("ShiftArray")]
		public void JavaScriptTests(string testName) => JsTestsRunner.Run(_jsEngine, testName);
		
		[TestCase("Clone")]
		[TestCase("TextContent")]
		[TestCase("AppendData")]
		[TestCase("AppendDataNull")]
		[TestCase("DeleteData")]
		[TestCase("InsertData")]
		[TestCase("ReplaceData")]
		[TestCase("SubstringData")]
		[TestCase("LengthTest")]
		[TestCase("AppendChildThrows")]
		public void CommentTests(string testName) => JsTestsRunner.Run(_jsEngine, testName);

		[TestCase("ChildElements")]
		[TestCase("DefaultEnctype")]
		public void FormTests(string testName) => JsTestsRunner.Run(_jsEngine, testName);
		
		[TestCase("EventType")]
		[TestCase("EventConstructor")]
		[TestCase("EventConstructorWithInit")]
		[TestCase("AddEventListenerCallOnce")]
		[TestCase("AddEventListenerPassiveTrue")]
		[TestCase("AddEventListenerPassiveFalse")]
		[TestCase("AddEventListenerCaptureOptionTrue")]
		[TestCase("AddEventListenerCaptureOptionFalse")]
		[TestCase("AddEventListenerTwice")]
		[TestCase("AddEventListenerTwiceAndRemove")]
		public void EventTests(string testName) => JsTestsRunner.Run(_jsEngine, testName);

		[TestCase("SetItem")]
		[TestCase("SetItemTwice")]
		[TestCase("RemoveItem")]
		[TestCase("Clear")]
		[TestCase("GetAbsentItem")]
		[TestCase("GetKey")]
		[TestCase("SetByIndexer")]
		[TestCase("LocalStorage")]
		[TestCase("SessionStorage")]
		[TestCase("StoreObjectItem")]
		[TestCase("StoreNumberItem")]
		[TestCase("StoreBoolItem")]
		[TestCase("StoreBoolKey")]
		public void StorageTests(string testName)
		{
			if (_jsEngine == JsEngines.Jurassic && (testName == "SetByIndexer" ||
			                                        testName == "StoreBoolItem" ||
			                                        testName == "StoreNumberItem"))
			{
				Assert.Ignore("Jurassic limitation");
			}
				
		
			JsTestsRunner.Run(_jsEngine, testName);
		} 
	    
		[TestCase("Construct")]
		[TestCase("ConstructWithDefaultLength")]
		[TestCase("ConstructWithDefaultOffset")]
		[TestCase("SetInt16DefaultEndian")]
		[TestCase("SetInt16")]
		
		[TestCase("SetInt8ByIndexer")]
		[TestCase("SetInt16ByIndexer")]
		[TestCase("SetInt32ByIndexer")]
		[TestCase("SetUInt8ByIndexer")]
		[TestCase("SetUInt16ByIndexer")]
		[TestCase("SetUInt32ByIndexer")]
		[TestCase("SetFloat32ByIndexer")]
		[TestCase("SetFloat64ByIndexer")]
		
		[TestCase("ArrayBufferType")]
		[TestCase("Int8ArrayType")]
		[TestCase("Uint8ArrayType")]
		[TestCase("Int16ArrayType")]
		[TestCase("Uint16ArrayType")]
		[TestCase("Int32ArrayType")]
		[TestCase("Uint32ArrayType")]
		[TestCase("Float32ArrayType")]
		[TestCase("Float64ArrayType")]
		[TestCase("DataViewType")]
		[TestCase("NewArrayBuffer")]
		
		
		[TestCase("Int16FromSize")]
		[TestCase("UInt16FromSize")]
		[TestCase("Int8FromSize")]
		[TestCase("UInt8FromSize")]
		[TestCase("Int32FromSize")]
		[TestCase("UInt32FromSize")]
		[TestCase("Float32FromSize")]
		[TestCase("Float64FromSize")]
		
		[TestCase("Int16FromArrayBuffer")]
		[TestCase("Int16FromArray")]
		[TestCase("Int16FromArrayWithFloats")]
		[TestCase("Uint16InstantiatedFromArray")]
		[TestCase("Uint16InstantiatedFromSignedArray")]
		[TestCase("BytesPerElement")]
		
		[TestCase("Uint8ArrayFromNull")]
		[TestCase("Uint16ArrayFromNull")]
		[TestCase("Int8ArrayFromNull")]
		[TestCase("Int16ArrayFromNull")]
		public void DataViewTests(string testName) => JsTestsRunner.Run(_jsEngine, testName);
		
		[TestCase("XMLHttpRequestType")]
		[TestCase("XMLHttpRequestPrototype")]
		[TestCase("XmlHttpRequestStaticFields")]
		[TestCase("NewXmlHttpRequest")]
		public void XmlHttpRequestTests(string testName) => JsTestsRunner.Run(_jsEngine, testName);

		[TestCase("FunctionsDefined")]
		public void WindowTests(string testName) => JsTestsRunner.Run(_jsEngine, testName);
		
		[TestCase("StyleOfCustom")]
		[TestCase("StyleRead")]
		[TestCase("StyleWrite")]
		public void StyleTests(string testName) => JsTestsRunner.Run(_jsEngine, testName);
	}
}
