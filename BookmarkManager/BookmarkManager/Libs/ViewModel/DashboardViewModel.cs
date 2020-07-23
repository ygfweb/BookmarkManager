using BookmarkManager.Libs.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookmarkManager.Libs.ViewModel
{
    public class DashboardViewModel:BasePropertyObservable
    {
        private BookmarkGridViewModel _GridViewModel = null;
        public BookmarkGridViewModel GridViewModel
        {
            get { return _GridViewModel; }
            set
            {
                _GridViewModel = value;
                this.OnPropertyChanged(nameof(GridViewModel));
            }
        }
    }
}
