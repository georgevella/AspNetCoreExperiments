using System;
using System.Collections.Concurrent;
using System.Reflection;
using Glyde.Web.Api.Versioning;

namespace Glyde.Web.Api.Resources
{
    public class ResourceMetadataProvider : IResourceMetadataProvider
    {
        private static readonly ConcurrentDictionary<Type, ResourceMetadata> ResourceMetadataCache =
            new ConcurrentDictionary<Type, ResourceMetadata>();

        public ResourceMetadata GetMetadataFor<TResource>()
            where TResource : IResource
        {
            return GetMetadataFor(typeof(TResource).GetTypeInfo());
        }

        public ResourceMetadata GetMetadataFor(TypeInfo resourceType)
        {
            return ResourceMetadataCache.GetOrAdd(resourceType.AsType(), type =>
            {
                string resourceName = null;
                int? version = null;

                var resourceNameAttribute = resourceType.GetCustomAttribute<ResourceAttribute>();

                if (resourceNameAttribute == null)
                {
                    // resource attribute is missing, use type name

                    resourceName = resourceType.Name.ToLower();

                    // fall back to conventions
                    if (resourceName != "resource" &&
                        resourceName.EndsWith("resource", StringComparison.OrdinalIgnoreCase))
                        resourceName = resourceName.Remove(resourceName.Length - "resource".Length);
                }
                else
                {
                    resourceName = resourceNameAttribute.Name;
                    if (resourceNameAttribute.GetResourceVersion(out int v))
                        version = v;
                }

                if (version == null)
                    version = resourceType.DetermineVersionFromNamespace();

                Type resourceIdType = null;

                if (resourceType.BaseType.IsConstructedGenericType &&
                    resourceType.BaseType.GetGenericTypeDefinition() == typeof(Resource<>))
                {
                    resourceIdType = resourceType.BaseType.GenericTypeArguments[0];
                }
                else
                {
                    // TODO: improve this to check whole hierarchy, or to determine via conventions
                    throw new NotSupportedException();
                }

                return new ResourceMetadata(resourceName, version.Value, resourceIdType);
            });
        }
    }
}