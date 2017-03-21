using System;
using System.Net;
using System.Net.Http;
using FluentAssertions;
using Glyde.Configuration;
using Glyde.Configuration.Models;
using Glyde.Web.Api.Client;
using Glyde.Web.Api.Client.Configuration;
using Glyde.Web.Api.Common.Tests.Helpers;
using Glyde.Web.Api.Common.Tests.Models;
using Glyde.Web.Api.Resources;
using Moq;
using Xunit;

namespace Glyde.Web.Api.Common.Tests
{
    public class ApiClientFactoryUnitTests
    {

        [Fact]
        public void ShouldReturnProperlyInstanciatedApiClientForType()
        {
            var cs = new ConfigurationService(new ConfigurationSection[]
            {
                new ApiClientConfiguration()
                {
                    KnownResources =
                    {
                        new ApiClientResourceConfiguration()
                        {
                            ResourceNames =
                            {
                                "testbyconvention"
                            },
                            BaseAddress = new Uri("http://lollol.com/api")
                        }
                    }
                }
            });

            var factory = new ApiClientFactory(new TestableHttpClientFactory(), cs, new ResourceMetadataProvider());

            var client = factory.GetClientFor<TestByConventionResource>();
            client.Should().BeOfType<ApiClient<TestByConventionResource, int>>();
        }

        [Fact]
        public void ShouldInvokeHttpGetWhenCallingGetAll()
        {
            var cs = new ConfigurationService(new ConfigurationSection[]
            {
                new ApiClientConfiguration()
                {
                    KnownResources =
                    {
                        new ApiClientResourceConfiguration()
                        {
                            ResourceNames =
                            {
                                "testbyconvention"
                            },
                            BaseAddress = new Uri("http://lollol.com/api/")
                        }
                    }
                }
            });

            var httpClientFactory = new TestableHttpClientFactory(content: new StringContent("[]"));

            var factory = new ApiClientFactory(httpClientFactory, cs, new ResourceMetadataProvider());

            var client = factory.GetClientFor<TestByConventionResource>();
            client.Should().BeOfType<ApiClient<TestByConventionResource, int>>();

            var result = client.GetAll().Result;

            httpClientFactory.HasExecuted.Should().BeTrue();

            httpClientFactory.Request.RequestUri.Should().Be(new Uri("http://lollol.com/api/v1/testbyconvention"));
            httpClientFactory.Request.Method.Should().Be(HttpMethod.Get);
        }
    }
}
