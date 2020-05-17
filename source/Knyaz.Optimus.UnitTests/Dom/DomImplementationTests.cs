using Knyaz.NUnit.AssertExpressions;
using Knyaz.Optimus.Dom;
using NUnit.Framework;

namespace Knyaz.Optimus.Tests.Dom
{
	[TestFixture]
    class DomImplementationTests
    {
		[Test]
		public void CreateHTMLDocument() => 
			DomImplementation.Instance.CreateHtmlDocument("NewDoc").Assert(doc =>
				doc.Title == "NewDoc" &&
				doc.DocType.Name == "html" &&
				doc.Body != null);

	    [Test]
	    public void CreateDocumentHtml() =>
		    DomImplementation.Instance.CreateDocument("http://www.w3.org/1999/xhtml","html").Assert(doc => doc.DocumentElement.TagName == "HTML");
	    
	    [Test]
	    public void CreateDocumentSvg() => 
		    DomImplementation.Instance.CreateDocument("http://www.w3.org/2000/svg","svg").Assert(doc => doc.DocumentElement.TagName == "SVG");
    }
}