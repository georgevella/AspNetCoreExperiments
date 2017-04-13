using System;
using System.Collections.Generic;
using System.Text;
using Glyde.Web.Api.Resources;

namespace Glyde.Bookmarks.LinkManagement.Resources
{
    [Resource("tags", Version = 1)]
    public class Tag : Resource<string>
    {
        public string Id { get; set; }
    }
}
