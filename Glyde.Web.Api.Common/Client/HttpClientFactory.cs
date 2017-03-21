using System;
using System.Collections.Concurrent;
using System.Net.Http;

namespace Glyde.Web.Api.Client
{
    public class HttpClientFactory : BaseHttpClientFactory
    {
        private static readonly ConcurrentDictionary<HttpClientKey, HttpClient> ClientCache = new ConcurrentDictionary<HttpClientKey, HttpClient>();

        public override HttpClient GetHttpClient(HttpClientSettings settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            //var baseUriBuilder = new UriBuilder(settings.BaseAddress);
            //baseUriBuilder.Fragment = baseUriBuilder.Query = baseUriBuilder.Path = string.Empty;

            return ClientCache.GetOrAdd(new HttpClientKey(settings.BaseAddress, settings.UseProxy, settings.ProxyAddress),
                key => new HttpClient(new HttpClientHandler()
                {
                                        
                })
                {
                    BaseAddress = EnsureBaseAddressTerminatesWithSlash(settings.BaseAddress)
                });
        }
    }
}