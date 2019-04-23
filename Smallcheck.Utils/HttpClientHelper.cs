using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace Smallcheck.Utils
{
    /// <summary>
    /// HttpCient Helper
    /// </summary>
    public class HttpClientHelper
    {
        #region HTTP POST请求 +static string HttpPost(string requestUri, Dictionary<string, string> postData, Dictionary<string, string> headers = null, CookieCollection cookies = null)
        /// <summary>
        /// HTTP POST请求
        /// </summary>
        /// <param name="requestUri">请求URL</param>
        /// <param name="postData">请求数据</param>
        /// <param name="headers">请求头</param>
        /// <param name="cookies">请求Cookies</param>
        /// <returns>响应数据 非200响应 返回为null</returns>
        public static string HttpPost(string requestUri, Dictionary<string, string> postData, Dictionary<string, string> headers = null, CookieCollection cookies = null)
        {
            CookieContainer cookieContainer = new CookieContainer();
            using (HttpClientHandler handler = new HttpClientHandler { CookieContainer = cookieContainer })
            using (HttpClient client = new HttpClient(handler))
            {
                client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
                // 添加Http headers
                if (headers != null)
                {
                    foreach (var item in headers)
                    {
                        // 使用TryAddWithoutValidation而非Add方法能避免 无效格式 错误
                        client.DefaultRequestHeaders.TryAddWithoutValidation(item.Key, item.Value);
                    }
                }
                // 添加 cookie
                if (cookies != null)
                {
                    cookieContainer.Add(cookies);
                }
                HttpContent postContent = new FormUrlEncodedContent(postData);
                HttpResponseMessage response = client.PostAsync(requestUri, postContent).Result;
                if (response.IsSuccessStatusCode)
                {
                    return response.Content.ReadAsStringAsync().Result;
                }
                return null;
            }
        }
        #endregion
    }
}
