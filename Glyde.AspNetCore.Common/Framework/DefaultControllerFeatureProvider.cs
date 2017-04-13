using System.Reflection;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Glyde.AspNetCore.Framework
{
    public class DefaultControllerFeatureProvider : ControllerFeatureProvider
    {
        protected override bool IsController(TypeInfo typeInfo)
        {
            if (GenericRestfulApiControllerFeatureProvider.IsApiController(typeInfo))
            {
                // these types of controllers are registered and handled by someone else
                return false;
            }

            return base.IsController(typeInfo);
        }
    }
}