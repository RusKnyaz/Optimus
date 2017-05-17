using System;
using System.Linq;

namespace Knyaz.Optimus.Dom.Elements
{
	/// <summary>
	/// The THEAD, TFOOT, and TBODY elements.
	/// </summary>
	public sealed class HtmlTableSectionElement : HtmlElement
	{
		public HtmlTableSectionElement(Document ownerDocument, string tagName) : base(ownerDocument, tagName)
		{
			Rows = new HtmlCollection(() => ChildNodes.OfType<HtmlTableRowElement>());
		}

		/// <summary>
		/// Delete a row from this section.
		/// </summary>
		public void DeleteRow(int index)
		{
			var row = Rows[index];
			row.ParentNode.RemoveChild(row);
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
		public HtmlCollection Rows { get; private set; }

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
		public HtmlElement InsertRow(int index = -1)
		{
			var row = (HtmlElement)OwnerDocument.CreateElement(TagsNames.Tr);

			var rows = Rows;

			if (rows.Count == 0 || index == -1)
			{
				AppendChild(row);
			}
			else
			{
				InsertBefore(row, rows[index]);
			}

			return row;
		}
	}
}