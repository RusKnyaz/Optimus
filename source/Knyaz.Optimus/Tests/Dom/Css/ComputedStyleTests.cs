#if NUNIT
using System.IO;
using System.Linq;
using System.Text;
using Knyaz.Optimus.TestingTools;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.Dom.Css
{
	[TestFixture]
	public class ComputedStyleTests
	{
		private Engine Load(string html)
		{
			var engine = new Engine() { ComputedStylesEnabled = true };
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
			elt.GetComputedStyle().Assert(style => style.GetPropertyValue("font-size") == "3.8em");
		}

		[Test]
		public void GetDisplayDefaultStyle()
		{
			var engine = Load("<body><div id=d><span id=s>ABC</span><span>123</span></div></body>");
			var spanDisplay = engine.Document.GetElementById("s").GetComputedStyle().GetPropertyValue("display");
			Assert.AreEqual(spanDisplay, "inline");
			var divDislpay = engine.Document.GetElementById("d").GetComputedStyle().GetPropertyValue("display");
			Assert.AreEqual(divDislpay, "block");
		}

		[Test]
		public void InheritTest()
		{
			var engine = Load("<body style='color:red'><div id=d style='color:inherit'></div></body>");
			var div = engine.Document.GetElementById("d");
			var color = div.GetComputedStyle().GetPropertyValue("color");
			Assert.AreEqual("red", color);
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

		[Test]
		public void EmFontSize()
		{
			var engine = Load("<div id=d1 style='font-size:10px'><div id=d2 style='font-size:2em'><div id=d3 style='font-size:1.5em'></div></div></div>");

			Assert.AreEqual("10px", engine.Document.GetElementById("d1").GetComputedStyle().GetPropertyValue("font-size"));
			Assert.AreEqual("20px", engine.Document.GetElementById("d2").GetComputedStyle().GetPropertyValue("font-size"));
			Assert.AreEqual("30px", engine.Document.GetElementById("d3").GetComputedStyle().GetPropertyValue("font-size"));
		}
	}
}
#endif