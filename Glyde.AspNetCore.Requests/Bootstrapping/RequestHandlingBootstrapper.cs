using Glyde.Configuration;
using Glyde.Di;

namespace Glyde.AspNetCore.Requests.Bootstrapping
{
    public class RequestHandlingBootstrapper : IDependencyInjectionBootstrapper
    {
        public void RegisterServices(IDependencyInjectionConfigurationBuilder serviceProviderConfigurationBuilder,
            IConfigurationProvider configurationProvider)
        {
            serviceProviderConfigurationBuilder.AddScopedService<IRequestContext, RequestContextWrapper>();
        }
    }
}