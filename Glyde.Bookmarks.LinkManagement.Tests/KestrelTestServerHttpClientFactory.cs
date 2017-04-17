using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Glyde.AspNetCore.Bootstrapping;
using Glyde.Web.Api.Client;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.TestHost;
namespace Glyde.Bookmarks.LinkManagement.Tests
{
    public class KestrelTestServerHttpClientFactory : BaseHttpClientFactory
    {
        private readonly HttpClient _client;

        public KestrelTestServerHttpClientFactory()
        {
            var builder = new WebHostBuilder()
                .UseGlydeBootstrappingForApi();

            var server = new TestServer(builder)
            {
                BaseAddress = new Uri("http://localhost")
            };
            _client = server.CreateClient();
        }

        public override HttpClient GetHttpClient(HttpClientSettings settings)
        {
            return _client;
        }
    }
}