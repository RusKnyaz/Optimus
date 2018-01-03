using System;
using System.Collections.Concurrent;
using System.Net;
using System.Threading.Tasks;

namespace Knyaz.Optimus.ResourceProviders
{
	internal class PredictedResourceProvider : IResourceProvider
	{
		private readonly ConcurrentDictionary<IRequest, Task<IResource>> _preloadedResources
        			= new ConcurrentDictionary<IRequest, Task<IResource>>();

		public PredictedResourceProvider(IResourceProvider resourceProvider)
		{
			_resourceProvider = resourceProvider;
		}

		private readonly IResourceProvider _resourceProvider;
		public string Root
		{
			get { return _resourceProvider.Root; }
			set { _resourceProvider.Root = value; }
		}

		public event Action<string> OnRequest
		{
			add { _resourceProvider.OnRequest += value; }
			remove { _resourceProvider.OnRequest -= value; }
		}

		public event EventHandler<ReceivedEventArguments> Received
		{
			add { _resourceProvider.Received += value; }
			remove { _resourceProvider.Received -= value; }
		}

		public Task<IResource> SendRequestAsync(IRequest req) => 
			_preloadedResources.TryRemove(req, out var preloaded)
			? preloaded
			: _resourceProvider.SendRequestAsync(req);

		public IRequest CreateRequest(string path) => _resourceProvider.CreateRequest(path);

		public void Preload(string uri)
		{
			var request = _resourceProvider.CreateRequest(uri);

			if (_preloadedResources.ContainsKey(request))
				return;

			var task = _resourceProvider.SendRequestAsync(request);
			_preloadedResources.AddOrUpdate(request, task, (s, task1) => task1);
		}

		public void Clear() => _preloadedResources.Clear();

		public CookieContainer CookieContainer => _resourceProvider.CookieContainer;
	}
}