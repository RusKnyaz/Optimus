using System;
using System.IO;
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
		private readonly Func<Request, HttpClient> _getClientFn;
		
		public HttpResourceProvider(AuthenticationHeaderValue auth, Action<HttpClientHandler> handlerConfig)
		{
			_getClientFn = req =>
			{
				var handler = new HttpClientHandler {CookieContainer = req.Cookies};
				
				handlerConfig?.Invoke(handler);
				
				return new HttpClient(handler)
				{
					Timeout = req.Timeout > 0
						? TimeSpan.FromMilliseconds(req.Timeout)
						: Timeout.InfiniteTimeSpan,
					DefaultRequestHeaders = {Authorization = auth},
				};
			};
		}

		private async Task<IResource> SendRequestEx(Request request)
		{
			var req = MakeWebRequest(request);
			
			using (var client = _getClientFn(request))
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
		
		public Task<IResource> SendRequestAsync(Request request) => SendRequestEx(request);

		private HttpRequestMessage MakeWebRequest(Request request)
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
