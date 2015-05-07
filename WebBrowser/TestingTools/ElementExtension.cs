using WebBrowser.Dom.Elements;

namespace WebBrowser.TestingTools
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
	}
}
