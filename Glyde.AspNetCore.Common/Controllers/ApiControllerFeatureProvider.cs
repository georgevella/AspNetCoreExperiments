using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Glyde.Web.Api.Controllers;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Glyde.AspNetCore.Controllers
{
    public class ApiControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        private readonly IApiControllerMetadataProvider _apiControllerMetadataProvider;
        private static readonly Type WrapperType = typeof(ApiControllerWrapper<,,>);

        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            foreach (var part in parts.OfType<AssemblyPart>())
            {
                var wrapperTypes = part.Types.Where(_apiControllerMetadataProvider.IsApiController).Select(MakeApiControllerWrapper).ToList();
                foreach (var wrapperType in wrapperTypes)
                {
                    feature.Controllers.Add(wrapperType);
                }
            }            
        }

        public ApiControllerFeatureProvider(IApiControllerMetadataProvider apiControllerMetadataProvider)
        {
            _apiControllerMetadataProvider = apiControllerMetadataProvider;
        }

        public TypeInfo MakeApiControllerWrapper(TypeInfo type)
        {
            var apiControllerMetadata = _apiControllerMetadataProvider.GetMetadataFor(type);
            
            var specificWrapperType = WrapperType.MakeGenericType(
                type.AsType(), 
                apiControllerMetadata.ResourceType.AsType(), 
                apiControllerMetadata.ResourceIdType.AsType()
                );

            return specificWrapperType.GetTypeInfo();
        }
    }
}