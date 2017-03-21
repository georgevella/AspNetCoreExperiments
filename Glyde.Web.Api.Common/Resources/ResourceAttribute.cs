using System;

namespace Glyde.Web.Api.Resources
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ResourceAttribute : Attribute
    {
        private int? _version;
        public string Name { get; set; }

        public int Version
        {
            get { return _version?? 1; }
            set { _version = value; }
        }

        public bool GetResourceVersion(out int version)
        {
            version = _version ?? 1;
            return _version.HasValue;
        }

        public ResourceAttribute(string name)
        {
            Name = name;
        }
    }
}