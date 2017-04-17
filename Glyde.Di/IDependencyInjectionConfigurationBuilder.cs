namespace Glyde.Di
{
    public interface IDependencyInjectionConfigurationBuilder
    {
        void AddTransientService<TContract, TService>()
            where TService : class, TContract where TContract : class;

        void AddSingletonService<TContract, TService>()
            where TService : class, TContract where TContract : class;

        void AddSingletonService<TService>()
            where TService : class;

        void AddScopedService<TContract, TService>()
            where TService : class, TContract where TContract : class;
    }
}