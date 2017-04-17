using System;
using System.Linq;
using System.Reflection;
using Glyde.AspNetCore.Controllers;
using Glyde.Web.Api.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Glyde.AspNetCore.Versioning
{
    public class DefaultControllerVersioningConvention : IApplicationModelConvention
    {
        private readonly string _prefix;
        private readonly bool _requiresVersion;

        public DefaultControllerVersioningConvention(string prefix)
        {
            if (prefix == null)
                throw new ArgumentNullException(nameof(prefix));

            _prefix = prefix.ToLower();
            _requiresVersion = _prefix.Contains("[version]");
        }

        public void Apply(ApplicationModel application)
        {
            AttributeRouteModel prefixRouteModel = null;
            if (!_requiresVersion)
            {
                prefixRouteModel = new AttributeRouteModel(new RouteAttribute(_prefix));
            }

            foreach (var controller in application.Controllers)
            {
                if (_requiresVersion && !controller.ControllerType.GetCustomAttributes<IgnoreVersioningConventionAttribute>(true).Any())
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