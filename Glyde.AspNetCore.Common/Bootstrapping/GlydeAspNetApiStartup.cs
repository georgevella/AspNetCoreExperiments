using System.Linq;
using Glyde.AspNetCore.Controllers;
using Glyde.AspNetCore.Framework;
using Glyde.AspNetCore.Versioning;
using Glyde.Web.Api.Resources;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;

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

            // register generic Restful API controller support
            mvcBuilder.PartManager.FeatureProviders.Clear();
            mvcBuilder.PartManager.FeatureProviders.Add(new GenericRestfulApiControllerFeatureProvider());
            mvcBuilder.PartManager.FeatureProviders.Add(new DefaultControllerFeatureProvider());

            services.Configure<MvcOptions>(options =>
            {
                var jsonFormatter = options.OutputFormatters.OfType<JsonOutputFormatter>().FirstOrDefault();
                if (jsonFormatter != null)
                {
                    options.OutputFormatters.Clear();
                    options.OutputFormatters.Add(jsonFormatter);
                }

                options.Conventions.Add(new ApiControllerConvension("api/v[version]", new ResourceMetadataProvider()));
                options.Conventions.Add(new ApiPrefixConvention("api/v[version]"));
            });

        }        
    }
}