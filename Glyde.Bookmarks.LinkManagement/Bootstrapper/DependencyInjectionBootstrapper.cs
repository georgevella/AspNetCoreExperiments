using Glyde.Bookmarks.LinkManagement.Controllers;
using Glyde.Bookmarks.LinkManagement.Services;
using Glyde.Configuration;
using Glyde.Di;

namespace Glyde.Bookmarks.LinkManagement.Bootstrapper
{
    public class DependencyInjectionBootstrapper : IDependencyInjectionBootstrapper
    {
        public void RegisterServices(IDependencyInjectionConfigurationBuilder serviceProviderConfigurationBuilder,
            IConfigurationService configurationService)
        {
            serviceProviderConfigurationBuilder.AddScopedService<IBookmarkStorage, BookmarkStorage>();
        }
    }
}