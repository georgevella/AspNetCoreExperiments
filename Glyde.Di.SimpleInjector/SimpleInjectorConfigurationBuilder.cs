using System;
using SimpleInjector;

namespace Glyde.Di.SimpleInjector
{
    public class SimpleInjectorConfigurationBuilder : IDependencyInjectionConfigurationBuilder
    {
        private readonly Container _container;

        public SimpleInjectorConfigurationBuilder(Container container)
        {
            _container = container;
        }

        public void AddTransientService<TContract, TService>() where TContract : class where TService : class, TContract
        {
            _container.Register<TContract, TService>(Lifestyle.Transient);
        }

        public void AddSingletonService<TContract, TService>() where TContract : class where TService : class, TContract
        {
            _container.Register<TContract, TService>(Lifestyle.Singleton);
        }

        public void AddScopedService<TContract, TService>() where TContract : class where TService : class, TContract
        {
            _container.Register<TContract, TService>(Lifestyle.Scoped);
        }

        public void AddScopedService<TContract>(Func<TContract> factoryMethod ) where TContract : class
        {
            _container.Register(factoryMethod, Lifestyle.Scoped);
        }
    }
}