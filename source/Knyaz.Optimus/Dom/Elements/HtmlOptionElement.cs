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

		public string Value
		{
			get { return GetAttribute("value", string.Empty); }
			set { SetAttribute("value", value); }
		}

		//todo: fix it;
		public string Text
		{
			get { return InnerHTML;}
			set { InnerHTML = value; }
		}
	}
}