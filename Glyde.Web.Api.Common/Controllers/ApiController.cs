using System.Collections.Generic;
using System.Threading.Tasks;
using Glyde.Web.Api.Resources;

namespace Glyde.Web.Api.Controllers
{

    public abstract class ApiController<TResource, TResourceId> : IApiController<TResource, TResourceId>
        where TResource : Resource<TResourceId>
    {
        public virtual async Task<IEnumerable<TResource>> GetAll()
        {
            throw new System.NotImplementedException();
        }

        public virtual async Task<TResource> Get(TResourceId id)
        {
            throw new System.NotImplementedException();
        }

        public virtual async Task<bool> Update(TResourceId id, TResource resource)
        {
            throw new System.NotImplementedException();
        }

        public virtual async Task<TResourceId> Create(TResource resource)
        {
            throw new System.NotImplementedException();
        }

        public virtual async Task<bool> Delete(TResourceId id)
        {
            throw new System.NotImplementedException();
        }
    }

    public interface IApiController<TResource, TResourceId>
        where TResource : Resource<TResourceId>
    {
        Task<IEnumerable<TResource>> GetAll();
        Task<TResource> Get(TResourceId id);
        Task<bool> Update(TResourceId id, TResource resource);
        Task<TResourceId> Create(TResource resource);
        Task<bool> Delete(TResourceId id);
    }
}