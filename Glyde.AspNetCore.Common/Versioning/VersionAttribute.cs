using System;

namespace Glyde.AspNetCore.Versioning
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