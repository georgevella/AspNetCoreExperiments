using System;
using System.Linq;
using Glyde.Configuration;
using Glyde.Web.Api.Client.Configuration;
using Glyde.Web.Api.Resources;

namespace Glyde.Web.Api.Client
{
    public class ApiClientFactory : IApiClientFactory
    {
        private readonly IConfigurationService _configurationService;
        private readonly IResourceMetadataProvider _resourceMetadataProvider;

        public ApiClientFactory(IConfigurationService configurationService, IResourceMetadataProvider resourceMetadataProvider)
        {
            _configurationService = configurationService;
            _resourceMetadataProvider = resourceMetadataProvider;
        }

        public IApiClient<TResource> GetClientFor<TResource>()
            where TResource : IResource
        {
            var resourceMetadata = _resourceMetadataProvider.GetMetadataFor<TResource>();

            var apiClientConfiguration = _configurationService.Get<ApiClientConfiguration>();
            var resourceConfiguration = apiClientConfiguration.KnownResources.FirstOrDefault(x => x.ResourceNames.Contains(resourceMetadata.Name));

            var httpClientSettings = new HttpClientSettings()
            {
                BaseAddress = resourceConfiguration.BaseAddress,
                UseProxy = apiClientConfiguration.UseProxy ,
                ProxyAddress = apiClientConfiguration.ProxyAddress ?? resourceConfiguration.ProxyAddress
            };

            if (resourceConfiguration.UseProxy.HasValue)
                httpClientSettings.UseProxy = resourceConfiguration.UseProxy.Value;

            var httpClient = HttpClientFactory.GetHttpClient(httpClientSettings);
            var relativeUri = new Uri($"v{resourceMetadata.Version}/{resourceMetadata.Name}");

            var apiClientType = typeof(ApiClient<,>).MakeGenericType(typeof(TResource), resourceMetadata.ResourceIdType);

            return (IApiClient<TResource>) Activator.CreateInstance(apiClientType, httpClient, relativeUri);
        }
    }
}