namespace Knyaz.Optimus.Dom.Elements
{
	public class HtmlHtmlElement : HtmlElement
	{
		public HtmlHtmlElement(Document ownerDocument) : base(ownerDocument, TagsNames.Html)
		{
		}

		public override string InnerHTML
		{
			get { return base.InnerHTML; }
			set
			{
				AppendChild(OwnerDocument.CreateElement("HEAD"));
				AppendChild(OwnerDocument.CreateElement("BODY"));
				DocumentBuilder.Build(this, value, NodeSources.Script);
			}
		}
	}
}
