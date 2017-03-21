using System;

namespace Glyde.Web.Api.Versioning
{
    public class VersionAttribute : Attribute
    {
        public int Version { get; }

        public VersionAttribute(int version)
        {
            Version = version;
        }
    }
}