using System;
using System.Linq;

namespace WebBrowser.Dom.Elements
{
	/// <summary>
	/// http://www.w3.org/TR/2012/WD-html5-20121025/elements.html#htmlelement
	/// </summary>
	public class HtmlElement : Element
	{
		public HtmlElement(Document ownerDocument, string tagName)
			: base(ownerDocument, tagName)
		{
			Style = new CssStyleDeclaration();
		}

		public bool Hidden { get; set; }
		
		public void Click()
		{
			var evt = new Event { Type = "click", Target = this };
			DispatchEvent(evt);
		}

		public CssStyleDeclaration Style { get; private set; }

		protected override void UpdatePropertyFromAttribute(string value, string invariantName)
		{
			base.UpdatePropertyFromAttribute(value, invariantName);

			if (invariantName == "style")
			{
				if (!string.IsNullOrEmpty(value))
				{
					var styleParts = value.Split(';');
					foreach (var stylePart in styleParts.Where(s => !string.IsNullOrEmpty(s)))
					{
						var keyValue = stylePart.Split(':');
						if (keyValue.Length != 2)
							throw new Exception("Invalid style definition: " + stylePart);
						//todo: handle duplicates
						Style.Properties.Add(keyValue[0], keyValue[1]);
					}
				}
			} 
			else if (invariantName == "hidden")
			{
				Hidden = value == "true";
			}
		}
	}
}
