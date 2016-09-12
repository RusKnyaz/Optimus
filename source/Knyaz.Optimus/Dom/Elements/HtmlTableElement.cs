using System;
using System.Collections.Generic;
using System.Linq;

namespace Knyaz.Optimus.Dom.Elements
{
	public class HtmlTableElement : HtmlElement
	{
		public HtmlTableElement(Document ownerDocument) : base(ownerDocument, TagsNames.Table)
		{
			Rows = new HtmlCollection(GetRows);
			TBodies = new HtmlCollection(() => ChildNodes.OfType<HtmlTableSectionElement>().Where(x => x.TagName == TagsNames.TBody));
		}

		/// <summary>
		/// Create a new table caption object or return an existing one.
		/// </summary>
		public HtmlElement CreateCaption()
		{
			if (Caption == null)
				AppendChild(OwnerDocument.CreateElement(TagsNames.Caption));
			return Caption;
		}

		/// <summary>
		/// Create a table footer row or return an existing one.
		/// </summary>
		public HtmlElement CreateTFoot()
		{
			if (TFoot == null)
				AppendChild(OwnerDocument.CreateElement(TagsNames.TFoot));
			return TFoot;
		}

		/// <summary>
		/// Create a table header row or return an existing one.
		/// </summary>
		public HtmlElement CreateTHead()
		{
			if (THead == null)
				AppendChild(OwnerDocument.CreateElement(TagsNames.THead));
			return THead;
		}

		/// <summary>
		/// Delete the table caption, if one exists.
		/// </summary>
		public void DeleteCaption()
		{
			var cap = Caption;
			cap?.ParentNode.RemoveChild(cap);
		}

		/// <summary>
		/// Delete a table row.
		/// </summary>
		public void DeleteRow(int index)
		{
			var row = Rows[index];
			row.ParentNode.RemoveChild(row);
		}

		/// <summary>
		/// Delete the footer from the table, if one exists.
		/// </summary>
		public void DeleteTFoot()
		{
			var tfoot = TFoot;
			tfoot?.ParentNode.RemoveChild(tfoot);
		}

		/// <summary>
		/// Delete the header from the table, if one exists.
		/// </summary>
		public void DeleteTHead()
		{
			var thead = THead;
			thead.ParentNode.RemoveChild(thead);
		}

		/// <summary>
		/// Specifies the table's position with respect to the rest of the document.
		/// </summary>
		/// <returns></returns>
		public string Align
		{
			get { return GetAttribute("align", string.Empty);}
			set { SetAttribute("align", value);}
		}

		/// <summary>
		/// Cell background color.
		/// </summary>
		public string BgColor
		{
			get { return GetAttribute<string>("bgcolor", null); }
			set { SetAttribute("bgcolor", value); }
		}

		/// <summary>
		/// The width of the border around the table.
		/// </summary>
		public string Border
		{
			get { return GetAttribute("border", string.Empty); }
			set { SetAttribute("border", value); }
		}

		/// <summary>
		/// Returns the table's CAPTION, or void if none exists.
		/// </summary>
		public HtmlTableCaptionElement Caption
		{
			get { return ChildNodes.OfType<HtmlTableCaptionElement>().FirstOrDefault(); }
			set
			{
				var cap = Caption;
				if (cap == null)
				{
					var fc = FirstChild;
					if (fc != null)
						InsertBefore(value, fc);
					else
						AppendChild(value);
				}
				else
				{
					cap.ParentNode.ReplaceChild(value, cap);
				}
			}
		}

		/// <summary>
		/// Specifies the horizontal and vertical space between cell content and cell borders.
		/// </summary>
		public string CellPadding
		{
			get { return GetAttribute("cellpadding", string.Empty); }
			set { SetAttribute("cellpadding", value); }
		}

		/// <summary>
		/// Specifies the horizontal and vertical separation between cells.
		/// </summary>
		public string CellSpacing
		{
			get { return GetAttribute("cellspacing", string.Empty); }
			set { SetAttribute("cellspacing", value); }
		}

		/// <summary>
		/// Specifies which external table borders to render. One of:void|above|below|hsides|lhs|rhs|vsides|box|border
		/// </summary>
		public string Frame
		{
			get { return GetAttribute("frame", string.Empty); }
			set { SetAttribute("frame", value); }
		}

		IEnumerable<HtmlElement> GetRows()
		{
			foreach (var childNode in ChildNodes.OfType<HtmlElement>())
			{
				if (childNode is HtmlTableRowElement)
					yield return childNode;
				else if (childNode is HtmlTableSectionElement)
				{
					foreach (var sectionRow in childNode.ChildNodes.OfType<HtmlTableRowElement>())
						yield return sectionRow;
				}
			}
		}

		/// <summary>
		/// Returns a collection of all the rows in the table, including all in THEAD, TFOOT, all TBODY elements.
		/// </summary>
		public HtmlCollection Rows { get; private set; }

		/// <summary>
		/// Specifies which internal table borders to render. On of:none|groups|rows|cols|all
		/// </summary>
		public string Rules
		{
			get { return GetAttribute("rules", string.Empty); }
			set { SetAttribute("rules", value); }
		}

		/// <summary>
		/// Description about the purpose or structure of a table.
		/// </summary>
		public string Summary
		{
			get { return GetAttribute("summary", string.Empty); }
			set { SetAttribute("summary", value); }
		}

		/// <summary>
		/// Returns a collection of the table bodies(including implicit ones).
		/// </summary>
		public HtmlCollection TBodies { get; private set; }

		/// <summary>
		/// Returns the table's TFOOT, or null if none exists.
		/// </summary>
		public HtmlTableSectionElement TFoot
		{
			get { return ChildNodes.OfType<HtmlTableSectionElement>().FirstOrDefault(x => x.TagName == TagsNames.TFoot); }
			set
			{
				var tfoot = TFoot;
				if (tfoot == null)
				{
					AppendChild(value);
				}
				else
				{
					tfoot.ParentNode.ReplaceChild(value, tfoot);
				}
			}
		}

		/// <summary>
		/// Returns the table's THEAD, or null if none exists.
		/// </summary>
		public HtmlTableSectionElement THead
		{
			get { return ChildNodes.OfType<HtmlTableSectionElement>().FirstOrDefault(x => x.TagName == TagsNames.THead); }
			set
			{
				var thead = THead;
				if (thead == null)
				{
					AppendChild(value);
				}
				else
				{
					thead.ParentNode.ReplaceChild(value, thead);
				}
			}
		}

		/// <summary>
		/// Specifies the desired table width.
		/// </summary>
		public string Width
		{
			get { return GetAttribute("width", string.Empty); }
			set { SetAttribute("width", value); }
		}

		/// <summary>
		/// Append row at end
		/// </summary>
		/// <returns></returns>
		public HtmlElement InsertRow()
		{
			return InsertRow(-1);
		}

		/// <summary>
		/// Insert a new empty row in the table.
		/// </summary>
		public HtmlElement InsertRow(int index)
		{
			var row = (HtmlTableRowElement)OwnerDocument.CreateElement(TagsNames.Tr);

			var rows = Rows.ToList();
			if(index > rows.Count)
				throw new Exception("The index provided (" + index + ") is greater than the number of rows in the table ("+rows.Count+")");

			if (rows.Count == 0 || index == -1)
			{
				if (TBodies.Count == 0)
				{
					var tbody = OwnerDocument.CreateElement(TagsNames.TBody);
					tbody.AppendChild(row);
					AppendChild(tbody);

				}
				else
				{
					TBodies.Last().AppendChild(row);
				}
			}
			else
			{
				var beforeRow = rows[index];
				beforeRow.ParentNode.InsertBefore(row, beforeRow);
			}

			return row;
		}
	}
}
