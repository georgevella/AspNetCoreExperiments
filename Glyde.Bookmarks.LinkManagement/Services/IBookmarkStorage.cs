using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Glyde.Bookmarks.LinkManagement.Entities;
using Glyde.NoSql.AzureTables;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace Glyde.Bookmarks.LinkManagement.Services
{
    public interface IBookmarkStorage
    {
        Task<IEnumerable<BookmarkDao>> GetBookmarks();

        Task AddBookmark(BookmarkDao bookmarkDao);
    }

    class BookmarkStorage : IBookmarkStorage
    {
        private static readonly AzureTablesDataStore Store = new AzureTablesDataStore();

        public BookmarkStorage()
        {
            
        }

        public async Task<IEnumerable<BookmarkDao>> GetBookmarks()
        {
            return await Store.GetAll<BookmarkDao>();
        }

        public async Task AddBookmark(BookmarkDao bookmarkDao)
        {
            await Store.Add(bookmarkDao);
        }
    }
}