using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Glyde.AspNetCore.Controllers
{
    public class ApiControllerConvension : IApplicationModelConvention
    {
        public void Apply(ApplicationModel application)
        {
            foreach (var controller in application.Controllers)
            {
                TypeInfo resourceIdType;
                TypeInfo resourceType;
                if (IsApiController(controller.ControllerType, out resourceType, out resourceIdType))
                {
                    var resourceNameAttribute = resourceType.GetCustomAttribute<ResourceNameAttribute>();

                    // TODO fallback if attribute missing

                    foreach (var controllerSelector in controller.Selectors)
                    {
                        controllerSelector.AttributeRouteModel = new AttributeRouteModel(new RouteAttribute(resourceNameAttribute.Name));
                    }

                }
            }
        }

        public bool IsApiController(TypeInfo type, out TypeInfo resourceType, out TypeInfo resourceIdType)
        {
            resourceType = null;
            resourceIdType = null;

            if (type.AsType() == typeof(object))
            {
                // object is the root of all
                return false;
            }

            if (type.BaseType.IsConstructedGenericType &&
                type.BaseType.GetGenericTypeDefinition() == typeof(ApiController<,>))
            {
                var typeArguments = type.BaseType.GenericTypeArguments;

                resourceType = typeArguments[0].GetTypeInfo();
                resourceIdType = typeArguments[1].GetTypeInfo();

                return true;
            }

            if (type.BaseType != null && type.BaseType != typeof(object))
            {
                return IsApiController(type.BaseType.GetTypeInfo(), out resourceType, out resourceIdType);
            }

            return false;
        }
    }
}