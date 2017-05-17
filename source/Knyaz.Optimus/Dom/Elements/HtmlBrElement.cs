namespace Knyaz.Optimus.Dom.Elements
{
	/// <summary>
	/// https://www.w3.org/2003/01/dom2-javadoc/org/w3c/dom/html2/HTMLBRElement.html
	/// </summary>
	public sealed class HtmlBrElement : HtmlElement
	{
		public HtmlBrElement(Document ownerDocument) : base(ownerDocument, TagsNames.Br)
		{
		}

		public string Clear
		{
			get { return GetAttribute("clear", string.Empty); }
			set { SetAttribute("clear", value);}
		}
	}
}
