using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BookmarkManager.Libs.Net.Common;

namespace BookmarkManager.Libs.Net
{
    /// <summary>
    /// HTTP客户端，对多次请求共享相同的cookie，并自动设置Referer
    /// </summary>
    public class HttpClient
    {
        /// <summary>
        /// cookie容器
        /// </summary>
        protected CookieContainer CookieContainer { get; set; } = new CookieContainer();
        /// <summary>
        /// 线程锁
        /// </summary>
        protected readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        /// <summary>
        /// 是否启用频率控制
        /// </summary>
        public bool IsEnableFrequency { get; set; } = false;
        /// <summary>
        /// 上次访问网址
        /// </summary>
        protected string Referer { get; set; } = "";
        /// <summary>
        /// 调用频率最小时间,单位毫秒，默认值为3秒
        /// </summary>
        public int MinTime { get; set; } = 3 * 1000;
        /// <summary>
        /// 调用频率最大时间，单位毫秒，默认值为10秒
        /// </summary>
        public int MaxTime { get; set; } = 10 * 1000;

        /// <summary>
        /// 代理设置，默认为空
        /// </summary>
        public IWebProxy Proxy { get; set; } = null;

        /// <summary>
        /// 发送HTTP请求
        /// </summary>
        public async Task<HttpResponse> SendAsync(HttpRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            // 限制调用频率
            if (this.IsEnableFrequency)
            {
                Random random = new Random(Guid.NewGuid().GetHashCode());
                int time = random.Next(this.MinTime, this.MaxTime);
                await Task.Delay(time);
            }
            await this.semaphore.WaitAsync();
            try
            {
                if (request.CookieContainer != null)
                {
                    // 如果request的cookie容器不为null，则覆盖HttpClient的cookie
                    this.CookieContainer = request.CookieContainer;
                }
                else
                {
                    // 如果request的cookie容器为null，则设置其cookie，使其能共享cookie
                    request.CookieContainer = this.CookieContainer;
                }

                HttpResponse response = await request.SendAsync();
                this.CookieContainer.Add(response.CookieCollection);
                return response;
            }
            finally
            {
                this.semaphore.Release();
            }
        }
        /// <summary>
        /// 创建HTTP请求
        /// </summary>
        protected HttpRequest CreateHttpRequest(string url)
        {
            HttpRequest request = new HttpRequest(url);
            request.Proxy = this.Proxy;
            return request;
        }

        /// <summary>
        /// 发送Get请求
        /// </summary>
        public async Task<HttpResponse> GetAsync(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentNullException(nameof(url));
            }
            HttpRequest request = CreateHttpRequest(url);
            request.Method = "GET";
            return await this.SendAsync(request);
        }

        /// <summary>
        ///  POST（表单）
        /// </summary>
        public async Task<HttpResponse> PostFormAsync(string url, Dictionary<string, string> postParameters)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentNullException(nameof(url));
            }

            if (postParameters == null || postParameters.Count == 0)
            {
                throw new ArgumentNullException(nameof(postParameters));
            }
            // 构建表单数据
            StringBuilder sb = new StringBuilder();
            foreach (var item in postParameters)
            {
                sb.Append($"{item.Key}={item.Value}&");
            }
            string postData = sb.ToString().Trim('&');
            HttpRequest request = this.CreateHttpRequest(url);
            request.Method = "POST";
            request.ContentType = MimeTypes.Form;
            request.SetPostData(postData);
            return await this.SendAsync(request);
        }

        /// <summary>
        /// POST(Json数据)
        /// </summary>
        public async Task<HttpResponse> PostJsonAsync(string url, string json)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentNullException(nameof(url));
            }

            if (string.IsNullOrWhiteSpace(json))
            {
                throw new ArgumentNullException(nameof(json));
            }
            HttpRequest request = this.CreateHttpRequest(url);
            request.Method = "POST";
            request.ContentType = MimeTypes.JSON;
            request.SetPostData(json);
            return await this.SendAsync(request);
        }
    }
}

