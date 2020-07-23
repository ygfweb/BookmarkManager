using BookmarkManager.Libs.Commands;
using BookmarkManager.Libs.Orm;
using BookmarkManager.Libs.Tools;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BookmarkManager.Libs.ViewModel
{
    public class LoginWindowViewModel : BasePropertyObservable
    {
        private AppConfig AppConfig { get; set; } = AppConfig.Load();

        /// <summary>
        /// 创建文件命令
        /// </summary>
        public ICommand CreateFile
        {
            get
            {
                return new RelayCommand(_CreateFile);
            }
        }

        /// <summary>
        /// 打开文件命令
        /// </summary>
        public ICommand OpenFile
        {
            get
            {
                return new RelayCommand(_OpenFile);
            }
        }
        /// <summary>
        /// 登录命令
        /// </summary>
        public ICommand Login
        {
            get
            {
                return new RelayCommand(_Login);
            }
        }

        private string _FileName = "";
        /// <summary>
        /// 数据文件
        /// </summary>
        public string FileName
        {
            get { return _FileName; }
            set
            {
                _FileName = value;
                this.OnPropertyChanged(nameof(FileName));
            }
        }

        private string _Password = "";
        /// <summary>
        /// 登录密码
        /// </summary>
        public string Password
        {
            get { return _Password; }
            set
            {
                _Password = value;
                this.OnPropertyChanged(nameof(Password));
            }
        }

        private bool _IsWait = false;
        /// <summary>
        /// 是否处于等待
        /// </summary>
        public bool IsWait
        {
            get { return _IsWait; }
            set
            {
                _IsWait = value;
                this.OnPropertyChanged(nameof(IsWait));
            }
        }

        /// <summary>
        /// 是否记住文件
        /// </summary>
        public bool IsRememberFile
        {
            get 
            {
                return this.AppConfig.IsRememberFile; 
            }
            set
            {
                this.AppConfig.IsRememberFile = value;
                this.OnPropertyChanged(nameof(IsRememberFile));
            }
        }

        /// <summary>
        /// 登录窗口
        /// </summary>
        public LoginWindow View { get; set; }

        public LoginWindowViewModel()
        {
            this.FileName = AppConfig.FileName;
        }

        /// <summary>
        /// 创建文件
        /// </summary>
        private void _CreateFile()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "书签数据文件|*.bmdb";
            dialog.FileName = string.Empty;
            dialog.Title = "创建文件";
            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                this.IsWait = true;
                string fileName = dialog.FileName;
                File.WriteAllBytes(fileName, BookmarkManager.Properties.Resources.data);
                Thread.Sleep(500);
                this.FileName = fileName;
                this.IsWait = false;
            }
        }

        private void _OpenFile()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "书签数据文件|*.bmdb";
            dialog.FileName = string.Empty;
            dialog.Title = "选择文件";
            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                this.FileName = dialog.FileName;
                this.View.SetPasswordFocus();
            }
        }

        private void _Login()
        {
            string password =this.View.GetPassword();
            if (string.IsNullOrWhiteSpace(this.FileName))
            {
                MessageBox.Show("请先选择打开的文件", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!File.Exists(this.FileName))
            {
                MessageBox.Show("该文件在磁盘上不存在，请重新选择", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            GlobalVariables.DbFile = this.FileName;
            GlobalVariables.DbPassword = password;
            try
            {
                using (DbHelper db = new DbHelper())
                {
                }
                if (this.AppConfig.IsRememberFile)
                {
                    this.AppConfig.FileName = this.FileName;
                    this.AppConfig.Save();
                }
                else
                {
                    this.AppConfig.FileName = "";
                    this.AppConfig.Save();
                }
                MainWindow main = new MainWindow();
                LoginWindow login = Application.Current.MainWindow as LoginWindow;
                Application.Current.MainWindow = main;
                login.Close();
                main.Show();
            }
            catch (Exception ex)
            {
                this.View.SetPasswordFocus();
                if (ex.Message.StartsWith("File opened that is not a database file"))
                {
                    MessageBox.Show("密码错误，请重新输入", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show(ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                }                
            }
        }
    }
}
