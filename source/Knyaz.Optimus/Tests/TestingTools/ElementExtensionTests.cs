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
		[TestCase("<table><tbody><tr id=row_0><td></td></tr><tr id=row_1><td></td></tr></tbody></table>", "tr[id^='row_']", "row_0,row_1")]
		[TestCase("<table><tbody><tr id=row_0><td></td></tr><tr id=row_1><td></td></tr></tbody></table>", "tr[id^=row_]", "row_0,row_1")]
		[TestCase("<tr id=row_0><td column-name=Name id=c1></td><td  columne-name=Id id=c2></td></tr>", "[column-name='Name']", "c1")]
		[TestCase("<tr id=row_0><td column-name=Name id=c1></td><td  columne-name=Id id=c2></td></tr>", "[column-name=Name]", "c1")]
		public void Select(string html, string selector, string expectedIds)
		{
			var doc = new Document();
			doc.Write(html);

			var items = doc.Select(selector).Select(x => x.Id).ToArray();

			CollectionAssert.AreEqual(expectedIds.Split(','), items);
		}
	}
}
#endif