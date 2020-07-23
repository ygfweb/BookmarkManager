using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BookmarkManager.Libs.Tools
{
    public static class AppHelper
    {
        /// <summary>
        /// 创建文件
        /// </summary>
        public static string CreateFile()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "书签数据文件|*.bmdb";
            dialog.FileName = string.Empty;
            dialog.Title = "创建文件";
            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                string fileName = dialog.FileName;
                File.WriteAllBytes(fileName, BookmarkManager.Properties.Resources.data);
                Thread.Sleep(1000);
                return fileName;
            }
            else
            {
                return "";
            }
        }

        public static string OpenFile()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "书签数据文件|*.bmdb";
            dialog.FileName = string.Empty;
            dialog.Title = "选择文件";
            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                return dialog.FileName;
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 设置鼠标指针为等待状态
        /// </summary>
        public static void SetWaitMouse()
        {
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
        }
        /// <summary>
        /// 设置鼠标指针为指针状态
        /// </summary>
        public static void SetArrowMouse()
        {
            Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
        }
    }
}
