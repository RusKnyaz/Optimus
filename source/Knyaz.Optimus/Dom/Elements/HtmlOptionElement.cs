namespace Knyaz.Optimus.Dom.Elements
{
	public class HtmlOptionElement : HtmlElement
	{
		public HtmlOptionElement(Document ownerDocument) : base(ownerDocument, TagsNames.Option)
		{
		}

		public string Name
		{
			get { return GetAttribute("name", string.Empty); }
			set { SetAttribute("name", value); }
		}
	}
}