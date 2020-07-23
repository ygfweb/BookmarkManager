using BookmarkManager.Libs.BLL;
using BookmarkManager.Libs.Commands;
using BookmarkManager.Libs.Entity;
using BookmarkManager.Libs.Model;
using BookmarkManager.Libs.Services;
using BookmarkManager.Libs.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookmarkManager.Libs.ViewModel
{
    public class CatalogTreeModel : BasePropertyObservable
    {
        private ObservableCollection<CatalogModel> _RootCatalogs = new ObservableCollection<CatalogModel>();
        public ObservableCollection<CatalogModel> RootCatalogs
        {
            get { return _RootCatalogs; }
            set
            {
                _RootCatalogs = value;
                this.OnPropertyChanged(nameof(RootCatalogs));
            }
        }
        private ObservableCollection<CatalogModel> _AllCatalogs = new ObservableCollection<CatalogModel>();
        public ObservableCollection<CatalogModel> AllCatalogs
        {
            get { return _AllCatalogs; }
            set
            {
                _AllCatalogs = value;
                this.OnPropertyChanged(nameof(AllCatalogs));
            }
        }

        /// <summary>
        /// 加载数据
        /// </summary>
        /// <returns></returns>
        public async Task LoadData()
        {
            // 清空现有列表
            this.RootCatalogs = new ObservableCollection<CatalogModel>();
            List<Catalog> allCatalogs = new List<Catalog>();
            allCatalogs.Add(new Catalog { Id = "", Name = "根目录", ParentId = null });
            allCatalogs.AddRange(await CatalogService.GetAll());
            List<CatalogModel> allModels = ObjectMapper.MapList<Catalog, CatalogModel>(allCatalogs);
            this.AllCatalogs = new ObservableCollection<CatalogModel>(allModels);
            CreateTreeData(this.AllCatalogs);
            this.RootCatalogs = new ObservableCollection<CatalogModel>(this.AllCatalogs.Where(p => p.ParentId == null));
            for (int i = 0; i < this.RootCatalogs.Count; i++)
            {
                RootCatalogs[i].IsExpanded = true;
            }
        }

        private void CreateTreeData(ObservableCollection<CatalogModel> allModels)
        {
            for (int i = 0; i < allModels.Count; i++)
            {
                CatalogModel model = allModels[i];
                ObservableCollection<CatalogModel> childs = new ObservableCollection<CatalogModel>();
                for (int j = 0; j < allModels.Count; j++)
                {
                    CatalogModel child = allModels[j];
                    if (child.ParentId == model.Id)
                    {
                        model.Children.Add(child);
                    }
                }
            }
        }

        /// <summary>
        /// 获取当前选中的模型对象
        /// </summary>
        /// <returns></returns>
        public CatalogModel GetCurrentSelect()
        {
            return this.AllCatalogs.Where(p => p.IsSelected == true).FirstOrDefault();
        }

        private void _GetAllChild(CatalogModel parentModel, List<CatalogModel> allChild)
        {
            List<CatalogModel> childs = this.AllCatalogs.Where(p => p.ParentId == parentModel.Id).ToList();
            allChild.AddRange(childs);
            foreach (var item in childs)
            {
                _GetAllChild(item, allChild);
            }
        }
        private List<CatalogModel> GetAllChild(CatalogModel parentModel)
        {
            List<CatalogModel> allChild = new List<CatalogModel>();
            _GetAllChild(parentModel, allChild);
            return allChild;
        }

        /// <summary>
        /// 是否可以拖动
        /// </summary>
        public bool IsCanDrop(CatalogModel sourceModel, CatalogModel targetModel)
        {
            if (sourceModel == null || targetModel == null)
            {
                return false;
            }
            // 如果源节点是根节点，则忽略
            if (sourceModel.ParentId == null)
            {
                return false;
            }
            // 如果节点被拖放到原父节点，则不做处理
            if (sourceModel.ParentId == targetModel.Id)
            {
                return false;
            }
            // 如果是自身则忽略
            if (sourceModel.Id == targetModel.Id)
            {
                return false;
            }
            // 如果节点被拖到其的子节点，则忽略
            List<CatalogModel> childs = GetAllChild(sourceModel);
            if (childs.Exists(p => p.Id == targetModel.Id))
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 获取父目录
        /// </summary>
        public CatalogModel GetModelById(string id)
        {
            return this.AllCatalogs.Where(p => p.Id == id).FirstOrDefault();
        }

        /// <summary>
        /// 移除对象（仅内存，不影响数据库）
        /// </summary>
        public void RemoveModel(CatalogModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            CatalogModel parentModel = this.GetModelById(model.ParentId);
            this.AllCatalogs.Remove(model);
            parentModel.Children.Remove(model);
        }

        public void AddModel(CatalogModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            CatalogModel parentModel = this.GetModelById(model.ParentId);
            this.AllCatalogs.Add(model);
            parentModel.Children.Add(model);
        }

        public void ReOrderChilds(CatalogModel parentModel)
        {
            if (parentModel == null)
            {
                throw new ArgumentNullException(nameof(parentModel));
            }
            List<CatalogModel> newOrderChilds = parentModel.Children.OrderByDescending(p => p.Order).ToList();
            parentModel.Children.Clear();
            foreach (var item in newOrderChilds)
            {
                parentModel.Children.Add(item);
            }
        }

        public void MoveModel(CatalogModel sourceModel, CatalogModel targetModel)
        {
            if (sourceModel == null)
            {
                throw new ArgumentNullException(nameof(sourceModel));
            }
            if (targetModel == null)
            {
                throw new ArgumentNullException(nameof(targetModel));
            }
            CatalogModel oldParent = this.GetModelById(sourceModel.ParentId);
            oldParent.Children.Remove(sourceModel);
            targetModel.Children.Add(sourceModel);
            sourceModel.ParentId = targetModel.Id;
            targetModel.IsExpanded = true;
            sourceModel.IsSelected = true;
        }
        /// <summary>
        /// 折叠所有
        /// </summary>
        public void CollapseAll()
        {
            for (int i = 0; i < this.AllCatalogs.Count; i++)
            {
                CatalogModel model = this.AllCatalogs[i];
                if (model.ParentId == null)
                {
                    model.IsExpanded = true;
                }
                else
                {
                    model.IsExpanded = false;
                }
            }
        }
        /// <summary>
        /// 展开所有
        /// </summary>
        public void ExpandAll()
        {
            for (int i = 0; i < this.AllCatalogs.Count; i++)
            {
                CatalogModel model = this.AllCatalogs[i];
                model.IsExpanded = true;
            }
        }

        //public class CreateCatalogCommand : BaseCommand
        //{
        //    private CatalogTreeModel TreeModel { get; set; }

        //    public CreateCatalogCommand(CatalogTreeModel treeModel)
        //    {
        //        TreeModel = treeModel ?? throw new ArgumentNullException(nameof(treeModel));
        //    }

        //    protected override bool DoCanExecute()
        //    {
        //        return TreeModel.GetCurrentSelect() != null;
        //    }

        //    protected override void DoExecute()
        //    {
        //        CatalogModel model = this.TreeModel.GetCurrentSelect();
        //        AddCatalogWindow addCatalog = new AddCatalogWindow();
        //        addCatalog.ParentModel = model;
        //        addCatalog.ParentUI = this;
        //        addCatalog.Owner = Window.GetWindow(this);
        //        if (addCatalog.ShowDialog() == true)
        //        {
        //            CatalogModel newModel = ObjectMapper.Map<Catalog, CatalogModel>(this.DoCatalog);
        //            TreeModel.AddModel(newModel);
        //            TreeModel.ReOrderChilds(model);
        //            model.IsExpanded = true;
        //            newModel.IsSelected = true;
        //        }
        //    }
        //}
    }
}

