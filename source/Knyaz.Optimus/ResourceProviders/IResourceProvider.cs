using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Knyaz.Optimus.ResourceProviders
{
    public class Request
    {
        public string Method;
        public Uri Url { get; }
        public readonly Dictionary<string, string> Headers;
        public int Timeout { get; set; }
        public byte[] Data;
        public CookieContainer Cookies;

        public Request(Uri url) => Url = url;

        public Request(string method, Uri url) : this(url)
        {
            Headers = new Dictionary<string, string>();
            Method = method;
        }

        public override int GetHashCode() => 
            ((Url?.ToString() ?? "<null>") + "()" + (Method ?? "<null>")).GetHashCode() ^ (Headers == null ? 0 : Headers.Count);

        public override bool Equals(object obj) => 
            obj is Request other 
            && Url == other.Url 
            && Method == other.Method 
            && ((Headers == null && other.Headers == null)
            || (Headers.Count == other.Headers.Count 
            && Headers.Keys.All(k => other.Headers.ContainsKey(k) && Headers[k].Equals(other.Headers[k]))));
    }

    
    /// <summary>
    /// Allows to get resources like files, html pages and etc (dependes on implementation).
    /// </summary>
    public interface IResourceProvider
    {
        /// <summary>
        /// Requests resource.
        /// </summary>
        Task<IResource> SendRequestAsync(Request request);
    }
}