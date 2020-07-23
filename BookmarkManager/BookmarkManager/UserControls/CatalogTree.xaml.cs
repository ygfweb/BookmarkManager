using BookmarkManager.Libs.BLL;
using BookmarkManager.Libs.Entity;
using BookmarkManager.Libs.Model;
using BookmarkManager.Libs.Services;
using BookmarkManager.Libs.Tools;
using BookmarkManager.Libs.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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
    /// CatalogTree.xaml 的交互逻辑
    /// </summary>
    public partial class CatalogTree : UserControl, INotifyPropertyChanged
    {
        public Catalog DoCatalog { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }
        private CatalogTreeModel _TreeModel = new CatalogTreeModel();
        public CatalogTreeModel TreeModel
        {
            get { return _TreeModel; }
            set
            {
                _TreeModel = value;
                this.OnPropertyChanged(nameof(TreeModel));
            }
        }

        public CatalogTree()
        {
            InitializeComponent();
            this.DataContext = TreeModel;
        }

        public async Task LoadData()
        {
            try
            {
                await this.TreeModel.LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 上一次鼠标左键点击位置
        /// </summary>
        private Point _lastMouseDown;
        /// <summary>
        /// 上次鼠标点击的按钮
        /// </summary>
        private bool _isCheckDrag = false;

        private void TreeViewItem_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            // 实现右键选中
            UIElement element = e.OriginalSource as UIElement;
            TreeViewItem container = GetNearestContainer(element);
            CatalogModel model = container.DataContext as CatalogModel;
            if (model != null)
            {
                model.IsSelected = true;                
            }
            _isCheckDrag = false;
        }

        private void TreeViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isCheckDrag = true;
        }

        private void TreeViewItem_LostFocus(object sender, RoutedEventArgs e)
        {
            _isCheckDrag = false;
        }

        #region 拖动


        private void TreeViewItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                _lastMouseDown = e.GetPosition(this);
                _isCheckDrag = true;
            }
            else
            {
                _isCheckDrag = false;
            }
        }

        private void TreeViewItem_MouseMove(object sender, MouseEventArgs e)
        {
            TreeViewItem treeViewItem = sender as TreeViewItem;
            if (e.LeftButton == MouseButtonState.Pressed && _isCheckDrag)
            {
                Point currentPositon = e.GetPosition(tv);
                if ((Math.Abs(currentPositon.X - _lastMouseDown.X) > SystemParameters.MinimumHorizontalDragDistance) || (Math.Abs(currentPositon.Y - _lastMouseDown.Y) > SystemParameters.MinimumHorizontalDragDistance))
                {
                    CatalogModel catalogModel = tv.SelectedItem as CatalogModel;
                    if (catalogModel != null && catalogModel.ParentId != null)
                    {
                        DragDrop.DoDragDrop(treeViewItem, catalogModel, DragDropEffects.Move);
                        e.Handled = true;
                    }
                }
            }
            Debug.WriteLine("MouseMove事件:" + _isCheckDrag.ToString());
        }

        private void SetHighlightColor(object originalSource)
        {
            if (originalSource != null && (originalSource as UIElement) != null)
            {
                UIElement element = originalSource as UIElement;
                TreeViewItem container = GetNearestContainer(element);
                if (container != null)
                {
                    container.Background = new SolidColorBrush(Color.FromRgb(255, 241, 118));
                }
            }
        }

        private void SetDefaultColor(object originalSource)
        {
            if (originalSource != null && (originalSource as UIElement) != null)
            {
                UIElement element = originalSource as UIElement;
                TreeViewItem container = GetNearestContainer(element);
                if (container != null)
                {
                    container.Background = new SolidColorBrush(Colors.White);
                }
            }
        }

        private void TreeViewItem_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(CatalogModel)) && (e.Data.GetData(typeof(CatalogModel)) as CatalogModel) != null)
            {
                e.Effects = DragDropEffects.Move;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            this.SetHighlightColor(e.OriginalSource);
            e.Handled = true;
        }


        private async void TreeViewItem_Drop(object sender, DragEventArgs e)
        {
            this.SetDefaultColor(e.OriginalSource);
            CatalogModel sourceModel = e.Data.GetData(typeof(CatalogModel)) as CatalogModel;
            FrameworkElement targetElement = e.OriginalSource as FrameworkElement;
            CatalogModel targetModel = targetElement == null ? null : targetElement.DataContext as CatalogModel;
            if (!this.TreeModel.IsCanDrop(sourceModel, targetModel))
            {
                return;
            }
            Catalog sourceCatalog = ObjectMapper.Map<CatalogModel, Catalog>(sourceModel);
            sourceCatalog.ParentId = targetModel.Id;
            try
            {
                await CatalogService.Update(sourceCatalog);
                this.TreeModel.MoveModel(sourceModel, targetModel);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                e.Handled = true;
            }
        }

        private void TreeViewItem_DragLeave(object sender, DragEventArgs e)
        {
            this.SetDefaultColor(e.OriginalSource);
            e.Handled = true;
        }

        /// <summary>
        /// 获取最近的控件
        /// </summary>
        private TreeViewItem GetNearestContainer(UIElement element)
        {
            // https://blog.csdn.net/dnazhd/article/details/105946255
            TreeViewItem container = element as TreeViewItem;
            while ((container == null) && (element != null))
            {
                element = VisualTreeHelper.GetParent(element) as UIElement;
                container = element as TreeViewItem;
            }
            return container;
        }

        #endregion

        #region 菜单处理
        private void mi_create_Click(object sender, RoutedEventArgs e)
        {
            CatalogModel model = this.TreeModel.GetCurrentSelect();
            if (model != null)
            {
                AddCatalogWindow addCatalog = new AddCatalogWindow();
                addCatalog.ParentModel = model;
                addCatalog.ParentUI = this;
                addCatalog.Owner = Window.GetWindow(this);
                if (addCatalog.ShowDialog() == true)
                {
                    CatalogModel newModel = ObjectMapper.Map<Catalog, CatalogModel>(this.DoCatalog);
                    TreeModel.AddModel(newModel);
                    TreeModel.ReOrderChilds(model);
                    model.IsExpanded = true;
                    newModel.IsSelected = true;
                }
            }
        }

        private void mi_modify_Click(object sender, RoutedEventArgs e)
        {
            CatalogModel model = this.TreeModel.GetCurrentSelect();
            if (model != null && model.ParentId != null)
            {
                ModifyCatalogWindow w = new ModifyCatalogWindow();
                w.DoModel = ObjectMapper.Map<CatalogModel, CatalogModel>(model);
                w.ParentUI = this;
                w.ParentName = this.TreeModel.GetModelById(model.ParentId).Name;
                w.Owner = Window.GetWindow(this);
                if (w.ShowDialog() == true)
                {
                    model.Name = this.DoCatalog.Name;
                    model.Order = this.DoCatalog.Order;
                    TreeModel.ReOrderChilds(this.TreeModel.GetModelById(model.ParentId));
                }
            }
        }
        private async void mi_delete_Click(object sender, RoutedEventArgs e)
        {
            CatalogModel model = this.TreeModel.GetCurrentSelect();
            if (model != null)
            {
                if (model.ParentId == null)
                {
                    MessageBox.Show("不允许删除根目录", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (MessageBox.Show($"将要删除目录<{model.Name}>,确认操作吗?", "提示", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) == MessageBoxResult.Yes)
                {
                    Catalog catalog = ObjectMapper.Map<CatalogModel, Catalog>(model);
                    try
                    {
                        await CatalogService.Delete(catalog);
                        this.TreeModel.RemoveModel(model);
                        CatalogModel parentModel = this.TreeModel.GetModelById(model.ParentId);
                        parentModel.IsSelected = true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void mi_collapse_all_Click(object sender, RoutedEventArgs e)
        {
            this.TreeModel.CollapseAll();
        }

        private void mi_expand_all_Click(object sender, RoutedEventArgs e)
        {
            this.TreeModel.ExpandAll();
        }




        #endregion

        private async void mi_MoveUp_Click(object sender, RoutedEventArgs e)
        {
            CatalogModel model = this.TreeModel.GetCurrentSelect();
            if (model!=null && !string.IsNullOrWhiteSpace(model.Id))
            {
                try
                {
                    Catalog catalog = ObjectMapper.Map<CatalogModel, Catalog>(model);
                    catalog.Order = catalog.Order + 1;
                    await CatalogService.Update(catalog);
                    model.Order = catalog.Order;
                    CatalogModel parentModel = TreeModel.GetModelById(model.ParentId);
                    TreeModel.ReOrderChilds(parentModel);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                }
               
            }
        }

        private async void mi_MoveDown_Click(object sender, RoutedEventArgs e)
        {
            CatalogModel model = this.TreeModel.GetCurrentSelect();
            if (model != null && !string.IsNullOrWhiteSpace(model.Id))
            {
                try
                {
                    Catalog catalog = ObjectMapper.Map<CatalogModel, Catalog>(model);
                    catalog.Order = catalog.Order - 1;
                    await CatalogService.Update(catalog);
                    model.Order = catalog.Order;
                    CatalogModel parentModel = TreeModel.GetModelById(model.ParentId);
                    TreeModel.ReOrderChilds(parentModel);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
        }
    }
}
