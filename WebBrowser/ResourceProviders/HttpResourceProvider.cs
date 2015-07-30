using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
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

		public HttpResponse SendRequest(HttpRequest request)
		{
			var webRequest = MakeWebRequest(request);
			var response = (HttpWebResponse) webRequest.GetResponse();
			return MakeResponse(response);
		}

		public async Task<HttpResponse> SendRequestAsync(HttpRequest request)
		{
			var webRequest = MakeWebRequest(request);
			var response = await webRequest.GetResponseAsync();
			return MakeResponse((HttpWebResponse)response);
		}

		private HttpResponse MakeResponse(HttpWebResponse response)
		{
			return new HttpResponse(response.StatusCode, response.GetResponseStream(), response.Headers);
		}

		private HttpWebRequest MakeWebRequest(HttpRequest request)
		{
			var uri = request.Url;
			var u = uri[0] == '/' ? new Uri(new Uri(Root), uri) : new Uri(uri);
			var result = WebRequest.CreateHttp(u);
			result.Method = request.Method;
			result.CookieContainer = _cookies;
			return result;
		}
	}

	public interface IHttpResourceProvider
	{
		HttpResponse SendRequest(HttpRequest request);
		Task<HttpResponse> SendRequestAsync(HttpRequest request);
		string Root { get; set; }
	}

	public class HttpRequest : IRequest
	{
		public string Method;
		public string Url { get; set; }
		public Dictionary<string, string> Headers;
		//todo: body

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
		public HttpResponse(HttpStatusCode statusCode, Stream stream, WebHeaderCollection headers)
		{
			StatusCode = statusCode;
			Stream = stream;
			Headers = headers;
			Type= ResourceTypes.Html;//todo: fix it
		}

		public HttpStatusCode StatusCode { get; private set; }
		public WebHeaderCollection Headers { get; private set; }
		public ResourceTypes Type { get; private set; }
		public Stream Stream { get; private set; }
	}
}
