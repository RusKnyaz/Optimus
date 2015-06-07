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
		Task<IResource> GetResourceAsync(string url);
	}
}
