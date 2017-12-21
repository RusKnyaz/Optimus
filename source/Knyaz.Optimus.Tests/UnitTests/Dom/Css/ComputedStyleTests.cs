#if NUNIT
using System.IO;
using System.Linq;
using System.Text;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.TestingTools;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.Dom.Css
{
	[TestFixture]
	public class ComputedStyleTests
	{
		private Engine Load(string html)
		{
			var engine = new Engine { ComputedStylesEnabled = true };
			engine.Load(new MemoryStream(Encoding.UTF8.GetBytes(html)));
			return engine;
		}

		[Test]
		public void ComputedStyleTest()
		{
			var engine =
				Load(
					"<head><style>div{display:inline-block}.a{width:100px;height:100px}</style></head><body><div class=a id=d></div></body>");
			var lastStyleSheet = engine.Document.StyleSheets.Last();
			Assert.AreEqual(2, lastStyleSheet.CssRules.Count);
			var div = engine.Document.GetElementById("d");
			engine.Window.GetComputedStyle(div).Assert(style => style.GetPropertyValue("width") == "100px");
		}

		[Test]
		public void ComputedStyleIsAlwaysActual()
		{
			var engine = Load("<head><style>div{color:red}</style><body><div id=d></div>");
			var doc = engine.Document;
			var div = doc.GetElementById("d");
			var divStyle = div.GetComputedStyle();
			Assert.AreEqual("red", divStyle.GetPropertyValue("color"));
			var newStyle = doc.CreateElement("style");
			newStyle.InnerHTML = "div{color:green}";
			doc.Head.AppendChild(newStyle);
			Assert.AreEqual("green", divStyle.GetPropertyValue("color"));
		}

		[Test]//Verified in Chrome
		public void SpecificHtml5TestCom()
		{
			var engine = Load(@"<head><style>.pointsPanel h2 > strong {
			font-size: 3.8em;
		}</style></head><body><div class=""pointsPanel""><h2><strong id=test></strong></h2></div></body>");
			var doc = engine.Document;
			var elt = doc.GetElementById("test");
			elt.GetComputedStyle().Assert(style => style.GetPropertyValue("font-size") == "91.2px");
		}

		[Test]
		public void GetDisplayDefaultStyle()
		{
			var engine = Load("<body><div id=d><span id=s>ABC</span><span>123</span></div></body>");
			var spanDisplay = engine.Document.GetElementById("s").GetComputedStyle().GetPropertyValue("display");
			Assert.AreEqual("inline", spanDisplay);
			var divDislpay = engine.Document.GetElementById("d").GetComputedStyle().GetPropertyValue("display");
			Assert.AreEqual("block", divDislpay);
		}

		[Test]
		public void InheritTest()
		{
			var engine = Load("<body style='color:red'><div id=d style='color:inherit'></div></body>");
			var div = engine.Document.GetElementById("d");
			var color = div.GetComputedStyle().GetPropertyValue("color");
			Assert.AreEqual("red", color);
		}

		[Test]
		public void SetReset()
		{
			var engine = Load("<body><div id=d></div></body>");
			var div = engine.Document.GetElementById("d") as HtmlElement;
			var computedStyle = div.GetComputedStyle();
			Assert.AreEqual("block", computedStyle.GetPropertyValue("display"));
			div.Style.Display = "none";
			Assert.AreEqual("none", computedStyle.GetPropertyValue("display"));
			div.Style.Display = "";
			Assert.AreEqual("block", computedStyle.GetPropertyValue("display"));
		}

		[Test]
		public void SetResetViaIndexer()
		{
			var engine = Load("<body><div id=d></div></body>");
			var div = engine.Document.GetElementById("d") as HtmlElement;
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
		public void OverrrideProperty(string css, string expectedColor)
		{
			var engine = Load("<head><style>"+css+"</style></head><body><div id=d></div></body>");
			var div = engine.Document.GetElementById("d");
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
		public void FontSize(string html, string expectedSize)
		{
			var engine = Load(html);

			Assert.AreEqual(expectedSize, engine.Document.GetElementById("d").GetComputedStyle().GetPropertyValue("font-size"));
		}

		[TestCase("<style>@media screen{div{color:red}}</style><div id=d></div>", "red")]
		public void Media(string html, string expectedColor)
		{
			var engine = Load(html);
			var div = engine.Document.GetElementById("d");
			var style = engine.Window.GetComputedStyle(div);
			Assert.AreEqual(expectedColor, style.GetPropertyValue("color"));
		}

		[Test]
		public void EmSize()
		{
			var engine = Load("<style>div{font-size:12px; height:2em; border:0.5em}</style><div id=d></div>");
			var div = engine.Document.GetElementById("d");
			engine.Window.GetComputedStyle(div).Assert(style => 
				style.GetPropertyValue("height") == "24px" &&
				style.GetPropertyValue("border-left-width") == "6px");
		}

		[Test]
		public void GetAutoMargin()
		{
			var engine = Load("<style>div{margin:0px auto}</style><div id=d></div>");
			var div = engine.Document.GetElementById("d");
			engine.Window.GetComputedStyle(div).Assert(style =>
				style.GetPropertyValue("margin-left") == "auto" &&
				style.GetPropertyValue("margin-right") == "auto");
		}

		[Test]
		public void GetRelativeWidth()
		{
			var engine = Load("<div  style='padding:10px;margin:10px;border:10px solid red;width:100px;box-sizing:content-box'><div style='padding:10px;margin:10px;border:10px solid blue;width:100%;box-sizing:content-box' id=d></div></div>");
			var div = engine.Document.GetElementById("d");
			engine.Window.GetComputedStyle(div).Assert(style => style.GetPropertyValue("width") == "100%");
		}

		[TestCase("<style>.button.save { width:50% }</style><div class='button save' id=d></div>")]
		[TestCase(@"<style>.page > .column .left,
.page > .column.right {	width: 50%;}</style><div class='page'><div class='column'><div class='left' id=d></div></div></div>")]
		public void ComplexSelector(string html)
		{
			var engine = Load(html);
			var div = engine.Document.GetElementById("d");
			Assert.IsNotNull(div);
			engine.Window.GetComputedStyle(div).Assert(style => style.GetPropertyValue("width") == "50%");
		}

		[Test]
		public void ColorFromParent()
		{
			var engine = Load(@"<style>.resultsTable table thead tr {
	color: #fff;
	background: #0092bf;
}</style><div class='resultsTable'><table><thead><tr><td id=d></td></tr></thead></table></div>
");
			var div = engine.Document.GetElementById("d");
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
		public void Priority(string css, string expectedValue)
		{
			var engine = Load("<style>"+css+"</style><body><div id=d1 class=c1><div id=d2 class=c2></div></div></body>");
			var div = engine.Document.GetElementById("d2");
			Assert.IsNotNull(div);
			div.GetComputedStyle().Assert(style => style.GetPropertyValue("color") == expectedValue);
		}

		[Test]
		public void StyleIsHightPriority()
		{
			var engine = Load("<style>#d2{color:green}</style><body><div id=d1 class=c1><div id=d2 class=c2 style='color:red'></div></div></body>");
			var div = engine.Document.GetElementById("d2");
			Assert.IsNotNull(div);
			div.GetComputedStyle().Assert(style => style.GetPropertyValue("color") == "red");
		}
	}
}
#endif