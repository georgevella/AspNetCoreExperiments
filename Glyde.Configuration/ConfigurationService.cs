using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Glyde.Configuration.Models;

namespace Glyde.Configuration
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly Dictionary<Type, ConfigurationSection> _configurationSections;

        public ConfigurationService(IEnumerable<ConfigurationSection> configurationSections )
        {
            _configurationSections = configurationSections.ToDictionary(x => x.GetType());
        }
        public T Get<T>()
            where T: ConfigurationSection
        {
            return (T)_configurationSections[typeof(T)];
        }
    }
}