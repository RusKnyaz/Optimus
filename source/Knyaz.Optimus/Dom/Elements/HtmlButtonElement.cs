using System.Collections.Generic;
using System.Linq;
using Knyaz.Optimus.Dom.Events;
using Knyaz.Optimus.ScriptExecuting;

namespace Knyaz.Optimus.Dom.Elements
{
	/// <summary>
	/// Represents a &lt;BUTTON&gt; HTML element.
	/// </summary>
	[JsName("HTMLButtonElement")]
	public sealed class HtmlButtonElement : HtmlElement, IFormElement
	{
		private static class Defaults
		{
			public const string Type = "submit";
			public const string Value = "";
			public const string Name = "";
			public const string AccessKey = "";
			public const long TabIndex = 0;
		}

		private static readonly string[] AvailableTypes = {"submit", "button", "reset"};

		internal HtmlButtonElement(Document ownerDocument) : base(ownerDocument, TagsNames.Button) { }

		protected override void CallDirectEventSubscribers(Event evt)
		{
			base.CallDirectEventSubscribers(evt);

			if (evt.Type == "click" && !evt.IsDefaultPrevented())
				Form?.RaiseSubmit(evt.Target as HtmlElement);
		}

		/// <summary>
		/// Is a string indicating the behavior of the button. This is an enumerated attribute with the following possible values:
		/// "submit": The button submits the form. This is the default value if the attribute is not specified, HTML5 or if it is dynamically changed to an empty or invalid value.
		/// "reset": The button resets the form.
		/// "button": The button does nothing.
		/// "menu": The button displays a menu.
		/// </summary>
		public string Type
		{
			get => GetAttribute("type", AvailableTypes, Defaults.Type);
			set => SetAttribute("type", value);
		}

		/// <summary>
		/// Gets or sets 'value' attribute value representing the current form control value of the button.
		/// </summary>
		public string Value
		{
			get => GetAttribute("value", Defaults.Value);
			set => SetAttribute("value", value);
		}

		/// <summary>
		/// Gets or sets 'disabled' attribute value indicating whether or not the control is disabled, meaning that it does not accept any clicks.
		/// </summary>
		public bool Disabled
		{
			get => GetAttribute("disabled") != null;
			set => SetAttribute("disabled", value ? "" : null);
		}

		/// <summary>
		/// Gets or sets 'name' attribute value representing the name of the object when submitted with a form. 
		/// </summary>
		public string Name
		{
			get => GetAttribute("name", Defaults.Name);
			set => SetAttribute("name", value);
		}

		/// <summary>
		/// Gets or sets 'accessKey' attribute value that is a String indicating the single-character keyboard key to give access to the button.
		/// </summary>
		public string AccessKey
		{
			get => GetAttribute("accessKey", Defaults.AccessKey);
			set => SetAttribute("accessKey", value);
		}

		/// <summary>
		/// Gets or sets 'tabIndex' attribute value that represents this element's position in the tabbing order.
		/// </summary>
		public long TabIndex
		{
			get => GetAttribute("tabIndex", Defaults.TabIndex);
			set => SetAttribute("tabIndex", value.ToString());
		}

		/// <summary>
		/// Is a <see cref="HtmlFormElement"/> reflecting the form that this button is associated with.
		/// </summary>
		public HtmlFormElement Form => this.FindOwnerForm();

		/// <summary>
		/// Gets or sets a string reflecting the URI of a resource that processes information submitted by the button. If specified, this attribute overrides the action attribute of the &lt;form&gt; element that owns this element.
		/// </summary>
		public string FormAction
		{
			get => GetAttribute("formaction", string.Empty);
			set => SetAttribute("formaction", value);
		}

		/// <summary>
		/// Gets or sets the 'formMethod' attribute reflecting the HTTP method that the browser uses to submit the form. If specified, this attribute overrides the method attribute of the &lt;form&gt; element that owns this element.
		/// </summary>
		public string FormMethod
		{
			get => GetAttribute("formmethod", string.Empty);
			set => SetAttribute("formmethod", value);
		}

		/// <summary>
		/// Gets or sets the 'formEnctype' attribute reflecting the type of content that is used to submit the form to the server.
		/// </summary>
		public string FormEnctype
		{
			get => GetAttribute("formenctype", string.Empty);
			set => SetAttribute("formenctype", value);
		}

		/// <summary>
		/// Gets or sets the 'formTarget' attribute reflecting a name or keyword indicating where to display the response that is received after submitting the form.
		/// </summary>
		public string FormTarget
		{
			get => GetAttribute("formtarget", string.Empty);
			set => SetAttribute("formtarget", value);
		}

		/// <summary>
		/// Is a Boolean indicating whether or not the control should have input focus when the page loads.
		/// </summary>
		public bool Autofocus
		{
			get => HasAttribute("autofocus");
			set => SetFlagAttribute("autofocus", value);
		}
		
		/// <summary>
		/// Gets a collection containing the &lt;label&gt; elements associated with the &lt;button&gt; element.
		/// </summary>
		public IReadOnlyCollection<HtmlLabelElement> Labels =>
			string.IsNullOrEmpty(Id) 
				? Enumerable.Empty<HtmlLabelElement>().ToList().AsReadOnly()
				: OwnerDocument.GetElementsByTagName("label")
					.OfType<HtmlLabelElement>()
					.Where(x => x.HtmlFor == Id)
					.ToList()
					.AsReadOnly();
	}
}
