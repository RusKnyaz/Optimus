using System;
using System.Threading.Tasks;

namespace Knyaz.Optimus.ResourceProviders
{
	public class DocumentResourceProvider : IResourceProvider
	{
		private readonly IResourceProvider _provider;
		private readonly LinkProvider _linkProvider;
		private readonly Func<string> _userAgentFn;

		public DocumentResourceProvider(IResourceProvider provider, LinkProvider linkProvider, Func<string> userAgentFn)
		{
			_provider = provider;
			_linkProvider = linkProvider;
			_userAgentFn = userAgentFn;
		}

		public IRequest CreateRequest(Uri url)
		{
			var request = _provider.CreateRequest(_linkProvider.MakeUri(url.ToString()));
			PrepareRequest(request);
			return request;
		}
		
		private void PrepareRequest(IRequest request)
		{
			if (request is HttpRequest httpReq)
			{
				var ua = _userAgentFn?.Invoke();
				if(ua != null)
					httpReq.Headers["User-Agent"] = ua;
			}
		}

		public Task<IResource> SendRequestAsync(IRequest request)
		{
			OnRequest?.Invoke(request.Url);

			return _provider.SendRequestAsync(request).ContinueWith(t =>
			{
				try
				{
					Received?.Invoke(this, new ReceivedEventArguments(request, t.Result));
				}
				catch { }
				return t.Result;
			});
		}

		public event Action<Uri> OnRequest;
		public event EventHandler<ReceivedEventArguments> Received;
	}

	public class ReceivedEventArguments : EventArgs
	{
		public readonly IRequest Request;
		public readonly IResource Resource;

		public ReceivedEventArguments(IRequest request, IResource resource)
		{
			Request = request;
			Resource = resource;
		}
	}
}
