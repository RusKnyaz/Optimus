using NUnit.Framework;

namespace Knyaz.Optimus.Tests.JsTests
{
	/// <summary>
	/// This stuff runs JS tests from Resources/JsTests/ElementTests.js file.
	/// </summary>
	[TestFixture(JsEngines.Jint)]
	[TestFixture(JsEngines.Jurassic)]
	public class ElementJsTests
	{
		private readonly JsEngines _jsEngine;

		public ElementJsTests(JsEngines jsEngine)
		{
			_jsEngine = jsEngine;
		}

		[TestCase("SetParent")]
		[TestCase("SetOwnerDocument")]
		[TestCase("Remove")]
		[TestCase("AttributesLength")]
		[TestCase("AttributesGetByName")]
		[TestCase("SetOnClickAttribute")]
		[TestCase("SetOnClickAttributePropogation")]
		[TestCase("EventHandlingOrder")]
		[TestCase("EventHandlingOrderCapturingString")]
		[TestCase("EventHandlingOrderCapturingUndefined")]
		[TestCase("EventListenerParams")]
		[TestCase("EventHandlerParams")]
		[TestCase("AttrEventHandlerParams")]
		[TestCase("AddRemoveEventListener")]
		[TestCase("AddTwoEventListeners")]
		[TestCase("RemoveEventListenerInsideHandler")]
		[TestCase("RemoveOtherEventListenerInsideHandler")]
		[TestCase("SetTextContent")]
		[TestCase("SetTextContentRemovesChildren")]
		[TestCase("GetTextContent")]
		[TestCase("SetTextContentEmpty")]
		[TestCase("TableDoesNotAcceptDivs")]
		[TestCase("TableCreatedBodies")]
		[TestCase("TableWrongTBody")]
		[TestCase("SetInnerHtml")]
		[TestCase("SetInnerHtmlText")]
		[TestCase("AppendAttributeThrows")]
		[TestCase("TextElementDispatchesEvent")]
		[TestCase("Prototype")]
		[TestCase("HiddenExist")]
		[TestCase("DataSetExists")]
		[TestCase("DataSetFromAttribute")]
		[TestCase("DataSetToAttribute")]
		[TestCase("DataSetToExistingAttribute")]
		[TestCase("DataSetFromAbsentAttribute")]
		[TestCase("DataSetByIndexer")]
		[TestCase("DataSetCapitalizeFirstLetter")]
		[TestCase("DataSetWrongAttributeName")]
		[TestCase("Node")]
		[TestCase("SetChildNode")]
		[TestCase("ChildNodesIsNodeList")]
		[TestCase("ChildNodesIsLive")]
		public void ElementTests(string testName)
		{
			if (_jsEngine == JsEngines.Jurassic && (testName == "DataSetToAttribute" || testName == "DataSetToExistingAttribute"))
				Assert.Ignore("Ignored due to bug in jurassic");
		
			JsTestsRunner.Run(_jsEngine, testName);
		} 

		[TestCase("InputChangeCheckedOnClick")]
		[TestCase("InputCheckChangingPrevented")]
		[TestCase("PreventDefaultDoesNotStopPropogation")]
		[TestCase("PreventInCaptureEventListener")]
		[TestCase("PreventInBubbleEventListener")]
		[TestCase("DefaultActionOrder")]
		[TestCase("InputCheckChangingPreventedByReturnValue")]
		[TestCase("ClickOnLabel")]
		[TestCase("UncheckInHandler")]
		[TestCase("UncheckInHandlerAndPreventDefault")]
		[TestCase("CheckInHandlerAndPreventDefault")]
		[TestCase("LabelClickEventsOrder")]
		[TestCase("LabelClickEventsOrderInDocument")]
		[TestCase("LabelClickEventsOrderInDocumentPreventDefault")]
		public void HtmlInputElementTests(string testName) => JsTestsRunner.Run(_jsEngine, testName);
	    
		[TestCase("Control")]
		[TestCase("ControlInDocument")]
		public void HtmlLabelElementTests(string testName) => JsTestsRunner.Run(_jsEngine, testName);

		[TestCase("Form")]
		[TestCase("NestedOptgroups")]
		[TestCase("SetLabelSetsAttribute")]
		[TestCase("GetLabelGetsAttributeIfExists")]
		[TestCase("GetLabelGetsTextIfThereIsNoAttribute")]
		[TestCase("GetLabelWhenAttributeIsEmpty")]
		[TestCase("DivInOptionSkipped")]
		public void HtmlOptionElementTests(string testName) => JsTestsRunner.Run(_jsEngine, testName);

	   
		[TestCase("ImageType")]
		[TestCase("NoArgumentsCtor")]
		[TestCase("OneArgumentCtor")]
		[TestCase("TwoArgumentsCtor")]
		[TestCase("SetWidthAndHeight")]
		[TestCase("TypeOfNewImage")]
		[TestCase("PrototypeOfNewImage")]
		[TestCase("NewImageInstanceOfHTMLImageElement")]
		public void HtmlImageElementTests(string testName) => JsTestsRunner.Run(_jsEngine, testName);
		
		[TestCase("Node")]
		[TestCase("Element")]
		[TestCase("HTMLAnchorElement")]
		[TestCase("HTMLBodyElement")]
		[TestCase("HTMLButtonElement")]
		[TestCase("HTMLBRElement")]
		[TestCase("HTMLDivElement")]
		[TestCase("HTMLDocument")]
		[TestCase("HTMLElement")]
		[TestCase("HTMLIFrameElement")]
		[TestCase("HTMLImageElement")]
		[TestCase("HTMLInputElement")]
		[TestCase("HTMLLabelElement")]
		[TestCase("HTMLLinkElement")]
		[TestCase("HTMLOptGroupElement")]
		[TestCase("HTMLOptionElement")]
		[TestCase("HTMLTextAreaElement")]
		[TestCase("HTMLSelectElement")]
		[TestCase("HTMLUnknownElement")]
		[TestCase("HTMLFormElement")]
		[TestCase("HTMLHtmlElement")]
		[TestCase("HTMLScriptElement")]
		[TestCase("HTMLStyleElement")]
		[TestCase("HTMLTableElement")]
		[TestCase("HTMLTableRowElement")]
		[TestCase("HTMLTableColElement")]
		[TestCase("HTMLTableCaptionElement")]
		[TestCase("HTMLTableSectionElement")]
		[TestCase("Comment")]
		[TestCase("Text")]
		[TestCase("Attr")]
		public void ElementTypesTests(string testName) => JsTestsRunner.Run(_jsEngine, testName);

		[TestCase("DefaultValues")]
		[TestCase("SetText")]
		[TestCase("GetText")]
		[TestCase("RelListContains")]
		[TestCase("ChangeRelGetRelList")]
		[TestCase("AddToRelList")]
		[TestCase("ClickOnClick")]
		public void HtmlAnchorElementTests(string testName) => JsTestsRunner.Run(_jsEngine, testName);
		
		[TestCase("Prototype")]
		[TestCase("ChildNodes")]
		public void HtmlBodyElementTests(string testName) => JsTestsRunner.Run(_jsEngine, testName);
	}
}
