using System.Collections.Generic;
using System.Reflection;
using Glyde.Bootstrapper;
using Glyde.Configuration;

namespace Glyde.Di.Bootstrapping
{
    public abstract class DependencyInjectionBootstrapperStage : BootstrapperStage<IDependencyInjectionBootstrapper>
    {
        public override void Run(IEnumerable<Assembly> assemblies)
        {
            var bootstrappers = GetBootstrappers(assemblies);
            var serviceProviderConfigurator = CreateConfigurationBuilder();

            foreach (var bootstrapper in bootstrappers)
            {
                bootstrapper.RegisterServices(serviceProviderConfigurator, ConfigurationProvider);
            }
        }

        protected abstract IDependencyInjectionConfigurationBuilder CreateConfigurationBuilder();

        public IConfigurationProvider ConfigurationProvider { get; set; }
    }
}