namespace Knyaz.Optimus.Dom.Elements
{
	public sealed class HtmlIFrameElement : HtmlElement
	{
		public static class Defaults
		{
			public const string Name = "";
			public const string Src = "";
			public const string Srcdoc = "";
			public const string Width = "";
			public const string Height = "";
			public const string Align = "";
			public const string FrameBorder = "";
			public const string LongDesc = "";
			public const string Scrolling = "";
		}

		public HtmlIFrameElement(Document ownerDocument) : base(ownerDocument, TagsNames.IFrame)
		{
		}

		public string Align
		{
			get { return GetAttribute("align", Defaults.Align); }
			set { SetAttribute("align", value); }
		}

		public string FrameBorder
		{
			get { return GetAttribute("frameBorder", Defaults.FrameBorder); }
			set { SetAttribute("frameBorder", value); }
		}

		/// <summary>
		/// URI [IETF RFC 2396] designating a long description of this image or frame
		/// </summary>
		public string LongDesc
		{
			get { return GetAttribute("longDesc", Defaults.LongDesc); }
			set { SetAttribute("longDesc", value); }
		}

		public string Name
		{
			get { return GetAttribute("name", Defaults.Name); }
			set { SetAttribute("name", value); }
		}

		public string Scrolling
		{
			get { return GetAttribute("scrolling", Defaults.Scrolling); }
			set { SetAttribute("scrolling", value); }
		}

		/// <summary>
		/// The src attribute gives the address of a page that the iframe is to contain.
		/// </summary>
		public string Src
		{
			get { return GetAttribute("src", Defaults.Src); }
			set { SetAttribute("src", value); }
		}

		public string Width
		{
			get { return GetAttribute("width", Defaults.Width); }
			set { SetAttribute("width", value); }
		}

		public string Height
		{
			get { return GetAttribute("height", Defaults.Height); }
			set { SetAttribute("height", value); }
		}

		/// <summary>
		/// The document this frame contains, if there is any and it is available, or null otherwise.
		/// todo: implement document loading
		/// </summary>
		public Document ContentDocument { get; internal set; }
	}
}
