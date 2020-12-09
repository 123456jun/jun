using CefSharp;
using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TradeWolfRobot.EmailAutomation.View;

namespace TradeWolfRobot.EmailAutomation
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            CefInitialize();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
        public static void CefInitialize()
        {
            CefSettings setting = new CefSettings();
            CefSharpSettings.LegacyJavascriptBindingEnabled = true;
            CefSharpSettings.WcfEnabled = true;

            setting.Locale = "locales";
            //添加日志
            setting.LogSeverity = LogSeverity.Error;
            //缓存目录
            //setting.CachePath = "cache";
            //启动WebRTC
            setting.CefCommandLineArgs.Add("enable-media-stream", "1");
            //禁用代理服务器
            //setting.CefCommandLineArgs.Add("no-proxy-server", "1");
            //禁用gpu
            setting.CefCommandLineArgs.Add("disable-gpu", "1");
            //                    "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/78.0.3904.108 Safari/537.36";
            //setting.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.83 Safari/537.36";
            setting.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.102 Safari/537.36 Edg/85.0.564.51";
            //setting.UserAgent = "User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:80.0) Gecko/20100101 Firefox/80.0";
            
              //   setting.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/78.0.3904.108 Safari/537.36";
            //setting.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";
            
            //setting.Locale = "zh-CN";
            //存储cookie会话
            //setting.PersistSessionCookies = true;
       
            Cef.Initialize(setting);
        }
    }
}
