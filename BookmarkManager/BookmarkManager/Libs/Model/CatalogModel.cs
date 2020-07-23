using BookmarkManager.Libs.Entity;
using BookmarkManager.Libs.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace BookmarkManager.Libs.Model
{
    public class CatalogModel : BasePropertyObservable
    {
        private string _Id = "";
        public string Id
        {
            get { return _Id; }
            set
            {
                _Id = value;
                this.OnPropertyChanged(nameof(Id));
            }
        }

        private string _ParentId = "";
        public string ParentId
        {
            get { return _ParentId; }
            set
            {
                _ParentId = value;
                this.OnPropertyChanged(nameof(ParentId));
            }
        }

        private string _Name = "";
        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                this.OnPropertyChanged(nameof(Name));
            }
        }


        private long _Order = 0;
        public long Order
        {
            get { return _Order; }
            set
            {
                _Order = value;
                this.OnPropertyChanged(nameof(Order));
            }
        }

        public ObservableCollection<CatalogModel> _Children = new ObservableCollection<CatalogModel>();
        public ObservableCollection<CatalogModel> Children
        {
            get { return _Children; }
            set
            {
                _Children = value;
                this.OnPropertyChanged(nameof(_Children));
            }
        }

        private bool _IsExpanded = false;
        /// <summary>
        /// 是否展开
        /// </summary>
        public bool IsExpanded
        {
            get { return _IsExpanded; }
            set
            {
                _IsExpanded = value;
                this.OnPropertyChanged(nameof(IsExpanded));
            }
        }

        private bool _IsSelected = false;
        /// <summary>
        /// 是否选中
        /// </summary>
        public bool IsSelected
        {
            get { return _IsSelected; }
            set
            {
                _IsSelected = value;
                this.OnPropertyChanged(nameof(IsSelected));
            }
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
