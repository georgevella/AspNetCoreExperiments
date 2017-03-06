using System;

namespace Glyde.Bookmarks.LinkManagement.Entities
{
    public class BookmarkStatisticsDao
    {
        public int AccessCount { get; set; }

        public DateTime? LastAccessed { get; set; }
    }
}