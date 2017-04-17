using System.Collections.Generic;
using System.Threading.Tasks;
using Glyde.Web.Api.Resources;

namespace Glyde.Web.Api.Client
{
    public interface IApiClient<TResource>
        where TResource : IResource
    {
        Task<IEnumerable<TResource>> GetAll();
        Task<TResource> Create(TResource resource);

        Task<TResource> Update(object id, TResource resource);
        Task<TResource> Get(object id);
        Task Delete(object id);
    }

    public interface IApiClient<TResource, in TResourceId>
        where TResource : Resource<TResourceId>

    {
        Task<IEnumerable<TResource>> GetAll();
        Task<TResource> Create(TResource resource);

        Task<TResource> Update(TResourceId id, TResource resource);
        Task<TResource> Get(TResourceId id);
        Task Delete(TResourceId id);
    }
}