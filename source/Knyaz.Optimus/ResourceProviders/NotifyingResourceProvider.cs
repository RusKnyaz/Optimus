using System;
using System.Threading.Tasks;

namespace Knyaz.Optimus.ResourceProviders
{
	public class NotifyingResourceProvider : IResourceProvider
	{
		private readonly IResourceProvider _provider;
		
		public NotifyingResourceProvider(IResourceProvider provider)
		{
			_provider = provider;
		}

		public Task<IResource> SendRequestAsync(Request request)
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
		public readonly Request Request;
		public readonly IResource Resource;

		public ReceivedEventArguments(Request request, IResource resource)
		{
			Request = request;
			Resource = resource;
		}
	}
}
