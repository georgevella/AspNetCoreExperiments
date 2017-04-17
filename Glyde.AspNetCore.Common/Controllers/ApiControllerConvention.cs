using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Glyde.Web.Api.Controllers;
using Glyde.Web.Api.Resources;
using Glyde.Web.Api.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Glyde.AspNetCore.Controllers
{
    public class ApiControllerConvention : IApplicationModelConvention
    {
        private readonly string _prefix;
        private readonly IApiControllerMetadataProvider _apiControllerMetadataProvider;
        private readonly IResourceMetadataProvider _resourceMetadataProvider;

        public ApiControllerConvention(string prefix, IApiControllerMetadataProvider apiControllerMetadataProvider, IResourceMetadataProvider resourceMetadataProvider)
        {
            if (prefix == null)
                throw new ArgumentNullException(nameof(prefix));
            if (apiControllerMetadataProvider == null)
                throw new ArgumentNullException(nameof(apiControllerMetadataProvider));
            if (resourceMetadataProvider == null)
                throw new ArgumentNullException(nameof(resourceMetadataProvider));

            _prefix = prefix;
            _apiControllerMetadataProvider = apiControllerMetadataProvider;
            _resourceMetadataProvider = resourceMetadataProvider;

            if (!_prefix.Contains("[version]"))
            {
                throw new ArgumentException($"[version] placement not available in prefix '{prefix}'.", nameof(prefix));
            }
        }

        public void Apply(ApplicationModel application)
        {
            foreach (var controller in application.Controllers)
            {
                if (!IsWrappedApiController(controller.ControllerType, out TypeInfo resourceType, out TypeInfo resourceIdType))
                    continue;
                
                var resourceMetadata = _resourceMetadataProvider.GetMetadataFor(resourceType);

                foreach (var controllerSelector in controller.Selectors)
                {
                    var finalRouteModel = AttributeRouteModel.CombineAttributeRouteModel(
                        new AttributeRouteModel(
                            new RouteAttribute($"{_prefix.Replace("[version]", resourceMetadata.Version.ToString())}")),
                        new AttributeRouteModel(new RouteAttribute(resourceMetadata.Name))
                    );
                    controllerSelector.AttributeRouteModel = finalRouteModel;
                }

                controller.ControllerName = $"{resourceMetadata.Name}Controller";
            }
        }

        public static bool IsWrappedApiController(TypeInfo type, out TypeInfo resourceType, out TypeInfo resourceIdType)
        {
            resourceType = null;
            resourceIdType = null;

            if (type.AsType() == typeof(object))
            {
                // object is the root of all
                return false;
            }

            if (type.IsGenericType &&
                type.GetGenericTypeDefinition() == typeof(ApiControllerWrapper<,,>))
            {
                var typeArguments = type.GenericTypeArguments;

                resourceType = typeArguments[1].GetTypeInfo();
                resourceIdType = typeArguments[2].GetTypeInfo();

                return true;
            }

            if (type.BaseType != null && type.BaseType != typeof(object))
            {
                return IsWrappedApiController(type.BaseType.GetTypeInfo(), out resourceType, out resourceIdType);
            }

            return false;
        }
    }
}