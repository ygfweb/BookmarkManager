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
    /// AddCatalogWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AddCatalogWindow : FlatWindow
    {
        public CatalogModel ParentModel { get; set; }
        public CatalogTree ParentUI { get; set; }
        public AddCatalogWindow()
        {
            InitializeComponent();
        }

        private void FlatWindow_ContentRendered(object sender, EventArgs e)
        {
            tb_name.Focus();
        }

        private async void save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tb_name.Text))
            {
                MessageBox.Show("目录名称不能为空", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                tb_name.Focus();
                tb_name.SelectAll();
                return;
            }
            if (!StringHelper.IsLong(tb_order.Text.Trim()))
            {
                MessageBox.Show("目录顺序必须是有效的整数", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                tb_order.Focus();
                tb_order.SelectAll();
                return;
            }

            pb.Visibility = Visibility.Visible;
            try
            {
                Catalog newObj = new Catalog
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = tb_name.Text.Trim(),
                    ParentId = ParentModel.Id,
                    Order = Convert.ToInt32(tb_order.Text)
                };
                await CatalogService.Insert(newObj);
                this.ParentUI.DoCatalog = newObj;
                this.DialogResult = true;
            }
            catch (Exception ex)
            {
                pb.Visibility = Visibility.Hidden;
                MessageBox.Show(ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FlatWindow_Loaded(object sender, RoutedEventArgs e)
        {
            tb_parent.Text = ParentModel.Name;
        }
    }
}
