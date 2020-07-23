using BookmarkManager.Libs.Commands;
using BookmarkManager.Libs.Tools;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BookmarkManager.Libs.ViewModel
{
    /// <summary>
    /// 主窗口视图模型
    /// </summary>
    public class MainWindowViewModel
    {
        public DashboardViewModel DashboardViewModel { get; set; }
        public MainWindow View { get; set; }

        private ICommand _BackFile;
        public ICommand BackFile
        {
            get
            {
                if (_BackFile == null)
                {
                    _BackFile = new BackFileCommand();
                }
                return _BackFile;
            }
            set { _BackFile = value; }
        }

        private ICommand _ChangePassword;
        public ICommand ChangePassword
        {
            get
            {
                if (_ChangePassword == null)
                {
                    _ChangePassword = new ChangePasswordCommand(this.View);
                }
                return _ChangePassword;
            }
            set { _ChangePassword = value; }
        }


        public class BackFileCommand : BaseCommand
        {
            protected override bool DoCanExecute()
            {
                return !string.IsNullOrWhiteSpace(GlobalVariables.DbFile);
            }
            protected override void DoExecute()
            {
                FileInfo file = new FileInfo(GlobalVariables.DbFile);
                string fileName = Path.GetFileNameWithoutExtension(file.Name);
                string catalogName = Path.GetDirectoryName(file.Name);
                string time = DateTime.Now.ToString("yyyy-MM-dd(HHmmss)");
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.Title = "备份文件";
                dialog.FileName = $"{fileName} {time}.bmdb";
                dialog.Filter = "书签文件|*.bmdb";
                if (dialog.ShowDialog() == true)
                {
                    file.CopyTo(dialog.FileName);
                    MessageBox.Show("备份成功", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        public class ChangePasswordCommand : BaseCommand
        {
            private Window Window { get; set; }

            public ChangePasswordCommand(Window window)
            {
                Window = window ?? throw new ArgumentNullException(nameof(window));
            }

            protected override bool DoCanExecute()
            {
                return !string.IsNullOrWhiteSpace(GlobalVariables.DbFile);
            }
            protected override void DoExecute()
            {
                ChangePasswordWindow window = new ChangePasswordWindow();
                window.Owner = this.Window;
                window.ShowDialog();
            }
        }
    }
}

