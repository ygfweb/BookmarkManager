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
    /// ModifyCatalogWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ModifyCatalogWindow : FlatWindow
    {
        public CatalogModel DoModel { get; set; }
        public CatalogTree ParentUI { get; set; }
        public string ParentName { get; set; }

        public ModifyCatalogWindow()
        {
            InitializeComponent();
        }

        private void FlatWindow_ContentRendered(object sender, EventArgs e)
        {
            tb_name.Focus();
            tb_name.SelectAll();
        }

        private void FlatWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.tb_name.Text = DoModel.Name;
            this.tb_parent.Text = ParentName;
            tb_order.Text = DoModel.Order.ToString();
        }

        private async void save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tb_name.Text.Trim()))
            {
                MessageBox.Show("目录名称不能为空");
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
                this.DoModel.Name = tb_name.Text.Trim();
                this.DoModel.Order = Convert.ToInt64(tb_order.Text.Trim());
                Catalog catalog = ObjectMapper.Map<CatalogModel, Catalog>(this.DoModel);
                await CatalogService.Update(catalog);
                this.ParentUI.DoCatalog = catalog;
                pb.Visibility = Visibility.Hidden;
                this.DialogResult = true;
            }
            catch (Exception ex)
            {
                pb.Visibility = Visibility.Hidden;
                MessageBox.Show(ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
