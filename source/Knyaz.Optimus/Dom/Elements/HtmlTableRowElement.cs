using System.Linq;
using Knyaz.Optimus.Tools;

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
			var cell = Cells[index];
			cell.ParentNode.RemoveChild(cell);
		}

		/// <summary>
		/// Horizontal alignment of data within cells of this row.
		/// </summary>
		public string Align
		{
			get { return GetAttribute("align", string.Empty); }
			set { SetAttribute("align", value); }
		}

		/// <summary>
		/// Background color for rows.
		/// </summary>
		public string BgColor
		{
			get { return GetAttribute<string>("bgcolor", null); }
			set { SetAttribute("bgcolor", value); }
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
		/// This is in logical order and not in document order.
		/// </summary>
		public int RowIndex
		{
			get
			{
				var table = ParentNode as HtmlTableElement ?? ParentNode.ParentNode as HtmlTableElement;
				return table.Rows.IndexOf(this);
			}
		}

		/// <summary>
		/// The index of this row, relative to the current section ( THEAD, TFOOT, or TBODY), starting from 0.
		/// </summary>
		public int SectionRowIndex
		{
			get
			{
				var rows = (ParentNode as HtmlTableElement)?.Rows ?? ((HtmlTableSectionElement) ParentNode).Rows;
				return rows.IndexOf(this);
			}
		}

		/// <summary>
		/// Vertical alignment of data within cells of this row.
		/// </summary>
		public string VAlign
		{
			get { return GetAttribute("valign", string.Empty); }
			set { SetAttribute("valign", value); }
		}


		/// <summary>
		/// Insert an empty TD cell into this row.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public HtmlElement InsertCell(int index = -1)
		{
			var newCell = (HtmlElement)OwnerDocument.CreateElement("td");

			var cells = Cells;
			if (cells.Count == 0 || index == -1)
				AppendChild(newCell);

			var cell = Cells[index];
			InsertBefore(newCell, cell);

			return newCell;
		}
	}
}
