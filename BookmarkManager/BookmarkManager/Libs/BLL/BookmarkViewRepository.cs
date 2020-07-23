using BookmarkManager.Libs.Entity;
using BookmarkManager.Libs.Orm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookmarkManager.Libs.BLL
{
    public static class BookmarkViewRepository
    {
        /// <summary>
        /// 获取指定目录的书签
        /// </summary>
        public static async Task<List<BookmarkView>> GetListByCatalog(DbHelper db,string catalogId)
        {
            if (db == null)
            {
                throw new ArgumentNullException(nameof(db));
            }
            return await db.SelectMore<BookmarkView>("select * from [BookmarkView] where [CatalogId]=@CatalogId;", new { CatalogId = catalogId });
        }
        /// <summary>
        /// 获取指定ID的书签
        /// </summary>
        public static async Task<BookmarkView> GetById(DbHelper db,string id)
        {
            if (db == null)
            {
                throw new ArgumentNullException(nameof(db));
            }
            return await db.SelectOne<BookmarkView>("select * from [BookmarkView] where Id=@Id;", new { Id = id });
        }
        /// <summary>
        /// 获取所有书签
        /// </summary>
        public static async Task<List<BookmarkView>> GetAll(DbHelper db)
        {
            if (db == null)
            {
                throw new ArgumentNullException(nameof(db));
            }
            return await db.SelectMore<BookmarkView>("select * from [BookmarkView];");
        }
    }
}

