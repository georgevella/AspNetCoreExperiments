using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Glyde.AspNetCore.Bootstrapping;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;

namespace Glyde.Bookmarks.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls("http://0.0.0.0:8181/")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseGlydeBootstrappingForApi()
                .Build();

            host.Run();
        }
    }
}
