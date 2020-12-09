using CefSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CefWebBrowser;
using System.Threading;
using Newtonsoft.Json;
using CefWebBrowser.CefModel;
using CefSharpWebBrowser.CefHelper;
using CefWebBrowser.CefHelper;
using CefSharpWebBrowser.CefModel;
using TradeWolfRobot.EmailAutomation.Model;

namespace TradeWolfRobot.EmailAutomation.Email
{
    /// <summary>
    /// 网易163邮箱
    /// </summary>
    public class NetEase163Email : EmailBase
    {
        string msgbox = "js-component-msgbox nui-msgbox";
        string close = "js-component-button nui-mainBtn nui-btn";
        JsExecuteHelp _jsExecuteHelp;
        public NetEase163Email(CefWebBrowserControl CefWebBrowser) : base(CefWebBrowser)
        {
            EmailName = "163Email";
            _jsExecuteHelp = new JsExecuteHelp(CefWebBrowser);
        }
        public NetEase163Email() : base()
        {
            if (CefWebBrowser == null)
            {
                CefWebBrowser = new CefWebBrowserControl("https://mail.163.com/");
            }
            else
            {
                CefWebBrowser.LoadNewUrl("https://mail.163.com/");
            }
            EmailName = "163Email";
            _jsExecuteHelp = new JsExecuteHelp(CefWebBrowser);
        }
        /// <summary>
        /// 点击写信按键
        /// </summary>
        public override JsExecuteResult ClickWriteEmail()
        {
            ClickCloseLabel();
            Thread.Sleep(2000);
            string writeButton = "js-component-component ra0 mD0";
            DomNameOrId = writeButton;
            return _jsExecuteHelp.ExecuteClickByClass(DomNameOrId);
            //string jsString = @"document.getElementsByClassName('js-component-component ra0 mD0')[0].click()";
            //return CefWebBrowser.ExecuteJs(jsString);
        }
        /// <summary>
        /// 写入收件人邮箱地址 
        /// </summary>
        /// <param name="emailAddress">邮箱地址</param>
        public override JsExecuteResult WriteEmailAddress(string emailAddress)
        {

            string jsString = $" document.getElementsByClassName('nui-editableAddr-ipt')[0].value='{emailAddress}'";
            JsExecuteResult jsExecuteResult = CefWebBrowser.ExecuteJs(jsString);

            try
            {
                //获取收件人的绝对位置
                string jsAddresseeTop = "document.getElementsByClassName('js-component-emailcontainer nui-multiLineIpt C-multiLineIpt nui-ipt')[0].getBoundingClientRect().top";
                string jsAddresseeLeft = "document.getElementsByClassName('js-component-emailcontainer nui-multiLineIpt C-multiLineIpt nui-ipt')[0].getBoundingClientRect().left";
                string AddresseeTop = CefWebBrowser.ExecuteJs(jsAddresseeTop).Result;
                string AddresseeLeft = CefWebBrowser.ExecuteJs(jsAddresseeLeft).Result;
                float top = float.Parse(AddresseeTop);
                float left = float.Parse(AddresseeLeft);
                //再次点击收件人文本框
                CefWebBrowser.CefMouseClick(top + 9.0f, left + 9.0f);
                Thread.Sleep(500);
            }
            catch (Exception ex)
            {
                jsExecuteResult.Success = false;
                return jsExecuteResult;
            }
            return jsExecuteResult;


            //string jsString = $"document.getElementsByClassName('js-component-emailtips nui-ipt-placeholder')[0].innerText='';" +
            //    $" document.getElementsByClassName('nui-editableAddr-ipt')[0].value='{emailAddress}'";
            //JsExecuteResult jsExecuteResult = CefWebBrowser.ExecuteJs(jsString);
            //return jsExecuteResult;
        }
        /// <summary>
        /// 写入邮件的主题 
        /// </summary>
        /// <param name="emailTheme">邮件主题</param>
        public override JsExecuteResult WriteEmailTheme(string emailTheme)
        {
            string jsString = @"document.getElementsByClassName('nui-ipt-input')[2].value='';
                                document.getElementsByClassName('nui-ipt-input')[2].value='" + emailTheme + "'";
            return CefWebBrowser.ExecuteJs(jsString);
        }
        /// <summary>
        /// 写入邮件正文
        /// </summary>
        /// <param name="body">邮件正文</param>
        public override JsExecuteResult WriteEmailBody(string emailBody)
        {
            //进入源码模式
            string jsString = @"document.getElementsByClassName('ico ico-editor ico-editor-source')[0].click();";
            JsExecuteResult jsExecuteResult = CefWebBrowser.ExecuteJs(jsString);
            //写入源码
            jsString = $"document.getElementsByClassName('APP-editor-textarea')[0].value=`{emailBody}`";
            jsExecuteResult = CefWebBrowser.ExecuteJs(jsString);
            //退出源码模式
            jsString = "document.getElementsByClassName('APP-editor-source-code-exit')[0].click();";
            jsExecuteResult = CefWebBrowser.ExecuteJs(jsString);
            return jsExecuteResult;
        }
        /// <summary>
        /// 点击邮件发送按键
        /// </summary>
        public override JsExecuteResult ClickSendEmail()
        {
            string jsString = @"document.getElementsByClassName('js-component-icon nui-ico nui-ico-sent  nui-ico-sent-white')[0].click()";
            JsExecuteResult jsExecuteResult = CefWebBrowser.ExecuteJs(jsString);
            Thread.Sleep(3 * 1000);
            if (CefWebBrowser.GetHtmlText().Contains(msgbox))
            {
                jsString = $"document.getElementsByClassName('{msgbox}')[0].getElementsByClassName('{close}')[0].click();";
                jsExecuteResult = CefWebBrowser.ExecuteJs(jsString);
                jsExecuteResult.Success = false;
            }
            return jsExecuteResult;
        }
        /// <summary>
        /// 点击关闭标签
        /// </summary>
        /// <returns></returns>
        private JsExecuteResult ClickCloseLabel()
        {
            string jsString = @"var close= document.getElementsByClassName('js-component-icon nui-ico nui-ico-close');
                                var i=0;
                                while(i<close.length)
                                {
                                    close[i].click();
                                    i++;
                                }";
            JsExecuteResult jsExecuteResult = CefWebBrowser.ExecuteJs(jsString);
            if (CefWebBrowser.GetHtmlText().Contains(msgbox))
            {
                jsString = $"document.getElementsByClassName('{msgbox}')[0].getElementsByClassName('{close}')[0].click();";
                jsExecuteResult = CefWebBrowser.ExecuteJs(jsString);
                jsExecuteResult.Success = false;
                Thread.Sleep(1 * 1000);
            }
            return jsExecuteResult;
        }

        public override object GetCookie()
        {
            return CefCookieHelper.GetCookieList("163.com");
            //return CefWebBrowser.GetCookieList("163.com");
        }
        public override void SetCookie(object cookie)
        {
            List<Cookie> cookieList = new List<Cookie>();
            cookieList = cookie as List<Cookie>;
            if (cookie != null)
            {
                if (cookieList == null)
                {
                    cookieList = JsonConvert.DeserializeObject<List<Cookie>>(cookie.ToString());
                }
                if (cookieList != null)
                {
                    CefCookieHelper.SetCookie("https://mail.163.com/", cookieList);
                }
            }
        }
        public override LoginState IsLogin(string account, string password, int recursionCount = 1)
        {

            LoginState loginState = new LoginState();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            string loginClassStr = "main-inner";
            string loginsuccessClassStr = "gWel frame-main";
            //是否为登录页面
            bool isLoginPage = false;

            while (true)
            {
                Thread.Sleep(1000);
                if (_StopTask)
                {
                    return loginState;
                }
                if (!isLoginPage)
                {
                    stopwatch.Restart();
                    //出现登录页面，10s等待是否能自动登录
                    if (CefWebBrowser.GetHtmlText().Contains(loginClassStr))
                    {
                        //设置等待时间10s
                        LoginOutTime = LoginTestingTime;
                    }
                    isLoginPage = true;
                }
                if (CefWebBrowser.GetHtmlText().Contains(loginsuccessClassStr))
                {
                    //邮箱自动登录成功
                    loginState.IsLoginSuccess = true;
                    return loginState;
                }
                if (stopwatch.ElapsedMilliseconds > LoginOutTime)
                {
                    break;
                }
            }
            if (!isLoginPage && !CefWebBrowser.GetHtmlText().Contains(loginClassStr))
            {
                if (recursionCount == 0)
                {
                    loginState.IsLoginOutTime = true;
                    return loginState;
                }
                CefWebBrowser.Reload();
                recursionCount -= 1;
                //递归一次
                return IsLogin(account, password, recursionCount);
            }
            if (account == "" || password == "")
            {
                loginState.IsLoginSuccess = false;
                return loginState;
            }
            IFrame frame = null;
            List<long> iflist = CefWebBrowser.GetFrameIdentifiersList();
            foreach (var item in iflist)
            {
                IFrame f = CefWebBrowser._ChromiumWebBrowser.GetBrowser().GetFrame(item);
                if (f.Url.Contains("dl.reg.163.com"))
                {
                    frame = f;
                }
            }
            if (frame == null)
            {
                loginState.IsLoginSuccess = false;
                return loginState;
            }
            //获取账号绝对位置
            string jsString = $"document.getElementsByClassName('j-inputtext dlemail j-nameforslide')[0].value='';document.getElementsByClassName('j-inputtext dlemail j-nameforslide')[0].value='{account}'";
            string jsStr = "j-inputtext dlemail j-nameforslide";
            JsExecuteResult jsExecuteResult = _jsExecuteHelp.GetDomAbsLocationByClass(jsStr, null, frame);
            try
            {
                jsStr = "loginDiv";
                jsExecuteResult = _jsExecuteHelp.GetDomAbsLocationById(jsStr);
                CefWebBrowser.ExecuteJs(jsString, frame);
                Thread.Sleep(1000);
                //获取密码绝对位置
                jsStr = "j-inputtext dlpwd";
                _jsExecuteHelp.InJectGetLocationJs();
                jsExecuteResult = _jsExecuteHelp.GetDomAbsLocationByClass(jsStr, null, frame);
                jsString = $"document.getElementsByClassName('j-inputtext dlpwd')[0].value='';document.getElementsByClassName('j-inputtext dlpwd')[0].value='{password}'; ";
                CefWebBrowser.ExecuteJs(jsString, frame);
                Thread.Sleep(1000);
                //自动登录checkbox框
                string jsGetAutomaticLogonClassStr = "document.getElementsByClassName('b-unlogn j-unlogn')[0].getElementsByTagName('span')[0].className";
                jsExecuteResult = CefWebBrowser.ExecuteJs(jsGetAutomaticLogonClassStr, frame);
                if (jsExecuteResult.Success)
                {
                    if (jsExecuteResult.Result == "u-checkbox tabfocus")
                    {
                        string jsAutomaticLogonStr = "document.getElementById('un-login').click();";
                        CefWebBrowser.ExecuteJs(jsAutomaticLogonStr, frame);
                        Thread.Sleep(1000);
                    }
                }
                Thread.Sleep(1000);
                //点击登录
                jsString = "document.getElementById('dologin').click();";
                CefWebBrowser.ExecuteJs(jsString, frame);

            }
            catch (Exception ex)
            {

            }
            LoginOutTime = 10 * 1000;
            stopwatch.Restart();
            while (true)
            {
                //登录成功
                if (CefWebBrowser.GetHtmlText().Contains(loginsuccessClassStr))
                {
                    loginState.IsLoginSuccess = true;
                    return loginState;
                }
                jsStr = "document.getElementsByClassName('yidun_tips__text yidun-fallback__tip')[0].innerText";//document.getElementsByClassName('yidun_tips')[0]
                jsExecuteResult = CefWebBrowser.ExecuteJs(jsStr, frame);
                if (jsExecuteResult.Result != "")
                {
                    loginState.IsVCode = true;
                }
                //账号或者密码错误
                jsStr = "document.getElementById('nerror').innerText;";
                jsExecuteResult = CefWebBrowser.ExecuteJs(jsStr, frame);
                if (jsExecuteResult.Result != "")
                {
                    stopwatch.Stop();
                    Thread.Sleep(100);
                    if (CefWebBrowser.GetHtmlText().Contains("yidun yidun--light yidun--float yidun--point yidun--button"))
                    {
                        loginState.IsVCode = true;
                        return loginState;
                    }
                    loginState.IsLoginSuccess = false;
                    return loginState;
                }
                //登录超时
                if (stopwatch.ElapsedMilliseconds >= LoginOutTime)
                {
                    stopwatch.Stop();
                    loginState.IsLoginSuccess = false;
                    return loginState;
                }
                Thread.Sleep(100);
            }
        }
    

        public override bool IsNeedVerificationCode()
        {
            return false;
        }
        public override JsExecuteResult ClickCCList(List<string> emailStrList, out List<string> errorEmailList)
        {
            throw new NotImplementedException();
        }

        public override JsExecuteResult ClickBCCList(List<string> emailStrList, out List<string> errorEmailList)
        {
            throw new NotImplementedException();
        }
    }
}

