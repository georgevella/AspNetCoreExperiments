using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Glyde.Configuration.Models;

namespace Glyde.Configuration
{
    public interface IConfigurationService
    {
        T Get<T>()
            where T : ConfigurationSection;

        Task<T> GetAsync<T>()
            where T : ConfigurationSection;
    }

    public class ConfigurationService : IConfigurationService
    {
        public T Get<T>()
            where T: ConfigurationSection
        {
            throw new NotImplementedException();
        }

        public Task<T> GetAsync<T>()
            where T : ConfigurationSection
        {
            throw new NotImplementedException();
        }
    }
}
