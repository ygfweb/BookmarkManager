using BookmarkManager.Libs.Tools;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BookmarkManager.Libs.Orm
{
    public class DbHelper : OrmHelper
    {
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public static string GetConnectionString()
        {
            SQLiteConnectionStringBuilder sb = new SQLiteConnectionStringBuilder();
            sb.DataSource = GlobalVariables.DbFile;
            sb.Version = 3;
            sb.BinaryGUID = false;
            sb.CacheSize = 2000;
            sb.FailIfMissing = true;
            return sb.ToString();
        }


        public DbHelper() : base(GetConnectionString(), GlobalVariables.DbPassword)
        {

        }

        /// <summary>
        /// 创建数据库文件
        /// </summary>
        public static void Create(string file)
        {
            File.WriteAllBytes(file, BookmarkManager.Properties.Resources.data);
            Thread.Sleep(1000);
        }
    }
}
