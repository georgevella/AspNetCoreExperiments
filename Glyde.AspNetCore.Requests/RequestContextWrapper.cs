using Glyde.AspNetCore.Requests.Extensions;
using Microsoft.AspNetCore.Http;

namespace Glyde.AspNetCore.Requests
{
    internal class RequestContextWrapper : IRequestContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRequestContext _wrappedInstance;

        public RequestContextWrapper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _wrappedInstance = httpContextAccessor.HttpContext.GetRequestContext();
        }

        public TData Get<TData>() where TData : class, IRequestData
        {
            return _wrappedInstance.Get<TData>();
        }
    }
}