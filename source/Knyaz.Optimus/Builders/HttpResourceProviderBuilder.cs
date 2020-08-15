using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Knyaz.Optimus.ResourceProviders
{
    /// <summary>
    /// Allows to configure and build <see cref="HttpResourceProvider"/> object.
    /// </summary>
    public class HttpResourceProviderBuilder
    {
        private WebProxy _proxy;
        private AuthenticationHeaderValue _auth;
        private Action<HttpClientHandler> _handler;

        /// <summary>
        /// setup basic authorization login/password
        /// </summary>
        /// <returns></returns>
        public HttpResourceProviderBuilder Basic(string userName, string password)
        {
            _auth = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(
                System.Text.Encoding.ASCII.GetBytes($"{userName}:{password}")));
            return this;
        }
        
        public HttpResourceProviderBuilder Proxy(WebProxy proxy)
        {
            _proxy = proxy;
            return this;
        }

        internal HttpResourceProvider Build()
        {
            return new HttpResourceProvider(_auth, handler =>
            {
                if (_proxy != null)
                {
                    handler.Proxy = _proxy;
                    handler.UseProxy = true;
                }
                
                _handler?.Invoke(handler);
            });
        }

        public HttpResourceProviderBuilder ConfigureClientHandler(Action<HttpClientHandler> handler)
        {
            _handler = handler;
            return this;
        }
    }
}