using System;

namespace Knyaz.Optimus.Dom.Elements
{
	public class HtmlTableElement : HtmlElement
	{
		public HtmlTableElement(Document ownerDocument) : base(ownerDocument, TagsNames.Table)
		{
		}

		/// <summary>
		/// Create a new table caption object or return an existing one.
		/// </summary>
		public HtmlElement CreateCaption()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Create a table footer row or return an existing one.
		/// </summary>
		public HtmlElement CreateTFoot()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Create a table header row or return an existing one.
		/// </summary>
		public HtmlElement CreateTHead()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Delete the table caption, if one exists.
		/// </summary>
		public void DeleteCaption()
		{
			
		}

		/// <summary>
		/// Delete a table row.
		/// </summary>
		public void DeleteRow(int index)
		{
			
		}

		/// <summary>
		/// Delete the footer from the table, if one exists.
		/// </summary>
		public void DeleteTFoot()
		{
			
		}

		/// <summary>
		/// Delete the header from the table, if one exists.
		/// </summary>
		public void DeleteTHead()
		{
			
		}

		/// <summary>
		/// Specifies the table's position with respect to the rest of the document.
		/// </summary>
		/// <returns></returns>
		public string Align
		{
			get { throw new NotImplementedException();}
			set { throw new NotImplementedException();}
		}

		/// <summary>
		/// Cell background color.
		/// </summary>
		public string BgColor
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		/// <summary>
		/// The width of the border around the table.
		/// </summary>
		public string Border
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Returns the table's CAPTION, or void if none exists.
		/// </summary>
		public HtmlTableCaptionElement Caption
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Specifies the horizontal and vertical space between cell content and cell borders.
		/// </summary>
		public string CellPadding
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Specifies the horizontal and vertical separation between cells.
		/// </summary>
		public string CellSpacing
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Specifies which external table borders to render.
		/// </summary>
		public string Frame
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Returns a collection of all the rows in the table, including all in THEAD, TFOOT, all TBODY elements.
		/// </summary>
		public HtmlCollection Rows
		{
			get { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Specifies which internal table borders to render.
		/// </summary>
		public string Rules
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Description about the purpose or structure of a table.
		/// </summary>
		public string Summary
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Returns a collection of the table bodies(including implicit ones).
		/// </summary>
		public HtmlCollection TBodies
		{
			get { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Returns the table's TFOOT, or null if none exists.
		/// </summary>
		public HtmlTableSectionElement TFoot
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Returns the table's THEAD, or null if none exists.
		/// </summary>
		public HtmlTableSectionElement THead
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Specifies the desired table width.
		/// </summary>
		public string Width
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Insert a new empty row in the table.
		/// </summary>
		public HtmlElement InsertRow(int index)
		{
			throw new NotImplementedException();
		}
	}
}
