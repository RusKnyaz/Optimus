using System;
using System.Net;
using System.Threading.Tasks;

namespace Knyaz.Optimus.ResourceProviders
{
    /// <summary>
    /// Allows to get resources like files, html pages and etc (dependes on implementation).
    /// </summary>
    public interface ISpecResourceProvider
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
    
    /// <summary>
    /// Allows to get resources like files, html pages and etc.
    /// </summary>
    public interface IResourceProvider : ISpecResourceProvider
    {
        /// <summary>
        /// Called on request send.
        /// </summary>
        event Action<Uri> OnRequest;
        
        /// <summary>
        /// Called on request handled and data received.
        /// </summary>
        event EventHandler<ReceivedEventArguments> Received;

        CookieContainer CookieContainer { get; }
    }
}