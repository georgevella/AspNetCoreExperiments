using Glyde.Web.Api.Resources;

namespace Glyde.Web.Api.Client
{
    public interface IApiClientFactory
    {
        IApiClient<TResource> GetClientFor<TResource>()
            where TResource : IResource;

        IApiClient<TResource, TResourceId> GetClientFor<TResource, TResourceId>()
            where TResource : Resource<TResourceId>;
    }
}