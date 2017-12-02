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
			if (!string.IsNullOrEmpty(formId))
			{
				var form = element.OwnerDocument.GetElementById(formId) as HtmlFormElement;
				if (form != null)
					return form;
			}
			return element.Ancestors().OfType<HtmlFormElement>().FirstOrDefault();
		}
	}
}
