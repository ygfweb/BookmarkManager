using BookmarkManager.Libs.Entity;
using BookmarkManager.Libs.Orm;
using BookmarkManager.Libs.Tools;
using BookmarkManager.Libs.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookmarkManager.Libs.BLL
{
    public class CatalogRepository
    {
        public static async Task<int> Insert(DbHelper db, Catalog catalog)
        {
            if (db == null)
            {
                throw new ArgumentNullException(nameof(db));
            }
            if (catalog == null)
            {
                throw new ArgumentNullException(nameof(catalog));
            }
            return await db.Insert(catalog);
        }

        public static async Task<int> Update(DbHelper db, Catalog catalog)
        {
            if (db == null)
            {
                throw new ArgumentNullException(nameof(db));
            }
            if (catalog == null)
            {
                throw new ArgumentNullException(nameof(catalog));
            }
            return await db.Update(catalog);
        }

        /// <summary>
        /// 获取子目录数量
        /// </summary>
        public static async Task<int> GetChildCount(DbHelper db, string parentCatalogId)
        {
            if (db == null)
            {
                throw new ArgumentNullException(nameof(db));
            }
            return await db.SelectIntScalar("select count(*) from [Catalog] where [ParentId] = @ParentId;", new { ParentId = parentCatalogId });
        }

        /// <summary>
        /// 获取指定的目录，如果不存在，则返回NULL
        /// </summary>
        /// <param name="id"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public static async Task<Catalog> GetCatalog(DbHelper db,string id)
        {
            if (db == null)
            {
                throw new ArgumentNullException(nameof(db));
            }
            return await db.SelectOne<Catalog>("select * from [Catalog] where [Id]=@Id  ORDER BY [Order] DESC;", new { Id = id });
        }


        public static async Task<int> Delete(DbHelper db, Catalog catalog)
        {
            if (db == null)
            {
                throw new ArgumentNullException(nameof(db));
            }
            if (catalog == null)
            {
                throw new ArgumentNullException(nameof(catalog));
            }
            int bookmarkCount = await BookmarkRepository.GetCountByCatalog(db, catalog.Id);
            if (bookmarkCount > 0)
            {
                throw new Exception("该目录拥有部分收藏夹，禁止删除");
            }
            int childCount = await GetChildCount(db,catalog.Id);
            if (childCount > 0)
            {
                throw new Exception("该目录拥有部分子目录，禁止删除");
            }
            return await db.Delete(catalog);
        }

        public static async Task<List<Catalog>> GetAll(DbHelper db)
        {
            if (db == null)
            {
                throw new ArgumentNullException(nameof(db));
            }
            return await db.SelectMore<Catalog>("select * from [Catalog] ORDER BY [Order] DESC;");
        }

        public static async Task<bool> IsExist(DbHelper db, string id)
        {
            if (db == null)
            {
                throw new ArgumentNullException(nameof(db));
            }
            int count = await db.SelectIntScalar("select count(*)  from [Catalog] where [Id]=@id", new { Id = id });
            return count > 0;
        }
    }
}
