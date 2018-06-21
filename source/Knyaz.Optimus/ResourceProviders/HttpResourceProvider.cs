using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Knyaz.Optimus.ResourceProviders
{
	/// <summary>
	/// Allows to request web resources. 
	/// </summary>
	class HttpResourceProvider : IResourceProvider
	{
		private readonly Func<HttpRequest, HttpClient> _getClientFn;

		public readonly CookieContainer CookieContainer;

		public HttpResourceProvider(CookieContainer cookies, 
			WebProxy proxy,
			AuthenticationHeaderValue auth)
		{
			CookieContainer = cookies;
			
			_getClientFn = req => new HttpClient(new HttpClientHandler
			{
				CookieContainer = cookies,
				Proxy = proxy,
				UseProxy = proxy != null,
			})
			{
				Timeout = req.Timeout > 0
				? TimeSpan.FromMilliseconds(req.Timeout)
				: Timeout.InfiniteTimeSpan,
				DefaultRequestHeaders = {Authorization = auth}
			};
		}

		public IRequest CreateRequest(Uri url) => new HttpRequest("GET", url);

		private async Task<IResource> SendRequestEx(IRequest request)
		{
			var httpRequest = request as HttpRequest;

			var req = MakeWebRequest(httpRequest);
			
			using (var client = _getClientFn(httpRequest))
			using (var response = await client.SendAsync(req))
			using (var content = response.Content)
			{
				var result = await content.ReadAsByteArrayAsync();
				return new HttpResponse(
					response.StatusCode,
					new MemoryStream(result),
					content.Headers.ToString(),
					content.Headers.ContentType?.ToString(),
					response.RequestMessage.RequestUri);
			}
		}
		
		public Task<IResource> SendRequestAsync(IRequest request) => SendRequestEx(request);

		private HttpRequestMessage MakeWebRequest(HttpRequest request)
		{
			var u = request.Url;
			var resultRequest = new HttpRequestMessage(new HttpMethod(request.Method.ToUpperInvariant()), u);

			if (request.Data != null && resultRequest.Method.Method != "GET")
			{
				resultRequest.Content = new StreamContent(new MemoryStream(request.Data));
			}
			
			if(request.Headers != null)
			foreach (var keyValue in request.Headers)
			{
				switch (keyValue.Key)
				{
					case "Content-Type":
						resultRequest.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(keyValue.Value);
						break;
					default:
						resultRequest.Headers.Add(keyValue.Key, keyValue.Value);
						break;
				}
			}

			return resultRequest;
		}
	}

	public class HttpRequest : IRequest
	{
		public string Method;
		public Uri Url { get; }
		public readonly Dictionary<string, string> Headers;
		public int Timeout { get; set; }
		public byte[] Data;

		public HttpRequest(string method, Uri url)
		{
			Headers = new Dictionary<string, string>();
			Method = method;
			Url = url;
		}

		public override int GetHashCode() => 
			((Url?.ToString() ?? "<null>") + "()" + (Method ?? "<null>")).GetHashCode() ^ Headers.Count;

		public override bool Equals(object obj) => 
			obj is HttpRequest other 
		    && Url == other.Url 
		    && Method == other.Method 
		    && Headers.Count == other.Headers.Count 
		    && Headers.Keys.All(k => other.Headers.ContainsKey(k) && Headers[k].Equals(other.Headers[k]));
	}

	public class HttpResponse : IResource
	{
		public HttpResponse(
			HttpStatusCode statusCode, 
			Stream stream, 
			string headers, 
			string contentType = ResourceTypes.Html, 
			Uri uri = null)
		{
			StatusCode = statusCode;
			Stream = stream;
			Headers = headers;
			Type = contentType;
			Uri = uri;
		}

		public HttpStatusCode StatusCode { get; }
		public string Headers { get; }
		public string Type { get; }
		public Stream Stream { get; }
		public Uri Uri { get; }
	}
}
