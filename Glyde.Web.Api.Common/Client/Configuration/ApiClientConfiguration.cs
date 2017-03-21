using System;
using System.Collections.Generic;
using Glyde.Configuration.Models;

namespace Glyde.Web.Api.Client.Configuration
{
    public class ApiClientConfiguration : ConfigurationSection
    {
        public IList<ApiClientResourceConfiguration> KnownResources { get; } = new List<ApiClientResourceConfiguration>();

        public Uri ProxyAddress { get; set; }

        public bool UseProxy { get; set; }
    }
}