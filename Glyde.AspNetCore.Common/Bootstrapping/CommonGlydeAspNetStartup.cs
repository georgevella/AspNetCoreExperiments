using System;
using System.Linq;
using System.Reflection;
using Glyde.AspNetCore.ApiExplorer;
using Glyde.AspNetCore.Controllers;
using Glyde.AspNetCore.Versioning;
using Glyde.Di.SimpleInjector;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Logging;
using SimpleInjector;
using SimpleInjector.Integration.AspNetCore;
using SimpleInjector.Integration.AspNetCore.Mvc;

namespace Glyde.AspNetCore.Bootstrapping
{
    public abstract class CommonGlydeAspNetStartup
    {
        private readonly Container _container = new Container();
        private readonly SimpleInjectorDiBootstrapperStage _diBootstrapperStage;

        protected CommonGlydeAspNetStartup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            _container.Options.DefaultScopedLifestyle = new AspNetRequestLifestyle();

            _diBootstrapperStage = new SimpleInjectorDiBootstrapperStage(new SimpleInjectorConfigurationBuilder(_container));
        }

        public IConfigurationRoot Configuration { get; }

        protected void ConfigureGlydeServices(ApplicationPartManager applicationPartManager, IServiceCollection services)
        {
            _container.RegisterSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //services.AddTransient<IApplicationModelProvider, GlydeApplicationModelProvider>();

            // add application services via bootstrapping
            var thisAssembly = Assembly.GetEntryAssembly();

            var dependencyContext = DependencyContext.Load(thisAssembly);
            var ownAssemblies = dependencyContext.RuntimeLibraries
                .Where(r => r.Name.StartsWith("Glyde", StringComparison.OrdinalIgnoreCase))
                .SelectMany(l => l.GetDefaultAssemblyNames(dependencyContext).Select(Assembly.Load))
                .ToList();
            ownAssemblies.Add(thisAssembly);

            _diBootstrapperStage.Run(ownAssemblies);
            ownAssemblies.ForEach(assembly => applicationPartManager.ApplicationParts.Add(new AssemblyPart(assembly)));

            // setup di integration
            services.AddSingleton<IControllerActivator>(new SimpleInjectorControllerActivator(_container));
            services.AddSingleton<IViewComponentActivator>(new SimpleInjectorViewComponentActivator(_container));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public virtual void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseSimpleInjectorAspNetRequestScoping(_container);

            app.UseMvc();
        }
    }
}