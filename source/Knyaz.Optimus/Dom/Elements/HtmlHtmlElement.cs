using Knyaz.Optimus.ScriptExecuting;

namespace Knyaz.Optimus.Dom.Elements
{
	/// <summary>
	/// Represents &lt;HTML&gt; element.
	/// </summary>
	[JsName("HTMLHtmlElement")]
	public sealed class HtmlHtmlElement : HtmlElement
	{
		internal HtmlHtmlElement(Document ownerDocument) : base(ownerDocument, TagsNames.Html){}

		/// <summary>
		/// Gets or sets inner html of the 'HTML' element.
		/// </summary>
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
