using BookmarkManager.Libs.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookmarkManager.Libs.Model
{
    public class BookmarkModel : BasePropertyObservable
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
        private string _CatalogId = "";
        public string CatalogId
        {
            get { return _CatalogId; }
            set
            {
                _CatalogId = value;
                this.OnPropertyChanged(nameof(CatalogId));
            }
        }

        private string _SiteId = "";
        public string SiteId
        {
            get { return _SiteId; }
            set
            {
                _SiteId = value;
                this.OnPropertyChanged(nameof(SiteId));
            }
        }

        private string _Title = "";
        public string Title
        {
            get { return _Title; }
            set
            {
                _Title = value;
                this.OnPropertyChanged(nameof(Title));
            }
        }

        private string _Url = "";
        public string Url
        {
            get { return _Url; }
            set
            {
                _Url = value;
                this.OnPropertyChanged(nameof(Url));
            }
        }


        private string _Memo = "";
        public string Memo
        {
            get { return _Memo; }
            set
            {
                _Memo = value;
                this.OnPropertyChanged(nameof(Memo));
            }
        }

        private bool _IsMark = false;
        public bool IsMark
        {
            get { return _IsMark; }
            set
            {
                _IsMark = value;
                this.OnPropertyChanged(nameof(IsMark));
            }
        }
    }
}
