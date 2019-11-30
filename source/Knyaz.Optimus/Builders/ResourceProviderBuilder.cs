using System;

namespace Knyaz.Optimus.ResourceProviders
{
	/// <summary>
	/// Configures Resource provider.
	/// </summary>
	public class ResourceProviderBuilder
	{
		private Func<IResourceProvider> _httpFn;
		private bool _usePrediction = false;
		private Action<Request> _onRequset;
		private Action<ReceivedEventArguments> _onResponse;


		/// <summary>
		/// Sets the resource provider to be used for http/https requests handling.
		/// </summary>
		public ResourceProviderBuilder Http(IResourceProvider httpResourceProvider)
		{
			_httpFn = () => httpResourceProvider;
			return this;
		}
		
		/// <summary>
		/// Configures HttpResourceProvider.
		/// </summary>
		public ResourceProviderBuilder Http(Action<HttpResourceProviderBuilder> configureHttpResourceProvider = null)
		{
			var builder = new HttpResourceProviderBuilder();
			configureHttpResourceProvider?.Invoke(builder);
			_httpFn = () => builder.Build();
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

		/// <summary>
		/// Adds the callback function to be called before the request sent and before the response handled by the engine. 
		/// </summary>
		/// <param name="onRequest">The function that can modify the any request before sending.</param>
		/// <param name="onResponse">The function that can read the response before handling by engine.</param>
		public ResourceProviderBuilder Notify(Action<Request> onRequest, Action<ReceivedEventArguments> onResponse)
		{
			_onRequset = onRequest;
			_onResponse = onResponse;
			return this;
		}

		public IResourceProvider Build()
		{
			IResourceProvider resourceProvider = new ResourceProvider(_httpFn?.Invoke(), new FileResourceProvider());

			if (_onRequset != null || _onResponse != null)
				resourceProvider = new NotifyingResourceProvider(resourceProvider, _onRequset, _onResponse);
            
			return _usePrediction
				? new PredictedResourceProvider(resourceProvider)
				: resourceProvider;
		}
	}
}