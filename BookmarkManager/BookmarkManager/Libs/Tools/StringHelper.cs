using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookmarkManager.Libs.Tools
{
    public class StringHelper
    {
        public static bool IsNullOrWhiteSpace(string value)
        {
            if (value == null) return true;
            return string.IsNullOrEmpty(value.Trim());
        }
        /// <summary>
        /// 判断字符串是否是URL
        /// </summary>
        public static bool IsUrl(string url)
        {
            Uri uriResult;
            return Uri.TryCreate(url, UriKind.Absolute, out uriResult)  && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        /// <summary>
        /// 检查字符串是否是整数
        /// </summary>
        public static bool IsLong(string text)
        {
            long n;
            bool isNumeric = long.TryParse(text, out n);
            return isNumeric;
        }
    }
}
