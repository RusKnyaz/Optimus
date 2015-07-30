using System;
using System.Threading.Tasks;
using WebBrowser.ResourceProviders;

namespace WebBrowser
{
	public interface IResourceProvider
	{
		IHttpResourceProvider HttpResourceProvider { get; }
		string Root { get; set; }
		event Action<string> OnRequest;
		event Action<string> Received;
		Task<IResource> GetResourceAsync(string url);

		T CreateRequest<T>(string url) where T: class, IRequest;
		Task<IResource> GetResourceAsync(IRequest req);
	}
}
