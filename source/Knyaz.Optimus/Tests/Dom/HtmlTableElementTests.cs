#if NUNIT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
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
		public void InsertRowTest()
		{
			var row = _table.InsertRow(0);

			_table.Assert(table => 
				table.Rows.Count == 1 &&
				table.TBodies.Count == 1 &&
				table.ChildNodes.Count == 1 &&
				((HtmlElement)table.ChildNodes[0]).TagName == "TBODY");
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
	}
}
#endif