#if NUNIT
using Knyaz.Optimus.Dom.Css;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.Dom.Css
{
	[TestFixture]
	public class CssStyleDeclarationTests
	{
		[Test, Ignore]
		public void SetOneCssTest()
		{
			var style = new CssStyleDeclaration {CssText = "background-color:green"};
			Assert.AreEqual("green", style.GetPropertyValue("background-color"));
		}
	}
}
#endif