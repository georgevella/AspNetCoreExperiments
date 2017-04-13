using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Glyde.AspNetCore.Controllers;
using Glyde.Web.Api.Controllers;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Glyde.AspNetCore.Framework
{
    public class GenericRestfulApiControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        private static readonly Type WrapperType = typeof(ApiControllerWrapper<,,>);

        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            foreach (var part in parts.OfType<AssemblyPart>())
            {
                var wrapperTypes = part.Types.Where(IsApiController).Select(MakeApiControllerWrapper).ToList();
                foreach (var wrapperType in wrapperTypes)
                {
                    feature.Controllers.Add(wrapperType);
                }
            }            
        }

        public static bool IsApiController(TypeInfo type)
        {
            if (type.IsAbstract)
                return false;
            if (type.IsInterface)
                return false;

            if (type.AsType() == typeof(object))
            {
                // object is the root of all
                return false;
            }

            if (type.BaseType.IsConstructedGenericType &&
                type.BaseType.GetGenericTypeDefinition() == typeof(ApiController<,>))
            {
                return true;
            }

            if (type.BaseType != null && type.BaseType != typeof(object))
            {
                return IsApiController(type.BaseType.GetTypeInfo());
            }

            return false;
        }

        public static bool IsApiController(TypeInfo type, out TypeInfo resourceType, out TypeInfo resourceIdType)
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

        public static TypeInfo MakeApiControllerWrapper(TypeInfo type)
        {
            TypeInfo resourceIdType;
            TypeInfo resourceType;
            if (!IsApiController(type, out resourceType, out resourceIdType))
            {
                throw new InvalidOperationException();
            }

            var specificWrapperType = WrapperType.MakeGenericType(new[]
            {
                type.AsType(),
                resourceType.AsType(),
                resourceIdType.AsType()
            });

            return specificWrapperType.GetTypeInfo();
        }
    }
}