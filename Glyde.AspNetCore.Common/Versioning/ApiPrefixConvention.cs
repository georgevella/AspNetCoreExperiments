using System;
using System.Linq;
using System.Reflection;
using Glyde.AspNetCore.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Glyde.AspNetCore.Versioning
{
    public class ApiPrefixConvention : IApplicationModelConvention
    {
        private readonly string _prefix;

        public ApiPrefixConvention(string prefix)
        {
            _prefix = prefix.ToLower();
        }

        public void Apply(ApplicationModel application)
        {
            AttributeRouteModel prefixRouteModel = null;
            var requiresVersion = true;
            if (!_prefix.Contains("[version]"))
            {
                prefixRouteModel = new AttributeRouteModel(new RouteAttribute(_prefix));
                requiresVersion = false;
            }

            foreach (var controller in application.Controllers)
            {
                if (requiresVersion)
                {
                    var version = 1;
                    var versionAttribute =
                        controller.Attributes.FirstOrDefault(m => m is VersionAttribute) as VersionAttribute;
                    if (versionAttribute != null)
                        version = versionAttribute.Version;

                    prefixRouteModel =
                        new AttributeRouteModel(new RouteAttribute(_prefix.Replace("[version]", version.ToString())));
                }



                

                var matchedSelectors = controller.Selectors.Where(x => x.AttributeRouteModel != null).ToList();
                if (matchedSelectors.Any())
                {
                    foreach (var selectorModel in matchedSelectors)
                    {
                        selectorModel.AttributeRouteModel = AttributeRouteModel.CombineAttributeRouteModel(prefixRouteModel,
                            selectorModel.AttributeRouteModel);
                    }
                }
            }
        }
    }
}