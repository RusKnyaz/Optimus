using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBrowser.Dom.Elements
{
	/// <summary>
	/// http://www.w3.org/TR/2012/WD-html5-20121025/elements.html#htmlelement
	/// </summary>
	public class HtmlElement : Element
	{
		public HtmlElement(string tagName):base(tagName)
		{
			
		}

		public bool Hidden { get; set; }
		
		public void Click()
		{
			var evt = new Event { Type = "click", Target = this };
			DispatchEvent(evt);
		}
	}
}
