using WebBrowser.ScriptExecuting;

namespace WebBrowser.Dom.Elements
{
	/// <summary>
	/// http://www.w3.org/TR/html-markup/input.text.html
	/// </summary>
	public class HtmlInputElement : HtmlElement, IHtmlInputElement
	{
		public HtmlInputElement(Document ownerDocument) : base(ownerDocument, "input")
		{
			Type = "text";
		}

		public string Value { get; set; }
		public bool Disabled { get; set; }
		public string Type { get; set; }
		public bool Readonly { get; set; }
		public bool Required { get; set; }
		public bool Checked { get; set; }

		protected override void UpdatePropertyFromAttribute(string value, string invariantName)
		{
			base.UpdatePropertyFromAttribute(value, invariantName);

			switch (invariantName)
			{
				case "type":
					Type = value;
					break;
				case "value":
					Value = value;
					break;
				case "checked":
					Checked = value == "true";
					break;
			}
		}
	}

	[DomItem]
	public interface IHtmlInputElement
	{
		string Value { get; set; }
		bool Disabled { get; set; }
	}
}
