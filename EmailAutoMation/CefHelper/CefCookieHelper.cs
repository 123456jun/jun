using CefSharp;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CefSharp.WinForms;
namespace CefWebBrowser.CefHelper
{
    public class CefCookieHelper
    {
        //ICookieManager CefCookieManager;
        //public CefCookieHelper()
        //{
        //    CefCookieManager = Cef.GetGlobalCookieManager();
        //}
        //public List<Cookie> GetAllCookieAnsyc()
        //{
        //    List<Cookie> cookieList = new List<Cookie>();
        //    var cookieVisitor = new TaskCookieVisitor();
        //    Task<List<Cookie>> cooklist = CefCookieManager.VisitAllCookiesAsync();
        //    cooklist.Wait();
        //    cookieList = cooklist.Result;
        //    return cookieList;
        //}
        ///// <summary>
        ///// 隔离cookie
        ///// </summary>
        //public void IsolationCookie(string accountName)
        //{
        //    RequestContextSettings requestContextSettings = new RequestContextSettings
        //    {
        //        PersistSessionCookies = false,
        //        PersistUserPreferences = false
        //    };
        //    RequestContext context = new RequestContext(new RequestContextSettings()
        //    {
        //        CachePath = "AppCaches/" + accountName,
        //    });
        //}
        /// <summary>
        /// 获取特定Url的cookie
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static List<Cookie> GetCookieList(string url = null, List<string> urlList = null)
        {
            List<Cookie> resultCookieList = new List<Cookie>();
            ICookieManager cefCookieManager = Cef.GetGlobalCookieManager();
            List<Cookie> cookieList = new List<Cookie>();
            var cookieVisitor = new TaskCookieVisitor();
            Task<List<Cookie>> cooklistTask = cefCookieManager.VisitAllCookiesAsync();
            cooklistTask.Wait();
            cookieList = cooklistTask.Result;
            if (urlList != null)
            {
                foreach (var item in cookieList)
                {
                    foreach (var urls in urlList)
                    {
                        if (item.Domain == null)
                        {
                            continue;
                        }
                        if (item.Domain.ToLower().Contains(urls.ToLower()))
                        {
                            resultCookieList.Add(item);
                            break;
                        }
                    }
                }
                return resultCookieList;
            }
            else if (url != null)
            {
                if (url.Contains("http"))
                {
                    url = new Uri(url).Host;
                }
                url = url.ToLower();
                foreach (var item in cookieList)
                {
                    if (item.Domain == null)
                        continue;
                    if (item.Domain.ToLower().Contains(url))
                    {
                        resultCookieList.Add(item);
                    }
                }
            }
            else
            {
                resultCookieList = cookieList;
            }
            return resultCookieList;
        }

        /// <summary>
        /// 设置特定Url的Cookie
        /// </summary>
        /// <param name="url"></param>
        /// <param name="cookieList"></param>
        public static void SetCookie(string url, object cookie)
        {
            List<Cookie> cookieList = new List<Cookie>();
            cookieList = cookie as List<Cookie>;
            if (cookie != null)
            {
                if (cookieList == null)
                {
                    try
                    {
                        cookieList = JsonConvert.DeserializeObject<List<Cookie>>(cookie.ToString());
                    }
                    catch (Exception ex)
                    {
                        return;
                    }
                }
            }
            ICookieManager cefCookieManager = Cef.GetGlobalCookieManager();
            if (cookieList != null)
            {
                for (int i = 0; i < cookieList.Count; i++)
                {
                    cefCookieManager.SetCookie(url, cookieList[i]);
                }
            }
        }


        public static void DeleteCookie(string url, string cookieJson = null)
        {
            ICookieManager cefCookieManager = Cef.GetGlobalCookieManager();
            if (cookieJson != null)
            {
       
                List<Cookie> cookies = JsonConvert.DeserializeObject<List<Cookie>>(cookieJson);
                foreach (var item in cookies)
                {
                    cefCookieManager.DeleteCookiesAsync(url, item.Name).Wait();
                }
            }
            else
            {
                cefCookieManager.DeleteCookiesAsync(url).Wait();
            }
        }

        /// <summary>
        /// 获取或删除cookie
        /// </summary>
        /// <param name="url">特定的url</param>
        /// <param name="isDelte">true 删除cookie false获取cookie</param>
        /// <returns></returns>
        public static async Task<List<Cookie>> GetUrlCookiesAsync(ICookieManager cefCookieManager, string url, bool isDelte = true)
        {
            CookieVisitor vistor = new CookieVisitor(isDelte);
            cefCookieManager.VisitUrlCookies(url, true, vistor);
            List<Cookie> cookieList = await vistor.Task;
            return cookieList;
        }
        private class CookieVisitor : ICookieVisitor
        {
            /// <summary>
            /// 回调发送cookie
            /// </summary>
            public event Action<Cookie> GetCookieAction;
            private readonly TaskCompletionSource<List<Cookie>> Source;
            public Task<List<Cookie>> Task;
            private List<Cookie> CookieList = new List<Cookie>();
            bool IsDeleteCookie = false;
            public void Dispose()
            {
                if (Source.Task.Status != TaskStatus.RanToCompletion)
                {
                    Source.SetResult(CookieList);
                }
            }
            public bool Visit(Cookie cookie, int count, int total, ref bool deleteCookie)
            {
                CookieList.Add(cookie);
                deleteCookie = IsDeleteCookie;
                GetCookieAction?.Invoke(cookie);
                if (count == total - 1)
                {
                    Source.SetResult(CookieList);
                }
                else if (total == 0)
                {
                    Source.SetResult(CookieList);
                }
                return true;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="isDeleteCookie">是否删除当前cookie</param>
            public CookieVisitor(bool isDeleteCookie)
            {
                IsDeleteCookie = isDeleteCookie;
                Source = new TaskCompletionSource<List<Cookie>>();
                Task = Source.Task;
            }
        }
    }
}
