#if NUNIT
using Knyaz.Optimus.Dom.Css;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.Dom.Css
{
	[TestFixture(Ignore = true, IgnoreReason = "Not implemented yet")]
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
	}
}
#endif