using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.Dom.Events;
using Knyaz.Optimus.Dom.Interfaces;

namespace Knyaz.Optimus.TestingTools
{
	public enum MouseButtons
	{
		/// <summary> Main button, usually the left button or the un-initialized state </summary>
		Main = 0,
		/// <summary> Auxiliary button, usually the wheel button or the middle button (if present) </summary>
		Auxiliary = 1,
		/// <summary> Secondary button, usually the right button </summary>
		Secondary = 2,
		/// <summary> Fourth button, typically the Browser Back button </summary>
		Fourth = 3,
		/// <summary> Fifth button, typically the Browser Forward button </summary>
		Fifth = 4 

	}
	
	/// <summary>
	/// Contains helper methods for work with <see cref="Engine"/>.
	/// </summary>
	public static class ElementExtension
	{
		/// <summary>
		/// Emulate entering text by user into input textbox.
		/// </summary>
		public static void EnterText(this HtmlInputElement input, string text)
		{
			input.Value = text;
			var evt = input.OwnerDocument.CreateEvent("Event");
			evt.InitEvent("change", false, false);
			input.DispatchEvent(evt);
		}

		/// <summary>
		/// Emulate entering text by user into input textbox.
		/// </summary>
		public static void EnterText(this HtmlTextAreaElement input, string text)
		{
			input.Value = text;
			var evt = input.OwnerDocument.CreateEvent("Event");
			evt.InitEvent("change", false, false);
			input.DispatchEvent(evt);
		}

		public static void DoMouseClick(this HtmlElement elt, MouseButtons button = MouseButtons.Main)
		{
			var evt = new MouseEvent(elt.OwnerDocument, "click", new MouseEventInitOptions {
				Button = (short)button
			});
			evt.InitEvent("click", true, true);
			elt.DispatchEvent(evt);
		}

		/// <summary>
		/// Gets the all element styles. Shortened form for to call Element.OwnerDocument.DefaultView.GetComputedStyle(elt).
		/// </summary>
		/// <param name="elt">The element to get style for.</param>
		public static ICssStyleDeclaration GetComputedStyle(this Element elt) =>
			elt.OwnerDocument.DefaultView.GetComputedStyle(elt);
	}
}
