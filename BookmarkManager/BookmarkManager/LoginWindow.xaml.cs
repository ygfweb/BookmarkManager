using BookmarkManager.Libs.Orm;
using BookmarkManager.Libs.Tools;
using BookmarkManager.Libs.ViewModel;
using SiHan.WPF.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BookmarkManager
{
    /// <summary>
    /// LoginWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWindow : FlatWindow
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void FlatWindow_ContentRendered(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(tb_file.Text))
            {
                tb_password.Focus();
            }
        }

        private void FlatWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            LoginWindowViewModel viewModel = this.DataContext as LoginWindowViewModel;
            viewModel.View = this;
        }
        /// <summary>
        /// 获取密码
        /// </summary>
        public string GetPassword()
        {
            return tb_password.Password.Trim();
        }

        /// <summary>
        /// 设置密码框焦点
        /// </summary>
        public void SetPasswordFocus()
        {
            tb_password.Focus();
            tb_password.SelectAll();
        }
    }
}
