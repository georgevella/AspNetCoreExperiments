using System;
using Glyde.Di.Bootstrapping;

namespace Glyde.Di.SimpleInjector
{
    public class SimpleInjectorDiBootstrapperStage : DependencyInjectionBootstrapperStage
    {
        private readonly IDependencyInjectionConfigurationBuilder _configurationBuilder;

        public SimpleInjectorDiBootstrapperStage(IDependencyInjectionConfigurationBuilder configurationBuilder)
        {
            _configurationBuilder = configurationBuilder;
        }

        protected override IDependencyInjectionConfigurationBuilder CreateConfigurationBuilder()
        {
            return _configurationBuilder;
        }
    }
}