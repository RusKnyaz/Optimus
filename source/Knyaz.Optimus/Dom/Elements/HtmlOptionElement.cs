using Knyaz.Optimus.ScriptExecuting;
using Knyaz.Optimus.Tools;

namespace Knyaz.Optimus.Dom.Elements
{
	/// <summary>
	/// Represents &lt;OPTION&gt; element.
	/// </summary>
	[JsName("HTMLOptionElement")]
	public sealed class  HtmlOptionElement : HtmlElement
	{
		internal HtmlOptionElement(Document ownerDocument) : base(ownerDocument, TagsNames.Option){}

		/// <summary>
		/// Gets or sets the 'name' attribute value.
		/// </summary>
		public string Name
		{
			get => GetAttribute("name", string.Empty);
			set => SetAttribute("name", value);
		}

		/// <summary>
		/// Reflects the value of the value HTML attribute, if it exists; otherwise reflects value of the Node.textContent property.
		/// </summary>
		public string Value
		{
			get => HasAttribute("value") ? GetAttribute("value", string.Empty) : TextContent;
			set => SetAttribute("value", value);
		}

		/// <summary>
		/// Contains the text content of the element.
		/// </summary>
		public string Text
		{
			get => TextContent;
			set => TextContent = value;
		}

		/// <summary>
		/// Indicates whether the option is currently selected.
		/// </summary>
		public bool Selected
		{
			get => HasAttribute("selected");
			set => SetFlagAttribute("selected", value);
		}
		
		/// <summary>
		/// Gets or sets 'disabled' attribute value indicating whether or not the control is disabled, meaning that it does not accept any clicks.
		/// </summary>
		public bool Disabled
		{
			get => GetAttribute("disabled") != null;
			set => SetAttribute("disabled", value ? "" : null);
		}

		public string Label
		{
			get => HasAttribute("label") ? GetAttribute("label") : Text;
			set => SetAttribute("label", value);
		}

		public int Index => ParentSelect?.Options.IndexOf(this) ?? 0;
		
		private HtmlSelectElement ParentSelect => 
			ParentNode as HtmlSelectElement ??
			(ParentNode as HtmlOptGroupElement)?.ParentNode as HtmlSelectElement;

		/// <summary>
		/// Is a <see cref="HtmlFormElement"/> reflecting the form that this option is associated with.
		/// </summary>
		public HtmlFormElement Form => ParentSelect?.Form;

		/// <summary>
		/// Contains the initial value of the selected HTML attribute, indicating whether the option is selected by default or not.
		/// </summary>
		public bool DefaultSelected { get; set; }
	}
}