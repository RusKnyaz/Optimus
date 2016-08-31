#if NUNIT
using Knyaz.Optimus.Dom.Css;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.Dom.Css
{
	[TestFixture]
	public class CssStyleDeclarationTests
	{
		[Test]
		public void SetOneCssTest()
		{
			var style = new CssStyleDeclaration {CssText = "background-color:green"};
			Assert.AreEqual("green", style.GetPropertyValue("background-color"));
		}

		[Test]
		public void AddRuleTest()
		{
			var style = new CssStyleSheet();
			style.InsertRule("div {width:100px}", 0);
			Assert.AreEqual(1, style.CssRules.Count);
			var rule = style.CssRules[0] as CssStyleRule;
			Assert.IsNotNull(rule);
			Assert.AreEqual("div", rule.SelectorText);
			Assert.AreEqual("100px", rule.Style.GetPropertyValue("width"));
		}

		private CssStyleDeclaration Style(string cssText)
		{
			return new CssStyleDeclaration() {CssText = cssText};
		}

		[Test, Ignore]
		public void SetBackground()
		{
			Style("background:#ffffff").Assert(style => 
				style.GetPropertyValue("background-color") == "#ffffff");
		}

		[Test, Ignore]
		public void SetBorderTest()
		{
			Style("border:1px solid white").Assert(style => 
				style.GetPropertyValue("border-top-width") == "1px" &&
				style.GetPropertyValue("border-top-style") == "solid" &&
				style.GetPropertyValue("border-top-color") == "white" &&
			
				style.GetPropertyValue("border-right-width") == "1px" &&
				style.GetPropertyValue("border-right-style") == "solid" &&
				style.GetPropertyValue("border-right-color") == "white" &&

				style.GetPropertyValue("border-bottom-width") == "1px" &&
				style.GetPropertyValue("border-bottom-style") == "solid" &&
				style.GetPropertyValue("border-bottom-color") == "white" &&

				style.GetPropertyValue("border-left-width") == "1px" &&
				style.GetPropertyValue("border-left-style") == "solid" &&
				style.GetPropertyValue("border-left-color") == "white");
		}

		[Test, Ignore]
		public void SetPadding()
		{
			Style("padding:1px 2px 3px 4px").Assert(style =>
				style.GetPropertyValue("padding-top") == "1px" &&
				style.GetPropertyValue("padding-right") == "2px" &&
				style.GetPropertyValue("padding-bottom") == "3px" &&
				style.GetPropertyValue("padding-left") == "4px");
		}

		[Test, Ignore]
		public void SetMargin()
		{
			Style("margin:1px 2px 3px 4px").Assert(style =>
				style.GetPropertyValue("margin-top") == "1px" &&
				style.GetPropertyValue("margin-right") == "2px" &&
				style.GetPropertyValue("margin-bottom") == "3px" &&
				style.GetPropertyValue("margin-left") == "4px");
		}

		[Test, Ignore]
		public void SetFont()
		{
			Style("font:12px Arial, sans-Serif;").Assert(style =>
				style.GetPropertyValue("font-size") == "12px" &&
				style.GetPropertyValue("font-family") == "Arial, sans-Serif;");
		}
	}
}
#endif