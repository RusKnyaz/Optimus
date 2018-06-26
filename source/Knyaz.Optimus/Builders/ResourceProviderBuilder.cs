using System;

namespace Knyaz.Optimus.ResourceProviders
{
	/// <summary>
	/// Configures Resource provider.
	/// </summary>
	public class ResourceProviderBuilder
	{
		private HttpResourceProviderBuilder _httpBuilder;
		private bool _usePrediction = false;
		private Action<Request> _onRequset;
		private Action<ReceivedEventArguments> _onResponse;

		/// <summary>
		/// Configures HttpResourceProvider.
		/// </summary>
		public ResourceProviderBuilder Http(Action<HttpResourceProviderBuilder> configureHttpResourceProvider = null)
		{
			_httpBuilder = new HttpResourceProviderBuilder();
			configureHttpResourceProvider?.Invoke(_httpBuilder);
			return this;
		}

		/// <summary>
		/// Enables premature http resources loading.
		/// </summary>
		/// <returns></returns>
		public ResourceProviderBuilder UsePrediction()
		{
			_usePrediction = true;
			return this;
		}

		public ResourceProviderBuilder Notify(Action<Request> onRequest, Action<ReceivedEventArguments> onResponse)
		{
			_onRequset = onRequest;
			_onResponse = onResponse;
			return this;
		}

		public IResourceProvider Build()
		{
			IResourceProvider resourceProvider = new ResourceProvider(_httpBuilder?.Build(), new FileResourceProvider());

			if (_onRequset != null || _onResponse != null)
				resourceProvider = new NotifyingResourceProvider(resourceProvider, _onRequset, _onResponse);
            
			return _usePrediction
				? new PredictedResourceProvider(resourceProvider)
				: resourceProvider;
		}
	}
}