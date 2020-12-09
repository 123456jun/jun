using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using CefWebBrowser;
using CefWebBrowser.CefModel;
using EmailAutoMation.Model;
using System.Windows.Forms;
using CefSharpWebBrowser.CefHelper;
using TradeWolfRobot.EmailAutomation.Model;
using TradeWolfRobot.EmailAutomation.Execute;

namespace TradeWolfRobot.EmailAutomation
{
    /// <summary>
    /// 是否加载成功
    /// </summary>
    /// <param name="isLoad"></param>
    public delegate void FailedToLoadHander(bool isLoad);
    /// <summary>
    /// 是否登录成功
    /// </summary>
    /// <param name="isLogin"></param>
    public delegate void LoginHandler(bool isLogin);

    abstract public class EmailBase : JsLogHelper
    {
        public bool _StopTask = false;

        //登录超时
        public int LoginOutTime = 30 * 1000;
        /// <summary>
        /// 登录检测时间 
        /// </summary>
        public int LoginTestingTime = 10 * 1000;
        List<string> _errorEmailList = new List<string>();

        public List<string> ErrorEmailList { get; set; } = new List<string>();

        /// <summary>
        /// 邮箱名称
        /// </summary>
        public string EmailName { get; set; }


        public CefWebBrowserControl CefWebBrowser;
        /// <summary>
        /// 每个步骤间隔
        /// </summary>
        public int ExecuteInterval = 3 * 1000;


        public int CefBrowserLoadTimeOut = 30 * 1000; //cef的加载时间
        /// <summary>
        /// 定时器刷新页面
        /// </summary>
        public System.Timers.Timer Timer = new System.Timers.Timer();
        /// <summary>
        /// 页面加载失败
        /// </summary>
        public event FailedToLoadHander FailedToLoadEvent;
        /// <summary>
        /// 登录失效
        /// </summary>
        public event LoginHandler LoginEvent;
        /// <summary>
        /// 是否登录成功
        /// </summary>
        public bool IsLoginSuccess
        {
            get { return false; }
        }
        public EmailBase(CefWebBrowserControl cefWebBrowser)
        {
            CefWebBrowser = cefWebBrowser;
            Timer.Interval = 300 * 1000;
            Timer.Elapsed += Timer_Elapsed;
            Timer.Start();
        }
        public EmailBase()
        {

        }
        public EmailBase(string cachePath)
        {

        }
        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            CefWebBrowser.Reload();
        }
        /// <summary>
        /// 执行邮件发送
        /// </summary>
        /// <returns></returns>
        public JsExecuteResult ExecuteEmailSend(EmailInfo emailInfo)
        {
            ErrorEmailList.Clear();
            
            if (EmailExecute._stopSend == true)
            {
                EmailExecute.AutoResetEvent.WaitOne();
            }
            JsExecuteResult jsResult = ClickWriteEmail();
            if (jsResult.IsStop || jsResult == null || jsResult.Success == false || _StopTask)
            {
                jsResult.IsStop = true;
                return jsResult;
            }
            Thread.Sleep(ExecuteInterval);
            if (EmailExecute._stopSend == true)
            {
                EmailExecute.AutoResetEvent.WaitOne();
            }
            jsResult = WriteEmailAddress(emailInfo.Addressee);
            if (jsResult.IsStop || jsResult == null || jsResult.Success == false || _StopTask)
            {
                jsResult.IsStop = true;
                return jsResult;
            }
            Thread.Sleep(ExecuteInterval);
            jsResult = ClickCCList(emailInfo.CCAddress,out _errorEmailList);
            if (jsResult.IsStop || jsResult == null || jsResult.Success == false || _StopTask)
            {
                jsResult.IsStop = true;
                return jsResult;
            }
            Thread.Sleep(ExecuteInterval);
            if (EmailExecute._stopSend == true)
            {
                EmailExecute.AutoResetEvent.WaitOne();
            }
            jsResult = ClickBCCList(emailInfo.BCCAddress,out _errorEmailList);
            if (jsResult.IsStop || jsResult == null || jsResult.Success == false || _StopTask)
            {
                jsResult.IsStop = true;
                return jsResult;
            }
            Thread.Sleep(ExecuteInterval);
            if (EmailExecute._stopSend == true)
            {
                EmailExecute.AutoResetEvent.WaitOne();
            }
            jsResult = WriteEmailTheme(emailInfo.Theme);
            if (jsResult.IsStop || jsResult == null || jsResult.Success == false || _StopTask)
            {
                jsResult.IsStop = true;
                return jsResult;
            }
            Thread.Sleep(ExecuteInterval);
            if (EmailExecute._stopSend == true)
            {
               EmailExecute.AutoResetEvent.WaitOne();
            }
            jsResult = WriteEmailBody(emailInfo.Body);
            if (jsResult.IsStop || jsResult == null || jsResult.Success == false || _StopTask)
            {
                jsResult.IsStop = true;
                return jsResult;
            }
            Thread.Sleep(ExecuteInterval);
            if (EmailExecute._stopSend == true)
            {
                EmailExecute.AutoResetEvent.WaitOne();
            }
            jsResult = ClickSendEmail();
            return jsResult;
        }
        /// <summary>
        /// 页面加载是否成功
        /// </summary>
        /// <returns></returns>
        public bool IsLoadPageSuccess()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (!CefWebBrowser.IsLoad)
            {
                sw.Stop();
                if (sw.ElapsedMilliseconds >= CefBrowserLoadTimeOut)
                {
                    Console.WriteLine(sw.ElapsedMilliseconds);
                    return false;
                }
                else
                {
                    sw.Start();
                }
            }
            return true;
        }
        /// <summary>
        /// 停止发送
        /// </summary>
        public void StopTask()
        {
            _StopTask = true;
            if (CefWebBrowser != null)
            {
                CefWebBrowser.StopWhile = true;
            }
        }
        public void DeleteCookie()
        {
            CefWebBrowser.DeleteLocalCookie();
        }
        /// <summary>
        /// 设置Cookie
        /// </summary>
        abstract public void SetCookie(object cookie);
        /// <summary>
        /// 获取Cookie
        /// </summary>
        /// <returns></returns>
        abstract public object GetCookie();


        /// <summary>
        /// 是否登录成功
        /// </summary>
        /// <param name="account">账号</param>
        /// <param name="password">密码</param>
        /// <param name="recursionCount">递归次数</param>
        /// <returns></returns>
        abstract public LoginState IsLogin(string account, string password, int recursionCount = 1);

        /// <summary>
        /// 进入写信页面
        /// </summary>
        abstract public JsExecuteResult ClickWriteEmail();
        /// <summary>
        /// 写入收件人邮箱地址 
        /// </summary>
        /// <param name="emailAddress">邮箱地址</param>
        abstract public JsExecuteResult WriteEmailAddress(string emailAddress);
        /// <summary>
        /// 写入邮件的主题 
        /// </summary>
        /// <param name="emailTheme"></param>
        /// <returns></returns>
        abstract public JsExecuteResult WriteEmailTheme(string emailTheme);
        /// <summary>
        /// 写入邮件正文
        /// </summary>
        /// <param name="body">邮件正文</param>
        abstract public JsExecuteResult WriteEmailBody(string emailBody);
        /// <summary>
        /// 点击邮件按键
        /// </summary>
        abstract public JsExecuteResult ClickSendEmail();
        /// <summary>
        /// 是否需要验证码
        /// </summary>
        /// <returns></returns>
        abstract public bool IsNeedVerificationCode();

        


        /// <summary>
        /// 写入抄送
        /// </summary>
        /// <param name="emailStrList">抄送邮箱列表</param>
        /// <param name="errorEmailList">错误邮箱列表</param>
        /// <returns></returns>
        abstract public JsExecuteResult ClickCCList(List<string> emailStrList,out List<string> errorEmailList);

        /// <summary>
        /// 写入密送
        /// </summary>
        /// <param name="emailStrList">密送邮箱列表</param>
        /// <param name="errorEmailList">错误邮箱列表</param>
        /// <returns></returns>
        abstract public JsExecuteResult ClickBCCList(List<string> emailStrList, out List<string> errorEmailList);
    }
}
