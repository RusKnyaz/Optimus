namespace WebBrowser.Dom.Elements
{
	/// <summary>
	/// Represents textarea html element (see http://www.w3.org/TR/html5/forms.html#the-textarea-element).
	/// Todo: complete implementation
	/// </summary>
	public class HtmlTextAreaElement : HtmlElement
	{
		static class Defaults
		{
			public static bool Autofocus = false;
			public static bool Disabled = false;
			public static ulong Cols = 20ul;
			public static string Placeholder = string.Empty;
			public static bool Required = false;
			public static bool Readonly = false;
			public static ulong Rows = 2ul;
		}

		public HtmlTextAreaElement(Document ownerDocument) : base(ownerDocument, "textarea")
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


		//		 attribute DOMString autocomplete;
//           attribute DOMString dirName;
//  readonly attribute HTMLFormElement? form;
//           attribute long maxLength;
//           attribute long minLength;
//           attribute DOMString name;
//           attribute DOMString wrap;
//
//  readonly attribute DOMString type;
//           attribute DOMString defaultValue;
//  readonly attribute unsigned long textLength;
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
	}
}
