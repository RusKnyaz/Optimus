namespace Knyaz.Optimus.Dom.Elements
{
	/// <summary>
	/// Represents &lt;IFRAME&gt; element.
	/// </summary>
	public sealed class HtmlIFrameElement : HtmlElement
	{
		private static class Defaults
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

		internal HtmlIFrameElement(Document ownerDocument) : base(ownerDocument, TagsNames.IFrame)
		{
		}

		/// <summary>
		/// Gets or sets the 'align' attribute value that specifies the alignment of the frame with respect to the surrounding context.
		/// </summary>
		public string Align
		{
			get => GetAttribute("align", Defaults.Align);
			set => SetAttribute("align", value);
		}

		/// <summary>
		/// URI [IETF RFC 2396] designating a long description of this image or frame
		/// </summary>
		public string LongDesc
		{
			get { return GetAttribute("longDesc", Defaults.LongDesc); }
			set { SetAttribute("longDesc", value); }
		}

		/// <summary>
		/// Gets or sets the 'name' attribute value that reflects the name HTML attribute, containing a name by which to refer to the frame.
		/// </summary>
		public string Name
		{
			get { return GetAttribute("name", Defaults.Name); }
			set { SetAttribute("name", value); }
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
