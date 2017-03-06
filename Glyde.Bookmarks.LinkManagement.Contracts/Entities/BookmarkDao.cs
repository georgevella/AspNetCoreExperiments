using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.WindowsAzure.Storage.Table;

namespace Glyde.Bookmarks.LinkManagement.Entities
{
    public class BookmarkDao
    {
        [Key]
        public string Id { get; set; }

        public string Title { get; set; }
        
        public string Url { get; set; }

        public List<TagReferenceDao> Tags { get; set; } = new List<TagReferenceDao>();

        public BookmarkStatisticsDao Statistics { get; set; } = new BookmarkStatisticsDao();

        public DateTime CreationDateTime { get; set; } = DateTime.Now;

        public DateTime LastModifiedDateTime { get; set; } = DateTime.Now;
    }
}