namespace Knyaz.Optimus.Dom.Elements
{
	/// <summary>
	/// Represents &lt;HTML&gt; element.
	/// </summary>
	public sealed class HtmlHtmlElement : HtmlElement
	{
		internal HtmlHtmlElement(Document ownerDocument) : base(ownerDocument, TagsNames.Html){}

		public override string InnerHTML
		{
			get => base.InnerHTML;
			set
			{
				AppendChild(OwnerDocument.CreateElement("HEAD"));
				AppendChild(OwnerDocument.CreateElement("BODY"));
				DocumentBuilder.Build(this, value, NodeSources.Script);
			}
		}
	}
}
