#if NUNIT
using NUnit.Framework;
using WebBrowser.Dom;
using WebBrowser.Dom.Elements;

namespace WebBrowser.Tests.Dom
{
	[TestFixture]
	public class HtmlElementTests
	{
		[Test]
		public void SetStyleFromAttribute()
		{
			var doc = new Document(null);
			var elem = doc.CreateElement("div") as HtmlElement;
			Assert.IsNotNull(elem);
			elem.SetAttribute("style","color:black");
			Assert.AreEqual("black", elem.Style.GetPropertyValue("color"));
		}
	}
}
#endif