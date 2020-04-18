using System.Linq;
using System.Threading.Tasks;
using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.TestingTools;
using Knyaz.Optimus.Tests.TestingTools;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.Dom.Css
{
	[TestFixture]
	public class ComputedStyleTests
	{
		private async Task<Document> Load(string html)
		{
			var engine = CreateEngine(html);
			return (await engine.OpenUrl("http://localhost")).Document;
		}

		private static Engine CreateEngine(string html)
		{
			var resources = Mocks.ResourceProvider("http://localhost", html);
			var engine = TestingEngine.BuildJintCss(resources);
			return engine;
		}

		[Test]
		public async Task EmptyDocumentContainsNoStyleSheets() =>
			(await Load("")).Assert(doc => doc.StyleSheets.Count == 0);

		[Test]
		public async Task ComputedStyleTest()
		{
			var engine = CreateEngine(
					"<head><style>div{display:inline-block}.a{width:100px;height:100px}</style></head><body><div class=a id=d></div></body>");
			var document = (await engine.OpenUrl("http://localhost")).Document;
			var lastStyleSheet = document.StyleSheets.Last();
			Assert.AreEqual(2, lastStyleSheet.CssRules.Count);
			var div = document.GetElementById("d");
			engine.Window.GetComputedStyle(div).Assert(style => style.GetPropertyValue("width") == "100px");
		}

		[Test]
		public async Task ComputedStyleIsAlwaysActual()
		{
			var doc = await Load("<head><style>div{color:red}</style><body><div id=d></div>");
			var div = doc.GetElementById("d");
			var divStyle = div.GetComputedStyle();
			Assert.AreEqual("red", divStyle.GetPropertyValue("color"));
			var newStyle = doc.CreateElement("style");
			newStyle.InnerHTML = "div{color:green}";
			doc.Head.AppendChild(newStyle);
			Assert.AreEqual("green", divStyle.GetPropertyValue("color"));
		}

		[Test]//Verified in Chrome
		public async Task SpecificHtml5TestCom()
		{
			var doc = await Load(@"<head><style>.pointsPanel h2 > strong {
			font-size: 3.8em;
		}</style></head><body><div class=""pointsPanel""><h2><strong id=test></strong></h2></div></body>");
			var elt = doc.GetElementById("test");
			elt.GetComputedStyle().Assert(style => style.GetPropertyValue("font-size") == "91.2px");
		}

		[Test]
		public async Task GetDisplayDefaultStyle()
		{
			var document = await Load("<body><div id=d><span id=s>ABC</span><span>123</span></div></body>");
			var spanDisplay = document.GetElementById("s").GetComputedStyle().GetPropertyValue("display");
			Assert.AreEqual("inline", spanDisplay);
			var divDislpay = document.GetElementById("d").GetComputedStyle().GetPropertyValue("display");
			Assert.AreEqual("block", divDislpay);
		}

		[Test]
		public async Task InheritTest()
		{
			var document = await Load("<body style='color:red'><div id=d style='color:inherit'></div></body>");
			var div = document.GetElementById("d");
			var color = div.GetComputedStyle().GetPropertyValue("color");
			Assert.AreEqual("red", color);
		}

		[Test]
		public async Task SetReset()
		{
			var document = await Load("<body><div id=d></div></body>");
			var div = document.GetElementById("d") as HtmlElement;
			var computedStyle = div.GetComputedStyle();
			Assert.AreEqual("block", computedStyle.GetPropertyValue("display"));
			div.Style.Display = "none";
			Assert.AreEqual("none", computedStyle.GetPropertyValue("display"));
			div.Style.Display = "";
			Assert.AreEqual("block", computedStyle.GetPropertyValue("display"));
		}

		[Test]
		public async Task SetResetViaIndexer()
		{
			var document = await Load("<body><div id=d></div></body>");
			var div = document.GetElementById("d") as HtmlElement;
			var computedStyle = div.GetComputedStyle();
			Assert.AreEqual("block", computedStyle.GetPropertyValue("display"));
			div.Style["display"] = "none";
			Assert.AreEqual("none", computedStyle.GetPropertyValue("display"));
			div.Style["display"] = "";
			Assert.AreEqual("block", computedStyle.GetPropertyValue("display"));
		}

		[TestCase("div{color:Red} div{color:Blue}", "Blue")]
		[TestCase("div{color:Red !important} div{color:Blue}", "Red")]
		[TestCase("div{color:Red !important} div{color:Blue !important}", "Blue")]
		public async Task  OverrrideProperty(string css, string expectedColor)
		{
			var document = await Load("<head><style>"+css+"</style></head><body><div id=d></div></body>");
			var div = document.GetElementById("d");
			Assert.AreEqual(expectedColor, div.GetComputedStyle().GetPropertyValue("color"));
		}

		[TestCase("<div id=d style='font-size:10px'><div style='font-size:200%'><div style='font-size:150%'></div></div></div>", "10px")]
		[TestCase("<div style='font-size:10px'><div id=d style='font-size:200%'><div style='font-size:150%'></div></div></div>", "20px")]
		[TestCase("<div style='font-size:10px'><div style='font-size:200%'><div id=d style='font-size:150%'></div></div></div>", "30px")]
		[TestCase("<div style='font-size:10px'><div style='font-size:200%'><div id=d></div></div></div>", "20px")]
		[TestCase("<div id=d style='font-size:10px'><div style='font-size:2em'><div id=d3 style='font-size:1.5em'></div></div></div>", "10px")]
		[TestCase("<div style='font-size:10px'><div id=d style='font-size:2em'><div style='font-size:1.5em'></div></div></div>", "20px")]
		[TestCase("<div style='font-size:10px'><div style='font-size:2em'><div id=d style='font-size:1.5em'></div></div></div>", "30px")]
		[TestCase("<div id=d></div>", "16px")]
		[TestCase("<style>html{font-size:1.1em}</style><div id=d></div>", "17.6px")]
		public async Task FontSize(string html, string expectedSize)
		{
			var document = await Load(html);

			Assert.AreEqual(expectedSize, document.GetElementById("d").GetComputedStyle().GetPropertyValue("font-size"));
		}

		[TestCase("<style>@media screen{div{color:red}}</style><div id=d></div>", "red")]
		public async Task Media(string html, string expectedColor)
		{
			var engine = CreateEngine(html);
			var document = (await engine.OpenUrl("http://localhost")).Document;
			var div = engine.Document.GetElementById("d");
			var style = engine.Window.GetComputedStyle(div);
			Assert.AreEqual(expectedColor, style.GetPropertyValue("color"));
		}

		[Test]
		public async Task EmSize()
		{
			var engine = CreateEngine("<style>div{font-size:12px; height:2em; border:0.5em}</style><div id=d></div>");
			var document = (await engine.OpenUrl("http://localhost")).Document;
			var div = document.GetElementById("d");
			engine.Window.GetComputedStyle(div).Assert(style => 
				style.GetPropertyValue("height") == "24px" &&
				style.GetPropertyValue("border-left-width") == "6px");
		}

		[Test]
		public async Task GetAutoMargin()
		{
			var engine = CreateEngine("<style>div{margin:0px auto}</style><div id=d></div>");
			var page = await engine.OpenUrl("http://localhost");
			var div = page.Document.GetElementById("d");
			engine.Window.GetComputedStyle(div).Assert(style =>
				style.GetPropertyValue("margin-left") == "auto" &&
				style.GetPropertyValue("margin-right") == "auto");
		}

		[Test]
		public async Task GetRelativeWidth()
		{
			var engine = CreateEngine("<div  style='padding:10px;margin:10px;border:10px solid red;width:100px;box-sizing:content-box'><div style='padding:10px;margin:10px;border:10px solid blue;width:100%;box-sizing:content-box' id=d></div></div>");
			var page = await engine.OpenUrl("http://localhost");
			var div = page.Document.GetElementById("d");
			engine.Window.GetComputedStyle(div).Assert(style => style.GetPropertyValue("width") == "100%");
		}

		[TestCase("<style>.button.save { width:50% }</style><div class='button save' id=d></div>")]
		[TestCase(@"<style>.page > .column .left,
.page > .column.right {	width: 50%;}</style><div class='page'><div class='column'><div class='left' id=d></div></div></div>")]
		public async Task ComplexSelector(string html)
		{
			var engine = CreateEngine(html);
			var page = await engine.OpenUrl("http://localhost");
			var div = page.Document.GetElementById("d");
			Assert.IsNotNull(div);
			engine.Window.GetComputedStyle(div).Assert(style => style.GetPropertyValue("width") == "50%");
		}

		[Test]
		public async Task ColorFromParent()
		{
			var engine = CreateEngine(@"<style>.resultsTable table thead tr {
	color: #fff;
	background: #0092bf;
}</style><div class='resultsTable'><table><thead><tr><td id=d></td></tr></thead></table></div>
");
			var page = await engine.OpenUrl("http://localhost");
			var div = page.Document.GetElementById("d");
			Assert.IsNotNull(div);
			engine.Window.GetComputedStyle(div).Assert(style => style.GetPropertyValue("color") == "#fff");
		}

		[TestCase("#d2 {color:green} * {color:red}", "green")]
		[TestCase("#d2 {color:green} div {color:red}", "green")]
		[TestCase("#d2 {color:green} #d2 {color:red}", "red")]
		[TestCase("#d1 div {color:green} div #d2 {color:red}", "red")]
		[TestCase("div #d2 {color:red} #d1 div {color:green} ", "green")]
		[TestCase("#d1 #d2 {color:red} #d1 div {color:green} ", "red")]

		[TestCase("#d2 {color:green}</style><style> * {color:red}", "green")]
		[TestCase(" * {color:red}</style><style>#d2 {color:green}", "green")]
		[TestCase("div{color:green} div {color:red}","red")]
		[TestCase("div,span{color:red} div{color:green}", "green")]
		public async Task Priority(string css, string expectedValue)
		{
			var document = await Load("<style>"+css+"</style><body><div id=d1 class=c1><div id=d2 class=c2></div></div></body>");
			var div = document.GetElementById("d2");
			Assert.IsNotNull(div);
			div.GetComputedStyle().Assert(style => style.GetPropertyValue("color") == expectedValue);
		}

		[Test]
		public async Task StyleIsHightPriority()
		{
			var document = await Load("<style>#d2{color:green}</style><body><div id=d1 class=c1><div id=d2 class=c2 style='color:red'></div></div></body>");
			var div = document.GetElementById("d2");
			Assert.IsNotNull(div);
			div.GetComputedStyle().Assert(style => style.GetPropertyValue("color") == "red");
		}

		[Test]
		public async Task SetCssTextForElementNotInDocument()
		{
			var document = await Load("");
			var div = (HtmlElement)document.CreateElement("div");
			div.Style.CssText = "background-color:rgba(1,1,1,.5)";
			Assert.AreEqual("rgba(1,1,1,.5)", div.Style.BackgroundColor);
		}
		
		[Test]
		public async Task SetCssTextForElementInDocument()
		{
			var document = await Load("<div id=d></div>");
			var div = (HtmlElement)document.GetElementById("d");
			div.Style.CssText = "background-color:rgba(1,1,1,.5)";
			Assert.AreEqual("rgba(1,1,1,.5)", div.Style.BackgroundColor);
		}
	}
}