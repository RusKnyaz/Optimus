using System;
using System.IO;
using System.Threading;
using WebBrowser.ResourceProviders;
using WebBrowser.ScriptExecuting;

namespace WebBrowser.Dom
{
	[DomItem]
	public interface IXmlHttpRequest
	{
		void Open(string method, string url, bool? async, string username, string password);
		int ReadyState { get; }
		object ResponseXML { get; }
		string ResponseText { get; }
		string StatusText { get; }
		int Status { get; }
		void SetRequestHeader(string name, string value);
		string GetAllResponseHeaders();
		event Action OnReadyStateChange;
		event Action OnLoad;
		event Action OnError;
	}

	/// <summary>
	/// https://xhr.spec.whatwg.org/
	/// </summary>
	public class XmlHttpRequest : IXmlHttpRequest
	{
		private readonly IHttpResourceProvider _httpResourceProvider;
		private readonly SynchronizationContext _context;
		private HttpRequest _request;
		private bool _async;
		private HttpResponse _response;
		private Lazy<string> _data;
		private int _readyState;

		public XmlHttpRequest(IHttpResourceProvider httpResourceProvider, SynchronizationContext context)
		{
			_httpResourceProvider = httpResourceProvider;
			_context = context;
			ReadyState = UNSENT;
			_data = new Lazy<string>(() =>_response.Stream.ReadToEnd());
		}

		public void Open(string method, string url)
		{
			Open(method, url, true, null, null);
		}

		public void Open(string method, string url, bool? async)
		{
			Open(method, url, async, null, null);
		}

		public void Open(string method, string url, bool? async, string username, string password)
		{
			_request = new HttpRequest(method, url);
			_async = async ?? true;
			//todo: username, password
			ReadyState = OPENED;
		}

		public const ushort UNSENT = 0;
		public const ushort OPENED = 1;
		public const ushort HEADERS_RECEIVED = 2;
		public const ushort LOADING = 3;
		public const ushort DONE = 4;

		public int ReadyState
		{
			get { return _readyState; }
			private set
			{
				_readyState = value; 
				CallInContext(OnReadyStateChange);
			}
		}

		public object ResponseXML { get { return _data.Value;}}
		public string ResponseText { get { return _data.Value; } }

		public void SetRequestHeader(string name, string value)
		{
			_request.Headers.Add(name, value);
		}

		public string GetAllResponseHeaders()
		{
			if (_response.Headers == null)
				return "";
			//todo: check
			return _response.Headers.ToString();
		}

		public string StatusText { get { return _response.StatusCode.ToString(); } }//todo: is it right?
		public int Status { get { return (int)_response.StatusCode; } }

		public event Action OnReadyStateChange;
		public event Action OnLoad;
		public event Action OnError;

		public async void Send(object data)
		{
			//todo: use data
			if (_async)
			{
				ReadyState = LOADING;
				try
				{
					_response = await _httpResourceProvider.SendRequestAsync(_request);
				}
				catch (Exception e)
				{
					ReadyState = DONE;
					CallInContext(OnError);
					return;
				}
				ReadyState = DONE;
				CallInContext(OnLoad);
			}
			else
			{
				try
				{
					_response = _httpResourceProvider.SendRequest(_request);
				}
				catch (Exception exception)
				{
					ReadyState = DONE;
					CallInContext(OnError);
				}
				ReadyState = DONE;
				CallInContext(OnLoad);
			}
		}

		public void Send()
		{
			Send(null);
		}

		private void CallInContext(Action action)
		{
			_context.Send(x => ((Action)x).Fire(), action);
		}
	}

	public static class ActionExtension
	{
		public static void Fire(this Action action)
		{
			if (action != null) action();
		}
	}

	public static class StreamExtension
	{
		public static string ReadToEnd(this Stream stream)
		{
			using (var reader = new StreamReader(stream))
				return reader.ReadToEnd();
		}
	}
} 