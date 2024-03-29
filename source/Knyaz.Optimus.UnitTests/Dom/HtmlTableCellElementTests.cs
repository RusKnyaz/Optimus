﻿using Knyaz.NUnit.AssertExpressions;
using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Elements;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.Dom
{
	[TestFixture]
	public class HtmlTableCellElementTests
	{
		private HtmlDocument _document;

		[SetUp]
		public void SetUp()
		{
			_document = new HtmlDocument();
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