using System.Net;

namespace Knyaz.Optimus.ResourceProviders
{
    /// <summary>
    /// Allows to configure and build <see cref="HttpResourceProvider"/> object.
    /// </summary>
    public class HttpResourceProviderBuilder
    {
        private CookieContainer _cookieContainer;
        private WebProxy _proxy;


        public HttpResourceProviderBuilder Cookies(CookieContainer cookieContainer)
        {
            _cookieContainer = cookieContainer;
            return this;
        }

        public HttpResourceProviderBuilder Proxy(WebProxy proxy)
        {
            _proxy = proxy;
            return this;
        }

        internal HttpResourceProvider Build()
        {
            return new HttpResourceProvider(_cookieContainer ?? new CookieContainer(), _proxy);
        }
    }
}