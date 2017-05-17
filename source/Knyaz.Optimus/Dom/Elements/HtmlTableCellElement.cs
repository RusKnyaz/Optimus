using System;
using Knyaz.Optimus.Tools;

namespace Knyaz.Optimus.Dom.Elements
{
	/// <summary>
	/// https://www.w3.org/2003/01/dom2-javadoc/org/w3c/dom/html2/HTMLTableCellElement.html
	/// </summary>
	public sealed class HtmlTableCellElement : HtmlElement
	{
		public HtmlTableCellElement(Document ownerDocument, string tagName) : base(ownerDocument, tagName)
		{
		}

		/// <summary>
		/// Abbreviation for header cells.
		/// </summary>
		public string Abbr
		{
			get {return GetAttribute("abbr", string.Empty);}
			set { SetAttribute("abbr", value); }
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
			get { return GetAttribute("axis", string.Empty); }
			set { SetAttribute("axis", value); }
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
		public int CellIndex => ((HtmlTableRowElement) ParentNode).Cells.IndexOf(this);

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
			get { return GetAttribute("colSpan", 1); }
			set
			{
				SetAttribute("colSpan", value.ToString());
			}
		}

		/// <summary>
		/// List of id attribute values for header cells.
		/// </summary>
		/// <returns></returns>
		public string Headers
		{
			get { return GetAttribute("headers", string.Empty); }
			set { SetAttribute("headers", value); }
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
			get { return HasAttribute("nowrap"); }
			set
			{
				if (value)
					SetAttribute("nowrap", "nowrap");
				else
					RemoveAttribute("nowrap");
			}
		}

		/// <summary>
		/// Number of rows spanned by cell.
		/// </summary>
		/// <returns></returns>
		public int RowSpan
		{
			get { return GetAttribute("rowSpan", 1); }
			set { SetAttribute("rowSpan", value.ToString()); }
		}

		/// <summary>
		/// Scope covered by header cells.
		/// </summary>
		/// <returns></returns>
		public string Scope
		{
			get { return GetAttribute("scope", string.Empty); }
			set { SetAttribute("scope", value); }
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
