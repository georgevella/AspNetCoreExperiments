using System;
using System.Linq;
using Glyde.AspNetCore.Controllers;
using Glyde.Bookmarks.LinkManagement.Entities;
using Glyde.Bookmarks.LinkManagement.Resources;
using Glyde.Bookmarks.LinkManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace Glyde.Bookmarks.LinkManagement.Controllers
{
    public class BookmarksController : ApiController<Bookmark, string>
    {
        public BookmarksController(IBookmarkStorage bookmarkStorage)
        {
            CreateAction = async bookmark =>
            {
                var dao = new BookmarkDao()
                {
                    Id = Guid.NewGuid().ToString("D"),
                    Title = bookmark.Title,
                    Url = bookmark.Url
                };

                await bookmarkStorage.AddBookmark(dao);

                return dao.Id;
            };

            GetAllAction = async () =>
            {
                var bookmarksDao = await bookmarkStorage.GetBookmarks();

                return bookmarksDao.Select(x => new Bookmark()
                {
                    Url = x.Url,
                    Title = x.Title
                }).ToList();
            };
        }
    }
}