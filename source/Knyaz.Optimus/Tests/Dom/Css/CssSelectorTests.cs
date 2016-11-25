#if NUNIT
using System.IO;
using System.Text;
using Knyaz.Optimus.Dom.Css;
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

		[TestCase("*", @"<body><div class=""pointsPanel""><h2><strong name=match></strong></h2></div></body>")]
		[TestCase(".pointsPanel strong", @"<body><div class=""pointsPanel""><strong name=match></strong></div></body>")]
		[TestCase(".pointsPanel strong", @"<body><div class=""pointsPanel""><h2><strong name=match></strong></h2></div></body>")]
		[TestCase(".pointsPanel h2 > strong", @"<body><div class=""pointsPanel""><h2><strong name=match></strong></h2></div></body>")]
		[TestCase(".pointsPanel *", @"<body><div class=""pointsPanel""><h2><strong name=match></strong></h2></div></body>")]
		[TestCase("div *", "<body name=notmatch><div name=match><strong name=match><h2 name=match>a</h2></strong></div></body>")]
		[TestCase(".button.save", "<body><div class='button save' name=match></div></body>")]
		[TestCase(".button.save", "<body><div class='button' name=nomatch></div></body>")]
		[TestCase(".button.save", "<body><div class='save' name=nomatch></div></body>")]
		[TestCase(".button .save", "<body><div class='button save' name=notmatch></div></body>")]
		[TestCase(".resultsTable table thead tr","<div class='resultsTable'><table><thead><tr name=match><td name=nomatch></td></tr></thead></table></div>")]
		public void MatchChildTest(string selectorText, string html)
		{
			var engine = Load(html);
			var selector = new CssSelector(selectorText);
			var matchElts = engine.Document.GetElementsByName("match");
			foreach (var matchElt in matchElts)
			{
				Assert.IsTrue(selector.IsMatches(matchElt));
			}


			var notMatchElt = engine.Document.GetElementsByName("notmatch");
			foreach (var elt in notMatchElt)
			{
				Assert.IsFalse(selector.IsMatches(elt), elt.ToString());
			}
		}
	}
}
#endif