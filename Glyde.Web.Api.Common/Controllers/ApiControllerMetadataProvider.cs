using System;
using System.Reflection;

namespace Glyde.Web.Api.Controllers
{
    public class ApiControllerMetadataProvider : IApiControllerMetadataProvider
    {
        public bool IsApiController(TypeInfo type)
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

        public ApiControllerMetadata GetMetadataFor(TypeInfo type)
        {
            if (type.IsAbstract)
                throw new ArgumentException();
            if (type.IsInterface)
                throw new ArgumentException();

            if (type.AsType() == typeof(object))
            {
                // object is the root of all
                throw new ArgumentException();
            }

            if (type.BaseType.IsConstructedGenericType &&
                type.BaseType.GetGenericTypeDefinition() == typeof(ApiController<,>))
            {
                var typeArguments = type.BaseType.GenericTypeArguments;

                return new ApiControllerMetadata(typeArguments[0].GetTypeInfo(), typeArguments[1].GetTypeInfo());
            }

            if (type.BaseType != null && type.BaseType != typeof(object))
            {
                return GetMetadataFor(type.BaseType.GetTypeInfo());
            }

            throw new InvalidOperationException();
        }
    }

    public interface IApiControllerMetadataProvider
    {
        ApiControllerMetadata GetMetadataFor(TypeInfo type);
        bool IsApiController(TypeInfo type);
    }
}