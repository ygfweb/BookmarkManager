using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Text;
using System.Threading.Tasks;

namespace BookmarkManager.Libs.Net
{
    /// <summary>
    /// HTTP请求工厂类
    /// </summary>
    public static class RequestFactory
    {
        /// <summary>
        /// 创建HTTP请求
        /// </summary>
        public static HttpWebRequest CreateHttpWebRequest(HttpRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            // 无视证书(这一句一定要写在创建连接的前面)
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, errors) => { return true; };
            HttpWebRequest webRequest = WebRequest.CreateHttp(request.Url);
            if (!string.IsNullOrWhiteSpace(request.Host))
            {
                webRequest.Host = request.Host;
            }
            webRequest.Method = request.Method.ToUpper();
            webRequest.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore); // 禁用缓存
            webRequest.Timeout = request.Timeout;
            webRequest.ReadWriteTimeout = request.ReadWriteTimeout;
            webRequest.CookieContainer = request.CookieContainer;
            webRequest.Referer = request.Referer;
            webRequest.ContentType = request.ContentType;
            webRequest.AllowAutoRedirect = request.AllowAutoRedirect;
            if (request.MaximumAutomaticRedirections > 0)
            {
                webRequest.MaximumAutomaticRedirections = request.MaximumAutomaticRedirections;
            }
            webRequest.CookieContainer = request.CookieContainer;
            webRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3";
            webRequest.Headers.Add("accept-language", "zh-CN,zh;q=0.9,en;q=0.8");
            webRequest.Headers.Add("accept-encoding", "gzip, deflate, br");
            webRequest.UserAgent = request.UserAgent;
            webRequest.ProtocolVersion = System.Net.HttpVersion.Version11;
            if (request.HeaderCollection != null && request.HeaderCollection.Count > 0)
            {
                foreach (string key in request.HeaderCollection.Keys)
                {
                    webRequest.Headers.Add(key, request.HeaderCollection[key]);
                }
            }
            webRequest.Proxy = request.Proxy;
            if (request.PostData != null && request.PostData.Length > 0)
            {
                webRequest.ContentLength = request.PostData.Length;
                webRequest.GetRequestStream().Write(request.PostData, 0, request.PostData.Length);
            }

            return webRequest;
        }
    }
}
