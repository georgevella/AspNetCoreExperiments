using System;

namespace Glyde.Web.Api.Versioning
{
    [AttributeUsage( AttributeTargets.Class )]
    public class IgnoreVersioningConventionAttribute : Attribute
    {
        
    }
}