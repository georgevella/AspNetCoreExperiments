using System;
using System.Reflection;

namespace Glyde.Web.Api.Resources
{
    public class ResourceMetadata
    {
        internal ResourceMetadata(string name, int version, Type resourceIdType)
        {
            Name = name;
            Version = version;
            ResourceIdType = resourceIdType;
        }

        public string Name { get; }

        public int Version { get; }

        public Type ResourceIdType { get; }
    }
}