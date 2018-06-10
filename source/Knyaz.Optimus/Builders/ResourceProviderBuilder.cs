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

        public IResourceProvider Build()
        {
            var resourceProvider = new ResourceProvider(_httpBuilder?.Build(), new FileResourceProvider());
            return _usePrediction
                ? (IResourceProvider) new PredictedResourceProvider(resourceProvider)
                : resourceProvider;
        }
    }
}