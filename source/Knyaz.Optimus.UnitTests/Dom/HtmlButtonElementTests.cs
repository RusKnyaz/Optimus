using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Elements;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.Dom
{
	[TestFixture]
	public class HtmlButtonElementTests
	{
		private Document _document;
		private HtmlButtonElement _button;

		[SetUp]
		public void SetUp()
		{
			_document = new Document();
			_button = (HtmlButtonElement)_document.CreateElement("button");
		}


		[TestCase(null, "submit")]
		[TestCase("asd", "submit")]
		[TestCase("button", "button")]
		[TestCase("reset", "reset")]
		public void SetGetType(string setType, string getType)
		{
			_button.Type = setType;
			Assert.AreEqual(setType, _button.GetAttribute("type"));
			Assert.AreEqual(getType, _button.Type);
		}

		[Test]
		public void Defaults()
		{
			_button.Assert(b => b.Disabled == false);
		}

		[TestCase("<button id=b autofocus></button>", true)]
		[TestCase("<button id=b autofocus=1></button>", true)]
		[TestCase("<button id=b autofocus=true></button>", true)]
		[TestCase("<button id=b autofocus=false></button>", true)]
		[TestCase("<button id=b></button>", false)]
		public void Autofocus(string html, bool expectedAutofocus)
		{
			var doc = new Document();
			var container = doc.CreateElement("div");
			container.InnerHTML = html;
			((HtmlButtonElement)container.FirstChild)
				.Assert(button => 
					button.Autofocus == expectedAutofocus);
		}

		[Test]
		public void Labels()
		{
			var doc = new Document();
			doc.Write("<button id=b></button><label for=b>label1</label><label for=b>label2</label>");

			var button = (HtmlButtonElement)doc.GetElementById("b");
			Assert.AreEqual(2, button.Labels.Count);
		}
	}
}