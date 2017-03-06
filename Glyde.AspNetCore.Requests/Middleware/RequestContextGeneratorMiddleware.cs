using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Glyde.AspNetCore.Requests.Middleware
{
    public class RequestContextGeneratorMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceProvider _serviceProvider;

        public RequestContextGeneratorMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
        {
            _next = next;
            _serviceProvider = serviceProvider;
        }

        public Task Invoke(HttpContext context)
        {
            context.Items.Add(typeof (IRequestContext),
                new RequestContext(context, _serviceProvider));
            return _next(context);
        }
    }
}