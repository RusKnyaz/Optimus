using System.Net;
using Knyaz.Optimus.Dom;

namespace Knyaz.Optimus
{
    public class Page
    {
        public Document Document { get; }
        internal Page( Document document) => Document = document;
    }

    public class HttpPage : Page
    {
        public HttpStatusCode HttpStatusCode { get; }

        internal HttpPage(Document document, HttpStatusCode statusCode) : base(document) =>
            HttpStatusCode = statusCode;
    }
}