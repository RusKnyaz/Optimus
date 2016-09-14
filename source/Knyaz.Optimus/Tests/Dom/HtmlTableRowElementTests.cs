#if NUNIT
using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Elements;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.Dom
{
	[TestFixture]
	public class HtmlTableRowElementTests
	{
		private Document _document;
		private HtmlTableElement _table;

		[SetUp]
		public void SetUp()
		{
			_document = new Document();
			_table = (HtmlTableElement) _document.CreateElement(TagsNames.Table);
			_document.Body.AppendChild(_table);
		}

		[Test]
		public void RowIndex()
		{
			_table.InnerHTML = "<caption>a</caption><thead><tr></tr></thead><tbody><tr></tr><tr id=x></tr></tbody>";
			(_document.GetElementById("x") as HtmlTableRowElement).Assert(row => 
				row.RowIndex == 2 &&
				row.SectionRowIndex == 1);
		}
	}
}
#endif