using System.IO;
using Knyaz.Optimus.Dom.Css.Expression;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.UnitTests.Dom.Css.Expressions
{
	[TestFixture]
	public static class CssExpressionParserTests
	{
		[TestCase(".a")]
		[TestCase("#m>div a,#m>div span")]
		[TestCase("#sizzle1>.blockUI")]
		public static void TestSelector(string css) => 
			Assert.AreEqual(css, CssExpressionParser.Parse(new StringReader(css)).ToString());
	}
}