#if NUNIT
using System.Linq;
using NUnit.Framework;
using WebBrowser.Dom;
using WebBrowser.Dom.Elements;

namespace WebBrowser.Tests.Dom
{
	[TestFixture]
	public class NodeTests
	{
		[Test]
		public void CloneNodeAttributeWithQuotes()
		{
			var elem = (Element)DocumentBuilder.Build("<div data-bind='template:\"itemTemplate\"'></div>").Single();
			var clone = (Element)elem.CloneNode();
			Assert.AreEqual("template:\"itemTemplate\"", elem.GetAttribute("data-bind"));
			Assert.AreEqual("template:\"itemTemplate\"", clone.GetAttribute("data-bind"));
		}
	}
}
#endif