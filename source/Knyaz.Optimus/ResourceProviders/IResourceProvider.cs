using System;
using System.Threading.Tasks;

namespace Knyaz.Optimus.ResourceProviders
{
	public interface IResourceProvider
	{
		string Root { get; set; }
		event Action<string> OnRequest;
		event EventHandler<ReceivedEventArguments> Received;
		Task<IResource> GetResourceAsync(IRequest req);
		IRequest CreateRequest(string uri);
	}

	public static class ResourceProviderExtension
	{
		public static Task<IResource> GetResourceAsync(this IResourceProvider provider, string uri)
		{
			if (string.IsNullOrEmpty(uri))
				throw new ArgumentOutOfRangeException("uri");

			var req = provider.CreateRequest(uri);

			return provider.GetResourceAsync(req);
		}
	}

	public class ReceivedEventArguments : EventArgs
	{
		public IRequest Request { get; private set; }
		public IResource Resource { get; private set; }

		public ReceivedEventArguments(IRequest request, IResource resource)
		{
			Request = request;
			Resource = resource;
		}
	}
}
