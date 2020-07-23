using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace BookmarkManager.Libs.Net
{
    /// <summary>
    /// 编码转换器
    /// </summary>
    internal static class EncodingConverter
    {
        /// <summary>
        /// 获取HTML内容的编码
        /// </summary>
        /// <param name="htmlBytes">HTML字节内容</param>
        /// <param name="charSet">HTML字符集</param>
        /// <returns></returns>
        public static Encoding GetEncoding(byte[] htmlBytes, string charSet = "")
        {
            if (htmlBytes == null)
            {
                throw new ArgumentNullException(nameof(htmlBytes));
            }
            Encoding encoding = Encoding.UTF8;
            Match meta = Regex.Match(Encoding.Default.GetString(htmlBytes), "<meta([^<]*)charset=([^<]*)[\"']", RegexOptions.IgnoreCase);
            string charter = (meta.Groups.Count > 1) ? meta.Groups[2].Value.ToLower() : string.Empty;
            if (charter.Length > 2)
                encoding = Encoding.GetEncoding(charter.Trim().Replace("\"", "").Replace("'", "").Replace(";", "").Replace("iso-8859-1", "gbk"));
            else
            {
                if (string.IsNullOrWhiteSpace(charSet))
                {
                    encoding = Encoding.UTF8;
                }
                else
                {
                    encoding = Encoding.GetEncoding(charSet);
                }
            }
            return encoding;
        }

        /// <summary>
        /// 获取自动编码的HTML内容（即无视编码）
        /// </summary>
        /// <param name="htmlBytes">HTML内容</param>
        /// <param name="charSet">响应字符集</param>
        /// <returns></returns>
        public static string GetAutoEncodingString(byte[] htmlBytes, string charSet = "")
        {
            if (htmlBytes == null || htmlBytes.Length == 0)
            {
                return "";
            }
            else
            {
                Encoding encoding = GetEncoding(htmlBytes, charSet);
                return encoding.GetString(htmlBytes);
            }
        }

        /// <summary>
        /// 获取HTML内容
        /// </summary>
        /// <param name="htmlBytes">响应内容</param>
        /// <param name="charSet">字符集</param>
        /// <param name="isAutoEncoding">是否自动编码</param>
        /// <returns></returns>
        public static string GetHtmlString(byte[] htmlBytes, string charSet = "", bool isAutoEncoding = false)
        {
            if (htmlBytes == null)
            {
                return "";
            }
            else
            {
                if (isAutoEncoding)
                {
                    return GetAutoEncodingString(htmlBytes, charSet);
                }
                else
                {
                    return Encoding.UTF8.GetString(htmlBytes);
                }
            }
        }
    }
}


