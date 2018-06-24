using System;
using System.Threading.Tasks;

namespace Knyaz.Optimus.ResourceProviders
{
	public class NotifyingResourceProvider : IResourceProvider
	{
		private readonly Action<Request> _onRequest;
		private readonly Action<ReceivedEventArguments> _onRespnose;
		private readonly IResourceProvider _provider;
		
		public NotifyingResourceProvider(IResourceProvider provider, 
			Action<Request> onRequest, 
			Action<ReceivedEventArguments> onResponse)
		{
			_onRequest = onRequest;
			_onRespnose = onResponse;
			_provider = provider;
		}

		public Task<IResource> SendRequestAsync(Request request)
		{
			_onRequest?.Invoke(request);
			
			return _onRespnose == null ? _provider.SendRequestAsync(request)
				: _provider.SendRequestAsync(request).ContinueWith(t =>
			{
				try
				{
					_onRespnose(new ReceivedEventArguments(request, t.Result));
				}
				catch { }
				return t.Result;
			});
		}
	}

	public class ReceivedEventArguments : EventArgs
	{
		public readonly Request Request;
		public readonly IResource Resource;

		public ReceivedEventArguments(Request request, IResource resource)
		{
			Request = request;
			Resource = resource;
		}
	}
}
