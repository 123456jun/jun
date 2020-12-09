using CefWebBrowser;
using RobotForeignWorkflow.Data;
using RobotForeignWorkflow.Model.ConfigModel;
using RobotForeignWorkflow.Model.EmailModel;
using RobotForeignWorkflow.Model.FrontModel;
using RobotForeignWorkflow.Model.SocialPlatFormModel;
using System.Collections.Generic;
using System.Windows.Forms;
using RobotForeignWorkflow.Model;
using System;
using System.IO;
using Newtonsoft.Json;

namespace RobotForeignWorkflow
{
    public static class GlobalClass
    {
        public static bool IsDebug = false;

        public static bool IsStop = false;
        public static Dictionary<string, AccountData> SocailAccountDic { get; set; } = new Dictionary<string, AccountData>();
        public delegate string StopTaskHandle();
        /// <summary>
        /// 全局的停止事件
        /// </summary>
        public static event StopTaskHandle _StopTaskEvent;
        /// <summary>
        /// 停止任务
        /// </summary>
        public static string _StopTask()
        {
            return _StopTaskEvent?.Invoke();
        }
        /// <summary>
        /// 窗体
        /// </summary>
        public static Dictionary<string, Form> FormDic = new Dictionary<string, Form>();
        /// /// <summary>
        /// 加载状态
        /// </summary>
        public static bool IsLoaded = true;

        public static string SharpToJsName = "SharpObj";
        /// <summary>
        /// 邮箱账号最多个数
        /// </summary>
        public static int EmailAcountMaxCount = 3;
        /// <summary>
        /// 存放所有窗体
        /// </summary>
        public static Dictionary<string, CefWebBrowserControl> CefWebBrowserDic = new Dictionary<string, CefWebBrowserControl>();
        /// <summary>
        /// 所有账号信息状态
        /// </summary>
        public static Dictionary<string, EmailplatForm> SocialplatFormListDic { get; set; } = null;
        public static List<string> GetCache()
        {
            List<string> cachePath = new List<string>();
            for (int i = 1; i <= EmailAcountMaxCount; i++)
            {
                cachePath.Add("cachePath" + i);
            }
            return cachePath;
        }
        public static string GetDBFileName()
        {
            string path = Application.StartupPath + "\\Data\\RobotForeign.db";
            return path;
        }

    
    }
}
