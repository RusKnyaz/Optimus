using System;
using System.Threading.Tasks;

namespace Knyaz.Optimus.ResourceProviders
{
	public static class ResourceProviderExtension
	{
		public static Task<IResource> GetResourceAsync(this IResourceProvider provider, Uri uri) => 
			provider.SendRequestAsync(provider.CreateRequest(uri));
	}

	public class CommonResourceProvider
	{
		private readonly IResourceProvider _provider;
		private readonly LinkProvider _linkProvider;
		private readonly Func<string> _userAgentFn;

		public CommonResourceProvider(IResourceProvider provider, LinkProvider linkProvider, Func<string> userAgentFn)
		{
			_provider = provider;
			_linkProvider = linkProvider;
			_userAgentFn = userAgentFn;
		}

		public Task<IResource> GetResourceAsync(string path)
		{
			var req = _provider.CreateRequest(_linkProvider.MakeUri(path));
			if (req is HttpRequest httpReq)
			{
				var ua = _userAgentFn?.Invoke();
				if(ua != null)
					httpReq.Headers["User-Agent"] = ua;
			}
			return _provider.SendRequestAsync(req);
		}
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
