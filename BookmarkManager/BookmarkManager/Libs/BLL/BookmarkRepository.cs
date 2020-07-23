using BookmarkManager.Libs.Entity;
using BookmarkManager.Libs.Orm;
using BookmarkManager.Libs.Tools;
using BookmarkManager.Libs.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static BookmarkManager.UserControls.BookmarkGrid;

namespace BookmarkManager.Libs.BLL
{
    public class BookmarkRepository
    {
        /// <summary>
        /// 获取指定目录的书签
        /// </summary>
        public static async Task<List<Bookmark>> GetListByCatalog(DbHelper db, string catalogId)
        {
            if (db == null)
            {
                throw new ArgumentNullException(nameof(db));
            }
            return await db.SelectMore<Bookmark>("select * from Bookmark where [CatalogId] = @CatalogId;", new { CatalogId = catalogId });
        }

        /// <summary>
        /// 获取指定目录内的书签总数
        /// </summary>
        public static async Task<int> GetCountByCatalog(DbHelper db,string catalogId)
        {
            if (db == null)
            {
                throw new ArgumentNullException(nameof(db));
            }
            return await db.SelectIntScalar("select count(*) from [Bookmark] where [CatalogId] = @CatalogId;", new { CatalogId = catalogId});
        }

        /// <summary>
        /// 获取指定ID的书签
        /// </summary>
        public static async Task<Bookmark> GetById(DbHelper db,string id)
        {
            if (db == null)
            {
                throw new ArgumentNullException(nameof(db));
            }
            return await db.SelectOne<Bookmark>("select * from Bookmark where [Id] = @Id;", new { Id = id });
        }

        /// <summary>
        /// 检查所属目录是否存在指定URL的书签
        /// </summary>
        public static async Task<bool> IsExistUrl(DbHelper db, string catalogId, string url)
        {
            if (db == null)
            {
                throw new ArgumentNullException(nameof(db));
            }
            int count = await db.SelectIntScalar("select count(*) from [Bookmark] where [CatalogId]=@CatalogId and [Url]=@Url;", new { CatalogId = catalogId, Url = url });
            return count > 0;
        }

        /// <summary>
        /// 获取所有书签
        /// </summary>
        /// <returns></returns>
        public static async Task<List<BookmarkView>> GetAll(DbHelper db)
        {
            if (db == null)
            {
                throw new ArgumentNullException(nameof(db));
            }
            return await db.SelectMore<BookmarkView>("select * from [BookmarkView];");
        }

        /// <summary>
        /// 插入书签
        /// </summary>
        public static async Task<int> Insert(DbHelper db, Bookmark bookmark)
        {
            if (db == null)
            {
                throw new ArgumentNullException(nameof(db));
            }
            if (bookmark == null)
            {
                throw new ArgumentNullException(nameof(bookmark));
            }
            if (string.IsNullOrWhiteSpace(bookmark.Id))
            {
                throw new Exception("增加的书签ID不能为空");
            }
            if (string.IsNullOrWhiteSpace(bookmark.CatalogId))
            {
                throw new Exception("书签的所属目录不能为空");
            }
            if (await CatalogRepository.IsExist(db,bookmark.CatalogId) == false)
            {
                throw new Exception("书签所属的目录不存在或已被删除");
            }
            return await db.Insert(bookmark);
        }

        public static async Task<int> Update(DbHelper db,Bookmark bookmark)
        {
            if (db == null)
            {
                throw new ArgumentNullException(nameof(db));
            }
            if (bookmark == null)
            {
                throw new ArgumentNullException(nameof(bookmark));
            }
            if (string.IsNullOrWhiteSpace(bookmark.Id))
            {
                throw new Exception("增加的书签ID不能为空");
            }
            if (string.IsNullOrWhiteSpace(bookmark.CatalogId))
            {
                throw new Exception("书签的所属目录不能为空");
            }
            if (await CatalogRepository.IsExist(db, bookmark.CatalogId) == false)
            {
                throw new Exception("书签所属的目录不存在或已被删除");
            }
            return await db.Update(bookmark);
        }

        public static async Task<int> Delete(DbHelper db,Bookmark bookmark)
        {
            if (db == null)
            {
                throw new ArgumentNullException(nameof(db));
            }
            if (bookmark == null)
            {
                throw new ArgumentNullException(nameof(bookmark));
            }
            return await db.Delete(bookmark);
        }
    }
}
