using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace WebBrowser.ResourceProviders
{
	class HttpResourceProvider : IHttpResourceProvider
	{
		private readonly CookieContainer _cookies;

		public HttpResourceProvider(CookieContainer cookies)
		{
			_cookies = cookies;
		}

		public string Root { get; set; }
		public IRequest CreateRequest(string url)
		{
			return new HttpRequest("GET", url);
		}

		public IResource SendRequest(IRequest request)
		{
			var httpRequest = (HttpRequest) request;
			var webRequest = MakeWebRequest(httpRequest);
			var response = (HttpWebResponse) webRequest.GetResponse();
			return MakeResponse(response);
		}

		public async Task<IResource> SendRequestAsync(IRequest request)
		{
			var httpRequest = (HttpRequest)request;
			var webRequest = MakeWebRequest(httpRequest);
			var response = await webRequest.GetResponseAsync();
			return MakeResponse((HttpWebResponse)response);
		}

		private HttpResponse MakeResponse(HttpWebResponse response)
		{
			return new HttpResponse(response.StatusCode, response.GetResponseStream(), response.Headers, response.ContentType.ToLowerInvariant().Split(';')[0]);
		}

		private HttpWebRequest MakeWebRequest(HttpRequest request)
		{
			var uri = request.Url;
			var u = uri[0] == '/' ? new Uri(new Uri(Root), uri) : new Uri(uri);
			var result = WebRequest.CreateHttp(u);
			result.Timeout = request.Timeout == 0 ? Timeout.Infinite : request.Timeout;
			result.Method = request.Method;
			result.CookieContainer = _cookies;
			return result;
		}
	}

	public interface IHttpResourceProvider : ISpecResourceProvider
	{
		string Root { get; set; }
	}

	public class HttpRequest : IRequest
	{
		public string Method;
		public string Url { get; set; }
		public Dictionary<string, string> Headers;
		public int Timeout { get; set; }

		public byte[] Data;

		public HttpRequest(string method, string url)
		{
			Headers = new Dictionary<string, string>();
			Method = method;
			Url = url;
		}
	}

	public interface IRequest
	{
		string Url { get; }
	}

	public class HttpResponse : IResource
	{
		public HttpResponse(HttpStatusCode statusCode, Stream stream, WebHeaderCollection headers, string contentType)
		{
			StatusCode = statusCode;
			Stream = stream;
			Headers = headers;
			Type = contentType;
		}

		public HttpResponse(HttpStatusCode statusCode, Stream stream, WebHeaderCollection headers)
			:this(statusCode, stream, headers, ResourceTypes.Html) { }

		public HttpStatusCode StatusCode { get; private set; }
		public WebHeaderCollection Headers { get; private set; }
		public string Type { get; private set; }
		public Stream Stream { get; private set; }
	}
}
