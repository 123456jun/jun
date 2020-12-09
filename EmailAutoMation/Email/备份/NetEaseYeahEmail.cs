using CefSharp;
using CefSharpWebBrowser.CefHelper;
using CefSharpWebBrowser.CefModel;
using CefWebBrowser;
using CefWebBrowser.CefModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using TradeWolfRobot.EmailAutomation.Model;

namespace TradeWolfRobot.EmailAutomation.Email
{
    public class NetEaseYeahEmail : EmailBase
    {
        JsExecuteHelp _jsExecuteHelp;
        public NetEaseYeahEmail(CefWebBrowserControl CefWebBrowser) : base(CefWebBrowser)
        {
            EmailName = "YeahEmail";
            _jsExecuteHelp = new JsExecuteHelp(CefWebBrowser);
        }
        public NetEaseYeahEmail() : base()
        {
            if (CefWebBrowser == null)
            {
                CefWebBrowser = new CefWebBrowserControl("https://www.yeah.net/");
            }
            else
            {
                CefWebBrowser.LoadNewUrl("https://www.yeah.net/");
            }
            EmailName = "126Email";
            _jsExecuteHelp = new JsExecuteHelp(CefWebBrowser);
        }

        public override object GetCookie()
        {
            return "";
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
            JsExecuteResult jsExecuteResult = null;
            IFrame frame = null;
            List<long> iflist = CefWebBrowser.GetFrameIdentifiersList();
            foreach (var item in iflist)
            {
                IFrame f = CefWebBrowser._ChromiumWebBrowser.GetBrowser().GetFrame(item);
                if (f.Url.Contains("passport-v6.yeah.net"))
                {
                    frame = f;
                }
            };
            if (frame == null)
            {
                loginState.IsLoginSuccess = false;
                return loginState;
            }
            //获取账号绝对位置
            string jsString = $"document.getElementsByClassName('j-inputtext dlemail j-nameforslide')[0].value='';document.getElementsByClassName('j-inputtext dlemail j-nameforslide')[0].value='{account}'";
            string jsStr = "j-inputtext dlemail j-nameforslide";
            jsExecuteResult = _jsExecuteHelp.GetDomAbsLocationByClass(jsStr, null, frame);
            try
            {
                jsStr = "loginDiv";
                jsExecuteResult = _jsExecuteHelp.GetDomAbsLocationById(jsStr);
                CefWebBrowser.ExecuteJs(jsString, frame);
                Thread.Sleep(1000);
                //获取密码绝对位置
                jsStr = "j-inputtext dlpwd";
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
            //登录超时
            stopwatch.Restart();
            LoginOutTime = 10 * 1000;
            while (true)
            {
                if (_StopTask)
                {
                    return loginState;

                }
                if (CefWebBrowser.GetHtmlText().Contains(loginsuccessClassStr))
                {
                    loginState.IsLoginSuccess = true;
                    return loginState;
                }
                //验证码
                jsStr = "document.getElementsByClassName('yidun_tips__text yidun-fallback__tip')[0].innerText";//document.getElementsByClassName('yidun_tips')[0]
                jsExecuteResult = CefWebBrowser.ExecuteJs(jsStr, frame);
                if (jsExecuteResult.Result != "")
                {
                    loginState.IsVCode = true;
                    return loginState;
                }
                //账号密码错误
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
                //超时
                if (stopwatch.ElapsedMilliseconds >= LoginOutTime)
                {
                    stopwatch.Stop();
                    loginState.IsLoginSuccess = false;
                    return loginState;
                }
                Thread.Sleep(100);
            }
            #region
            //在登录界面
            //if (CefWebBrowser.GetHtmlText().Contains(loginClassStr))
            //{

            //    IFrame frame = null;
            //    List<long> iflist = CefWebBrowser.GetFrameIdentifiersList();
            //    foreach (var item in iflist)
            //    {
            //        IFrame f = CefWebBrowser._ChromiumWebBrowser.GetBrowser().GetFrame(item);
            //        if (f.Url.Contains("passport-v6.yeah.net"))
            //        {
            //            frame = f;
            //        }
            //    }
            //    //获取账号绝对位置
            //    string jsString = $"document.getElementsByClassName('j-inputtext dlemail j-nameforslide')[0].value='';document.getElementsByClassName('j-inputtext dlemail j-nameforslide')[0].value='{account}'";
            //    string jsStr = "j-inputtext dlemail j-nameforslide";
            //    _jsExecuteHelp.InJectGetLocationJs();
            //    JsExecuteResult jsExecuteResult = _jsExecuteHelp.GetDomAbsLocationByClass(jsStr, null, frame);
            //    try
            //    {
            //        DomAbsLocation domAbsLocation1 = JsonConvert.DeserializeObject<DomAbsLocation>(jsExecuteResult.Result);
            //        jsStr = "loginDiv";
            //        jsExecuteResult = _jsExecuteHelp.GetDomAbsLocationById(jsStr);
            //        DomAbsLocation domAbsLocation = JsonConvert.DeserializeObject<DomAbsLocation>(jsExecuteResult.Result);
            //        CefWebBrowser.CefMouseClick(domAbsLocation.top + domAbsLocation1.top + 5, domAbsLocation.left + domAbsLocation1.left + 15);
            //        CefWebBrowser.ExecuteJs(jsString, frame);
            //        Thread.Sleep(1000);
            //        //获取密码绝对位置
            //        jsStr = "j-inputtext dlpwd";
            //        _jsExecuteHelp.InJectGetLocationJs();
            //        jsExecuteResult = _jsExecuteHelp.GetDomAbsLocationByClass(jsStr, null, frame);
            //        DomAbsLocation domAbsLocation2 = JsonConvert.DeserializeObject<DomAbsLocation>(jsExecuteResult.Result);
            //        CefWebBrowser.CefMouseClick(domAbsLocation.top + domAbsLocation2.top + 5, domAbsLocation.left + domAbsLocation2.left + 15);
            //        jsString = $"document.getElementsByClassName('j-inputtext dlpwd')[0].value='';document.getElementsByClassName('j-inputtext dlpwd')[0].value='{password}'; ";
            //        CefWebBrowser.ExecuteJs(jsString, frame);
            //        Thread.Sleep(1000);
            //        //点击登录
            //        jsString = "document.getElementById('dologin').click();";
            //        CefWebBrowser.ExecuteJs(jsString, frame);
            //        Thread.Sleep(8 * 1000);
            //        jsStr = "document.getElementsByClassName('yidun_tips__text yidun-fallback__tip')[0].innerText";//document.getElementsByClassName('yidun_tips')[0]
            //        jsExecuteResult = CefWebBrowser.ExecuteJs(jsStr, frame);
            //        if (jsExecuteResult.Result == "点此进行验证")
            //        {
            //            loginState.IsVCode = true;
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine(ex.Message);
            //    }
            //    //验证成功点击登录
            //    while (loginState.IsVCode)
            //    {
            //        jsStr = "document.getElementsByClassName('yidun_tips__text yidun-fallback__tip')[0].innerText";//document.getElementsByClassName('yidun_tips')[0]
            //        jsExecuteResult = CefWebBrowser.ExecuteJs(jsStr, frame);
            //        if (jsExecuteResult.Result == "验证成功")
            //        {
            //            jsString = "document.getElementById('dologin').click();";
            //            CefWebBrowser.ExecuteJs(jsString, frame);
            //            break;
            //        }
            //    }
            //    //登录超时
            //    Stopwatch sw1 = new Stopwatch();
            //    sw1.Start();
            //    while (true)
            //    {
            //        if (CefWebBrowser.ExecuteJs(jsaddress).Result.Contains("mail.yeah.net/js6/main.jsp"))
            //        {

            //            loginState.IsLoginSuccess = true;
            //            return loginState;
            //        }
            //        jsStr = "document.getElementById('nerror').innerText;";
            //        jsExecuteResult = CefWebBrowser.ExecuteJs(jsStr, frame);
            //        if (jsExecuteResult.Result == "帐号或密码错误")
            //        {
            //            sw1.Stop();
            //            loginState.IsLoginSuccess = false;
            //            return loginState;
            //        }
            //        if (sw1.ElapsedMilliseconds >= LoginOutTime)
            //        {
            //            sw1.Stop();
            //            loginState.IsLoginSuccess = false;
            //            return loginState;
            //        }
            //        Thread.Sleep(500);
            //    }

            //}
            //else
            //{
            //    loginState.IsLoginSuccess = false;
            //    return loginState;
            //}
            #endregion
        }
        public override bool IsNeedVerificationCode()
        {
            return false;
        }
        /// <summary>
        /// 点击写信进入写信界面
        /// </summary>
        /// <returns></returns>
        public override JsExecuteResult ClickWriteEmail()
        {
            ClickCloseLabel();
            Thread.Sleep(2000);
            string number = "document.getElementsByClassName('js-component-icon nui-ico nui-ico-close').length";
            string jswriteBt = "js-component-component ra0 mD0";
            DomNameOrId = jswriteBt;
            JsExecuteResult jsExecuteResult = CefWebBrowser.ExecuteJs(number);
            try
            {
                //关掉之前打开的写信界面
                int len = int.Parse(jsExecuteResult.Result);
                if (len >= 1)
                {
                    for (int i = 0; i < len; i++)
                    {
                        string jsString = "document.getElementsByClassName('js-component-icon nui-ico nui-ico-close')[" + i + "].click()";
                        CefWebBrowser.ExecuteJs(jsString);
                    }
                }
            }
            catch
            {
                jsExecuteResult.Success = false;
                return jsExecuteResult;
            }
            jsExecuteResult = _jsExecuteHelp.ExecuteClickByClass(DomNameOrId);
            return jsExecuteResult;
        }
        public override void SetCookie(object cookie)
        {
             
        }
        /// <summary>
        /// 写入收件人地址
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <returns></returns>
        public override JsExecuteResult WriteEmailAddress(string emailAddress)
        {
            //点击收件人文本框
            string jsString = "js-component-emailcontainer nui-multiLineIpt C-multiLineIpt nui-ipt";
            DomNameOrId = jsString;
            _jsExecuteHelp.InJectGetLocationJs();
            JsExecuteResult jsExecuteResult = _jsExecuteHelp.GetDomAbsLocationByClass(DomNameOrId);
            Thread.Sleep(500);
            try
            {
                DomAbsLocation domAbsLocation = JsonConvert.DeserializeObject<DomAbsLocation>(jsExecuteResult.Result);
                CefWebBrowser.CefMouseClick(domAbsLocation.top + 5, domAbsLocation.left + 5);
                Thread.Sleep(500);
                //对收件人文本框写入信息
                jsString = $"document.getElementsByClassName('nui-editableAddr-ipt')[0].value='{emailAddress}'";
                jsExecuteResult = CefWebBrowser.ExecuteJs(jsString);
                CefWebBrowser.CefMouseClick(domAbsLocation.top + 5, domAbsLocation.left + 5);
                Thread.Sleep(500);
            }
            catch
            {
                jsExecuteResult.Success = false;
                return jsExecuteResult;
            }
            return jsExecuteResult;
        }
        /// <summary>
        /// 写入主题
        /// </summary>
        /// <param name="emailTheme"></param>
        /// <returns></returns>
        public override JsExecuteResult WriteEmailTheme(string emailTheme)
        {
            string jsString = @"document.getElementsByClassName('nui-ipt-input')[2].value='';
                                document.getElementsByClassName('nui-ipt-input')[2].value='" + emailTheme + "'";
            return CefWebBrowser.ExecuteJs(jsString);

        }

        /// <summary>
        /// 写入正文
        /// </summary>
        /// <param name="emailBody"></param>
        /// <returns></returns>
        public override JsExecuteResult WriteEmailBody(string emailBody)
        {
            //进入源码
            string jsString = @"document.getElementsByClassName('ico ico-editor ico-editor-source')[0].click();";
            JsExecuteResult jsExecuteResult = CefWebBrowser.ExecuteJs(jsString);
            //写入源码
            jsString = $"document.getElementsByClassName('APP-editor-textarea')[0].value=`{emailBody}`";
            jsExecuteResult = CefWebBrowser.ExecuteJs(jsString);
            //退出
            jsString = "document.getElementsByClassName('APP-editor-source-code-exit')[0].click();";
            jsExecuteResult = CefWebBrowser.ExecuteJs(jsString);
            return jsExecuteResult;
        }
        /// <summary>
        /// 点击发送
        /// </summary>
        /// <returns></returns>
        public override JsExecuteResult ClickSendEmail()
        {
            string jsString = @"document.getElementsByClassName('js-component-icon nui-ico nui-ico-sent  nui-ico-sent-white')[0].click()";
            JsExecuteResult jsExecuteResult = CefWebBrowser.ExecuteJs(jsString);
            Thread.Sleep(1000);
            jsString = "js-component-msgbox nui-msgbox";
            if (CefWebBrowser.GetHtmlText().Contains(jsString))
            {
                jsString = $"document.getElementsByClassName('{jsString}')[0].getElementsByClassName('js-component-button nui-mainBtn nui-btn ')[0].click();";
                CefWebBrowser.ExecuteJs(jsString);
                jsExecuteResult.Success = false;
                Thread.Sleep(500);
            }
            return jsExecuteResult;
        }
        /// <summary>
        /// 关闭标签
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
            Thread.Sleep(1000);
            if (CefWebBrowser.GetHtmlText().Contains("js-component-msgbox nui-msgbox"))
            {
                jsString = $"document.getElementsByClassName('js-component-msgbox nui-msgbox')[0].getElementsByClassName('js-component-button nui-btn')[0].click();";
                jsExecuteResult = CefWebBrowser.ExecuteJs(jsString);
                jsExecuteResult.Success = false;
                Thread.Sleep(1 * 1000);
            }
            return jsExecuteResult;
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
