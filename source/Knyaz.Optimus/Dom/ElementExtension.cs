using System.Linq;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.Tools;

namespace Knyaz.Optimus.Dom
{
	internal static class ElementExtension
	{
		public static HtmlFormElement FindOwnerForm(this Element element)
		{
			var formId = element.GetAttribute("form");
			return 
				!string.IsNullOrEmpty(formId) && element.OwnerDocument.GetElementById(formId) is HtmlFormElement form
				? form
				: element.Ancestors().OfType<HtmlFormElement>().FirstOrDefault();
		}
	}
}
