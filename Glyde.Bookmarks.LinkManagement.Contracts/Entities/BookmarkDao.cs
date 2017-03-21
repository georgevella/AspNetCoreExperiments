using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Glyde.Bookmarks.LinkManagement.Entities
{
    [Table("bookmarks")]
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