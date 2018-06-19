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

	    [Test]
	    public void CreateDocumentHtml()
	    {
		    new DomImplementation().CreateDocument("http://www.w3.org/1999/xhtml","html").Assert(doc => 
			    doc.DocumentElement.OuterHTML == "<html xmlns=\"http://www.w3.org/1999/xhtml\"></html>");
	    }
    }
}