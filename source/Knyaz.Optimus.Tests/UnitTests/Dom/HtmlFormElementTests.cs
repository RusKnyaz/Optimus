using System.Linq;
using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.TestingTools;
using Knyaz.Optimus.Tests.TestingTools;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.Dom
{
	[TestFixture]
	public class HtmlFormElementTests
	{
		private Document _document;
		private HtmlFormElement _form;

		[SetUp]
		public void SetUp()
		{
			_document = new Document();
			_form = (HtmlFormElement)_document.CreateElement("form");
		}

		[TestCase("GeT", "get")]
		[TestCase("get", "get")]
		[TestCase("", "get")]
		[TestCase("update", "get")]
		[TestCase("PoSt", "post")]
		public void MethodTest(string setValue, string getValue)
		{
			_form.Method = setValue;
			Assert.AreEqual(setValue, _form.GetAttribute("method"));
			Assert.AreEqual(getValue, _form.Method);
		}

		[Test]
		public void NeighbourElements()
		{
			var resources = Mocks.ResourceProvider(
				"http://a.bc", "<html><body><form id=f></form><input form=f/></body></html>");
			
			var engine = TestingEngine.BuildJint(resources);
			engine.OpenUrl("http://a.bc").Wait();
			engine.Document.Get<HtmlFormElement>("#f").FirstOrDefault()
				.Assert(form => form.Elements.Count == 1);
		}
	}
}