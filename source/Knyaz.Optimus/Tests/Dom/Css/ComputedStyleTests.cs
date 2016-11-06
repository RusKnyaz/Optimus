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

		[Test]
		public void SpecificHtml5TestCom()
		{
			var engine = Load(@"<head><style>.pointsPanel h2 > strong {
			font - size: 3.8em;
		}</style></head><body><div class=""pointsPanel""><h2><strong id=test></strong></h2></div></body>");
			var doc = engine.Document;
			var elt = doc.GetElementById("test");
			elt.GetComputedStyle().Assert(style => style.GetPropertyValue("font-size") == "60.8px");
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
	}
}
#endif