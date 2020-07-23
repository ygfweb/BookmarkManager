using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BookmarkManager.Libs.Tools
{
    public class EncodingStringWriter : StringWriter
    {
        public EncodingStringWriter(StringBuilder stringBuilder, Encoding encoding = null) : base(stringBuilder)
        {
            Encoding = encoding ?? Encoding.UTF8;
        }

        public override Encoding Encoding { get; }
    }
}
