#if NUNIT
using Knyaz.Optimus.Dom;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.Dom
{
	[TestFixture]
	class DocumentFragmentTests
	{
		[Test]
		public void CreateDocumentFragment()
		{
			var document = new Document();
			var x = document.CreateDocumentFragment();
			var el = document.CreateElement("div");
			x.AppendChild(el);
			Assert.AreEqual(document, el.OwnerDocument);
		}
	}
}
#endif