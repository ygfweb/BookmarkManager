using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace BookmarkManager.Libs.Entity
{
    public class Site
    {
        public string Id { get; set; }
        public string Host { get; set; }
        public byte[] Icon { get; set; }
    }
}
