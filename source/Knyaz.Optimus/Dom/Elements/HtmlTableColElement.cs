namespace Knyaz.Optimus.Dom.Elements
{
	/// <summary>
	/// https://www.w3.org/2003/01/dom2-javadoc/org/w3c/dom/html2/HTMLTableColElement.html
	/// </summary>
	public sealed class HtmlTableColElement : HtmlElement
	{
		public HtmlTableColElement(Document ownerDocument) : base(ownerDocument, TagsNames.Col)
		{
		}

		public string Align
		{
			get { return GetAttribute("align", string.Empty); }
			set { SetAttribute("align", value); }
		}

		/// <summary>
		/// Alignment character for cells in a column.
		/// </summary>
		/// <returns></returns>
		public string Ch
		{
			get { return GetAttribute("char"); }
			set { SetAttribute("char", value); }
		}

		/// <summary>
		/// Offset of alignment character.
		/// </summary>
		/// <returns></returns>
		public string ChOff
		{
			get { return GetAttribute("charoff"); }
			set { SetAttribute("charoff", value); }
		}

		/// <summary>
		/// Vertical alignment of data in cell.
		/// </summary>
		/// <returns></returns>
		public int Span
		{
			get { return GetAttribute("span", 1); }
			set { SetAttribute("valign", value.ToString()); }
		}

		/// <summary>
		/// Vertical alignment of data in cell.
		/// </summary>
		/// <returns></returns>
		public string VAlign
		{
			get { return GetAttribute("valign", string.Empty); }
			set { SetAttribute("valign", value); }
		}

		/// <summary>
		/// Cell width.
		/// </summary>
		/// <returns></returns>
		public string Width
		{
			get { return GetAttribute("width", string.Empty); }
			set { SetAttribute("width", value); }
		}
	}
}
