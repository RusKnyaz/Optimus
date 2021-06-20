using System.Net;
using Knyaz.Optimus.Dom;

namespace Knyaz.Optimus
{
    public class Page
    {
        public HtmlDocument Document { get; }
        internal Page( HtmlDocument document) => Document = document;
    }

    public class HttpPage : Page
    {
        public HttpStatusCode HttpStatusCode { get; }

        internal HttpPage(HtmlDocument document, HttpStatusCode statusCode) : base(document) =>
            HttpStatusCode = statusCode;
    }
}