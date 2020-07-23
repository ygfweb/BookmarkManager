using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace BookmarkManager.Libs.Net
{
    /// <summary>
    /// 扩展方法
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// 获取字节数组，如果响应内容是gzip格式，会自动进行解压
        /// </summary>
        public static byte[] GetBytes(this HttpWebResponse response)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }
            using (MemoryStream _stream = new MemoryStream())
            {
                //GZIIP处理
                if (response.ContentEncoding != null && response.ContentEncoding.Equals("gzip", StringComparison.InvariantCultureIgnoreCase))
                {
                    //开始读取流并设置编码方式
                    new GZipStream(response.GetResponseStream(), CompressionMode.Decompress).CopyTo(_stream, 1024);
                }
                else
                {
                    //开始读取流并设置编码方式
                    response.GetResponseStream().CopyTo(_stream, 1024);
                }
                //获取Byte
                return _stream.ToArray();
            }
        }
        /// <summary>
        /// 写入cookie到磁盘文件
        /// </summary>
        public static void WriteCookiesToDisk(this CookieContainer cookie, string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }
            using (Stream stream = File.Create(fileName))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, cookie);
            }
        }
        /// <summary>
        /// 从文件中读取cookie
        /// </summary>
        public static CookieContainer ReadCookiesFromDisk(this CookieContainer cookie, string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }
            FileInfo file = new FileInfo(fileName);
            if (!file.Exists)
            {
                throw new FileNotFoundException("待读取的文件不存在", file.FullName);
            }
            using (Stream stream = File.Open(fileName, FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return (CookieContainer)formatter.Deserialize(stream);
            }
        }
        /// <summary>
        /// 将HttpWebResponse转换为HttpResponse
        /// </summary>
        public static HttpResponse ToHttpResponse(this HttpWebResponse webResponse, Encoding encoding)
        {
            HttpResponse response = new HttpResponse()
            {
                CookieCollection = webResponse.Cookies,
                Header = webResponse.Headers,
                ResponseUri = webResponse.ResponseUri.ToString(),
                ResultByte = webResponse.GetBytes(),
                StatusCode = webResponse.StatusCode,
                StatusDescription = webResponse.StatusDescription,
                CharacterSet = webResponse.CharacterSet,
                Encoding = encoding
            };
            return response;
        }
    }
}
