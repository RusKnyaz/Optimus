using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.ScriptExecuting;
using System;

namespace Knyaz.Optimus.Dom
{
	/// <summary>
	/// Providing methods for documents creation.
	/// </summary>
	public class DomImplementation
	{
		[JsHidden]
		public static readonly DomImplementation Instance = new DomImplementation();
		
		internal DomImplementation()
		{
		}

		/// <summary>
		/// Creates a new HTML <see cref="Document"/>.
		/// </summary>
		/// <param name="title">Is a string containing the title to give the new HTML document.</param>
		[JsName("createHTMLDocument")]
		public Document CreateHtmlDocument(string title = "")
		{
			var docType = CreateDocumentType("html", string.Empty, string.Empty);
			var doc = CreateDocument("http://www.w3.org/1999/xhtml", "html", docType);
			doc.Title = title;
			return doc;
		}

		/// <summary>
		/// Creates a DocumentType object.
		/// </summary>
		/// <remarks>
		/// The DocumentType object can either be used with DOMImplementation.createDocument upon document creation or can be put into the document via methods like Node.insertBefore() or Node.replaceChild().
		/// </remarks>
		/// <param name="qualifiedNameStr">Is a string containing the qualified name, like svg:svg.</param>
		/// <param name="publicId">Is a string containing the PUBLIC identifier.</param>
		/// <param name="systemId">Is a string containing the SYSTEM identifiers.</param>
		public DocType CreateDocumentType(string qualifiedNameStr, string publicId, string systemId)
			=> new DocType(qualifiedNameStr, publicId, systemId);


		/// <summary>
		/// Creates and returns an <see cref="Document"/>.
		/// </summary>
		/// <param name="namespaceURI">Is a string containing the namespace URI of the document to be created, or null if the document doesn't belong to one.</param>
		/// <param name="qualifiedNameStr">Is a string containing the qualified name, that is an optional prefix and colon plus the local root element name, of the document to be created.</param>
		/// <param name="documentType">Is the DocumentType of the document to be created.</param>
		public Document CreateDocument(string namespaceURI, string qualifiedNameStr, DocType documentType = null)
		{
			if(qualifiedNameStr == null)
				throw new ArgumentNullException(nameof(qualifiedNameStr));
			
			/*The HTML namespace is: http://www.w3.org/1999/xhtml
			The MathML namespace is: http://www.w3.org/1998/Math/MathML
			The SVG namespace is: http://www.w3.org/2000/svg
			The XLink namespace is: http://www.w3.org/1999/xlink
			The XML namespace is: http://www.w3.org/XML/1998/namespace
			The XMLNS namespace is: http://www.w3.org/2000/xmlns/*/
			
			//todo: we have to do something with namespaceURI and qualifiedNameStr fields.
			return new Document(namespaceURI, qualifiedNameStr, documentType, null);
		}
	}
}
