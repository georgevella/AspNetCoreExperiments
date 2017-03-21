using Glyde.Configuration;
using Glyde.Di;

namespace Glyde.AspNetCore.Requests.Bootstrapping
{
    public class RequestHandlingBootstrapper : IDependencyInjectionBootstrapper
    {
        public void RegisterServices(IDependencyInjectionConfigurationBuilder serviceProviderConfigurationBuilder,
            IConfigurationService configurationService)
        {
            serviceProviderConfigurationBuilder.AddScopedService<IRequestContext, RequestContextWrapper>();
        }
    }
}