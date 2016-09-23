using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Elements;
using NUnit.Framework;

#if NUNIT
namespace Knyaz.Optimus.Tests.Dom
{
	[TestFixture]
	public class HtmlTableCellElementTests
	{
		private Document _document;

		[SetUp]
		public void SetUp()
		{
			_document = new Document();
		}

		[Test]
		public void GetColSpanTest()
		{
			_document.Write("<table><tr><td id=a colspan=2></td></tr></table>");
			(_document.GetElementById("a") as HtmlTableCellElement)
				.Assert(td => td.ColSpan == 2);
		}
	}
}
#endif