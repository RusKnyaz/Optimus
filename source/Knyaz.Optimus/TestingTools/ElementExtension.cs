using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.Dom.Interfaces;

namespace Knyaz.Optimus.TestingTools
{
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

		/// <summary>
		/// Gets the all element styles. Shortened form for to call Element.OwnerDocument.DefaultView.GetComputedStyle(elt).
		/// </summary>
		/// <param name="elt">The element to get style for.</param>
		public static ICssStyleDeclaration GetComputedStyle(this Element elt) =>
			elt.OwnerDocument.DefaultView.GetComputedStyle(elt);
	}
}
