using System;
using System.Net;
using System.Threading.Tasks;

namespace Knyaz.Optimus.ResourceProviders
{
    public interface IRequest
    {
        Uri Url { get; }
    }

    
    /// <summary>
    /// Allows to get resources like files, html pages and etc (dependes on implementation).
    /// </summary>
    public interface IResourceProvider
    {
        /// <summary>
        /// Creates resource request.
        /// </summary>
        IRequest CreateRequest(Uri url);
        
        /// <summary>
        /// Requests resource.
        /// </summary>
        Task<IResource> SendRequestAsync(IRequest request);
    }
    
    public static class ResourceProviderExtension
    {
        public static Task<IResource> GetResourceAsync(this IResourceProvider provider, Uri uri) => 
            provider.SendRequestAsync(provider.CreateRequest(uri));
    }
}