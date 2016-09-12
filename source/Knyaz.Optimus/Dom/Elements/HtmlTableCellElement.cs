using System;

namespace Knyaz.Optimus.Dom.Elements
{
	/// <summary>
	/// https://www.w3.org/2003/01/dom2-javadoc/org/w3c/dom/html2/HTMLTableCellElement.html
	/// </summary>
	public class HtmlTableCellElement : HtmlElement
	{
		public HtmlTableCellElement(Document ownerDocument) : base(ownerDocument, TagsNames.Td)
		{
		}

		/// <summary>
		/// Abbreviation for header cells.
		/// </summary>
		public string Abbr
		{
			get { throw new NotImplementedException();}
			set { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Horizontal alignment of data in cell.
		/// </summary>
		/// <returns></returns>
		public string Align
		{
			get { return GetAttribute("align", string.Empty); }
			set { SetAttribute("align", value); }
		}

		/// <summary>
		/// Names group of related headers.
		/// </summary>
		/// <returns></returns>
		public string Axis
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Cell background color.
		/// </summary>
		/// <returns></returns>
		public string BgColor
		{
			get { return GetAttribute<string>("bgcolor", null); }
			set { SetAttribute("bgcolor", value); }
		}

		/// <summary>
		/// The index of this cell in the row, starting from 0.
		/// </summary>
		/// <returns></returns>
		public int CellIndex
		{
			get { throw new NotImplementedException();}
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
		/// Number of columns spanned by cell.
		/// </summary>
		/// <returns></returns>
		public int ColSpan
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		/// <summary>
		/// List of id attribute values for header cells.
		/// </summary>
		/// <returns></returns>
		public string Headers
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Cell height.
		/// </summary>
		/// <returns></returns>
		public string Height
		{
			get { return GetAttribute("height", string.Empty); }
			set { SetAttribute("height", value); }
		}

		/// <summary>
		/// Suppress word wrapping.
		/// </summary>
		/// <returns></returns>
		public bool NoWrap
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Number of rows spanned by cell.
		/// </summary>
		/// <returns></returns>
		public int RowSpan
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Scope covered by header cells.
		/// </summary>
		/// <returns></returns>
		public string Scope
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
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
