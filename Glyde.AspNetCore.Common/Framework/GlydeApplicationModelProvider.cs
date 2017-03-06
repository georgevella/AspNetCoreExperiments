using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Glyde.AspNetCore.Framework
{
    public class GlydeApplicationModelProvider : IApplicationModelProvider
    {
        public void OnProvidersExecuting(ApplicationModelProviderContext context)
        {
            
        }

        public void OnProvidersExecuted(ApplicationModelProviderContext context)
        {
            
        }

        public int Order => -1000 + 10;
    }
}