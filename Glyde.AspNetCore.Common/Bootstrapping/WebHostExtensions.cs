using Microsoft.AspNetCore.Hosting;

namespace Glyde.AspNetCore.Bootstrapping
{
    public static class WebHostExtensions
    {
        public static IWebHostBuilder UseGlydeBootstrappingForMvc(this IWebHostBuilder webHostBuilder)
        {
            webHostBuilder.UseStartup<GlydeAspNetMvcStartup>();
            return webHostBuilder;
        }

        public static IWebHostBuilder UseGlydeBootstrappingForApi(this IWebHostBuilder webHostBuilder)
        {
            webHostBuilder.UseStartup<GlydeAspNetApiStartup>();
            return webHostBuilder;
        }
    }
}
