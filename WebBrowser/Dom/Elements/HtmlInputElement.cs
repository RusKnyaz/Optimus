using WebBrowser.ScriptExecuting;

namespace WebBrowser.Dom.Elements
{
	/// <summary>
	/// http://www.w3.org/TR/html-markup/input.text.html
	/// </summary>
	public class HtmlInputElement : HtmlElement, IHtmlInputElement, IResettableElement
	{
		static class Defaults
		{
			public static bool Disabled = false;
			public static bool Checked = false;
			public static bool Readonly = false;
			public static bool Required = false;
			public static string Value = string.Empty;
			public static string Type = "text";
			public static string Autocomplete = "on";
		}

		public HtmlInputElement(Document ownerDocument) : base(ownerDocument, TagsNames.Input)
		{
			Type = "text";
		}

		/// <summary>
		/// Specifies whether or not an input field should have autocomplete enabled. Available values: "on"|"off".
		/// </summary>
		public string Autocomplete
		{
			get { return GetAttribute("autocomplete", Defaults.Autocomplete); }
			set { SetAttribute("autocomplete", value); }
		}

		public string Value
		{
			get { return GetAttribute("value", Defaults.Value); }
			set { SetAttribute("value", value); }
		}

		public bool Disabled
		{
			get { return GetAttribute("disabled", Defaults.Disabled); }
			set { SetAttribute("disabled", value.ToString()); }
		}

		public string Type
		{
			get { return GetAttribute("type", Defaults.Value); }
			set { SetAttribute("type", value); }
		}

		public bool Readonly
		{
			get { return GetAttribute("readonly", Defaults.Readonly); }
			set { SetAttribute("readonly", value.ToString()); }
		}

		public bool Required
		{
			get { return GetAttribute("required", Defaults.Required); }
			set { SetAttribute("required", value.ToString()); }
		}

		public bool Checked
		{
			get { return GetAttributeNode("checked") != null; }
			set { SetAttribute("checked", value.ToString());}
		}

		void IResettableElement.Reset()
		{
			//todo: implement
			//The reset algorithm for input elements is to set the dirty value flag and dirty checkedness flag back to false, 
			//set the value of the element to the value of the value content attribute, if there is one, or the empty string otherwise, 
			//set the checkedness of the element to true if the element has a checked content attribute and false if it does not, 
			//empty the list of selected files, and then invoke the value sanitization algorithm, if the type attribute's current 
			//state defines one.

			throw new System.NotImplementedException();
		}
	}

	[DomItem]
	public interface IHtmlInputElement
	{
		string Value { get; set; }
		bool Disabled { get; set; }
	}
}
