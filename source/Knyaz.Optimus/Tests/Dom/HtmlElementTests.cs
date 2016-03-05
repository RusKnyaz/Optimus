#if NUNIT
using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Elements;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.Dom
{
	[TestFixture]
	public class HtmlElementTests
	{
		[Test]
		public void SetStyleFromAttribute()
		{
			var doc = new Document();
			var elem = doc.CreateElement("div") as HtmlElement;
			Assert.IsNotNull(elem);
			elem.SetAttribute("style","color:black");
			Assert.AreEqual("black", elem.Style.GetPropertyValue("color"));
		}
	}
}
#endif