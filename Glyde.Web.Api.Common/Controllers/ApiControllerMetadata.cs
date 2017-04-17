using System.Reflection;

namespace Glyde.Web.Api.Controllers
{
    public class ApiControllerMetadata
    {
        internal ApiControllerMetadata(TypeInfo resourceType, TypeInfo resourceIdType)
        {
            ResourceType = resourceType;
            ResourceIdType = resourceIdType;
        }

        public TypeInfo ResourceType { get; }

        public TypeInfo ResourceIdType { get; }
    }
}