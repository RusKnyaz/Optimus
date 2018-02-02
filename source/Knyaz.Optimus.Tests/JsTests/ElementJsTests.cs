﻿using NUnit.Framework;

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
		[TestCase("EventHandlingOrder")]
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
		[TestCase("SetInnerHtmlText")]
		[TestCase("AppendAttributeThrows")]
		public void ElementTests(string testName) => JsTestsRunner.Run(testName);

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
	    public void HtmlInputElementTests(string testName) => JsTestsRunner.Run(testName);
	    
	    [TestCase("Control")]
	    [TestCase("ControlInDocument")]
	    public void HtmlLabelElementTests(string testName) => JsTestsRunner.Run(testName);
    }
}
