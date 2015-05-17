using System;
using System.IO;
using System.Net;

namespace WebBrowser.Dom
{
	public class XmlHttpRequest
	{
		private HttpWebRequest _request;
		private bool _async;
		private HttpWebResponse _response;

		public XmlHttpRequest()
		{
			ReadyState = UNSENT;
		}

		public void Open(string method, string url, bool? async, string username, string password)
		{
			//todo: or we should do it through resource provider???
			_request = WebRequest.CreateHttp(url);
			_request.Method = method;
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

		public object ResponseXML
		{
			get
			{
				//todo: use Lazy
				//todo: parse
				using (var reader = new StreamReader(_response.GetResponseStream()))
				{
					return reader.ReadToEnd();
				}
			}
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
				ReadyState = LOADING;
				_response = (HttpWebResponse) await _request.GetResponseAsync();
				ReadyState = DONE;
				FireOnReadyStateChange();
			}
			else
			{
				ReadyState = LOADING;
				_response = (HttpWebResponse)_request.GetResponse();
				ReadyState = DONE;
				FireOnReadyStateChange();
			}
		}
	}
}