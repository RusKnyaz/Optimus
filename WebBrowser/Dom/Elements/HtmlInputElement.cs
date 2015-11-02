using WebBrowser.ScriptExecuting;

namespace WebBrowser.Dom.Elements
{
	/// <summary>
	/// http://www.w3.org/TR/html-markup/input.text.html
	/// </summary>
	public class HtmlInputElement : HtmlElement, IHtmlInputElement
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

		public HtmlInputElement(Document ownerDocument) : base(ownerDocument, "input")
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
			get { return GetAttribute("checked", Defaults.Checked); }
			set { SetAttribute("checked", value.ToString());}
		}
	}

	[DomItem]
	public interface IHtmlInputElement
	{
		string Value { get; set; }
		bool Disabled { get; set; }
	}
}
