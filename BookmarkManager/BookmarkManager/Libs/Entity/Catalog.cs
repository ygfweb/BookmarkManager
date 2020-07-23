using BookmarkManager.Libs.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookmarkManager.Libs.Entity
{
    public class Catalog : BasePropertyObservable
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
    }
}
