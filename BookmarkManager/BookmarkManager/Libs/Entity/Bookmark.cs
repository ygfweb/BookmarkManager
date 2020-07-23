using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace BookmarkManager.Libs.Entity
{
    public class Bookmark
    {
        public string Id { get; set; }
        public string CatalogId { get; set; }
        public string SiteId { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Memo { get; set; }
        public bool IsMark { get; set; }
    }
}



