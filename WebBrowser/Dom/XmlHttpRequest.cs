using System;
using WebBrowser.ResourceProviders;

namespace WebBrowser.Dom
{
	public class XmlHttpRequest
	{
		private readonly IHttpResourceProvider _httpResourceProvider;
		private HttpRequest _request;
		private bool _async;
		private HttpResponse _response;

		public XmlHttpRequest(IHttpResourceProvider httpResourceProvider)
		{
			_httpResourceProvider = httpResourceProvider;
			ReadyState = UNSENT;
		}

		public void Open(string method, string url, bool? async, string username, string password)
		{
			_request = new HttpRequest
				{
					Url = url,
					Method = method
				};
			_async = async ?? true;
			//todo: username, password
			ReadyState = OPENED;
		}

		public const ushort UNSENT = 0;
		public const ushort OPENED = 1;
		public const ushort HEADERS_RECEIVED = 2;
		public const ushort LOADING = 3;
		public const ushort DONE = 4;

		public int ReadyState { get; private set; }

		public object ResponseXML { get { return _response.Data;}
		}

		public void SetRequestHeader(string name, string value)
		{
			_request.Headers.Add(name, value);
		}

		public int Status
		{
			get { return (int)_response.StatusCode; }
		}

		public event Action OnReadyStateChange;

		private void FireOnReadyStateChange()
		{
			//todo: Invoke OnReadyStateChange in JS runtime thread.
			if (OnReadyStateChange != null)
				OnReadyStateChange();
		}

		public async void Send(object data)
		{
			if (_async)
			{
			/*	ReadyState = LOADING;
				_response = (HttpWebResponse) await _request.GetResponseAsync();
				ReadyState = DONE;
				FireOnReadyStateChange();*/
			}
			else
			{
				ReadyState = LOADING;
				_response = _httpResourceProvider.SendRequest(_request);
				ReadyState = DONE;
				FireOnReadyStateChange();
			}
		}
	}
}