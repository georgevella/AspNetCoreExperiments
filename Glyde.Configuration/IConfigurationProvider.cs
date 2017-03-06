using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Glyde.Configuration
{
    public interface IConfigurationProvider
    {
        T Get<T>();

        Task<T> GetAsync<T>();
    }

    public class ConfigurationProvider : IConfigurationProvider
    {
        public T Get<T>()
        {
            throw new NotImplementedException();
        }

        public Task<T> GetAsync<T>()
        {
            throw new NotImplementedException();
        }
    }
}
