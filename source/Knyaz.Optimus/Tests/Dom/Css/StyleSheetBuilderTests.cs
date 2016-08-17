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
        public void BuildStyelSheetTest()
        {
            StyleSheetBuilder.CreateStyleSheet(new StringReader("div{display:inline-block}.a{width:100px;height:100px}")).Assert(styleSheet =>
                styleSheet.CssRules.Count == 2 &&
                ((CssStyleRule)styleSheet.CssRules[0]).SelectorText == "div" &&
                ((CssStyleRule)styleSheet.CssRules[0]).Style.Length == 1 &&
                ((CssStyleRule)styleSheet.CssRules[1]).SelectorText == ".a" &&
                ((CssStyleRule)styleSheet.CssRules[1]).Style.Length == 2 &&
                ((CssStyleRule)styleSheet.CssRules[1]).Style.GetPropertyValue("height") == "100px");
        }
    }
}
#endif