using BookmarkManager.Libs.Orm;
using BookmarkManager.Libs.Tools;
using SiHan.WPF.UI;
using System;
using System.Collections.Generic;
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
    /// ChangePasswordWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ChangePasswordWindow : FlatWindow
    {
        public ChangePasswordWindow()
        {
            InitializeComponent();
        }

        private void FlatWindow_Loaded(object sender, RoutedEventArgs e)
        {
            pb_pwd.Password = GlobalVariables.DbPassword;
            pb_new_pwd.Password = GlobalVariables.DbPassword;
        }

        private void FlatWindow_ContentRendered(object sender, EventArgs e)
        {
            pb_pwd.Focus();
            pb_pwd.SelectAll();
        }

        private void FlatButton_Click(object sender, RoutedEventArgs e)
        {
            string pwd = pb_pwd.Password.Trim();
            string newPwd = pb_new_pwd.Password.Trim();
            if (pwd != newPwd)
            {
                MessageBox.Show("两次输入的密码不一致", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                pb_new_pwd.Focus();
                pb_new_pwd.SelectAll();
            }
            else
            {
                try
                {
                    //修改数据库密码
                    using (DbHelper db = new DbHelper())
                    {
                        db.ChangePassword(pwd);
                    }
                    // 修改全局密码字符
                    GlobalVariables.DbPassword = pwd;
                    // 验证密码是否生效
                    using (DbHelper db = new DbHelper()){}
                    MessageBox.Show("操作成功", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.DialogResult = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
