using System.Reflection;
using Glyde.AspNetCore.Controllers;
using Glyde.Web.Api.Controllers;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Glyde.AspNetCore.Framework
{
    public class DefaultControllerFeatureProvider : ControllerFeatureProvider
    {
        private readonly IApiControllerMetadataProvider _apiControllerMetadataProvider;

        public DefaultControllerFeatureProvider(IApiControllerMetadataProvider apiControllerMetadataProvider)
        {
            _apiControllerMetadataProvider = apiControllerMetadataProvider;
        }
        protected override bool IsController(TypeInfo typeInfo)
        {
            if (_apiControllerMetadataProvider.IsApiController(typeInfo))
            {
                // these types of controllers are registered and handled by someone else
                return false;
            }

            return base.IsController(typeInfo);
        }
    }
}