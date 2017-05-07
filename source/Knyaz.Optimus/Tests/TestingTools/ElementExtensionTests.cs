#if NUNIT
using System.Linq;
using Knyaz.Optimus.Dom;
using Knyaz.Optimus.TestingTools;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.TestingTools
{
	[TestFixture]
	public class ElementExtensionTests
	{
		[Test]
		public void Select()
		{
			var doc = new Document();
			doc.Write("<table><tbody><tr id=row_0><td></td></tr><tr id=row_1><td></td></tr></tbody></table>");

			var items = doc.Select("tr[id^='row_']").Select(x => x.Id).ToArray();

			CollectionAssert.AreEqual(new[] {"row_0","row_1"}, items);
		}
	}
}
#endif