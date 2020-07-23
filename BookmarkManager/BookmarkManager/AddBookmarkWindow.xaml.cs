using BookmarkManager.Libs.BLL;
using BookmarkManager.Libs.Entity;
using BookmarkManager.Libs.Model;
using BookmarkManager.Libs.Services;
using BookmarkManager.Libs.Tools;
using BookmarkManager.Libs.ViewModel;
using BookmarkManager.UserControls;
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
    /// AddBookmarkWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AddBookmarkWindow : FlatWindow
    {
        public BookmarkGridViewModel DoBookmarkGridViewModel { get; set; }

        public AddBookmarkWindow()
        {
            InitializeComponent();
        }
        private void FlatWindow_Loaded(object sender, RoutedEventArgs e)
        {
            BookmarkModel model = new BookmarkModel() { Id = Guid.NewGuid().ToString(), CatalogId = DoBookmarkGridViewModel.CurrentCatalogModel.Id };
            string str = Clipboard.GetText();
            if (!string.IsNullOrWhiteSpace(str) && StringHelper.IsUrl(str))
            {
                model.Url = str;
            }           
            this.DataContext = model;
        }
        private void FlatWindow_Activated(object sender, EventArgs e)
        {
            BookmarkModel model = this.DataContext as BookmarkModel;
            if (string.IsNullOrWhiteSpace(model.Url))
            {
                string str = Clipboard.GetText();
                if (!string.IsNullOrWhiteSpace(str) && StringHelper.IsUrl(str))
                {
                    model.Url = str;
                    tb_memo.Focus();
                    tb_memo.SelectAll();
                }
            }
        }
        private void FlatWindow_ContentRendered(object sender, EventArgs e)
        {
            BookmarkModel model = this.DataContext as BookmarkModel;
            if (!string.IsNullOrWhiteSpace(model.Url))
            {
                tb_memo.Focus();
                tb_memo.SelectAll();
            }
            else
            {
                tb_url.Focus();
                tb_url.SelectAll();
            }        
        }

        private async void FlatButton_Click(object sender, RoutedEventArgs e)
        {
            string url = this.tb_url.Text.Trim();
            if (string.IsNullOrWhiteSpace(url))
            {
                MessageBox.Show("URL不能为空", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                tb_url.Focus();
                tb_url.SelectAll();
                return;
            }
            if (string.IsNullOrWhiteSpace(tb_title.Text) && cb_NotAllowNet.IsChecked == true)
            {
                MessageBox.Show("当禁止从网络获取数据时，必须手工填写标题", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                tb_title.Focus();
                tb_title.SelectAll();
                return;
            }
            try
            {
                progressBar.Visibility = Visibility.Visible;
                btn_save.IsEnabled = false;
                BookmarkModel model = this.DataContext as BookmarkModel;
                BookmarkView bookmarkView = await BookmarkService.Insert(ObjectMapper.Map<BookmarkModel, Bookmark>(model),cb_NotAllowNet.IsChecked == true);
                this.DoBookmarkGridViewModel.DoBookmarkViewModel = ObjectMapper.Map<BookmarkView, BookmarkViewModel>(bookmarkView);
                progressBar.Visibility = Visibility.Hidden;
                this.DialogResult = true;
                btn_save.IsEnabled = true;
            }
            catch (Exception ex)
            {
                progressBar.Visibility = Visibility.Hidden;
                btn_save.IsEnabled = true;
                MessageBox.Show(ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                tb_url.Focus();
                tb_url.SelectAll();
            }
        }

        private async void btn_auto_Click(object sender, RoutedEventArgs e)
        {
            BookmarkModel model = this.DataContext as BookmarkModel;
            if (string.IsNullOrWhiteSpace(model.Url))
            {
                MessageBox.Show("必须先填入URL", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                tb_url.Focus();
                tb_url.SelectAll();
            }
            else
            {
                try
                {
                    progressBar.Visibility = Visibility.Visible;
                    string title = await NetHelper.GetTitle(model.Url);
                    model.Title = title;
                    progressBar.Visibility = Visibility.Hidden;
                    tb_memo.Focus();
                    tb_memo.SelectAll();
                }
                catch (Exception ex)
                {
                    progressBar.Visibility = Visibility.Hidden;
                    MessageBox.Show(ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                    tb_title.Focus();
                    tb_title.SelectAll();
                }
            }
        }
    }
}

