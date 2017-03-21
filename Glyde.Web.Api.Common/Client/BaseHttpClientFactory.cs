using System;
using System.Net.Http;

namespace Glyde.Web.Api.Client
{
    public abstract class BaseHttpClientFactory : IHttpClientFactory
    {
        public abstract HttpClient GetHttpClient(HttpClientSettings settings);

        protected Uri EnsureBaseAddressTerminatesWithSlash(Uri uri)
        {
            var builder = new UriBuilder(uri);

            if (!builder.Path.EndsWith("/"))
                builder.Path += "/";

            return builder.Uri;

        }
    }
}