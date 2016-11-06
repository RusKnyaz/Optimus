#if NUNIT
using System.IO;
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

	    [Test]
	    public void ImportTest()
	    {
		    var ss = StyleSheetBuilder.CreateStyleSheet(
			    new StringReader("@import \"a.css\"; div{background-color:red}"), s =>
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
    }
}
#endif