﻿using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Knyaz.Optimus.ResourceProviders
{
	public class PredictedResourceProvider : IResourceProvider
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

		public Task<IResource> GetResourceAsync(IRequest req)
		{
			Task<IResource> preloaded;
			return _preloadedResources.TryRemove(req, out preloaded)
				? preloaded
				: _resourceProvider.GetResourceAsync(req);
		}

		public IRequest CreateRequest(string uri)
		{
			return _resourceProvider.CreateRequest(uri);
		}

		public void Preload(string uri)
		{
			var request = _resourceProvider.CreateRequest(uri);

			if (_preloadedResources.ContainsKey(request))
				return;

			var task = _resourceProvider.GetResourceAsync(request);
			_preloadedResources.AddOrUpdate(request, task, (s, task1) => task1);
		}

		public void Clear()
		{
			_preloadedResources.Clear();
		}
	}
}