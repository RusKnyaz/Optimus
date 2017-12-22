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

		[Test]
		public void SetBackground()
		{
			Style("background:#ffffff").Assert(style => 
				style.GetPropertyValue("background-color") == "#ffffff");
		}

		[Test]
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

		[TestCase("1px", "1px", "1px", "1px", "1px")]
		[TestCase("1px 2px", "1px", "2px", "1px", "2px")]
		[TestCase("1px 2px 3px", "1px", "2px", "3px", "2px")]
		[TestCase("1px 2px 3px 4px", "1px", "2px", "3px", "4px")]
		public void SetPadding(string padding, string top, string right, string bottom ,string left)
		{
			Style("padding:" + padding).Assert(style =>
				style.GetPropertyValue("padding-top") == top &&
				style.GetPropertyValue("padding-right") == right &&
				style.GetPropertyValue("padding-bottom") == bottom &&
				style.GetPropertyValue("padding-left") == left);
		}

		[TestCase("1px", "1px", "1px", "1px", "1px")]
		[TestCase("1px 2px", "1px", "2px", "1px", "2px")]
		[TestCase("1px 2px 3px", "1px", "2px", "3px", "2px")]
		[TestCase("1px 2px 3px 4px", "1px", "2px", "3px", "4px")]
		public void SetMargin(string margin, string top, string right, string bottom, string left)
		{
			Style("margin:" + margin).Assert(style =>
				style.GetPropertyValue("margin-top") == top &&
				style.GetPropertyValue("margin-right") == right &&
				style.GetPropertyValue("margin-bottom") == bottom &&
				style.GetPropertyValue("margin-left") == left);
		}

		[TestCase("italic bold 12px/30px Georgia, serif", "italic", "normal", "bold", "12px", "Georgia,serif")]
		[TestCase("italic   bold 12px/30px Georgia, serif", "italic", "normal", "bold", "12px", "Georgia,serif")]
		[TestCase("12px Arial, sans-Serif", "normal", "normal", "normal", "12px", "Arial,sans-Serif")]
		[TestCase("normal small-caps 12px/14px fantasy", "normal", "small-caps", "normal", "12px", "fantasy")]
		public void SetFont(string font, string fontStyle, string fontVariant, string fontWeight, string fontSize, string fontFamily)
		{
			Style("font:"+ font).Assert(style =>
				style.GetPropertyValue("font-size") == fontSize &&
				style.GetPropertyValue("font-family") == fontFamily &&
				style.GetPropertyValue("font-weight") == fontWeight &&
				style.GetPropertyValue("font-style") == fontStyle &&
				style.GetPropertyValue("font-variant") == fontVariant);
		}

		[Test]
		public void SetFontLineHeight()
		{
			Style("font:12px/30px Arial").Assert(style =>
				style.GetPropertyValue("font-size") == "12px" &&
				style.GetPropertyValue("line-height") == "30px");
		}

		[Test]
		public void ValueWithSpaces()
		{
			Style("border-top: rgb(255, 255, 255) solid 1px").Assert(style => 
				style.GetPropertyValue("border-top-color") == "rgb(255, 255, 255)" &&
				style.GetPropertyValue("border-top-width") == "1px" &&
				style.GetPropertyValue("border-top-style") == "solid");
		}
	}
}