using System;
using System.Collections.Generic;

namespace Glyde.Web.Api.Client.Configuration
{
    public class ApiClientResourceConfiguration
    {
        public IList<string> ResourceNames { get; } = new List<string>();

        public Uri BaseAddress { get; set; }

        public Uri ProxyAddress { get; set; }

        public bool? UseProxy { get; set; }
    }
}