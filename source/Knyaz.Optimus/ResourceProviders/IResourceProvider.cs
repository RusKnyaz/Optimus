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

		public CommonResourceProvider(IResourceProvider provider, LinkProvider linkProvider)
		{
			_provider = provider;
			_linkProvider = linkProvider;
		}
		
		public Task<IResource> GetResourceAsync(string path) => 
			_provider.SendRequestAsync(_provider.CreateRequest(_linkProvider.MakeUri(path)));
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
