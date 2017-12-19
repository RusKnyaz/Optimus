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

		[Test]
		public void CloneDocumentFragment()
		{
			var document = new Document();
			var docFrag = document.CreateDocumentFragment();
			var el = document.CreateElement("div");
			docFrag.AppendChild(el);
			var clone = docFrag.CloneNode(true);

			Assert.AreEqual(1, clone.ChildNodes.Count);
		}
	}
}
#endif