using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Reflection;
using Glyde.Configuration;
using Glyde.Web.Api.Client.Configuration;
using Glyde.Web.Api.Resources;

namespace Glyde.Web.Api.Client
{
    public class ApiClientFactory : IApiClientFactory
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfigurationService _configurationService;
        private readonly IResourceMetadataProvider _resourceMetadataProvider;

        private readonly ConcurrentDictionary<Type, Func<HttpClient, Uri, object>> _typedApiClientFactory =
            new ConcurrentDictionary<Type, Func<HttpClient, Uri, object>>();

        public ApiClientFactory(IHttpClientFactory httpClientFactory, IConfigurationService configurationService,
            IResourceMetadataProvider resourceMetadataProvider)
        {
            _httpClientFactory = httpClientFactory;
            _configurationService = configurationService;
            _resourceMetadataProvider = resourceMetadataProvider;
        }

        public IApiClient<TResource> GetClientFor<TResource>()
            where TResource : IResource
        {
            var result = GetClientForImpl<TResource>();
            return (IApiClient<TResource>) result.factoryMethod(result.httpClient, result.relativeUri);
        }

        public IApiClient<TResource, TResourceId> GetClientFor<TResource, TResourceId>()
            where TResource : Resource<TResourceId>
        {
            var result = GetClientForImpl<TResource>();
            return (IApiClient<TResource, TResourceId>) result.factoryMethod(result.httpClient, result.relativeUri);
        }

        private (HttpClient httpClient, Uri relativeUri, Func<HttpClient, Uri, object> factoryMethod) GetClientForImpl
            <TResource>()
            where TResource : IResource
        {
            var resourceMetadata = _resourceMetadataProvider.GetMetadataFor<TResource>();

            var apiClientConfiguration = _configurationService.Get<ApiClientConfiguration>();
            var resourceConfiguration =
                apiClientConfiguration.KnownResources.FirstOrDefault(
                    x => x.ResourceNames.Contains(resourceMetadata.Name));

            var httpClientSettings = new HttpClientSettings
            {
                BaseAddress = resourceConfiguration.BaseAddress,
                UseProxy = apiClientConfiguration.UseProxy,
                ProxyAddress = apiClientConfiguration.ProxyAddress ?? resourceConfiguration.ProxyAddress
            };

            if (resourceConfiguration.UseProxy.HasValue)
                httpClientSettings.UseProxy = resourceConfiguration.UseProxy.Value;

            return (
                _httpClientFactory.GetHttpClient(httpClientSettings),
                new Uri($"api/v{resourceMetadata.Version}/{resourceMetadata.Name}", UriKind.Relative),
                _typedApiClientFactory.GetOrAdd(typeof(TResource), type => CreateConstructorInvoker(type, resourceMetadata))
                );
        }

        private static Func<HttpClient, Uri, object> CreateConstructorInvoker(Type type, ResourceMetadata resourceMetadata)
        {
            var apiClientType = typeof(ApiClient<,>).MakeGenericType(type, resourceMetadata.ResourceIdType);
            var ctor = apiClientType.GetTypeInfo().DeclaredConstructors.First();

            var httpClientParameter = Expression.Parameter(typeof(HttpClient), "httpClient");
            var resourceUriParameter = Expression.Parameter(typeof(Uri), "resourceUri");

            var newExpression = Expression.New(ctor, httpClientParameter, resourceUriParameter);

            var lambda = Expression.Lambda<Func<HttpClient, Uri, object>>(newExpression, httpClientParameter,
                resourceUriParameter);

            return lambda.Compile();
        }
    }
}