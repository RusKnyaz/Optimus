using System.Linq;
using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.TestingTools;
using Knyaz.Optimus.Tests.TestingTools;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests
{
	[TestFixture]
	public class HtmlFormElementTests
	{
		[Test]
		public void NeighbourElements()
		{
			var resources = Mocks.ResourceProvider(
				"http://a.bc", "<html><body><form id=f></form><input form=f/></body></html>");
			
			var engine = TestingEngine.BuildJint(resources);
			engine.OpenUrl("http://a.bc").Wait();
			var form = engine.Document.Get<HtmlFormElement>("#f").FirstOrDefault();
			Assert.IsNotNull(form);
			Assert.AreEqual(1, form.Elements.Count);
		}

		[Test]
		public void ElementsIsLiveCollection()
		{
			var document = DomImplementation.Instance.CreateHtmlDocument();
			var form = (HtmlFormElement)document.CreateElement("form");
			var elements = form.Elements; 
			Assume.That(elements.Count, Is.EqualTo(0));
			form.AppendChild(document.CreateElement("input"));
			Assert.AreEqual(1, elements.Count);
		}

		[TestCase("", ExpectedResult = "http://a.bc/Account/Login/")]
		[TestCase("Hoba", ExpectedResult = "http://a.bc/Account/Login/Hoba")]
		[TestCase("/hoba", ExpectedResult = "http://a.bc/hoba")]
		[TestCase("http://q.we", ExpectedResult = "http://q.we/")]
		public string GetAction(string attrValue)
		{
			var engine = TestingEngine.Build("http://a.bc/Account/Login/", $"<form id=f action=\"{attrValue}\"></form>");
			engine.OpenUrl("http://a.bc/Account/Login/").Wait();
			var form = (HtmlFormElement)engine.Document.GetElementById("f");
			return form.Action;
		}
		
		[TestCase("", ExpectedResult = "http://a.bc/Account/Login/")]
		[TestCase("Hoba", ExpectedResult = "http://a.bc/Account/Login/Hoba")]
		[TestCase("/hoba", ExpectedResult = "http://a.bc/hoba")]
		[TestCase("http://q.we", ExpectedResult = "http://q.we/")]
		public string SetGetAction(string actionValue)
		{
			var engine = TestingEngine.Build("http://a.bc/Account/Login/", "<form id=f></form>");
			engine.OpenUrl("http://a.bc/Account/Login/").Wait();
			var form = (HtmlFormElement)engine.Document.GetElementById("f");
			form.Action = actionValue;
			Assert.AreEqual(actionValue, form.GetAttribute("action"));
			return form.Action;
		}

		[TestCase("", ExpectedResult = "application/x-www-form-urlencoded")]
		[TestCase("application/x-www-form-urlencoded", ExpectedResult = "application/x-www-form-urlencoded")]
		[TestCase("invalid", ExpectedResult = "application/x-www-form-urlencoded")]
		[TestCase("text/plain", ExpectedResult = "text/plain")]
		[TestCase("TEXT/plain", ExpectedResult = "text/plain")]
		[TestCase("multipart/form-data", ExpectedResult = "multipart/form-data")]
		public string EncTypeFromAttr(string encType)
		{
			var engine = TestingEngine.Build("http://a.bc/Account/Login/", $"<form id=f enctype=\"{encType}\"></form>");
			engine.OpenUrl("http://a.bc/Account/Login/").Wait();
			var form = (HtmlFormElement)engine.Document.GetElementById("f");
			return form.Enctype;
		}
		
		[TestCase("", ExpectedResult = "application/x-www-form-urlencoded")]
		[TestCase("invalid", ExpectedResult = "application/x-www-form-urlencoded")]
		[TestCase("text/plain", ExpectedResult = "text/plain")]
		[TestCase("TEXT/plain", ExpectedResult = "text/plain")]
		[TestCase("multipart/form-data", ExpectedResult = "multipart/form-data")]
		public string SetGetEncType(string encType)
		{
			var engine = TestingEngine.Build("http://a.bc/Account/Login/", "<form id=f></form>");
			engine.OpenUrl("http://a.bc/Account/Login/").Wait();
			var form = (HtmlFormElement)engine.Document.GetElementById("f");
			form.Enctype = encType;
			Assert.AreEqual(encType, form.GetAttribute("enctype"));
			return form.Enctype;
		}
	}
}
