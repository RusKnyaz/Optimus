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
		Task<IResource> GetResourceAsync(string url);
		Task<IResource> GetResourceAsync(IRequest req);
	}
}
