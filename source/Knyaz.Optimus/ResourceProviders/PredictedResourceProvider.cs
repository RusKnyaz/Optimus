using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Knyaz.Optimus.ResourceProviders
{
	internal class PredictedResourceProvider : IPredictedResourceProvider
	{
		private readonly ConcurrentDictionary<Request, Task<IResource>> _preloadedResources
        			= new ConcurrentDictionary<Request, Task<IResource>>();

		public PredictedResourceProvider(IResourceProvider resourceProvider)
		{
			_resourceProvider = resourceProvider;
		}

		private readonly IResourceProvider _resourceProvider;
		
		public Task<IResource> SendRequestAsync(Request req) => 
			_preloadedResources.TryRemove(req, out var preloaded)
			? preloaded
			: _resourceProvider.SendRequestAsync(req);


		public void Preload(Request request)
		{
			if (_preloadedResources.ContainsKey(request))
				return;

			var task = _resourceProvider.SendRequestAsync(request);
			_preloadedResources.AddOrUpdate(request, task, (s, task1) => task1);
		}

		public void Clear() => _preloadedResources.Clear();
	}

	public interface IPredictedResourceProvider : IResourceProvider
	{
		void Preload(Request request);
	}
}