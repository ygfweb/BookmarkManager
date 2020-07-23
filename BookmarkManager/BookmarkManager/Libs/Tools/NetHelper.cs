using BookmarkManager.Libs.Net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace BookmarkManager.Libs.Tools
{
    public class NetHelper
    {
        private static readonly string DefaultUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.116 Safari/537.36";

        /// <summary>
        /// 获取网页内容
        /// </summary>
        public static async Task<string> GetHtml(string url)
        {
            ServicePointManager.ServerCertificateValidationCallback = (a, b, c, d) => true;
            using (MyWebClient client = new MyWebClient())
            {
                client.Headers.Add("user-agent", DefaultUserAgent);
                byte[] data = await client.DownloadDataTaskAsync(url);
                string html = ReadAsString(data);
                return html;
            }
        }

       

        /// <summary> 
        /// 获取网页Title
        /// </summary> 
        public static async Task<string> GetTitle(string url)
        {
            string html = await GetHtml(url);
            // https://stackoverflow.com/questions/329307/how-to-get-website-title-from-c-sharp
            string title = Regex.Match(html, @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>", RegexOptions.IgnoreCase).Groups["Title"].Value;
            return title;
        }

        /// <summary>
        /// 获取网站Host地址
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetHost(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentNullException(url);
            }
            if (!StringHelper.IsUrl(url))
            {
                throw new Exception("该URL字符串是无效URL");
            }
            Uri uri = new Uri(url);
            return $"{uri.Scheme}://{uri.Host}";
        }

        /// <summary>
        /// 获取网站图标
        /// </summary>
        public static byte[] GetFavicon(string url)
        {
            try
            {
                string iconPath = GetHost(url) + "/favicon.ico";
                ServicePointManager.ServerCertificateValidationCallback = (a, b, c, d) => true;
                using (MyWebClient client = new MyWebClient())
                {
                   
                    client.Headers.Add("user-agent", DefaultUserAgent);
                    var data = client.DownloadData(iconPath);
                    if (data == null || data.Length==0)
                    {
                        return ObjectHelper.ImageToByte(BookmarkManager.Properties.Resources.Hyperlink);
                    }
                    else
                    {
                        // 测试转换是否正常
                        try
                        {
                            ByteArrayToBitmapImageConverter converter = new ByteArrayToBitmapImageConverter();
                            converter.ConvertByteArrayToBitMapImage(data);
                            return data;
                        }
                        catch
                        {
                            return ObjectHelper.ImageToByte(BookmarkManager.Properties.Resources.Hyperlink);
                        }
                    }
                }
            }
            catch
            {
                return ObjectHelper.ImageToByte(BookmarkManager.Properties.Resources.Hyperlink);
            }
           
        }

        private static string ReadAsString(byte[] buff)
        {
            // 通杀 https://www.cnblogs.com/swtseaman/archive/2012/10/04/2711836.html 代码在评论区
            Stream stream = new MemoryStream(buff);
            Int64 position = 0;
            try
            {
                stream.Position = position;
                StreamReader reader = new StreamReader(stream, new UTF8Encoding(false, true), true);
                return reader.ReadToEnd();
            }
            catch (DecoderFallbackException)
            {
                stream.Position = position;
                StreamReader reader2 = new StreamReader(stream, Encoding.Default);
                Debug.Print(reader2.CurrentEncoding.ToString());
                return reader2.ReadToEnd();
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message + ":" + ex.StackTrace);
            }
            return "";
        }
    }
}

