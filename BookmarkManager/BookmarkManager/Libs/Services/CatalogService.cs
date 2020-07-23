using BookmarkManager.Libs.BLL;
using BookmarkManager.Libs.Entity;
using BookmarkManager.Libs.Orm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookmarkManager.Libs.Services
{
    public static class CatalogService
    {
        public static async Task<int> Insert(Catalog catalog)
        {
            if (catalog == null)
            {
                throw new ArgumentNullException(nameof(catalog));
            }
            if (string.IsNullOrWhiteSpace(catalog.Id))
            {
                throw new Exception("目录ID不能为空");
            }
            using (DbHelper db= new DbHelper())
            {
                return await CatalogRepository.Insert(db, catalog);
            }
        }

        public static async Task<int> Update(Catalog catalog)
        {
            if (catalog == null)
            {
                throw new ArgumentNullException(nameof(catalog));
            }
            if (string.IsNullOrWhiteSpace(catalog.Id))
            {
                throw new Exception("目录ID不能为空");
            }
            using (DbHelper db = new DbHelper())
            {
                return await CatalogRepository.Update(db, catalog);
            }
        }
        public static async Task<int> Delete(Catalog catalog)
        {
            if (catalog == null)
            {
                throw new ArgumentNullException(nameof(catalog));
            }
            if (string.IsNullOrWhiteSpace(catalog.Id))
            {
                throw new Exception("目录ID不能为空");
            }
            using (DbHelper db = new DbHelper())
            {
                return await CatalogRepository.Delete(db, catalog);
            }
        }

        public static async Task<List<Catalog>> GetAll()
        {
            using (DbHelper db = new DbHelper())
            {
                return await CatalogRepository.GetAll(db);
            }
        }
    }
}
