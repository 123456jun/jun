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
using System.Threading.Tasks;
using TradeWolfRobot.EmailAutomation.Model;

namespace TradeWolfRobot.EmailAutomation.Email {
    public class Netease163EnterpriseEmail : EmailBase {

        JsExecuteHelp _jsExecuteHelp;
        public Netease163EnterpriseEmail(CefWebBrowserControl CefWebBrowser) : base(CefWebBrowser) {
            EmailName = "NeteaseEnterpriseEmail";
            _jsExecuteHelp = new JsExecuteHelp(CefWebBrowser);
        }

        public Netease163EnterpriseEmail() : base() {
            if (CefWebBrowser == null) {
                CefWebBrowser = new CefWebBrowserControl("https://qiye.163.com/login/");
            } else {
                CefWebBrowser.LoadNewUrl("https://qiye.163.com/login/");
            }
            EmailName = "NeteaseEnterpriseEmail";
            _jsExecuteHelp = new JsExecuteHelp(CefWebBrowser);
        }
        public Netease163EnterpriseEmail(string cachePath) : base(cachePath)
        {
            CefWebBrowser = new CefWebBrowserControl("https://qiye.163.com/login/", cachePath);
            EmailName = "NeteaseEnterpriseEmail";
            _jsExecuteHelp = new JsExecuteHelp(CefWebBrowser);
        }


        public override object GetCookie() {
            return "";
        }

        public override LoginState IsLogin(string account, string password, int recursionCount = 1) {
            JsExecuteResult jsExecuteResult = null;
            LoginState loginState = new LoginState();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            string loginIdStr = "loginBlock";
            string loginsuccessClassStr = "sH0 nui-scroll";
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
                    if (CefWebBrowser.GetHtmlText().Contains(loginIdStr)) {
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
            if (!isLoginPage && !CefWebBrowser.GetHtmlText().Contains(loginsuccessClassStr)) {
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

            try {
                Thread.Sleep(2 * 1000);
                if (CefWebBrowser.GetHtmlText().Contains("m-login m-login-with-ad js-loginpanel show-qrocde-panel")) {
                    string accountLoginStr = "document.getElementById('switchNormalCtrl').click()";
                    CefWebBrowser.ExecuteJs(accountLoginStr);
                }
                Thread.Sleep(2000);
                string jsUserInputString = "ipt js-value js-username";
                string jsUserString = $"document.getElementsByClassName('{jsUserInputString}')[0].value='';document.getElementsByClassName('{jsUserInputString}')[0].value='{account}'";
                CefWebBrowser.ExecuteJs(jsUserString);
                Thread.Sleep(1000);
                string jsPasswordInputString = "ipt js-value js-pwd";
                string jsPasswordString = $"document.getElementsByClassName('{jsPasswordInputString}')[0].value='';document.getElementsByClassName('{jsPasswordInputString}')[0].value='{password}'";
                CefWebBrowser.ExecuteJs(jsPasswordString);
                Thread.Sleep(1000);
                //自动登录checkbox框
                string jsAutomaticLoginStr = "document.getElementsByClassName('logincheck js-logincheck')[0].getElementsByTagName('span')[0].click();";
                string jsGetAutomaticLoginClassStr = "document.getElementsByClassName('logincheck js-logincheck')[0].getElementsByTagName('span')[0].className";
                jsExecuteResult = CefWebBrowser.ExecuteJs(jsGetAutomaticLoginClassStr);
                if (jsExecuteResult.Success) {
                    if (!jsExecuteResult.Result.Contains("icon-checkbox-checked")) {

                        CefWebBrowser.ExecuteJs(jsAutomaticLoginStr);
                        Thread.Sleep(1000);
                    }
                }
                Thread.Sleep(1000);
                //点击登录
                string jsLoginBtString = "document.getElementsByClassName('w-button w-button-account js-loginbtn')[0].click();";
                CefWebBrowser.ExecuteJs(jsLoginBtString);
                Thread.Sleep(2 * 1000);

            } catch (Exception ex) {

            }
            stopwatch.Restart();
            LoginOutTime = 10 * 1000;
            while (true) {
                if (_StopTask) {
                    return loginState;

                }
                if (CefWebBrowser.GetHtmlText().Contains(loginsuccessClassStr)) {
                    loginState.IsLoginSuccess = true;
                    return loginState;
                }
                //提示用户
                string jsString = "document.getElementsByClassName('m-verifycode')[0].style['display']";
                jsExecuteResult = CefWebBrowser.ExecuteJs(jsString);
                if (jsExecuteResult.Result == "block") {
                    stopwatch.Stop();
                    loginState.IsVCode = true;
                    return loginState;
                }
                string jsloginerrorClassStr = "document.getElementsByClassName('loginerror')[0].innerText";
                jsExecuteResult = CefWebBrowser.ExecuteJs(jsloginerrorClassStr);
                if (jsExecuteResult.Success) {
                    if (jsExecuteResult.Result.Contains("邮箱帐号和密码不匹配") || jsExecuteResult.Result.Contains("Email account and password do not match")) {
                        stopwatch.Stop();
                        loginState.IsLoginSuccess = false;
                        return loginState;
                    }
                }
                //超时
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

        public override void SetCookie(object cookie) {

        }
        /// <summary>
        /// 点击写信
        /// </summary>
        /// <returns></returns>
        public override JsExecuteResult ClickWriteEmail() {
            ClickCloseLabel();
            //点击写信
            string number = "document.getElementsByClassName('js-component-icon nui-ico nui-ico-close').length";
            string jsString = "js-component-component ra0 mD0";
            JsExecuteResult jsExecuteResult = CefWebBrowser.ExecuteJs(number);
            try {
                //关掉之前打开的写信界面
                if (int.Parse(jsExecuteResult.Result)>=1) {
                    jsString = "document.getElementsByClassName('js-component-icon nui-ico nui-ico-close')[0].click()";
                    CefWebBrowser.ExecuteJs(jsString);
                }
                Thread.Sleep(1000);
                DomNameOrId = jsString;
                jsExecuteResult = _jsExecuteHelp.GetDomAbsLocationByClass(DomNameOrId);
                if (jsExecuteResult.Success)
                {
                    DomAbsLocation domAbsLocation1 = JsonConvert.DeserializeObject<DomAbsLocation>(jsExecuteResult.Result);
                    CefWebBrowser.CefMouseClick(domAbsLocation1.top + 3, domAbsLocation1.left + 4);
                    Thread.Sleep(1000);
                }
            } catch {
                jsExecuteResult.Success = false;
                return jsExecuteResult;
            }
            return jsExecuteResult;
        }
        /// <summary>
        /// 写入收件人
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <returns></returns>
        public override JsExecuteResult WriteEmailAddress(string emailAddress) {
            string jsString = "nui-editableAddr-ipt";
            _jsExecuteHelp.InJectGetLocationJs();
            JsExecuteResult jsExecuteResult = _jsExecuteHelp.GetDomAbsLocationByClass(jsString);
            try
            {
                DomAbsLocation domAbsLocation1 = JsonConvert.DeserializeObject<DomAbsLocation>(jsExecuteResult.Result);
                CefWebBrowser.CefMouseClick(domAbsLocation1.top + 4, domAbsLocation1.left + 4);
                Thread.Sleep(500);
                CefWebBrowser.CefKeyBoardInput(emailAddress);
                Thread.Sleep(1000);
            }
            catch(Exception e)
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
        public override JsExecuteResult WriteEmailTheme(string emailTheme) {
            string jsString = "nui-ipt-input";//2
            JsExecuteResult jsExecuteResult = _jsExecuteHelp.GetDomAbsLocationByClass(jsString,2);
            try
            {
                DomAbsLocation domAbsLocation1 = JsonConvert.DeserializeObject<DomAbsLocation>(jsExecuteResult.Result);
                CefWebBrowser.CefMouseClick(domAbsLocation1.top + 4, domAbsLocation1.left + 4);
                Thread.Sleep(500);
                CefWebBrowser.CefKeyBoardInput(emailTheme);
                Thread.Sleep(1000);
            }
            catch (Exception e)
            {
                jsExecuteResult.Success = false;
                return jsExecuteResult;
            }
            return jsExecuteResult;

        }
        /// <summary>
        /// 写入正文
        /// </summary>
        /// <param name="emailBody"></param>
        /// <returns></returns>
        public override JsExecuteResult WriteEmailBody(string emailBody) {
            string jsString = "document.getElementsByClassName('ico ico-editor ico-editor-source')[0].click()";
            JsExecuteResult jsExecuteResult = CefWebBrowser.ExecuteJs(jsString);
            jsString = $"document.getElementsByClassName('APP-editor-textarea')[0].value=`{emailBody}`";
            jsExecuteResult = CefWebBrowser.ExecuteJs(jsString);
            jsString = $"document.getElementsByClassName('APP-editor-source-code-exit')[0].click()";
            jsExecuteResult = CefWebBrowser.ExecuteJs(jsString);
            return jsExecuteResult;
        }
        /// <summary>
        /// 点击发送
        /// </summary>
        /// <returns></returns>
        public override JsExecuteResult ClickSendEmail() {
            string sendBtStr = "js-component-button nui-mainBtn nui-btn nui-btn-hasIcon nui-mainBtn-hasIcon  ";
            _jsExecuteHelp.InJectGetLocationJs();
            JsExecuteResult jsExecuteResult = _jsExecuteHelp.GetDomAbsLocationByClass(sendBtStr);
            if (jsExecuteResult.Success)
            {
                DomAbsLocation domAbsLocation = JsonConvert.DeserializeObject<DomAbsLocation>(jsExecuteResult.Result);
                CefWebBrowser.CefMouseClick(domAbsLocation.top + 5, domAbsLocation.left + 5);
            }
            Thread.Sleep(1000);
            if (CefWebBrowser.GetHtmlText().Contains("js-component-msgbox nui-msgbox")) {
                string tipsBoxConfirmStr = "js-component-button nui-mainBtn nui-btn ";
                jsExecuteResult = _jsExecuteHelp.GetDomAbsLocationByClass(tipsBoxConfirmStr,2);
                if (jsExecuteResult.Success)
                {
                    DomAbsLocation domAbsLocation = JsonConvert.DeserializeObject<DomAbsLocation>(jsExecuteResult.Result);
                    CefWebBrowser.CefMouseClick(domAbsLocation.top + 5, domAbsLocation.left + 5);
                }
                jsExecuteResult.Success = false;
            }
            return jsExecuteResult;
        }
        /// <summary>
        /// 清除之前的写信标签
        /// </summary>
        /// <returns></returns>
        private JsExecuteResult ClickCloseLabel() {
            string closeLabelStr = "nui-tabs-item-close js-component-link";
            string len = "document.getElementsByClassName('js-component-icon nui-ico nui-ico-close').length";
            JsExecuteResult jsExecuteResult = CefWebBrowser.ExecuteJs(len);
            try
            {
                if (jsExecuteResult.Success)
                {
                    int number = int.Parse(jsExecuteResult.Result);
                    for (int i=0;i<number;i++)
                    {
                        jsExecuteResult = _jsExecuteHelp.GetDomAbsLocationByClass(closeLabelStr,i);
                        DomAbsLocation domAbsLocation = JsonConvert.DeserializeObject < DomAbsLocation>(jsExecuteResult.Result);
                        CefWebBrowser.CefMouseClick(domAbsLocation.top + 4, domAbsLocation.left + 10);
                    }
                }
                Thread.Sleep(2*1000);
                if (CefWebBrowser.GetHtmlText().Contains("js-component-msgbox nui-msgbox"))
                {
                    string jsString = $"document.getElementsByClassName('js-component-msgbox nui-msgbox')[0].getElementsByClassName('js-component-button nui-btn')[0].click();";
                    jsExecuteResult = CefWebBrowser.ExecuteJs(jsString);
                    jsExecuteResult.Success = false;
                    Thread.Sleep(1000);
                }
      
            }
            catch(Exception e)
            {
                jsExecuteResult.Success = false;
                return jsExecuteResult;
            }
            return jsExecuteResult;
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
            string click = " js-component-link nui-txt-link";//3
            string valText = "document.getElementsByClassName(' js-component-link nui-txt-link')[3].innerText";
            string input = "document.getElementsByClassName('kZ0 jL0cc')[0].getElementsByClassName('nui-editableAddr-ipt')[0]";
            string val = "";

            foreach (string item in emailStrList) {
                val += item + ";";
            }
            if (!CefWebBrowser.ExecuteJs(valText).Result.Contains("删除")) {
                JsExecuteResult jsExecuteResult = _jsExecuteHelp.GetDomAbsLocationByClass(click, 3);
                try
                {
                    DomAbsLocation domAbsLocation1 = JsonConvert.DeserializeObject<DomAbsLocation>(jsExecuteResult.Result);
                    CefWebBrowser.CefMouseClick(domAbsLocation1.top + 3, domAbsLocation1.left + 4);
                }
                catch (Exception e)
                {
                    jsExecuteResult.Success = false;
                    return jsExecuteResult;
                }
            }

            JsExecuteResult jsExecuteResult1= CefWebBrowser.ExecuteJs(input + ".focus()");
            CefWebBrowser.CefKeyBoardInput(val);

            DomAbsLocation location = GetElementLocation.get(CefWebBrowser, input);

            if (location != null) {
                CefWebBrowser.CefMouseClick(location.top, location.left, true);
            }

            return jsExecuteResult1;
        }

        public override JsExecuteResult ClickBCCList(List<string> emailStrList, out List<string> errorEmailList) {
            errorEmailList = null;
            //空的
            if (emailStrList == null)
            {
                JsExecuteResult jsExecuteResult0 = new JsExecuteResult();
                jsExecuteResult0.Success = true;
                return jsExecuteResult0;
            }
            string valText = "document.getElementsByClassName(' js-component-link nui-txt-link')[4].innerText";
            string click = " js-component-link nui-txt-link";//4
            string input = "document.getElementsByClassName('kZ0 jL0bcc')[0].getElementsByClassName('nui-editableAddr-ipt')[0]";
            string val = "";
            foreach (string item in emailStrList) {
                val += item + ";";
            }
            JsExecuteResult jsCantains = CefWebBrowser.ExecuteJs(valText);
            if (!jsCantains.Result.Contains("删除")) {
                JsExecuteResult jsExecuteResult = _jsExecuteHelp.GetDomAbsLocationByClass(click, 4);
                try
                {
                    DomAbsLocation domAbsLocation1 = JsonConvert.DeserializeObject<DomAbsLocation>(jsExecuteResult.Result);
                    CefWebBrowser.CefMouseClick(domAbsLocation1.top + 3, domAbsLocation1.left + 4);
                }
                catch (Exception e)
                {
                    jsExecuteResult.Success = false;
                    return jsExecuteResult;
                }
            }
            JsExecuteResult jsExecuteResult1=  CefWebBrowser.ExecuteJs(input + ".focus()");
            CefWebBrowser.CefKeyBoardInput(val);
            CefWebBrowser.CefKeyBoardInput(59);

            DomAbsLocation location = GetElementLocation.get(CefWebBrowser, input);

            if (location != null) {
                CefWebBrowser.CefMouseClick(location.top, location.left, true);
            }

            return jsExecuteResult1;
        }
    }
}
