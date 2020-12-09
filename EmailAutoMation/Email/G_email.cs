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
using System.Threading.Tasks;
using System.Windows.Forms;
using TradeWolfRobot.EmailAutomation.Model;
namespace TradeWolfRobot.EmailAutomation.Email {
    public class G_email : EmailBase {
        JsExecuteHelp _jsExecuteHelp;
        public G_email(CefWebBrowserControl CefWebBrowser) : base(CefWebBrowser) {
            EmailName = "G_email";
            _jsExecuteHelp = new JsExecuteHelp(CefWebBrowser);
        }
        public G_email() : base() {
            if (CefWebBrowser == null) {
                CefWebBrowser = new CefWebBrowserControl("https://mail.google.com/");
            } else {
                CefWebBrowser.LoadNewUrl("https://mail.google.com/");
            }
            EmailName = "G_email";
            _jsExecuteHelp = new JsExecuteHelp(CefWebBrowser);
        }
        public G_email(string cachePath) : base(cachePath)
        {
            CefWebBrowser = new CefWebBrowserControl("https://mail.google.com/", cachePath);
            EmailName = "G_email";
            _jsExecuteHelp = new JsExecuteHelp(CefWebBrowser);
        }

        public override void SetCookie(object cookie) {

        }

        public override object GetCookie() {
            return "";
        }


        public override LoginState IsLogin(string account, string password, int recursionCount = 1) {
            LoginState loginState = new LoginState();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            string loginClassStr = "OIPlvf";
            string loginsuccessClassStr = "akc aZ6 YX";
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
            //页面加载失败 重新加载
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
            string jsUserAndPasswordClassStr = "whsOnd zHQkBf";
            string jsNextStepBtClassStr = "VfPpkd-vQzf8d";
            //是否存在用户账号
            string jsIsUserString = "document.getElementsByClassName('wLBAL')[0].innerText";
            JsExecuteResult jsExecuteResult2 = CefWebBrowser.ExecuteJs(jsIsUserString);
            if (jsExecuteResult2.Success) {
                try {
                    //存在用户账号
                    string jsString = "document.getElementsByClassName('BHzsHc')[0].click();";
                    CefWebBrowser.ExecuteJs(jsString);
                    Thread.Sleep(1000);
                } catch (Exception e1) {
                    loginState.IsLoginSuccess = false;
                    return loginState;
                }
            }
            try {
                //输入账号
                string jsUserString = $"document.getElementsByClassName('{jsUserAndPasswordClassStr}')[0].value='';document.getElementsByClassName('{jsUserAndPasswordClassStr}')[0].value='{account}'";
                CefWebBrowser.ExecuteJs(jsUserString);
                Thread.Sleep(2 * 1000);
                //点击下一步
                string jsNextStepString = "document.getElementsByClassName('VfPpkd-vQzf8d')[0].click();";
                CefWebBrowser.ExecuteJs(jsNextStepString);
                Thread.Sleep(2 * 1000);
                //是否用户账号错误
                string jsErroString = "document.getElementsByClassName('o6cuMc')[0].innerText";
                JsExecuteResult jsExecuteResult1 = CefWebBrowser.ExecuteJs(jsErroString);
                if (jsExecuteResult1.Success) {
                    //用户账号错误
                    if (!(jsExecuteResult1.Result == "")) {
                        MessageBox.Show(jsExecuteResult1.Result, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        loginState.IsLoginSuccess = false;
                        return loginState;
                    }
                }
                //输入密码
                string jsPasswordString = $"document.getElementsByClassName('{jsUserAndPasswordClassStr}')[0].value='';document.getElementsByClassName('{jsUserAndPasswordClassStr}')[0].value='{password}'";
                CefWebBrowser.ExecuteJs(jsPasswordString);
                Thread.Sleep(1000);
                //点击下一步
                CefWebBrowser.ExecuteJs(jsNextStepString);
                Thread.Sleep(1000);
                //是否用户密码错误
                jsErroString = "document.getElementsByClassName('OyEIQ uSvLId')[0].innerText";
                jsExecuteResult1 = CefWebBrowser.ExecuteJs(jsErroString);
                if (jsExecuteResult1.Success) {
                    //密码错误
                    if (!(jsExecuteResult1.Result == "")) {
                        loginState.IsLoginSuccess = false;
                        return loginState;
                    }
                }

            } catch (Exception ex) {

            }
            stopwatch.Restart();
            LoginOutTime = 20 * 1000;
            bool isReload = false;
            while (true) {
                //是否登陆成功
                Thread.Sleep(100);
                if (CefWebBrowser.GetHtmlText().Contains(loginsuccessClassStr)) {
                    stopwatch.Stop();
                    loginState.IsLoginSuccess = true;
                    return loginState;
                }
                //超时
                if (stopwatch.ElapsedMilliseconds >= LoginOutTime) {
                    stopwatch.Stop();
                    loginState.IsLoginSuccess = false;
                    return loginState;
                }
                //自动登录成功
                if (CefWebBrowser.Address.Contains("google.com/?utm_source=")) {
                    if (!isReload) {
                        //刷新加载邮箱
                        CefWebBrowser.LoadNewUrl("https://mail.google.com/");
                        isReload = true;
                    }
                }
            }
        }
        /// <summary>
        /// 点击写信
        /// </summary>
        /// <returns></returns>
        public override JsExecuteResult ClickWriteEmail() {
            // string jsString = "document.getElementsByClassName('T-I T-I-KE L3')[0].click()";
            // JsExecuteResult jsExecuteResult = CefWebBrowser.ExecuteJs(jsString);
            string jsString = "T-I T-I-KE L3";//0
            JsExecuteResult jsExecuteResult = _jsExecuteHelp.GetDomAbsLocationByClass(jsString);
            try
            {
                DomAbsLocation domAbsLocation = JsonConvert.DeserializeObject<DomAbsLocation>(jsExecuteResult.Result);
                CefWebBrowser.CefMouseClick(domAbsLocation.top + 5, domAbsLocation.left + 30);
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
        /// 写入收件人
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <returns></returns>
        public override JsExecuteResult WriteEmailAddress(string emailAddress) {
         
            string jsString = $"document.getElementsByClassName('vO')[0].value='{emailAddress}'";
           JsExecuteResult jsExecuteResult = CefWebBrowser.ExecuteJs(jsString);
            Thread.Sleep(2 * 1000);
            return jsExecuteResult;
        }
        /// <summary>
        /// 写主题
        /// </summary>
        /// <param name="emailTheme"></param>
        /// <returns></returns>
        public override JsExecuteResult WriteEmailTheme(string emailTheme) {
            string jsString = "aoT";
            JsExecuteResult jsExecuteResult = _jsExecuteHelp.GetDomAbsLocationByClass(jsString);
            try
            {
                DomAbsLocation domAbsLocation = JsonConvert.DeserializeObject<DomAbsLocation>(jsExecuteResult.Result);
                CefWebBrowser.CefMouseClick(domAbsLocation.top + 5, domAbsLocation.left + 5);
                Thread.Sleep(1 * 1000);
                CefWebBrowser.CefKeyBoardInput(emailTheme);
                Thread.Sleep(1 * 1000);
            }
            catch (Exception e)
            {
                jsExecuteResult.Success = false;
                return jsExecuteResult;
            }
            return jsExecuteResult;
        }
        /// <summary>
        /// 写正文
        /// </summary>
        /// <param name="emailBody"></param>
        /// <returns></returns>
        public override JsExecuteResult WriteEmailBody(string emailBody) {
            string js = "function parseDom(arg) {var objE = document.createElement('div');objE.innerHTML = arg;return objE.childNodes;};";
            string jsString = $"document.getElementsByClassName('Am Al editable LW-avf tS-tW')[0].appendChild(parseDom(`{emailBody}`)[0])";
            JsExecuteResult jsExecuteResult = CefWebBrowser.ExecuteJs(js + jsString);
            Thread.Sleep(2 * 1000);
            return jsExecuteResult;
        }

        public override JsExecuteResult ClickSendEmail() {
            //点击发送
            string sendBtStr = "T-I J-J5-Ji aoO v7 T-I-atl L3";
            _jsExecuteHelp.InJectGetLocationJs();
            JsExecuteResult jsExecuteResult = _jsExecuteHelp.GetDomAbsLocationByClass(sendBtStr, null);
            if (jsExecuteResult.Success)
            {
                DomAbsLocation domAbsLocation = JsonConvert.DeserializeObject<DomAbsLocation>(jsExecuteResult.Result);
                CefWebBrowser.CefMouseClick(domAbsLocation.top + 5, domAbsLocation.left + 5);
            }
            Thread.Sleep(2 * 1000);
            //弹出提示
            if (CefWebBrowser.GetHtmlText().Contains("Kj-JD-K7 Kj-JD-K7-bsT")) {
                string tipsBoxConfirmStr = "J-at1-auR";
                _jsExecuteHelp.InJectGetLocationJs();
                jsExecuteResult = _jsExecuteHelp.GetDomAbsLocationByClass(tipsBoxConfirmStr, null);
                if (jsExecuteResult.Success)
                {
                    DomAbsLocation domAbsLocation = JsonConvert.DeserializeObject<DomAbsLocation>(jsExecuteResult.Result);
                    CefWebBrowser.CefMouseClick(domAbsLocation.top + 5, domAbsLocation.left + 5);
                }
                Thread.Sleep(2 * 1000);
                //获取关闭按钮
                try {
                    string tipBoxClose = "Ha";
                    jsExecuteResult = _jsExecuteHelp.GetDomAbsLocationByClass(tipBoxClose);
                    if (jsExecuteResult.Success)
                    {
                        DomAbsLocation domAbsLocation = JsonConvert.DeserializeObject<DomAbsLocation>(jsExecuteResult.Result);
                        CefWebBrowser.CefMouseClick(domAbsLocation.top + 2, domAbsLocation.left + 2);
                    }
                    Thread.Sleep(1 * 1000);
                } catch (Exception e1) {
                    Console.WriteLine(e1.Message);
                }
            }
            return jsExecuteResult;
        }

        public override bool IsNeedVerificationCode() {
            return false;
        }
        public override JsExecuteResult ClickCCList(List<string> emailStrList, out List<string> errorEmailList) {
            errorEmailList = null;
            //空的
            if (emailStrList == null)
            {
                JsExecuteResult jsExecuteResult0 = new JsExecuteResult();
                jsExecuteResult0.Success = true;
                return jsExecuteResult0;
            }
            string click = "document.getElementsByClassName('aB')[0].click()";
            string val = "document.getElementsByName('cc')[0].value='";
            foreach (string item in emailStrList)
            {
                val += item + ",";
            }

            val = val.Substring(0, val.Length - 1) + "'";

          JsExecuteResult jsExecuteResult=  CefWebBrowser.ExecuteJs(click);
            CefWebBrowser.ExecuteJs(val);
            Thread.Sleep(1000);
            return jsExecuteResult;

        }

        public override JsExecuteResult ClickBCCList(List<string> emailStrList, out List<string> errorEmailList) {
            errorEmailList = null;
            //document.getElementsByClassName("aB  gQ pB")[0].click() <--单击密送按钮-->
            //document.getElementsByName("bcc").value='2075383131@qq.com,3037105677@qq.com,cacode@163.com' <--添加多个人-->
            //空的
            if (emailStrList == null)
            {
                JsExecuteResult jsExecuteResult0 = new JsExecuteResult();
                jsExecuteResult0.Success = true;
                return jsExecuteResult0;
            }
            string click = "document.getElementsByClassName('aB')[1].click()";
            string val = "document.getElementsByName('bcc')[0].value='";
            foreach (string item in emailStrList)
            {
                val += item + ",";
            }

            val = val.Substring(0, val.Length - 1) + "'";

          JsExecuteResult jsExecuteResult=  CefWebBrowser.ExecuteJs(click);
            CefWebBrowser.ExecuteJs(val);
            Thread.Sleep(1000);
            return jsExecuteResult;
        }
    }
}
