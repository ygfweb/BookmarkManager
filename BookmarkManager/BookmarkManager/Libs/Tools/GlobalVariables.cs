using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace BookmarkManager.Libs.Tools
{
    /// <summary>
    /// 全局变量
    /// </summary>
    public static class GlobalVariables
    {
        public static App App { get; set; }
        /// <summary>
        /// 软件名称
        /// </summary>
        public static readonly string AppName = "书签管理软件";
        /// <summary>
        /// 数据库密码
        /// </summary>
        public static string DbPassword { get; set; } = "";
        /// <summary>
        /// 数据库文件路径
        /// </summary>
        public static string DbFile { get; set; } = "";
    }
}
