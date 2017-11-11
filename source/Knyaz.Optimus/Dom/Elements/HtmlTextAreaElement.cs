namespace Knyaz.Optimus.Dom.Elements
{
	/// <summary>
	/// Represents textarea html element (see http://www.w3.org/TR/html5/forms.html#the-textarea-element).
	/// Todo: complete implementation
	/// </summary>
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
			public static string DefaultValue = string.Empty;
		}

		public HtmlTextAreaElement(Document ownerDocument) : base(ownerDocument, TagsNames.Textarea)
		{
		}

		public bool Autofocus
		{
			get { return GetAttribute("autofocus", Defaults.Autofocus); }
			set { SetAttribute("autofocus", value ? "true" : "false"); }
		}

		public ulong Cols
		{
			get { return GetAttribute("cols", Defaults.Cols); }
			set { SetAttribute("cols", value.ToString());}
		}

		public ulong Rows
		{
			get { return GetAttribute("rows", Defaults.Rows); }
			set { SetAttribute("rows", value.ToString());}
		}

		public bool Disabled
		{
			get { return GetAttribute("disabled", Defaults.Disabled); }
			set { SetAttribute("disabled", value.ToString()); }
		}

		public bool Required
		{
			get { return GetAttribute("required", Defaults.Required); }
			set { SetAttribute("required", value.ToString()); }
		}

		public bool Readonly
		{
			get { return GetAttribute("readonly", Defaults.Readonly); }
			set { SetAttribute("readonly", value.ToString()); }
		}

		public string Placeholder
		{
			get { return GetAttribute("placeholder", Defaults.Placeholder); }
			set { SetAttribute("placeholder", value); }
		}

		public string Value
		{
			get { return InnerHTML ?? string.Empty; }
			set { InnerHTML = value ?? string.Empty; }
		}

		public string DirName
		{
			get { return GetAttribute("dirName", Defaults.DirName); }
			set { SetAttribute("dirName", value); }
		}

		public long MaxLength
		{
			get { return GetAttribute("maxLength", Defaults.MaxLength); }
			set { SetAttribute("maxLength", value.ToString()); }
		}

		public long MinLength
		{
			get { return GetAttribute("minLength", Defaults.MinLength); }
			set { SetAttribute("minLength", value.ToString()); }
		}

		public string Name
		{
			get { return GetAttribute("name", Defaults.Name); }
			set { SetAttribute("name", value); }
		}

		public string Wrap
		{
			get { return GetAttribute("wrap", Defaults.Wrap); }
			set { SetAttribute("wrap", value); }
		}

		public string DefaultValue
		{
			get { return GetAttribute("defaultValue", Defaults.DefaultValue); }
			set { SetAttribute("defaultValue", value); }
		}

		public ulong TextLength
		{
			get { return (ulong) (Value ?? string.Empty).Length; }
		}

		//todo: Check implementation
		override public string TextContent { get {return DefaultValue;} set { DefaultValue = value; } }

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
		public void Reset()
		{
			Value = TextContent;
		}

		public HtmlFormElement Form
		{
			get { return this.FindOwnerForm(); }
		}

		public override string ToString()
		{
			return "[object HTMLTextAreaElement]";
		}
	}
}
