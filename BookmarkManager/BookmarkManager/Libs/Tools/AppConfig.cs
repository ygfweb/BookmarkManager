using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


namespace BookmarkManager.Libs.Tools
{
    [Serializable]
    public class AppConfig
    {
        /// <summary>
        /// 是否记住上次打开的文件路径
        /// </summary>
        public bool IsRememberFile { get; set; } = true;
        /// <summary>
        /// 上次打开的文件
        /// </summary>
        public string FileName { get; set; } = "";

        /// <summary>
        /// 是否自动获取网站标题
        /// </summary>
        public bool IsAutoTitle { get; set; } = false;

        public static FileInfo GetConfigFile()
        {
            string path = Environment.CurrentDirectory + "/app.cfg";
            return new FileInfo(path);
        }

        public static AppConfig Load()
        {
            FileInfo file = GetConfigFile();
            if (!file.Exists)
            {
                return new AppConfig();
            }
            else
            {
                try
                {
                    return FileHelper.ReadFromBinaryFile<AppConfig>(file.FullName);
                }
                catch (Exception)
                {
                    return new AppConfig();
                }
            }
        }
        public void Save()
        {            
            FileInfo file = GetConfigFile();
            FileHelper.WriteToBinaryFile<AppConfig>(file.FullName, this);
        }
    }
}
