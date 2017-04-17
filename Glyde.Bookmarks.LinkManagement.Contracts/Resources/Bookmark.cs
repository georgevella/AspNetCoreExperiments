using System.Collections.Generic;
using Glyde.Web.Api.Resources;

namespace Glyde.Bookmarks.LinkManagement.Resources
{
    [Resource("bookmarks", Version = 1)]
    public class Bookmark : Resource<string>
    {
        public string Title { get; set; }   

        public string Url { get; set; }

        public string Id { get; set; }

        public IEnumerable<string> Tags { get; set; }
    }
}