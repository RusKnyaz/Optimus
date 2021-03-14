using System.Net;
using Knyaz.Optimus.ScriptExecuting;

namespace Knyaz.Optimus.Dom.Elements
{
	/// <summary>
	/// Represents textarea html element (see http://www.w3.org/TR/html5/forms.html#the-textarea-element).
	/// </summary>
	// Todo: complete implementation
	[JsName("HTMLTextAreaElement")]
	public sealed class HtmlTextAreaElement : HtmlElement, IResettableElement, IFormElement
	{
		static class Defaults
		{
			public static string Autocomplete = "on";
			public static bool Autofocus = false;
			public static bool Disabled = false;
			public static ulong Cols = 20ul;
			public static string Placeholder = string.Empty;
			public static bool Required = false;
			public static bool Readonly = false;
			public static ulong Rows = 2ul;
			public static string DirName = string.Empty;
			public static long MaxLength = -1;
			public static long MinLength = -1;
			public static string Name = string.Empty;
			public static string Wrap = string.Empty;
		}

		internal HtmlTextAreaElement(Document ownerDocument) : base(ownerDocument, TagsNames.Textarea){}

		/// <summary>
		/// Gets or sets the element's autofocus attribute, indicating that the control should have input focus when the page loads.
		/// </summary>
		public bool Autofocus
		{
			get => GetAttribute("autofocus", Defaults.Autofocus);
			set => SetAttribute("autofocus", value ? "true" : "false");
		}

		/// <summary>
		/// Gets or sets the element's cols attribute, indicating the visible width of the text area.
		/// </summary>
		public ulong Cols
		{
			get => GetAttribute("cols", Defaults.Cols);
			set => SetAttribute("cols", value.ToString());
		}

		/// <summary>
		/// Gets or sets the element's rows attribute, indicating the number of visible text lines for the control.
		/// </summary>
		public ulong Rows
		{
			get => GetAttribute("rows", Defaults.Rows);
			set => SetAttribute("rows", value.ToString());
		}

		/// <summary>
		/// Gets or sets the element's disabled attribute, indicating that the control is not available for interaction.
		/// </summary>
		public bool Disabled
		{
			get => GetAttribute("disabled", Defaults.Disabled);
			set => SetAttribute("disabled", value.ToString());
		}

		/// <summary>
		/// Gets or sets the element's required attribute, indicating that the user must specify a value before submitting the form.
		/// </summary>
		public bool Required
		{
			get => GetAttribute("required", Defaults.Required);
			set => SetAttribute("required", value.ToString());
		}

		/// <summary>
		/// Gets or sets the element's readonly attribute, indicating that the user cannot modify the value of the control.
		/// </summary>
		public bool ReadOnly
		{
			get => GetAttribute("readonly", Defaults.Readonly);
			set => SetAttribute("readonly", value.ToString());
		}

		/// <summary>
		/// Gets or sets the element's placeholder attribute, containing a hint to the user about what to enter in the control.
		/// </summary>
		public string Placeholder
		{
			get => GetAttribute("placeholder", Defaults.Placeholder);
			set => SetAttribute("placeholder", value);
		}

		private string _innerHTML;
		public override string InnerHTML
		{
			get => _innerHTML;
			set
			{
				_innerHTML = WebUtility.HtmlEncode(value);
				Value = value;
			}
		}

		/// <summary> Gets or sets the raw value contained in the control. </summary>
		public string Value { get; set; }

		
		public string DirName
		{
			get { return GetAttribute("dirName", Defaults.DirName); }
			set { SetAttribute("dirName", value); }
		}

		/// <summary>
		/// Gets or sets the element's maxlength attribute, indicating the maximum number of characters the user can enter. 
		/// </summary>
		public long MaxLength
		{
			get => GetAttribute("maxLength", Defaults.MaxLength);
			set => SetAttribute("maxLength", value.ToString());
		}

		/// <summary>
		/// Gets or sets the element's minlength attribute, indicating the minimum number of characters the user can enter. 
		/// </summary>
		public long MinLength
		{
			get => GetAttribute("minLength", Defaults.MinLength);
			set => SetAttribute("minLength", value.ToString());
		}

		/// <summary>
		/// Gets or sets the element's name attribute, containing the name of the control.
		/// </summary>
		public string Name
		{
			get => GetAttribute("name", Defaults.Name);
			set => SetAttribute("name", value);
		}

		/// <summary>
		/// Gets or sets the wrap HTML attribute, indicating how the control wraps text.
		/// </summary>
		public string Wrap
		{
			get => GetAttribute("wrap", Defaults.Wrap);
			set => SetAttribute("wrap", value);
		}

		/// <summary>
		/// Gets or sets the control's default value, which behaves like the Node.textContent property.
		/// </summary>
		public string DefaultValue
		{
			get => TextContent;
			set => TextContent = value;
		}

		/// <summary>
		/// Gets the length of the control's value.
		/// </summary>
		public ulong TextLength => (ulong) (Value ?? string.Empty).Length;

//  readonly attribute HTMLFormElement? form;
//
//  readonly attribute boolean willValidate;
//  readonly attribute ValidityState validity;
//  readonly attribute DOMString validationMessage;
//  boolean checkValidity();
//  void setCustomValidity(DOMString error);
//
//  readonly attribute NodeList labels;
//
//  void select();
//           attribute unsigned long selectionStart;
//           attribute unsigned long selectionEnd;
//           attribute DOMString selectionDirection;
//  void setRangeText(DOMString replacement);
//  void setRangeText(DOMString replacement, unsigned long start, unsigned long end, optional SelectionMode selectionMode = "preserve");
//  void setSelectionRange(unsigned long start, unsigned long end, optional DOMString direction);


		void IResettableElement.Reset()
		{
			Value = TextContent;
		}

		/// <summary>
		/// Returns a reference to the parent form element.
		/// </summary>
		public HtmlFormElement Form => this.FindOwnerForm();

		/// <summary>
		/// Returns "[object HTMLTextAreaElement]".
		/// </summary>
		public override string ToString() => "[object HTMLTextAreaElement]";
	}
}
