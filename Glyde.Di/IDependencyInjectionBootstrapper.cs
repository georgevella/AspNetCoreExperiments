using Glyde.Bootstrapper;
using Glyde.Configuration;

namespace Glyde.Di
{
    public interface IDependencyInjectionBootstrapper : IBootstrapper
    {
        void RegisterServices(IDependencyInjectionConfigurationBuilder serviceProviderConfigurationBuilder, IConfigurationService configurationService);
    }
}