namespace WebBrowser.Dom.Elements
{
	/// <summary>
	/// http://www.w3.org/TR/html-markup/input.text.html
	/// </summary>
	public class HtmlInputElement : HtmlElement
	{
		public HtmlInputElement():base("input"){}

		public string Value { get; set; }
		public bool Disabled { get; set; }
		public string Type { get; set; }
		public bool Readonly { get; set; }
		public bool Required { get; set; }
	}
}
