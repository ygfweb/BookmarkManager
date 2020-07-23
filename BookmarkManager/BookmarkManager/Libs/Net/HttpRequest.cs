using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using BookmarkManager.Libs.Net.Common;

namespace BookmarkManager.Libs.Net
{
    /// <summary>
    /// HTTP请求
    /// </summary>
    public class HttpRequest
    {
        /// <summary>
        /// 请求Url，不能为空
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 请求方法，默认为GET
        /// </summary>
        public string Method { get; set; } = "GET";

        /// <summary>
        /// 是否保持连接，默认为false
        /// </summary>
        public bool KeepAlive { get; set; } = false;

        /// <summary>
        /// 是否允许自动跳转，默认为false
        /// </summary>
        public bool AllowAutoRedirect { get; set; } = false;

        /// <summary>
        /// 建立连接所花费的时间，默认10秒
        /// </summary>
        public int Timeout { get; set; } = 10 * 1000;

        /// <summary>
        /// 建立连接后尝试读取或写入数据所花费的时间，默认30秒
        /// </summary>
        public int ReadWriteTimeout { get; set; } = 30 * 1000;

        /// <summary>
        /// 客户端访问信息，默认为WIN10的chrome浏览器
        /// </summary>
        public string UserAgent { get; set; } = UserAgents.PC;

        /// <summary>
        /// 数据的编码，默认为UTF8,如果需要对其自动识别，需要将其设置为null，常用编码有utf-8,gbk,gb2312
        /// </summary>
        public Encoding Encoding { get; set; } = Encoding.UTF8;

        /// <summary>
        /// 最大连接数
        /// </summary>
        public int Connectionlimit { get; set; } = 1024;

        /// <summary>
        /// 代理,默认为null
        /// </summary>
        public IWebProxy Proxy { get; set; } = null;

        /// <summary>
        /// HTTP头：Host，默认为空
        /// </summary>
        public string Host { get; set; } = String.Empty;

        /// <summary>
        /// cookie容器，默认为空
        /// </summary>
        public CookieContainer CookieContainer { get; set; } = null;

        /// <summary>
        /// 来源地址，上次访问地址，默认为空
        /// </summary>
        public string Referer { get; set; } = String.Empty;

        /// <summary>
        /// 设置请求将跟随的重定向的最大数目，默认为0
        /// </summary>
        public int MaximumAutomaticRedirections { get; set; } = 0;

        /// <summary>
        /// 请求返回类型，默认为text/html
        /// </summary>
        public string ContentType { get; set; } = MimeTypes.Html;

        /// <summary>
        /// HTTP头信息
        /// </summary>
        public WebHeaderCollection HeaderCollection { get; set; } = new WebHeaderCollection();

        /// <summary>
        /// Post请求时要发送的数据
        /// </summary>
        public byte[] PostData { get; set; } = null;


        /// <summary>
        /// HTTP请求
        /// </summary>
        /// <param name="url"></param>
        public HttpRequest(string url)
        {
            this.Url = url;
        }

        /// <summary>
        /// 设置POST提交的数据，使用本类的PostEncoding属性进行编码
        /// </summary>
        public void SetPostData(string data)
        {
            if (!string.IsNullOrWhiteSpace(data))
            {
                if (this.Encoding != null)
                {
                    this.PostData = this.Encoding.GetBytes(data);
                }
                else
                {
                    this.PostData = Encoding.UTF8.GetBytes(data);
                }
            }
            else
            {
                this.PostData = null;
            }
        }

        /// <summary>
        /// 设置HTTP头
        /// </summary>
        public void SetHttpHeader(string header, string value)
        {
            this.HeaderCollection.Set(header, value);
        }
        /// <summary>
        /// 创建原生的HttpWebRequest
        /// </summary>
        public HttpWebRequest CreateHttpWebRequest()
        {
            return RequestFactory.CreateHttpWebRequest(this);
        }

        /// <summary>
        /// 发送请求，并获取原生的HttpWebResponse，该返回值必须用using语句包裹
        /// </summary>
        public async Task<HttpWebResponse> GetResponseAsync()
        {
            HttpWebRequest request = this.CreateHttpWebRequest();
            try
            {
                return await request.GetResponseAsync() as HttpWebResponse;
            }
            catch (WebException e)
            {
                HttpWebResponse response = e.Response as HttpWebResponse;
                if (response == null)
                {
                    throw e;
                }
                else
                {
                    return response;
                }
            }
        }

        /// <summary>
        /// 发送HTTP请求，并获取响应内容，该方法会将响应内容一次性加载到内存，因此不能用于下载文件
        /// </summary>
        public async Task<HttpResponse> SendAsync()
        {
            HttpWebResponse webResponse = await this.GetResponseAsync();
            return webResponse.ToHttpResponse(this.Encoding);
        }    
    }
}


