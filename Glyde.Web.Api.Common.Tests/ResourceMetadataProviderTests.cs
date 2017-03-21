using System;
using FluentAssertions;
using Glyde.Web.Api.Common.Tests.Models;
using Glyde.Web.Api.Resources;
using Xunit;

namespace Glyde.Web.Api.Common.Tests
{
    public class ResourceMetadataProviderTests
    {
        [Fact]
        public void ShouldUseConventions()
        {
            var provider = new ResourceMetadataProvider();
            var metadata = provider.GetMetadataFor<TestByConventionResource>();

            metadata.Name.Should().Be("testbyconvention");
            metadata.ResourceIdType.Should().Be<int>();
            metadata.Version.Should().Be(1);
        }

        [Fact]
        public void ShouldUseAttributes()
        {
            var provider = new ResourceMetadataProvider();
            var metadata = provider.GetMetadataFor<TestResource>();

            metadata.Name.Should().Be("test");
            metadata.ResourceIdType.Should().Be<Guid>();
            metadata.Version.Should().Be(1);
        }

        [Fact]
        public void ShouldDetermineVersionFromNamespaceWithAttrribute()
        {
            var provider = new ResourceMetadataProvider();
            var metadata = provider.GetMetadataFor<Models.V2.TestResource>();

            metadata.Name.Should().Be("test");
            metadata.ResourceIdType.Should().Be<int>();
            metadata.Version.Should().Be(2);
        }

        [Fact]
        public void ShouldDetermineVersionFromNamespaceUseConventions()
        {
            var provider = new ResourceMetadataProvider();
            var metadata = provider.GetMetadataFor<Models.V2.TestByConventionResource>();

            metadata.Name.Should().Be("testbyconvention");
            metadata.ResourceIdType.Should().Be<int>();
            metadata.Version.Should().Be(2);
        }
        [Fact]
        public void ShouldUseVersionDeclaredInAttribute()
        {
            var provider = new ResourceMetadataProvider();
            var metadata = provider.GetMetadataFor<Models.V2.TestWithVersioningResource>();

            metadata.Name.Should().Be("testwithversion-ns");
            metadata.ResourceIdType.Should().Be<int>();
            metadata.Version.Should().Be(3);

            metadata = provider.GetMetadataFor<TestWithVersioningResource>();
            metadata.Name.Should().Be("testwithversion");
            metadata.ResourceIdType.Should().Be<int>();
            metadata.Version.Should().Be(3);
        }
    }
}