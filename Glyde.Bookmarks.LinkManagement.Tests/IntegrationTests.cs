using System;
using Glyde.Bookmarks.LinkManagement.Resources;
using Glyde.Configuration;
using Glyde.Web.Api.Client;
using Glyde.Web.Api.Client.Configuration;
using Glyde.Web.Api.Resources;
using Microsoft.AspNetCore.Hosting;
using Moq;
using Xunit;

namespace Glyde.Bookmarks.LinkManagement.Tests
{
    public class IntegrationTests
    {
        private KestrelTestServerHttpClientFactory _factory;

        public IntegrationTests()
        {
            _factory = new KestrelTestServerHttpClientFactory();            
        }

        [Fact]
        public void Test1()
        {
            Mock<IConfigurationService> m = new Mock<IConfigurationService>();
            m.Setup(x => x.Get<ApiClientConfiguration>()).Returns(new ApiClientConfiguration()
            {
                KnownResources =
                {
                    new ApiClientResourceConfiguration()
                    {
                        ResourceNames =
                        {
                            "bookmarks"
                        },
                    }
                }
            });

            var apiClientFactory = new ApiClientFactory(_factory, m.Object, new ResourceMetadataProvider());
            var client = apiClientFactory.GetClientFor<Bookmark, string>();
            client.GetAll();
        }
    }
}
