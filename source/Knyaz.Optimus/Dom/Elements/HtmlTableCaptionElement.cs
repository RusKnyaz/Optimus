using System;

namespace Knyaz.Optimus.Dom.Elements
{
	public class HtmlTableCaptionElement : HtmlElement
	{
		public HtmlTableCaptionElement(Document ownerDocument) : base(ownerDocument, TagsNames.Caption)
		{
		}

		public string Align
		{
			get { throw new NotImplementedException();}
			set { throw new NotImplementedException(); }
		}
	}
}