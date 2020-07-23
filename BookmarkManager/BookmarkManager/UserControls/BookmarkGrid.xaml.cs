using BookmarkManager.Libs.BLL;
using BookmarkManager.Libs.Entity;
using BookmarkManager.Libs.Model;
using BookmarkManager.Libs.Tools;
using BookmarkManager.Libs.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// BookmarkGrid.xaml 的交互逻辑
    /// </summary>
    public partial class BookmarkGrid : UserControl
    {
        public BookmarkGridViewModel ViewModel { get; set; } = new BookmarkGridViewModel();

        public BookmarkGrid()
        {
            InitializeComponent();
            this.DataContext = ViewModel;
        }

        /// <summary>
        /// 获取父窗体
        /// </summary>
        public Window GetWindow()
        {
            return Window.GetWindow(this);
        }

        public async Task FillData(CatalogModel catalogModel)
        {
            BookmarkGridViewModel model = this.DataContext as BookmarkGridViewModel;
            await model.FillData(catalogModel);
        }


        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            BookmarkViewModel bookmarkView = this.ViewModel.CurrentSelected;
            if (bookmarkView != null)
            {
                System.Diagnostics.Process.Start(bookmarkView.Url);
            }
        }

        /// <summary>
        /// 剪贴板数据
        /// </summary>
        [Serializable]
        public class ClipboardData
        {
            /// <summary>
            /// 所属目录ID
            /// </summary>
            public string CatalogId { get; set; } = "";
            public List<string> BookmarkList { get; set; } = new List<string>();
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            BookmarkGridViewModel model = this.DataContext as BookmarkGridViewModel;
            model.View = this;
        }
    }
}
