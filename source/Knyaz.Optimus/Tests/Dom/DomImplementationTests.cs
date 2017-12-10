#if NUNIT
using Knyaz.Optimus.Dom;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.Dom
{
	[TestFixture]
    class DomImplementationTests
    {
		[Test]
		public void CreateHTMLDocument() => 
			new DomImplementation().CreateHtmlDocument("NewDoc").Assert(doc =>
				doc.Title == "NewDoc" &&
				doc.DocType.Name == "html" &&
				doc.Body != null);
    }
}
#endif