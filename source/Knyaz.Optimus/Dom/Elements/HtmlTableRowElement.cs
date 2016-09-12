using System;
using System.Linq;

namespace Knyaz.Optimus.Dom.Elements
{
	/// <summary>
	/// https://www.w3.org/2003/01/dom2-javadoc/org/w3c/dom/html2/HTMLTableRowElement.html
	/// </summary>
	public class HtmlTableRowElement : HtmlElement
	{
		public HtmlTableRowElement(Document ownerDocument) : base(ownerDocument, TagsNames.Tr)
		{
			Cells = new HtmlCollection(() => ChildNodes.OfType<HtmlTableCellElement>());
		}

		/// <summary>
		/// Delete a cell from the current row.
		/// </summary>
		/// <param name="index"></param>
		public void DeleteCell(int index)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Horizontal alignment of data within cells of this row.
		/// </summary>
		public string Align
		{
			get { throw new NotImplementedException();}
			set { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Background color for rows.
		/// </summary>
		public string BgColor
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		/// <summary>
		/// The collection of cells in this row.
		/// </summary>
		public HtmlCollection Cells { get; private set; }

		/// <summary>
		/// Alignment character for cells in a column.
		/// </summary>
		public string Ch
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Offset of alignment character.
		/// </summary>
		public string ChOff
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}
		
		/// <summary>
		/// This is in logical order and not in document order.
		/// </summary>
		public int RowIndex
		{
			get { throw new NotImplementedException();}
		}

		/// <summary>
		/// The index of this row, relative to the current section ( THEAD, TFOOT, or TBODY), starting from 0.
		/// </summary>
		public int SectionRowIndex
		{
			get { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Vertical alignment of data within cells of this row.
		/// </summary>
		public string VAlign
		{
			get { throw new NotImplementedException();}
			set { throw new NotImplementedException(); }
		}


		/// <summary>
		/// Insert an empty TD cell into this row.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public HtmlElement InsertCell(int index)
		{
			throw new NotImplementedException();
		}
	}
}
