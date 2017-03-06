using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.DependencyInjection;

namespace Glyde.AspNetCore.Bootstrapping
{
    public class GlydeAspNetMvcStartup : CommonGlydeAspNetStartup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        public GlydeAspNetMvcStartup(IHostingEnvironment env) : base(env)
        {

        }

        public void ConfigureServices(IServiceCollection services)
        {
            var mvcBuilder = services.AddMvc();

            ConfigureGlydeServices(mvcBuilder.PartManager, services);
        }        
    }
}
