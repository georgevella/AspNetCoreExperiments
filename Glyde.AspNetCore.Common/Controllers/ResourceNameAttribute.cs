using System;

namespace Glyde.AspNetCore.Controllers
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ResourceNameAttribute : Attribute
    {
        public string Name { get; set; }

        public ResourceNameAttribute(string name)
        {
            Name = name;
        }
    }
}