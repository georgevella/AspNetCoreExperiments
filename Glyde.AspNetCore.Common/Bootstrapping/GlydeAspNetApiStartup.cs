using System.Linq;
using Glyde.AspNetCore.ApiExplorer;
using Glyde.AspNetCore.Controllers;
using Glyde.AspNetCore.Framework;
using Glyde.AspNetCore.Versioning;
using Glyde.Web.Api.Controllers;
using Glyde.Web.Api.Resources;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Glyde.AspNetCore.Bootstrapping
{
    public class GlydeAspNetApiStartup : CommonGlydeAspNetStartup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        public GlydeAspNetApiStartup(IHostingEnvironment env) : base(env)
        {

        }

        public void ConfigureServices(IServiceCollection services)
        {            
            var mvcBuilder = services.AddMvcCore().AddJsonFormatters();

            ConfigureGlydeServices(mvcBuilder.PartManager, services);

            var apiControllerMetadataProvider = new ApiControllerMetadataProvider();

            // register generic Restful API controller support
            mvcBuilder.PartManager.FeatureProviders.Clear();
            mvcBuilder.PartManager.FeatureProviders.Add(new ApiControllerFeatureProvider(apiControllerMetadataProvider));
            mvcBuilder.PartManager.FeatureProviders.Add(new DefaultControllerFeatureProvider(apiControllerMetadataProvider));

            mvcBuilder.AddApiExplorer();

            services.Configure<MvcOptions>(options =>
            {
                var jsonFormatter = options.OutputFormatters.OfType<JsonOutputFormatter>().FirstOrDefault();
                if (jsonFormatter != null)
                {
                    options.OutputFormatters.Clear();
                    options.OutputFormatters.Add(jsonFormatter);
                }

                options.Conventions.Add(new ApiControllerConvention("v[version]", apiControllerMetadataProvider, new ResourceMetadataProvider()));
                options.Conventions.Add(new DefaultControllerVersioningConvention("v[version]"));
                options.Conventions.Add(new ApiExplorerVisibilityEnabledConvention());
            });
        }
    }
}