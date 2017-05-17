using Knyaz.Optimus.Dom.Css;
using Knyaz.Optimus.Dom.Elements;

namespace Knyaz.Optimus.TestingTools
{
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

		public static ICssStyleDeclaration GetComputedStyle(this IElement elt)
		{
			return elt.OwnerDocument.DefaultView.GetComputedStyle(elt);
		}
	}
}
