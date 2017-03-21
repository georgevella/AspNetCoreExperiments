using System;
using System.Collections.Concurrent;
using System.Net.Http;

namespace Glyde.Web.Api.Client
{
    internal static class HttpClientFactory
    {
        private static readonly ConcurrentDictionary<HttpClientKey, HttpClient> ClientCache = new ConcurrentDictionary<HttpClientKey, HttpClient>();


        public static HttpClient GetHttpClient(HttpClientSettings settings)
        {
            //var baseUriBuilder = new UriBuilder(settings.BaseAddress);
            //baseUriBuilder.Fragment = baseUriBuilder.Query = baseUriBuilder.Path = string.Empty;

            return ClientCache.GetOrAdd(new HttpClientKey(settings.BaseAddress, settings.UseProxy, settings.ProxyAddress),
                key => new HttpClient(new HttpClientHandler()
                {

                }));
        }

    }

    internal class HttpClientSettings
    {
        public Uri BaseAddress { get; set; }

        public Uri ProxyAddress { get; set; }

        public bool UseProxy { get; set; }
    }
}