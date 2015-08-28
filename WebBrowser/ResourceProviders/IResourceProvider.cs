using System;
using System.Threading.Tasks;
using WebBrowser.ResourceProviders;

namespace WebBrowser
{
	public interface IResourceProvider
	{
		string Root { get; set; }
		event Action<string> OnRequest;
		event Action<string> Received;
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
}
