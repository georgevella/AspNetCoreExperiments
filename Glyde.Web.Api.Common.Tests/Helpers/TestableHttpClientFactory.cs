using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.SessionState;
using Glyde.Web.Api.Client;

namespace Glyde.Web.Api.Common.Tests.Helpers
{
    public class TestableHttpClientFactory : BaseHttpClientFactory
    {
        private readonly HttpMessageHandler _messageHandler;
        
        public HttpRequestMessage Request { get; protected set; }

        public bool HasExecuted { get; protected set; }

        //public TestableHttpClientFactory(Func<HttpRequestMessage, HttpResponseMessage> verificationFunc)
        //{
        //    _messageHandler = new TestableHttpMessageHandler(verificationFunc);
        //}

        public TestableHttpClientFactory(HttpStatusCode statusCode = HttpStatusCode.OK, HttpContent content = null, HttpResponseHeaders headers = null)
        {
            HasExecuted = false;
            _messageHandler = new TestableHttpMessageHandler((req) =>
            {
                HasExecuted = true;
                Request = req;
                var response = new HttpResponseMessage(HttpStatusCode.OK);

                if (content != null)
                    response.Content = content;

                if (headers != null)
                {
                    
                }

                return response;
            });
        }

        public override HttpClient GetHttpClient(HttpClientSettings settings)
        {
            return new HttpClient(_messageHandler)
            {
                BaseAddress = EnsureBaseAddressTerminatesWithSlash(settings.BaseAddress)
            };
        }

        internal class TestableHttpMessageHandler : HttpMessageHandler
        {
            private readonly Func<HttpRequestMessage, HttpResponseMessage> _verificationFunc;

            public TestableHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> verificationFunc)
            {
                _verificationFunc = verificationFunc;
            }
            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return await Task.Run(() => _verificationFunc(request), cancellationToken);
            }
        }
    }
}