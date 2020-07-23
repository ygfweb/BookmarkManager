using BookmarkManager.Libs.Commands;
using BookmarkManager.Libs.Entity;
using BookmarkManager.Libs.Model;
using BookmarkManager.Libs.Services;
using BookmarkManager.Libs.Tools;
using BookmarkManager.UserControls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static BookmarkManager.UserControls.BookmarkGrid;

namespace BookmarkManager.Libs.ViewModel
{
    /// <summary>
    /// 书签表格视图模型
    /// </summary>
    public class BookmarkGridViewModel : BasePropertyObservable
    {
        private ObservableCollection<BookmarkViewModel> _Data = new ObservableCollection<BookmarkViewModel>();
        public ObservableCollection<BookmarkViewModel> Data
        {
            get { return _Data; }
            set
            {
                _Data = value;
                this.OnPropertyChanged(nameof(Data));
            }
        }

        private BookmarkViewModel _CurrentSelected = null;
        public BookmarkViewModel CurrentSelected
        {
            get { return _CurrentSelected; }
            set
            {
                _CurrentSelected = value;
                this.OnPropertyChanged(nameof(CurrentSelected));
            }
        }

        private CatalogModel _CurrentCatalogModel = null;
        /// <summary>
        /// 当前目录
        /// </summary>
        public CatalogModel CurrentCatalogModel
        {
            get { return _CurrentCatalogModel; }
            set
            {
                _CurrentCatalogModel = value;
                this.OnPropertyChanged(nameof(CurrentCatalogModel));
            }
        }
        /// <summary>
        /// 当前操作的书签
        /// </summary>
        public BookmarkViewModel DoBookmarkViewModel { get; set; }

        private ICommand _AddBookmark = null;
        public ICommand AddBookmark
        {
            get
            {
                if (_AddBookmark == null)
                {
                    _AddBookmark = new AddBookmarkCommand(this.View, this);
                }
                return _AddBookmark;
            }
        }

        private ICommand _ModifyBookmark = null;
        public ICommand ModifyBookmark
        {
            get
            {
                if (_ModifyBookmark == null)
                {
                    _ModifyBookmark = new ModifyBookmarkCommand(this.View, this);
                }
                return _ModifyBookmark;
            }
        }

        private ICommand _DeleteBookmark = null;
        public ICommand DeleteBookmark
        {
            get
            {
                if (_DeleteBookmark == null)
                {
                    _DeleteBookmark = new DeleteBookmarkCommand(this);
                }
                return _DeleteBookmark;
            }
        }

        private ICommand _CopyLink = null;
        public ICommand CopyLink
        {
            get
            {
                if (_CopyLink == null)
                {
                    _CopyLink = new CopyLinkCommand(this);
                }
                return _CopyLink;
            }
        }

        private ICommand _Copy = null;
        public ICommand Copy
        {
            get
            {
                if (_Copy == null)
                {
                    _Copy = new CopyCommand(this, this.View);
                }
                return _Copy;
            }
        }

        private ICommand _Cut = null;
        public ICommand Cut
        {
            get
            {
                if (_Cut == null)
                {
                    _Cut = new CutCommand(this);
                }
                return _Cut;
            }
        }

        /// <summary>
        /// UI视图
        /// </summary>
        public BookmarkGrid View { get; set; }

        public async Task FillData(CatalogModel model)
        {
            this.CurrentCatalogModel = model;
            if (model == null || string.IsNullOrWhiteSpace(model.Id))
            {
                this.FillData(new List<BookmarkView>());
            }
            else
            {
                List<BookmarkView> bookmarks = await BookmarkService.GetListByCatalog(model.Id);
                this.FillData(bookmarks);
            }
        }

        /// <summary>
        /// 填充数据
        /// </summary>
        /// <param name="bookmarks"></param>
        public void FillData(List<BookmarkView> bookmarks)
        {
            this.Data.Clear();
            if (bookmarks != null)
            {
                List<BookmarkViewModel> bookmarkViewModels = ObjectMapper.MapList<BookmarkView, BookmarkViewModel>(bookmarks);
                List<BookmarkViewModel> isMarkBookmarks = bookmarkViewModels.Where(p => p.IsMark == true).ToList();
                foreach (var item in isMarkBookmarks)
                {
                    this.Data.Add(item);
                }
                List<BookmarkViewModel> notMarkBookmarks = bookmarkViewModels.Where(p => p.IsMark == false).ToList();
                foreach (var item in notMarkBookmarks)
                {
                    this.Data.Add(item);
                }
            }
        }
        /// <summary>
        /// 插入书签命令
        /// </summary>
        public class AddBookmarkCommand : BaseCommand
        {
            private BookmarkGrid View { get; set; }
            private BookmarkGridViewModel ViewModel { get; set; }
            public AddBookmarkCommand(BookmarkGrid view, BookmarkGridViewModel viewModel)
            {
                View = view ?? throw new ArgumentNullException(nameof(view));
                ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            }
            protected override bool DoCanExecute()
            {
                return this.ViewModel.CurrentCatalogModel != null && !string.IsNullOrWhiteSpace(this.ViewModel.CurrentCatalogModel.Id);
            }
            protected override void DoExecute()
            {
                AddBookmarkWindow window = new AddBookmarkWindow();
                window.DoBookmarkGridViewModel = this.ViewModel;
                window.Owner = View.GetWindow();
                if (window.ShowDialog() == true)
                {
                    this.ViewModel.Data.Add(this.ViewModel.DoBookmarkViewModel);
                    this.ViewModel.CurrentSelected = this.ViewModel.DoBookmarkViewModel;
                }
            }
        }

        public class ModifyBookmarkCommand : BaseCommand
        {
            private BookmarkGrid View { get; set; }
            private BookmarkGridViewModel ViewModel { get; set; }
            public ModifyBookmarkCommand(BookmarkGrid view, BookmarkGridViewModel viewModel)
            {
                View = view ?? throw new ArgumentNullException(nameof(view));
                ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            }
            protected override bool DoCanExecute()
            {
                return this.ViewModel.CurrentCatalogModel != null && !string.IsNullOrWhiteSpace(this.ViewModel.CurrentCatalogModel.Id) && ViewModel.CurrentSelected != null;
            }
            protected override void DoExecute()
            {
                ModifyBookmarkWindow window = new ModifyBookmarkWindow();
                window.DoBookmarkGridViewModel = this.ViewModel;
                window.Owner = View.GetWindow();
                if (window.ShowDialog() == true)
                {
                    BookmarkViewModel current = this.ViewModel.CurrentSelected;
                    ObjectMapper.CopyValues<BookmarkViewModel, BookmarkViewModel>(this.ViewModel.DoBookmarkViewModel, current);
                    this.ViewModel.CurrentSelected = this.ViewModel.CurrentSelected;
                }
            }
        }

        public class DeleteBookmarkCommand : BaseAsyncCommand
        {
            private BookmarkGridViewModel ViewModel { get; set; }

            public DeleteBookmarkCommand(BookmarkGridViewModel viewModel)
            {
                ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            }

            protected override bool DoBefore()
            {
                BookmarkViewModel current = this.ViewModel.CurrentSelected;
                if (MessageBox.Show($"将要删除书签：{current.Title} ,确认操作吗？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) == MessageBoxResult.Yes)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            protected override async Task DoExecute()
            {
                Debug.WriteLine("正在删除数据");
                BookmarkViewModel current = this.ViewModel.CurrentSelected;
                Bookmark bookmark = ObjectMapper.Map<BookmarkViewModel, Bookmark>(current);
                await BookmarkService.Delete(bookmark);
            }
            protected override bool DoCanExecute(object param)
            {
                return this.ViewModel.CurrentSelected != null;
            }
            protected override void DoSuccess()
            {
                Debug.WriteLine("正在移除数据");
                BookmarkViewModel current = this.ViewModel.CurrentSelected;
                this.ViewModel.Data.Remove(current);
                Debug.WriteLine("移除结束");
            }
        }

        public class CopyLinkCommand : BaseCommand
        {
            private BookmarkGridViewModel ViewModel { get; set; }

            public CopyLinkCommand(BookmarkGridViewModel viewModel)
            {
                ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            }
            protected override bool DoCanExecute()
            {
                return this.ViewModel.CurrentSelected != null;
            }
            protected override void DoExecute()
            {
                Clipboard.SetDataObject(this.ViewModel.CurrentSelected.Url);
            }
        }

        public class CopyCommand : BaseCommand
        {
            private BookmarkGridViewModel ViewModel { get; set; }
            private BookmarkGrid View { get; set; }

            public CopyCommand(BookmarkGridViewModel viewModel, BookmarkGrid view)
            {
                ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
                View = view ?? throw new ArgumentNullException(nameof(view));
            }
            protected override bool DoCanExecute()
            {
                return this.View.grid.SelectedItems != null && this.View.grid.SelectedItems.Count > 0;
            }
            protected override void DoExecute()
            {
                if (this.View.grid.SelectedItems != null)
                {
                    ClipboardData data = new ClipboardData();
                    data.CatalogId = this.ViewModel.CurrentCatalogModel.Id;
                    foreach (var item in this.View.grid.SelectedItems)
                    {
                        BookmarkViewModel bookmark = item as BookmarkViewModel;
                        if (bookmark != null)
                        {
                            data.BookmarkList.Add(bookmark.Id);
                        }
                    }
                    Clipboard.SetDataObject(data);
                }
            }
        }

        public class CutCommand : BaseAsyncCommand
        {
            private BookmarkGridViewModel ViewModel { get; set; }

            public CutCommand(BookmarkGridViewModel viewModel)
            {
                ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            }

            protected override bool DoCanExecute(object param)
            {
                IDataObject dataObject = Clipboard.GetDataObject();
                return dataObject.GetDataPresent(typeof(ClipboardData).FullName) && ViewModel.CurrentCatalogModel!=null && !string.IsNullOrWhiteSpace(ViewModel.CurrentCatalogModel.Id);
            }
            protected override async Task DoExecute()
            {
                IDataObject dataObject = Clipboard.GetDataObject();
                ClipboardData data = dataObject.GetData(typeof(ClipboardData).FullName) as ClipboardData;
                if (data != null && this.ViewModel.CurrentCatalogModel != null)
                {
                    if (this.ViewModel.CurrentCatalogModel.Id != data.CatalogId)
                    {
                        List<BookmarkView> bookmarks = await BookmarkService.Cut(data, this.ViewModel.CurrentCatalogModel);
                        List<BookmarkViewModel> bookmarkViewModels = ObjectMapper.MapList<BookmarkView, BookmarkViewModel>(bookmarks);
                        foreach (var item in bookmarkViewModels)
                        {
                            this.ViewModel.Data.Add(item);
                        }
                        Clipboard.Clear();
                    }
                }
            }
            protected override void DoSuccess()
            {
                CommandManager.InvalidateRequerySuggested();
                MessageBox.Show("操作成功", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}


