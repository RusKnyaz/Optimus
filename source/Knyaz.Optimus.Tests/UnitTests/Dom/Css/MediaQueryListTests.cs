using Knyaz.Optimus.Dom.Css;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.Dom.Css
{
	[TestFixture]
	public class MediaQueryListTests
	{
		[TestCase("Screen", 1000, false, "Screen", true)]
		[TestCase("Screen", 1000, false, "Print", false)]
		[TestCase("Screen", 1000, false, "All", true)]
		[TestCase("Screen", 1000, false, "All and (orientation:portrait)", true)]
		[TestCase("Screen", 1000, false, "All and (orientation:landscape)", false)]
		[TestCase("Screen", 1000, false, "(min-width:200px)", true)]
		[TestCase("Screen", 1000, false, "(max-width:200px)", false)]
		[TestCase("Screen", 100, false, "(min-width:200px)", false)]
		[TestCase("Screen", 100, false, "(max-width:200px)", true)]
		public void DeviceMatches(string device, int width, bool landscape, string mediaQuery, bool matches)
		{
			var mql = new MediaQueryList(mediaQuery, () => new MediaSettings {
				Device = device, Width = width, Landscape = landscape
			});

			Assert.AreEqual(matches, mql.Matches);
		}
	}
}