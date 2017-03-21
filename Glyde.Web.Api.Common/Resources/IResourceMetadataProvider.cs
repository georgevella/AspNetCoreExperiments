using System;
using System.Reflection;

namespace Glyde.Web.Api.Resources
{
    public interface IResourceMetadataProvider
    {
        ResourceMetadata GetMetadataFor<TResource>()
            where TResource : IResource;

        ResourceMetadata GetMetadataFor(TypeInfo resourceType);
    }
}