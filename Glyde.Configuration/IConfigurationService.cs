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
    }
}
