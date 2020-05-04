using System.Linq;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.TestingTools;
using Knyaz.Optimus.Tests.TestingTools;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests
{
	[TestFixture]
	public class HtmlFormElementTests
	{
		[Test]
		public void NeighbourElements()
		{
			var resources = Mocks.ResourceProvider(
				"http://a.bc", "<html><body><form id=f></form><input form=f/></body></html>");
			
			var engine = TestingEngine.BuildJint(resources);
			engine.OpenUrl("http://a.bc").Wait();
			var form = engine.Document.Get<HtmlFormElement>("#f").FirstOrDefault();
			Assert.IsNotNull(form);
			Assert.AreEqual(1, form.Elements.Count);
		}
	}
}