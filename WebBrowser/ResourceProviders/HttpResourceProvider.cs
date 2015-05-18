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

		public HttpResponse SendRequest(HttpRequest request)
		{
			var webRequest = MakeWebRequest(request);
			var response = (HttpWebResponse) webRequest.GetResponse();
			return MakeResponse(response);
		}

		private HttpResponse MakeResponse(HttpWebResponse response)
		{
			using (var reader = new StreamReader(response.GetResponseStream()))
			{
				return new HttpResponse(response.StatusCode, reader.ReadToEnd());
			}
		}

		private HttpWebRequest MakeWebRequest(HttpRequest request)
		{
			var result = WebRequest.CreateHttp(request.Url);
			result.Method = request.Method;
			result.CookieContainer = _cookies;
			return result;
		}
	}

	public interface IHttpResourceProvider
	{
		HttpResponse SendRequest(HttpRequest request);
	}

	public class HttpRequest
	{
		public string Method;
		public string Url;
		public Dictionary<string, string> Headers;
		//todo: body

		public HttpRequest()
		{
			Headers = new Dictionary<string, string>();
		}
	}

	public class HttpResponse
	{
		public HttpResponse(HttpStatusCode statusCode, string data)
		{
			StatusCode = statusCode;
			Data = data;
		}

		public HttpStatusCode StatusCode { get; private set; }
		public string Data { get; private set; }
	}
}
