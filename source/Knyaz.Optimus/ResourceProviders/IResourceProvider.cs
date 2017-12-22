using System;
using System.Threading.Tasks;

namespace Knyaz.Optimus.ResourceProviders
{
	public static class ResourceProviderExtension
	{
		public static Task<IResource> GetResourceAsync(this IResourceProvider provider, string uri)
		{
			if (string.IsNullOrEmpty(uri))
				throw new ArgumentOutOfRangeException("uri");

			var req = provider.CreateRequest(uri);

			return provider.SendRequestAsync(req);
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
