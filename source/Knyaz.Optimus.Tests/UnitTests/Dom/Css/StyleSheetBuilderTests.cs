using System.IO;
using System.Threading;
using Knyaz.Optimus.Dom.Css;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.Dom.Css
{
    [TestFixture]
    public class StyleSheetBuilderTests
    {
        [Test]
        public void BuildStyleSheetTest()
        {
            Build("div{display:inline-block}.a{width:100px;height:100px}").Assert(styleSheet =>
                styleSheet.CssRules.Count == 2 &&
                ((CssStyleRule)styleSheet.CssRules[0]).SelectorText == "div" &&
                ((CssStyleRule)styleSheet.CssRules[0]).Style.Length == 1 &&
                ((CssStyleRule)styleSheet.CssRules[1]).SelectorText == ".a" &&
                ((CssStyleRule)styleSheet.CssRules[1]).Style.Length == 2 &&
                ((CssStyleRule)styleSheet.CssRules[1]).Style.GetPropertyValue("height") == "100px");
        }

	    [TestCase("@import \"a.css\"; div{background-color:red}")]
	    [TestCase("@import url(a.css); div{background-color:red}")]
	    [TestCase("@import url(\"a.css\"); div{background-color:red}")]
	    [TestCase("@import url('a.css'); div{background-color:red}")]
	    public void ImportTest(string css)
	    {
		    var ss = StyleSheetBuilder.CreateStyleSheet(
			    new StringReader(css), s =>
				    s == "a.css" ? new StringReader("div{color:green}") : null);

			ss.Assert(styleSheet => 
				styleSheet.CssRules.Count == 2 &&
				((CssStyleRule)styleSheet.CssRules[0]).SelectorText == "div" &&
				((CssStyleRule)styleSheet.CssRules[0]).Style.Length == 1 &&
				((CssStyleRule)styleSheet.CssRules[0]).Style.GetPropertyValue("color") == "green" &&
				((CssStyleRule)styleSheet.CssRules[1]).SelectorText == "div" &&
				((CssStyleRule)styleSheet.CssRules[1]).Style.Length == 1 &&
				((CssStyleRule)styleSheet.CssRules[1]).Style.GetPropertyValue("background-color") == "red");
	    }

	    

	    private CssStyleSheet Build(string css)
	    {
		    return StyleSheetBuilder.CreateStyleSheet(new StringReader(css), s => null);
	    }

	    [Test]
	    public void MediaRule()
	    {
		    Build("@media all and (min-width:100px) { div {color:red} span {color:green }} a {color:green}")
				.Assert(s => s.CssRules.Count == 2 &&
				((CssMediaRule)s.CssRules[0]).CssRules.Count == 2 &&
				((CssStyleRule)s.CssRules[1]).SelectorText == "a");
	    }

	    [Test]
	    public void MediaRuleAfterStyle()
	    {
			Build("a {color:green} @media all and (min-width:100px) { div {color:red} span {color:green }}")
				.Assert(s => s.CssRules.Count == 2 &&
				((CssMediaRule)s.CssRules[1]).CssRules.Count == 2 &&
				((CssStyleRule)s.CssRules[0]).SelectorText == "a");
		}

	    [TestCase("@namespace url(http://www.w3.org/1999/xhtml); div {color:green}")]
		[TestCase("@font-face {} div {color:green}")]
		public void Smoke(string css)
	    {
		    var ss = Build(css);
	    }

	    [TestCase("body{background-color:red}")]
	    [TestCase("body{background-color:red !important}")]
	    [TestCase("@media all and (min-width:100px) {div{color:red} span{color:green}}")]
	    public void CssText(string css) => Build(css).Assert(x => 
		    x.CssRules.Count == 1 &&
		    x.CssRules[0].CssText == css);

	    [Test]
	    public void SetCssTextHaveNoEffect()
	    {
		    var styleSheet = Build("body{background-color:red}");
		    styleSheet.CssRules[0].CssText = "body{background-color:green}";
		    Assert.AreEqual("body{background-color:red}", styleSheet.CssRules[0].CssText);
	    }

	    [Test, MaxTime(30000)]
	    public void EmptyRule()
	    {
		    var ss = Build(@"a {}");
		    Assert.AreEqual(1, ss.CssRules.Count);
		    Assert.AreEqual(0, ((CssStyleRule)ss.CssRules[0]).Style.Length);
	    }

	    [Test, MaxTime(30000)]
	    public void EmptyRuleFollowedBy()
	    {
		    var ss = Build(@"a {} b{c:1}");
		    Assert.AreEqual(2, ss.CssRules.Count);
		    Assert.AreEqual(0, ((CssStyleRule)ss.CssRules[0]).Style.Length);
		    Assert.AreEqual(1, ((CssStyleRule)ss.CssRules[1]).Style.Length);
	    }
    }
}