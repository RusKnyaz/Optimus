#if NUNIT
using NUnit.Framework;
using WebBrowser.Dom;

namespace WebBrowser.Tests.Dom
{
	[TestFixture]
	class DocumentFragmentTests
	{
		[Test]
		public void CreateDocumentFragment()
		{
			var document = new Document(null);
			var x = document.CreateDocumentFragment();
			var el = document.CreateElement("div");
			x.AppendChild(el);
			Assert.AreEqual(document, el.OwnerDocument);
		}
	}
}
#endif