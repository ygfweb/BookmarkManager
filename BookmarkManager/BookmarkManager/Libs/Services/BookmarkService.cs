using BookmarkManager.Libs.BLL;
using BookmarkManager.Libs.Entity;
using BookmarkManager.Libs.Model;
using BookmarkManager.Libs.Orm;
using BookmarkManager.Libs.Tools;
using BookmarkManager.Libs.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BookmarkManager.UserControls.BookmarkGrid;

namespace BookmarkManager.Libs.Services
{
    /// <summary>
    /// 书签服务
    /// </summary>
    public static class BookmarkService
    {
        public static async Task<BookmarkView> Insert(Bookmark bookmark,bool isNotAllowNet)
        {
            if (bookmark == null)
            {
                throw new ArgumentNullException(nameof(bookmark));
            }
            using (DbHelper db = new DbHelper())
            {
                // 处理URL
                bookmark.Url = bookmark.Url.Trim();
                if (await BookmarkRepository.IsExistUrl(db,bookmark.CatalogId,bookmark.Url))
                {
                    throw new Exception("该目录已存在此URL，请勿重复增加");
                }
                // 处理title
                if (string.IsNullOrWhiteSpace(bookmark.Title))
                {
                    if (!isNotAllowNet)
                    {
                        bookmark.Title = await NetHelper.GetTitle(bookmark.Url);
                    }                   
                }
                bookmark.Title = bookmark.Title.Replace(System.Environment.NewLine, " ");
                // 处理Site
                string host = NetHelper.GetHost(bookmark.Url);
                Site site = null;
                bool isNewSite = false;
                if (await SiteRepository.IsExist(db,host))
                {
                    site = await SiteRepository.GetSiteWithHostAsync(db,host);
                    isNewSite = false;
                }
                else
                {
                    byte[] icon = null;
                    if (isNotAllowNet)
                    {
                        icon = ObjectHelper.ImageToByte(BookmarkManager.Properties.Resources.Hyperlink);
                    }
                    else
                    {
                        icon = NetHelper.GetFavicon(bookmark.Url);
                    }
                    site = new Site { Id = Guid.NewGuid().ToString(), Host = host, Icon = icon };
                    isNewSite = true;
                }
                bookmark.SiteId = site.Id;
                // 修改数据库
                using (var trans = db.BeginTransaction())
                {
                    try
                    {
                        if (isNewSite)
                        {
                            await SiteRepository.Insert(db, site);
                        }
                        await BookmarkRepository.Insert(db,bookmark);
                        trans.Commit();
                    }
                    catch (Exception)
                    {
                        trans.Rollback();
                        throw;
                    }
                }
                return await BookmarkViewRepository.GetById(db, bookmark.Id);
            }
        }

        /// <summary>
        /// 修改书签
        /// </summary>
        public static async Task<BookmarkView> Update(Bookmark bookmark)
        {
            if (bookmark == null)
            {
                throw new ArgumentNullException(nameof(bookmark));
            }
            using (DbHelper db = new DbHelper())
            {
                await BookmarkRepository.Update(db, bookmark);
                return await BookmarkViewRepository.GetById(db, bookmark.Id);
            }
        }
        /// <summary>
        /// 删除书签
        /// </summary>
        public static async Task<int> Delete(Bookmark bookmark)
        {
            if (bookmark == null)
            {
                throw new ArgumentNullException(nameof(bookmark));
            }
            using (DbHelper db= new DbHelper())
            {
               return await BookmarkRepository.Delete(db,bookmark);
            }
        }

        /// <summary>
        /// 剪切数据
        /// </summary>
        public static async Task<List<BookmarkView>> Cut(ClipboardData data, CatalogModel model)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            List<BookmarkView> bookmarks = new List<BookmarkView>();
            using (DbHelper db = new DbHelper())
            {
                using (var trans = db.BeginTransaction())
                {
                    try
                    {
                        foreach (var item in data.BookmarkList)
                        {
                            await db.ExecuteNonQuery("UPDATE [Bookmark] SET [CatalogId]=@CatalogId WHERE [Id]=@Id;", new { CatalogId = model.Id, Id = item });
                        }
                        trans.Commit();
                        foreach (var item in data.BookmarkList)
                        {
                            BookmarkView bookmarkView = await db.SelectOne<BookmarkView>("select * from [BookmarkView] where Id=@Id;", new { Id = item });
                            bookmarks.Add(bookmarkView);
                        }
                    }
                    catch (Exception)
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
            return bookmarks;
        }

        /// <summary>
        /// 获取指定目录内的书签集合
        /// </summary>
        public static async Task<List<BookmarkView>> GetListByCatalog(string catalogId)
        {
            if (string.IsNullOrWhiteSpace(catalogId))
            {
                return new List<BookmarkView>();
            }
            else
            {
                using (DbHelper db = new DbHelper())
                {
                    return await BookmarkViewRepository.GetListByCatalog(db, catalogId);
                }
            }
        }
    }
}
