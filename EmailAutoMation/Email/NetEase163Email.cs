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

namespace TradeWolfRobot.EmailAutomation.Email {
    /// <summary>
    /// 网易163邮箱
    /// </summary>
    public class NetEase163Email : EmailBase {
        string msgbox = "js-component-msgbox nui-msgbox";
        string close = "js-component-button nui-mainBtn nui-btn";
        JsExecuteHelp _jsExecuteHelp;
        public NetEase163Email(CefWebBrowserControl CefWebBrowser) : base(CefWebBrowser) {
            EmailName = "163Email";
            _jsExecuteHelp = new JsExecuteHelp(CefWebBrowser);
        }
        public NetEase163Email() : base() {
            if (CefWebBrowser == null) {
                CefWebBrowser = new CefWebBrowserControl("https://mail.163.com/");
            } else {
                CefWebBrowser.LoadNewUrl("https://mail.163.com/");
            }
            EmailName = "163Email";
            _jsExecuteHelp = new JsExecuteHelp(CefWebBrowser);
        }
        public NetEase163Email(string cachePath) : base(cachePath)
        {
            CefWebBrowser = new CefWebBrowserControl("https://mail.163.com/", cachePath);
            EmailName = "163Email";
            _jsExecuteHelp = new JsExecuteHelp(CefWebBrowser);
        }
        /// <summary>
        /// 点击写信按键
        /// </summary>
        public override JsExecuteResult ClickWriteEmail() {
            ClickCloseLabel();
            Thread.Sleep(2000);
            string writeButton = "js-component-component ra0 mD0";
            DomNameOrId = writeButton;
            JsExecuteResult jsExecuteResult = _jsExecuteHelp.GetDomAbsLocationByClass(writeButton);
            if (jsExecuteResult.Success)
            {
                DomAbsLocation domAbsLocation = JsonConvert.DeserializeObject<DomAbsLocation>(jsExecuteResult.Result);
                CefWebBrowser.CefMouseClick(domAbsLocation.top + 10, domAbsLocation.left + 15);
            }
            return jsExecuteResult;
        }
        /// <summary>
        /// 写入收件人邮箱地址 
        /// </summary>
        /// <param name="emailAddress">邮箱地址</param>
        public override JsExecuteResult WriteEmailAddress(string emailAddress) {

            string jsString = $" document.getElementsByClassName('nui-editableAddr-ipt')[0].value='{emailAddress}'";
            JsExecuteResult jsExecuteResult1 = CefWebBrowser.ExecuteJs(jsString);
            string jsStrClass = "js-component-emailcontainer nui-multiLineIpt C-multiLineIpt nui-ipt";
            JsExecuteResult jsExecuteResult = _jsExecuteHelp.GetDomAbsLocationByClass(jsStrClass);
            try
            {
                if (jsExecuteResult.Success)
                {
                    DomAbsLocation domAbsLocation = JsonConvert.DeserializeObject<DomAbsLocation>(jsExecuteResult.Result);
                    CefWebBrowser.CefMouseClick(domAbsLocation.top + 10, domAbsLocation.left + 15);
                    Thread.Sleep(500);
                }
            }
            catch (Exception ex)
            {
                jsExecuteResult.Success = false;
                return jsExecuteResult;
            }
            return jsExecuteResult1;
        }
        /// <summary>
        /// 写入邮件的主题 
        /// </summary>
        /// <param name="emailTheme">邮件主题</param>
        public override JsExecuteResult WriteEmailTheme(string emailTheme) {
            Thread.Sleep(1 * 1000);
            String themeInputClassStr = "nui-ipt-input";
            JsExecuteResult jsExecuteResult = _jsExecuteHelp.GetDomAbsLocationByClass(themeInputClassStr, 2);
            try
            {
                if (jsExecuteResult.Success)
                {
                    DomAbsLocation domAbsLocation = JsonConvert.DeserializeObject<DomAbsLocation>(jsExecuteResult.Result);
                    CefWebBrowser.CefMouseClick(domAbsLocation.top + 10, domAbsLocation.left + 15);
                    Thread.Sleep(500);
                    CefWebBrowser.CefKeyBoardInput(emailTheme);
                    Thread.Sleep(500);
                }
            }
            catch (Exception e)
            {
                jsExecuteResult.Success = false;
                return jsExecuteResult;
            }
            return jsExecuteResult;
        }
        /// <summary>
        /// 写入邮件正文
        /// </summary>
        /// <param name="body">邮件正文</param>
        public override JsExecuteResult WriteEmailBody(string emailBody) {
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
        public override JsExecuteResult ClickSendEmail() {
            string sendBtStr = "js-component-icon nui-ico nui-ico-sent  nui-ico-sent-white";
            JsExecuteResult jsExecuteResult = _jsExecuteHelp.GetDomAbsLocationByClass(sendBtStr, null);
            if (jsExecuteResult.Success)
            {
                DomAbsLocation domAbsLocation = JsonConvert.DeserializeObject<DomAbsLocation>(jsExecuteResult.Result);
                CefWebBrowser.CefMouseClick(domAbsLocation.top + 3, domAbsLocation.left + 5);
            }
            Thread.Sleep(3 * 1000);
            string jsStr = "document.getElementsByClassName('js-component-button nui-mainBtn nui-btn ').length";
           JsExecuteResult jsExecuteResult1 = CefWebBrowser.ExecuteJs(jsStr);
            if (jsExecuteResult1.Success)
            {
                while (jsExecuteResult1.Result.Contains("3"))
                {
                    if (CefWebBrowser.GetHtmlText().Contains(msgbox))
                    {
                        jsExecuteResult = _jsExecuteHelp.GetDomAbsLocationByClass(close, 2);
                        if (jsExecuteResult.Success)
                        {
                            DomAbsLocation domAbsLocation = JsonConvert.DeserializeObject<DomAbsLocation>(jsExecuteResult.Result);
                            CefWebBrowser.CefMouseClick(domAbsLocation.top + 10, domAbsLocation.left + 15);
                        }
                        jsExecuteResult.Success = false;
                        Thread.Sleep(2*1000);
                        jsExecuteResult1 = CefWebBrowser.ExecuteJs(jsStr);
                    }
                }
            }
            return jsExecuteResult;
        }
        /// <summary>
        /// 点击关闭标签
        /// </summary>
        /// <returns></returns>
        private JsExecuteResult ClickCloseLabel() {
            string closeLabel = "nui-tabs-item-close js-component-link";
            string len = $"document.getElementsByClassName('{closeLabel}').length";
            JsExecuteResult jsExecuteResult = CefWebBrowser.ExecuteJs(len);
            try
            {
                if (jsExecuteResult.Success)
                {
                    int number = int.Parse(jsExecuteResult.Result);
                    for (int i = 0; i < number; i++)
                    {
                        jsExecuteResult = _jsExecuteHelp.GetDomAbsLocationByClass(closeLabel, i);
                        DomAbsLocation domAbsLocation = JsonConvert.DeserializeObject<DomAbsLocation>(jsExecuteResult.Result);
                        CefWebBrowser.CefMouseClick(domAbsLocation.top + 4, domAbsLocation.left + 10);
                    }
                }
                Thread.Sleep(2 * 1000);
                if (CefWebBrowser.GetHtmlText().Contains("js-component-icon nui-ico nui-ico-close  nui-ico-close-dark"))
                {
                    string jsString = $"document.getElementsByClassName('js-component-msgbox nui-msgbox')[0].getElementsByClassName('js-component-button nui-btn')[0].getBoundingClientRect().";
                    // jsExecuteResult = CefWebBrowser.ExecuteJs(jsString);
                    JsExecuteResult jsExecuteResult1 = CefWebBrowser.ExecuteJs(jsString + "top");
                    JsExecuteResult jsExecuteResult2 = CefWebBrowser.ExecuteJs(jsString + "left");
                    if (jsExecuteResult1.Success && jsExecuteResult2.Success)
                    {
                        float top = float.Parse(jsExecuteResult1.Result);
                        float left = float.Parse(jsExecuteResult2.Result);
                        CefWebBrowser.CefMouseClick(top + 3, left + 5);
                    }
                    Thread.Sleep(1 * 1000);

                }
            }
            catch (Exception e)
            {
                jsExecuteResult.Success = false;
                return jsExecuteResult;
            }
            return jsExecuteResult;
        }

        public override object GetCookie() {
            return CefCookieHelper.GetCookieList("163.com");
            //return CefWebBrowser.GetCookieList("163.com");
        }
        public override void SetCookie(object cookie) {
            List<Cookie> cookieList = new List<Cookie>();
            cookieList = cookie as List<Cookie>;
            if (cookie != null) {
                if (cookieList == null) {
                    cookieList = JsonConvert.DeserializeObject<List<Cookie>>(cookie.ToString());
                }
                if (cookieList != null) {
                    CefCookieHelper.SetCookie("https://mail.163.com/", cookieList);
                }
            }
        }
        public override LoginState IsLogin(string account, string password, int recursionCount = 1) {

            LoginState loginState = new LoginState();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            string loginClassStr = "main-inner";
            string loginsuccessClassStr = "gWel frame-main";
            //是否为登录页面
            bool isLoginPage = false;

            while (true) {
                Thread.Sleep(1000);
                if (_StopTask) {
                    return loginState;
                }
                if (!isLoginPage) {
                    stopwatch.Restart();
                    //出现登录页面，10s等待是否能自动登录
                    if (CefWebBrowser.GetHtmlText().Contains(loginClassStr)) {
                        //设置等待时间10s
                        LoginOutTime = LoginTestingTime;
                    }
                    isLoginPage = true;
                }
                if (CefWebBrowser.GetHtmlText().Contains(loginsuccessClassStr)) {
                    //邮箱自动登录成功
                    loginState.IsLoginSuccess = true;
                    return loginState;
                }
                if (stopwatch.ElapsedMilliseconds > LoginOutTime) {
                    break;
                }
            }
            if (!isLoginPage && !CefWebBrowser.GetHtmlText().Contains(loginClassStr)) {
                if (recursionCount == 0) {
                    loginState.IsLoginOutTime = true;
                    return loginState;
                }
                CefWebBrowser.Reload();
                recursionCount -= 1;
                //递归一次
                return IsLogin(account, password, recursionCount);
            }
            if (account == "" || password == "") {
                loginState.IsLoginSuccess = false;
                return loginState;
            }
            IFrame frame = null;
            List<long> iflist = CefWebBrowser.GetFrameIdentifiersList();
            foreach (var item in iflist) {
                IFrame f = CefWebBrowser._ChromiumWebBrowser.GetBrowser().GetFrame(item);
                if (f.Url.Contains("dl.reg.163.com")) {
                    frame = f;
                }
            }
            if (frame == null) {
                loginState.IsLoginSuccess = false;
                return loginState;
            }
            //获取账号绝对位置
            string jsString = $"document.getElementsByClassName('j-inputtext dlemail j-nameforslide')[0].value='';document.getElementsByClassName('j-inputtext dlemail j-nameforslide')[0].value='{account}'";
            string jsStr = "j-inputtext dlemail j-nameforslide";
            JsExecuteResult jsExecuteResult = _jsExecuteHelp.GetDomAbsLocationByClass(jsStr, null, frame);
            try {
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
                if (jsExecuteResult.Success) {
                    if (jsExecuteResult.Result == "u-checkbox tabfocus") {
                        string jsAutomaticLogonStr = "document.getElementById('un-login').click();";
                        CefWebBrowser.ExecuteJs(jsAutomaticLogonStr, frame);
                        Thread.Sleep(1000);
                    }
                }
                Thread.Sleep(1000);
                //点击登录
                jsString = "document.getElementById('dologin').click();";
                CefWebBrowser.ExecuteJs(jsString, frame);

            } catch (Exception ex) {

            }
            LoginOutTime = 10 * 1000;
            stopwatch.Restart();
            while (true) {
                //登录成功
                if (CefWebBrowser.GetHtmlText().Contains(loginsuccessClassStr)) {
                    loginState.IsLoginSuccess = true;
                    return loginState;
                }
                jsStr = "document.getElementsByClassName('yidun_tips__text yidun-fallback__tip')[0].innerText";//document.getElementsByClassName('yidun_tips')[0]
                jsExecuteResult = CefWebBrowser.ExecuteJs(jsStr, frame);
                if (jsExecuteResult.Result != "") {
                    loginState.IsVCode = true;
                }
                //账号或者密码错误
                jsStr = "document.getElementById('nerror').innerText;";
                jsExecuteResult = CefWebBrowser.ExecuteJs(jsStr, frame);
                if (jsExecuteResult.Result != "") {
                    stopwatch.Stop();
                    Thread.Sleep(100);
                    if (CefWebBrowser.GetHtmlText().Contains("yidun yidun--light yidun--float yidun--point yidun--button")) {
                        loginState.IsVCode = true;
                        return loginState;
                    }
                    loginState.IsLoginSuccess = false;
                    return loginState;
                }
                //登录超时
                if (stopwatch.ElapsedMilliseconds >= LoginOutTime) {
                    stopwatch.Stop();
                    loginState.IsLoginSuccess = false;
                    return loginState;
                }
                Thread.Sleep(100);
            }
        }


        public override bool IsNeedVerificationCode() {
            return false;
        }

        public override JsExecuteResult ClickCCList(List<string> emailStrList, out List<string> errorEmailList) {
            errorEmailList = null;
            //document.getElementsByClassName(" js-component-link nui-txt-link")[3].click() <--单击抄送按钮-->
            //document.getElementsByClassName("kZ0 jL0cc")[0].getElementsByClassName("nui-editableAddr-ipt")[0].value="2075383131@qq.com;3037105677@qq.com;cacode@163.com" <--添加多个人-->
            //空的
            if (emailStrList == null)
            {
                JsExecuteResult jsExecuteResult0 = new JsExecuteResult();
                jsExecuteResult0.Success = true;
                return jsExecuteResult0;
            }
            string jsStr = "document.getElementsByClassName('nui-toolbar-ext')[0].getElementsByClassName(' js-component-link nui-txt-link')[1].";//4
            string input = "document.getElementsByClassName('kZ0 jL0cc')[0].getElementsByClassName('nui-editableAddr-ipt')[0]";
            string val = "";

            foreach (string item in emailStrList)
            {
                val += item + ";";
            }

            val = val.Substring(0, val.Length - 1);

            if (!CefWebBrowser.ExecuteJs(jsStr + "innerText").Result.Contains("删除"))
            {
                JsExecuteResult jsExecuteResult = CefWebBrowser.ExecuteJs(jsStr + "getBoundingClientRect().top");
                try
                {
                    float top = float.Parse(jsExecuteResult.Result);
                    jsExecuteResult = CefWebBrowser.ExecuteJs(jsStr + "getBoundingClientRect().left");
                    float left = float.Parse(jsExecuteResult.Result);
                    CefWebBrowser.CefMouseClick(top + 3, left + 3);
                    Thread.Sleep(1 * 1000);
                }
                catch (Exception e)
                {
                    jsExecuteResult.Success = false;
                    return jsExecuteResult;
                }
            }

            JsExecuteResult jsExecuteResult1 = CefWebBrowser.ExecuteJs(input + ".focus()");
            Thread.Sleep(1 * 1000);
            CefWebBrowser.CefKeyBoardInput(val);

            DomAbsLocation location = GetElementLocation.get(CefWebBrowser, input);

            if (location != null)
            {
                CefWebBrowser.CefMouseClick(location.top, location.left, true);
            }

            return jsExecuteResult1;
        }

        public override JsExecuteResult ClickBCCList(List<string> emailStrList, out List<string> errorEmailList) {
            errorEmailList = null;
            //空的
            if (emailStrList==null)
            {
                JsExecuteResult jsExecuteResult0 = new JsExecuteResult();
                jsExecuteResult0.Success = true;
                return jsExecuteResult0;
            }
            string jsStr = "document.getElementsByClassName('nui-toolbar-ext')[0].getElementsByClassName(' js-component-link nui-txt-link')[2].";
            string input = "document.getElementsByClassName('kZ0 jL0bcc')[0].getElementsByClassName('nui-editableAddr-ipt')[0]";
            string val = "";

            foreach (string item in emailStrList)
            {
                val += item + ";";
            }

            val = val.Substring(0, val.Length - 1);

            if (!CefWebBrowser.ExecuteJs(jsStr + "innerText").Result.Contains("删除"))
            {
                JsExecuteResult jsExecuteResult = CefWebBrowser.ExecuteJs(jsStr + "getBoundingClientRect().top");
                try
                {
                    float top = float.Parse(jsExecuteResult.Result);
                    jsExecuteResult = CefWebBrowser.ExecuteJs(jsStr + "getBoundingClientRect().left");
                    float left = float.Parse(jsExecuteResult.Result);
                    CefWebBrowser.CefMouseClick(top + 5, left + 5);
                    Thread.Sleep(1 * 1000);
                }
                catch (Exception e)
                {
                    jsExecuteResult.Success = false;
                    return jsExecuteResult;
                }
            }

            JsExecuteResult jsExecuteResult1 = CefWebBrowser.ExecuteJs(input + ".focus()");
            Thread.Sleep(1 * 1000);
            CefWebBrowser.CefKeyBoardInput(val);

            DomAbsLocation location = GetElementLocation.get(CefWebBrowser, input);

            if (location != null)
            {
                CefWebBrowser.CefMouseClick(location.top, location.left, true);
            }

            return jsExecuteResult1;
        }
    }
}

