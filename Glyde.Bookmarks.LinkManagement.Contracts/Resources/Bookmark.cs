using Glyde.AspNetCore.Controllers;

namespace Glyde.Bookmarks.LinkManagement.Resources
{
    [ResourceName("bookmarks")]
    public class Bookmark
    {
        public string Title { get; set; }   

        public string Url { get; set; }


    }
}