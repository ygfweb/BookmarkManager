using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace BookmarkManager.Libs.Net
{
    /// <summary>
    /// 代理工厂类
    /// </summary>
    public static class ProxyFactory
    {
        /// <summary>
        /// 创建系统默认的代理
        /// </summary>
        /// <returns></returns>
        public static IWebProxy GetSystemWebProxy()
        {
            return WebRequest.GetSystemWebProxy();
        }

        /// <summary>
        /// 获取代理
        /// </summary>
        /// <param name="proxyIp">代理IP</param>
        /// <param name="proxyPort">端口号</param>
        /// <returns></returns>
        public static IWebProxy GetProxy(string proxyIp, int proxyPort)
        {
            return new WebProxy(proxyIp, proxyPort);
        }

        /// <summary>
        /// 获取代理
        /// </summary>
        /// <param name="proxyIp">代理IP</param>
        /// <param name="proxyUserName">用户名</param>
        /// <param name="proxyPassword">密码</param>
        /// <returns></returns>
        public static IWebProxy GetProxy(string proxyIp, string proxyUserName, string proxyPassword)
        {
            WebProxy proxy = new WebProxy(proxyIp, false);
            proxy.Credentials = new NetworkCredential(proxyUserName, proxyPassword);
            return proxy;
        }

        /// <summary>
        /// 获取代理
        /// </summary>
        /// <param name="proxyIp">代理IP</param>
        /// <param name="proxyPort">端口号</param>
        /// <param name="proxyUserName">用户名</param>
        /// <param name="proxyPassword">密码</param>
        /// <returns></returns>
        public static IWebProxy GetProxy(string proxyIp, int proxyPort, string proxyUserName, string proxyPassword)
        {
            WebProxy proxy = new WebProxy(proxyIp, proxyPort);
            proxy.Credentials = new NetworkCredential(proxyUserName, proxyPassword);
            return proxy;
        }
    }
}
