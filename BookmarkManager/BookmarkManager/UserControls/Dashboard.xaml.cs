using BookmarkManager.Libs.Model;
using BookmarkManager.Libs.ViewModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BookmarkManager.UserControls
{
    /// <summary>
    /// Dashboard.xaml 的交互逻辑
    /// </summary>
    public partial class Dashboard : UserControl
    {
        public DashboardViewModel ViewModel { get; set; } = new DashboardViewModel();
        public Dashboard()
        {
            InitializeComponent();
            this.ViewModel.GridViewModel = bookmarkGrid.ViewModel;
        }

        public async Task LoadData()
        {
            await this.catalogTree.LoadData();
        }



        private async void Grid_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            CatalogModel model = e.NewValue as CatalogModel;
            try
            {
                await bookmarkGrid.FillData(model);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Error);
            }            
        }
    }
}
