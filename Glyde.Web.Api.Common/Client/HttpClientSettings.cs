using System;

namespace Glyde.Web.Api.Client
{
    public class HttpClientSettings
    {
        public Uri BaseAddress { get; set; }

        public Uri ProxyAddress { get; set; }

        public bool UseProxy { get; set; }
    }
}