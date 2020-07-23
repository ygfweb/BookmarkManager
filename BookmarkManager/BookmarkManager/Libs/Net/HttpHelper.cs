using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BookmarkManager.Libs.Net
{
    /// <summary>
    /// HTTP帮助类
    /// </summary>
    public static class HttpHelper
    {
        /// <summary>
        /// 发送GET请求
        /// </summary>
        public static async Task<HttpResponse> GetAsync(string url)
        {
            HttpClient client = new HttpClient();
            return await client.GetAsync(url);
        }


        /// <summary>
        /// POST(Json数据)
        /// </summary>
        public static async Task<HttpResponse> PostJsonAsync(string url, string json)
        {
            HttpClient client = new HttpClient();
            return await client.PostJsonAsync(url, json);
        }


        /// <summary>
        /// POST（表单）
        /// </summary>
        public static async Task<HttpResponse> PostFormAsync(string url, Dictionary<string, string> postParameters)
        {
           HttpClient client = new HttpClient();
           return await client.PostFormAsync(url, postParameters);
        }
    }
}

