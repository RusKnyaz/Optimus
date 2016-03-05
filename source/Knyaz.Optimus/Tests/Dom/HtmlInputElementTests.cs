#if NUNIT
using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Elements;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.Dom
{
	[TestFixture]
	public class HtmlInputElementTests
	{
		private Document _document;
		private HtmlInputElement _input;

		[SetUp]
		public void SetUp()
		{
			_document = new Document();
			_input = (HtmlInputElement)_document.CreateElement("input");
		}

		[Test]
		public void Defaults()
		{
			_input.Assert(input => 
				input.Type == "text" &&
				input.GetAttribute("type") == null 
				
				/* todo: implement default styles
				input.Style.Position == "absolute" &&
				input.Style.Display == "inline-block" &&
				input.Style.Top == "0px" &&
				input.Style.Left == "0px"*/);
		}
	}
}
#endif