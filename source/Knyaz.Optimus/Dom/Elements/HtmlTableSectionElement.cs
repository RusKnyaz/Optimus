using System;
using HtmlCollection = System.Collections.Generic.IReadOnlyList<Knyaz.Optimus.Dom.Elements.HtmlElement>;

namespace Knyaz.Optimus.Dom.Elements
{
	/// <summary>
	/// The THEAD, TFOOT, and TBODY elements.
	/// </summary>
	public class HtmlTableSectionElement : HtmlElement
	{
		public HtmlTableSectionElement(Document ownerDocument, string tagName) : base(ownerDocument, tagName)
		{
		}

		/// <summary>
		/// Delete a row from this section.
		/// </summary>
		public void DeleteRow(int index)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Horizontal alignment of data in cells.
		/// </summary>
		public string Align
		{
			get { return GetAttribute("align", string.Empty); }
			set { SetAttribute("align", value); }
		}

		/// <summary>
		/// Alignment character for cells in a column.
		/// </summary>
		public string Ch
		{
			get { return GetAttribute("char"); }
			set { SetAttribute("char", value); }
		}

		/// <summary>
		/// Offset of alignment character.
		/// </summary>
		public string ChOff
		{
			get { return GetAttribute("charoff"); }
			set { SetAttribute("charoff", value); }
		}

		/// <summary>
		/// The collection of rows in this table section.
		/// </summary>
		public HtmlCollection Rows
		{
			get { throw new NotImplementedException();}
		}

		/// <summary>
		/// Vertical alignment of data in cells.
		/// </summary>
		public string VAlign
		{
			get { return GetAttribute("valign", string.Empty); }
			set { SetAttribute("valign", value); }
		}

		/// <summary>
		/// Insert a row into this section.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public HtmlElement InsertRow(int index)
		{
			throw new NotImplementedException();
		}
	}
}