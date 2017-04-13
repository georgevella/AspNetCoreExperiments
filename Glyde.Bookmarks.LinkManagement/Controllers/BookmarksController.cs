using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Glyde.AspNetCore.Controllers;
using Glyde.Bookmarks.LinkManagement.Entities;
using Glyde.Bookmarks.LinkManagement.Resources;
using Glyde.Bookmarks.LinkManagement.Services;
using Glyde.Web.Api.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace Glyde.Bookmarks.LinkManagement.Controllers
{
    public class BookmarksController : ApiController<Bookmark, string>
    {
        private readonly IBookmarkStorage _bookmarkStorage;

        public override async Task<string> Create(Bookmark bookmark)
        {
            var dao = new BookmarkDao()
            {
                Id = Guid.NewGuid().ToString("D"),
                Title = bookmark.Title,
                Url = bookmark.Url
            };

            await _bookmarkStorage.AddBookmark(dao);

            return dao.Id;
        }

        public override async Task<IEnumerable<Bookmark>> GetAll()
        {
            var bookmarksDao = await _bookmarkStorage.GetBookmarks();

            return bookmarksDao.Select(x => new Bookmark()
            {
                Url = x.Url,
                Title = x.Title
            }).ToList();
        }

        public BookmarksController(IBookmarkStorage bookmarkStorage)
        {
            _bookmarkStorage = bookmarkStorage;
        }
    }
}