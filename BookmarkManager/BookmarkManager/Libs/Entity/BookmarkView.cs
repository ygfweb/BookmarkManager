using BookmarkManager.Libs.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace BookmarkManager.Libs.Entity
{
    public class BookmarkView
    {
        public string Id { get; set; }
        public string CatalogId { get; set; }
        public string SiteId { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Memo { get; set; }
        public bool IsMark { get; set; }
        public string Host { get; set; }
        public byte[] SiteIcon { get; set; }
        public string CatalogName { get; set; }
    }
}
