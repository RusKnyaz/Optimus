#if NUNIT
using System.IO;
using System.Text;
using Knyaz.Optimus.Dom.Css;
using Knyaz.Optimus.Dom.Elements;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.Dom.Css
{
	[TestFixture]
	public class CssSelectorTests
	{
		private Engine Load(string html)
		{
			var engine = new Engine() { ComputedStylesEnabled = true };
			engine.Load(new MemoryStream(Encoding.UTF8.GetBytes(html)));
			return engine;
		}

		[TestCase("*", @"<body><div class=""pointsPanel""><h2><strong id=test></strong></h2></div></body>")]
		[TestCase(".pointsPanel strong", @"<body><div class=""pointsPanel""><strong id=test></strong></div></body>")]
		[TestCase(".pointsPanel strong", @"<body><div class=""pointsPanel""><h2><strong id=test></strong></h2></div></body>")]
		[TestCase(".pointsPanel h2 > strong", @"<body><div class=""pointsPanel""><h2><strong id=test></strong></h2></div></body>")]
		[TestCase(".pointsPanel *", @"<body><div class=""pointsPanel""><h2><strong id=test></strong></h2></div></body>")]
		public void MatchChildTest(string selectorText, string html)
		{
			var engine = Load(html);
			var elt = engine.Document.GetElementById("test") as IElement;
			var selector = new CssSelector(selectorText);
			Assert.IsTrue(selector.IsMatches(elt));
		}
	}
}
#endif