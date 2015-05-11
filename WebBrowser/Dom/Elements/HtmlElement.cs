namespace WebBrowser.Dom.Elements
{
	/// <summary>
	/// http://www.w3.org/TR/2012/WD-html5-20121025/elements.html#htmlelement
	/// </summary>
	public class HtmlElement : Element
	{
		public HtmlElement(string tagName):base(tagName)
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
	}
}
