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
    /// ModifyBookmarkWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ModifyBookmarkWindow : FlatWindow
    {
        public BookmarkGridViewModel DoBookmarkGridViewModel { get; set; }

        public ModifyBookmarkWindow()
        {
            InitializeComponent();
            this.Loaded += ModifyBookmarkWindow_Loaded;
            this.ContentRendered += ModifyBookmarkWindow_ContentRendered;
        }

        private void ModifyBookmarkWindow_ContentRendered(object sender, EventArgs e)
        {
            tb_title.Focus();
            tb_title.SelectAll();
        }

        private void ModifyBookmarkWindow_Loaded(object sender, RoutedEventArgs e)
        {
            BookmarkViewModel bookmarkViewModel = this.DoBookmarkGridViewModel.CurrentSelected;
            BookmarkModel bookmarkModel = ObjectMapper.Map<BookmarkViewModel, BookmarkModel>(bookmarkViewModel);
            this.DataContext = bookmarkModel;
        }

        private async void btn_save_Click(object sender, RoutedEventArgs e)
        {
            string url = this.tb_url.Text.Trim();
            if (string.IsNullOrWhiteSpace(url))
            {
                MessageBox.Show("URL不能为空", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                tb_url.Focus();
                tb_url.SelectAll();
                return;
            }
            try
            {
                progressBar.Visibility = Visibility.Visible;
                btn_save.IsEnabled = false;
                BookmarkModel model = this.DataContext as BookmarkModel;
                BookmarkView bookmarkView = await BookmarkService.Update(ObjectMapper.Map<BookmarkModel, Bookmark>(model));
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

