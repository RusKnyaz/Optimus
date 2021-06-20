using Knyaz.Optimus.Dom.Events;
using Knyaz.Optimus.ScriptExecuting;

namespace Knyaz.Optimus.Dom.Elements
{
	/// <summary>
	/// Reflects 'label' html element
	/// </summary>
	[JsName("HTMLLabelElement")]
	public class HtmlLabelElement : HtmlElement
	{
		internal HtmlLabelElement(HtmlDocument ownerDocument) : base(ownerDocument, TagsNames.Label){}

		protected override void AfterEventDispatch(Event evt)
		{
			base.AfterEventDispatch(evt);

			if (evt.Type == "click" && !evt.IsDefaultPrevented())
				Control?.Click();
		}

		/// <summary>
		/// Gets or sets 'for' attribute value that represents id of the labeled element.
		/// </summary>
		public string HtmlFor
		{
			get => GetAttribute("for", string.Empty);
			set => SetAttribute("for", value);
		}
		
		/// <summary>
		/// Is a HtmlFormElement object representing the form with which the labeled control is associated, or null if there is no associated control.
		/// </summary>
		public HtmlFormElement Form => this.FindOwnerForm();

		/// <summary>
		/// Is a HtmlElement representing the control with which the label is associated.
		/// </summary>
		public HtmlElement Control => (HtmlElement)OwnerDocument?.GetElementById(HtmlFor);
	}
}