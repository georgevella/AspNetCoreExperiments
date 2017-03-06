using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Glyde.AspNetCore.Requests
{
    public class RequestContext : IRequestContext
    {
        private readonly HttpContext _context;
        private readonly IServiceProvider _serviceProvider;
        private readonly IDictionary<Type, object> _resultSet = new Dictionary<Type, object>();

        public TData Get<TData>() where TData : class, IRequestData
        {
            var dataType = typeof (TData);
            if (!_resultSet.ContainsKey(dataType))
            {
                try
                {
                    var service = _serviceProvider.GetService<IRequestContextDataGenerator<TData>>();
                    _resultSet[dataType] = service?.GenerateData(_context);
                }
                catch
                {
                    _resultSet[dataType] = null;
                }
            }

            return _resultSet[dataType] as TData;
        }

        public RequestContext(HttpContext context, IServiceProvider serviceProvider)
        {
            _context = context;
            _serviceProvider = serviceProvider;
        }
    }
}