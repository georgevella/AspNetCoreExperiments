using System;
using FluentAssertions;
using Glyde.Configuration;
using Glyde.Configuration.Models;
using Glyde.Web.Api.Client;
using Glyde.Web.Api.Client.Configuration;
using Glyde.Web.Api.Common.Tests.Models;
using Glyde.Web.Api.Resources;
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

            var factory = new ApiClientFactory(cs, new ResourceMetadataProvider());

            var client = factory.GetClientFor<TestByConventionResource>();
            client.Should().BeOfType<ApiClient<TestByConventionResource, int>>();
        }
    }
}
