using BookmarkManager.Libs.Entity;
using BookmarkManager.Libs.Orm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookmarkManager.Libs.BLL
{
    public class SiteRepository
    {
        public static async Task<Site> GetSiteWithIdAsync(DbHelper db,string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException(nameof(id));
            }
            if (db == null)
            {
                throw new ArgumentNullException(nameof(db));
            }
            return await db.SelectOne<Site>("select * from [Site] where [Id] = @id", new { Id = id });
        }

        public static async Task<Site> GetSiteWithHostAsync(DbHelper db,string host)
        {
            if (string.IsNullOrWhiteSpace(host))
            {
                throw new ArgumentNullException(nameof(host));
            }
            if (db == null)
            {
                throw new ArgumentNullException(nameof(db));
            }
            return await db.SelectOne<Site>("select * from [Site] where [Host] = @Host", new { Host = host });
        }

        public static async Task<bool> IsExist(DbHelper db,string host)
        {
            if (string.IsNullOrWhiteSpace(host))
            {
                throw new ArgumentNullException(nameof(host));
            }
            if (db == null)
            {
                throw new ArgumentNullException(nameof(db));
            }
            host = host.Trim().ToLower();
            int count = await db.SelectIntScalar("select count(*) from [Site] where [Host]=@Host", new { Host = host });
            return count > 0;
        }

        public static async Task Insert(DbHelper db,Site site)
        {
            if (site == null)
            {
                throw new ArgumentNullException(nameof(site));
            }
            if (db == null)
            {
                throw new ArgumentNullException(nameof(db));
            }
            if (string.IsNullOrWhiteSpace(site.Id))
            {
                throw new Exception("站点ID不能为空");
            }
            await db.Insert(site);
        }
    }
}
