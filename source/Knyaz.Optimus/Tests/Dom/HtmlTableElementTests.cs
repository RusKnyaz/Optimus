#if NUNIT
using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Elements;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.Dom
{
	[TestFixture]
	public class HtmlTableElementTests
	{
		private Document _document;
		HtmlTableElement _table;

		[SetUp]
		public void SetUp()
		{
			_document = new Document();
			_table = (HtmlTableElement)_document.CreateElement("TABLE");
		}

		[Test]
		public void Default()
		{
			_table.Assert(table => table.Rows.Count == 0
			                       && table.TBodies.Count == 0
								   && table.ChildNodes.Count == 0);
		}

		[Test]
		public void CreateCaption()
		{
			var caption = _table.CreateCaption();
			Assert.IsNotNull(caption);
			Assert.IsInstanceOf<HtmlTableCaptionElement>(caption);
		}

		[Test]
		public void CreateCaptionTwice()
		{
			var caption = _table.CreateCaption();
			var caption2 = _table.CreateCaption();
			Assert.AreEqual(caption, caption2);
		}

		[Test]
		public void AddCaptionAsChildNode()
		{
			var caption = _document.CreateElement("CAPTION");
			_table.AppendChild(caption);
			_table.Assert(table => table.Caption == caption);
		}

		[Test]
		public void AddCaptionTwice()
		{
			var cap1 = _document.CreateElement("CAPTION");
			var cap2 = _document.CreateElement("CAPTION");
			_table.AppendChild(cap1);
			_table.AppendChild(cap2);
			_table.Assert(table => table.Caption == cap1 
				&& table.ChildNodes.Count == 2 
				&& table.ChildNodes[1] == cap2);
		}

		[Test]
		public void InsertRowIntoEmptyTable()
		{
			var row = _table.InsertRow(0);

			_table.Assert(table => 
				table.Rows.Count == 1 &&
				table.TBodies.Count == 1 &&
				table.ChildNodes.Count == 1 &&
				((HtmlElement)table.ChildNodes[0]).TagName == "TBODY");
		}

		[Test]
		public void InsertRowIntoTableWithoutBodies()
		{
			var tr1 = _document.CreateElement("TR");
			_table.AppendChild(tr1);

			var tr2 = _table.InsertRow(0);

			_table.Assert(table => 
				table.Rows.Count == 2 &&
				table.Rows[0] == tr2 &&
				table.Rows[1] == tr1 &&
				table.TBodies.Count == 0);
		}

		[Test]
		public void InsertRowAtEnd()
		{
			_table.AppendChild(_document.CreateElement("TBODY"));
			_table.AppendChild(_document.CreateElement("TBODY"));
			_table.TBodies[1].AppendChild(_document.CreateElement("TR"));

			var row = _table.InsertRow();

			_table.Assert(table =>
				table.Rows.Count == 2 &&
				table.TBodies.Count == 2 &&
				table.ChildNodes.Count == 2 &&
				table.TBodies[1].ChildNodes[1] == row);
		}

		[Test]
		public void InsertRowAtEndIntoEmptyTable()
		{
			var row = _table.InsertRow();

			_table.Assert(table =>
				table.Rows.Count == 1 &&
				table.TBodies.Count == 1 &&
				table.ChildNodes.Count == 1 &&
				((HtmlElement)table.ChildNodes[0]).TagName == "TBODY");
		}

		[Test]
		public void InsertRowIntoTableWithTwoEmptyBodiesTest()
		{
			_table.AppendChild(_document.CreateElement("TBODY"));
			_table.AppendChild(_document.CreateElement("TBODY"));
			var row = _table.InsertRow(0);

			_table.Assert(table =>
				table.Rows.Count == 1 &&
				table.TBodies.Count == 2 &&
				table.ChildNodes.Count == 2 &&
				table.TBodies[0].ChildNodes.Count == 0 &&
				table.TBodies[1].ChildNodes.Count == 1);
		}

		[Test]
		public void InsertRowIntoTableWithTwoBodiesTest()
		{
			_table.AppendChild(_document.CreateElement("TBODY"));
			_table.TBodies[0].AppendChild(_document.CreateElement("TR"));
			_table.AppendChild(_document.CreateElement("TBODY"));
			var row = _table.InsertRow(0);

			_table.Assert(table =>
				table.Rows.Count == 2 &&
				table.TBodies.Count == 2 &&
				table.ChildNodes.Count == 2 &&
				table.TBodies[0].ChildNodes.Count == 2 &&
				table.TBodies[1].ChildNodes.Count == 0);
		}

		[Test]
		public void AddRowAsChildNode()
		{
			_table.AppendChild(_document.CreateElement("TR"));

			_table.Assert(table =>
				table.Rows.Count == 1 &&
				table.TBodies.Count == 0 &&
				table.ChildNodes.Count == 1 &&
				((HtmlElement)table.ChildNodes[0]).TagName == "TR");
		}

		[Test]
		public void AppendTBodies()
		{
			var tbody1 = (HtmlTableSectionElement)_document.CreateElement("TBODY");
			var tbody2 = (HtmlTableSectionElement)_document.CreateElement("TBODY");
			_table.AppendChild(tbody1);
			_table.AppendChild(tbody2);
			_table.Assert(table => table.TBodies[0] == tbody1 && table.TBodies[1] == tbody2);
		}

		[Test]
		public void CreateTHead()
		{
			var thead = _table.CreateTHead();
			Assert.IsNotNull(thead);
			Assert.AreEqual(thead, _table.THead);
		}

		[Test]
		public void CreateTHeadTwice()
		{
			var thead1 = _table.CreateTHead();
			var thead2 = _table.CreateTHead();
			_table.Assert(table => table.THead == thead1 &&
				table.THead == thead2 &&
				table.ChildNodes.Count == 1);
		}

		[Test]
		public void CreateTFoot()
		{
			var tfoot = _table.CreateTFoot();
			Assert.IsNotNull(tfoot);
			Assert.AreEqual(tfoot, _table.TFoot);
		}

		[Test]
		public void CreateTFootTwice()
		{
			var tfoot1 = _table.CreateTFoot();
			var tfoot2 = _table.CreateTFoot();
			_table.Assert(table => table.TFoot == tfoot1 &&
				table.TFoot == tfoot2 &&
				table.ChildNodes.Count == 1);
		}

		[Test]
		public void AppendTFoot()
		{
			var tfoot = _document.CreateElement("TFOOT");
			_table.AppendChild(tfoot);
			_table.Assert(table => table.TFoot == tfoot);
		}

		[Test]
		public void AppendTHead()
		{
			var thead = _document.CreateElement("THEAD");
			_table.AppendChild(thead);
			_table.Assert(table => table.THead == thead);
		}

		[Test]
		public void AppendTHeadTwice()
		{
			var th1 = _document.CreateElement("THEAD");
			th1.AppendChild(_document.CreateElement("TR"));
			var th2 = _document.CreateElement("THEAD");
			th2.AppendChild(_document.CreateElement("TR"));
			_table.AppendChild(th1);
			_table.AppendChild(th2);
			_table.Assert(table => 
				table.THead == th1 &&
				table.Rows.Count == 2 &&
				table.ChildNodes.Count == 2 &&
				table.ChildNodes[1] == th2);
		}

		[Test]
		public void SetInnerHTMLAddsBody()
		{
			_table.InnerHTML = "<TR></TR>";
			_table.Assert(table => table.TBodies.Count == 1 && table.Rows.Count == 1 && table.TBodies[0].ChildNodes.Count == 1);
		}

		[Test]
		public void RowsOrder()
		{
			var tb1 = _document.CreateElement("tbody");
			var tr1 = _document.CreateElement("tr");
			tr1.Id = "TR1";
			tb1.AppendChild(tr1);
			var tb2 = _document.CreateElement("tbody");
			var tr2 = _document.CreateElement("tr");
			tr2.Id = "TR2";
			tb2.AppendChild(tr2);

			var tr = _document.CreateElement("TR");
			tr.Id = "TR";

			_table.AppendChild(tb1);
			_table.AppendChild(tr);
			_table.AppendChild(tb2);
			_table.Assert(table => 
				table.Rows[0] == tr1 &&
				table.Rows[1] == tr &&
				table.Rows[2] == tr2);
		}

		[Test]
		public void RowsReference()
		{
			var rows = _table.Rows;
			_table.AppendChild(_document.CreateElement("tr"));
			Assert.AreEqual(1, rows.Count);
		}

		[Test]
		public void DeleteSingleCaption()
		{
			_table.CreateCaption();
			_table.DeleteCaption();
			_table.Assert(table => table.Caption == null);
		}

		[Test]
		public void DeleteCaptions()
		{
			_table.AppendChild(_document.CreateElement("caption"));
			_table.AppendChild(_document.CreateElement("caption"));
			_table.DeleteCaption();
			_table.Assert(table => table.Caption != null && table.ChildNodes.Count == 1);
			_table.DeleteCaption();
			_table.Assert(table => table.Caption == null && table.ChildNodes.Count == 0);
		}

		[Test]
		public void DeleteSingleTFoot()
		{
			_table.CreateTFoot();
			_table.DeleteTFoot();
			_table.Assert(table => table.ChildNodes.Count == 0 && table.TFoot == null);
		}

		[Test]
		public void DeleteTFoots()
		{
			_table.AppendChild(_document.CreateElement("tfoot"));
			_table.AppendChild(_document.CreateElement("tfoot"));
			_table.DeleteTFoot();
			_table.Assert(table => table.TFoot != null && table.ChildNodes.Count == 1);
			_table.DeleteTFoot();
			_table.Assert(table => table.TFoot == null && table.ChildNodes.Count == 0);
		}

		[Test]
		public void DeleteSingleTHead()
		{
			_table.CreateTHead();
			_table.DeleteTHead();
			_table.Assert(table => table.ChildNodes.Count == 0 && table.THead == null);
		}

		[Test]
		public void DeleteTHeads()
		{
			_table.AppendChild(_document.CreateElement("thead"));
			_table.AppendChild(_document.CreateElement("thead"));
			_table.DeleteTHead();
			_table.Assert(table => table.THead != null && table.ChildNodes.Count == 1);
			_table.DeleteTHead();
			_table.Assert(table => table.THead == null && table.ChildNodes.Count == 0);
		}

		[Test]
		public void DeleteRow()
		{
			_table.AppendChild(_document.CreateElement("tr"));
			_table.AppendChild(_document.CreateElement("tr"));
			_table.DeleteRow(0);
			_table.Assert(table => table.ChildNodes.Count == 1);
		}

		[Test]
		public void DeleteRowFromBody()
		{
			_table.AppendChild(_document.CreateElement("tbody"));
			_table.AppendChild(_document.CreateElement("tbody"));
			_table.TBodies[0].AppendChild(_document.CreateElement("tr"));
			_table.TBodies[1].AppendChild(_document.CreateElement("tr"));

			_table.DeleteRow(1);
			_table.Assert(table => table.Rows.Count == 1 &&
				table.TBodies[0].ChildNodes.Count == 1 &&
				table.TBodies[1].ChildNodes.Count == 0);
		}

		[Test]
		public void SetCaption()
		{
			_table.Caption = (HtmlTableCaptionElement)_document.CreateElement("caption");
			_table.Assert(table => table.ChildNodes.Count == 1 && table.Caption != null && table.Caption.ParentNode == table);
		}

		[Test]
		public void ReplaceCaption()
		{
			_table.Caption = (HtmlTableCaptionElement) _document.CreateElement("caption");
			var cap2 = (HtmlTableCaptionElement)_document.CreateElement("caption");
			_table.Caption = cap2;
			_table.Assert(table => table.ChildNodes.Count == 1 && table.Caption == cap2);
		}

		[Test]
		public void SetTFoot()
		{
			_table.TFoot = (HtmlTableSectionElement) _document.CreateElement("tfoot");
			_table.Assert(table => table.ChildNodes.Count == 1 && table.TFoot != null);
		}

		[Test]
		public void ReplaceTFoot()
		{
			_table.TFoot = (HtmlTableSectionElement) _document.CreateElement("tfoot");
			var tfoot2 = (HtmlTableSectionElement) _document.CreateElement("tfoot");
			_table.TFoot = tfoot2;
			_table.Assert(table => table.ChildNodes.Count == 1 && table.TFoot == tfoot2);
		}

		[Test]
		public void SetTHead()
		{
			_table.THead = (HtmlTableSectionElement) _document.CreateElement("thead");
			_table.Assert(table => table.ChildNodes.Count == 1 && table.THead != null);
		}

		[Test]
		public void ReplaceTHead()
		{
			_table.THead = (HtmlTableSectionElement) _document.CreateElement("thead");
			var thead2 = (HtmlTableSectionElement) _document.CreateElement("thead");
			_table.THead = thead2;
			_table.Assert(table => table.ChildNodes.Count == 1 && table.THead == thead2);
		}
	}
}
#endif