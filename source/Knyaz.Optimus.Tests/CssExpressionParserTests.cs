using System.IO;
using Knyaz.Optimus.Dom.Css.Expression;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.Dom.Css
{
	[TestFixture]
	public static class CssExpressionParserTests
	{
		[TestCase(".a")]
		[TestCase("#m>div a,#m>div span")]
		public static void TestSelector(string css) => 
			Assert.AreEqual(css, CssExpressionParser.Parse(new StringReader(css)).ToString());
	}
}