using CefSharp;
using CefWebBrowser;
using System.Collections.Generic;
using Newtonsoft.Json;
using CefWebBrowser.CefModel;
using CefSharpWebBrowser.CefHelper;
using CefSharpWebBrowser.CefModel;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using CefWebBrowser.CefHelper;
using System;
using TradeWolfRobot.EmailAutomation.Model;
using System.Threading.Tasks;

namespace TradeWolfRobot.EmailAutomation.Email {
    /// <summary>
    /// qq邮箱
    /// </summary>
    public class QqEmail : EmailBase {

        //QMVerify_QMDialog_verifycodeinput   验证码

        string verificationCode = "QMVerify_QMDialog_verifycodeinput";
        ///// <summary>
        ///// 取消自动登录
        ///// </summary>
        //public void CancelLogin()
        //{
        //    ICookieManager CefCookieManager = Cef.GetGlobalCookieManager();
        //    Task<int> t = CefCookieManager.DeleteCookiesAsync(".qq.com");
        //    t.Wait();
        //}


        //document.getElementById('QMconfirm_QMDialog_cancel');
        JsExecuteHelp _jsExecuteHelp;
        public QqEmail(CefWebBrowserControl CefWebBrowser) : base(CefWebBrowser) {
            EmailName = "qqEmail";
            _jsExecuteHelp = new JsExecuteHelp(CefWebBrowser);
        }
        public QqEmail() : base() {
            if (CefWebBrowser == null) {
                CefWebBrowser = new CefWebBrowserControl("https://mail.qq.com/");
            } else {
                CefWebBrowser.LoadNewUrl("https://mail.qq.com/");
            }
            EmailName = "qqEmail";
            _jsExecuteHelp = new JsExecuteHelp(CefWebBrowser);
        }
        public QqEmail(string cachePath) : base(cachePath)
        {
            CefWebBrowser = new CefWebBrowserControl("https://mail.qq.com/", cachePath);
            EmailName = "qqEmail";
            _jsExecuteHelp = new JsExecuteHelp(CefWebBrowser);
        }

        #region 点击写信

        /// <summary>
        /// 点击写信按键、并往html插入属性initlized=true
        /// </summary>
        public override JsExecuteResult ClickWriteEmail() {
            JsExecuteResult jsExecuteResult = new JsExecuteResult();
            string cancel = "QMconfirm_QMDialog_cancel";//取消草稿
            if (CefWebBrowser.IsExistenceText(cancel)) {
                DomNameOrId = cancel;//取消发送
               // string jsString = $"document.getElementById('{DomNameOrId}').click();";
               // jsExecuteResult = CefWebBrowser.ExecuteJs(jsString);
                jsExecuteResult = _jsExecuteHelp.GetDomAbsLocationById(DomNameOrId);
                if (jsExecuteResult.Success)
                {
                    DomAbsLocation domAbsLocation = JsonConvert.DeserializeObject<DomAbsLocation>(jsExecuteResult.Result);
                    CefWebBrowser.CefMouseClick(domAbsLocation.top+4,domAbsLocation.left+5);
                }
            }
            string noSave = "composeExitAlert_QMDialog_btn_delete_save";//  Id 不保存草稿                        
            //写信按键id
            string writeEmailButtonId = "composebtn";
            DomNameOrId = writeEmailButtonId;
            jsExecuteResult = _jsExecuteHelp.GetDomAbsLocationById(DomNameOrId);
            if (!LogJsExecuteStatus(CefWebBrowser.Address, jsExecuteResult)) {
                return jsExecuteResult;
            }
            try {
                CefSharpWebBrowser.CefModel.DomAbsLocation domAbsLocation = JsonConvert.DeserializeObject<CefSharpWebBrowser.CefModel.DomAbsLocation>(jsExecuteResult.Result);
                //点击写信按键
                CefWebBrowser.CefMouseClick(domAbsLocation.top + 5, domAbsLocation.left + 5);
                Thread.Sleep(500);
                if (CefWebBrowser.IsExistenceText(noSave)) {
                    DomNameOrId = noSave;
                    //不保存草稿
                    //  string jsString = $"document.getElementById('{DomNameOrId}').click();";
                    //  jsExecuteResult = CefWebBrowser.ExecuteJs(jsString);
                    jsExecuteResult = _jsExecuteHelp.GetDomAbsLocationById(DomNameOrId);
                    if (jsExecuteResult.Success)
                    {
                        DomAbsLocation domAbsLocation1 = JsonConvert.DeserializeObject<DomAbsLocation>(jsExecuteResult.Result);
                        CefWebBrowser.CefMouseClick(domAbsLocation1.top + 4, domAbsLocation1.left + 5);
                    }
                }
                return jsExecuteResult;
            } catch (System.Exception ex) {
                System.Console.WriteLine(ex.Message);
                throw;
            }
        }
        #endregion

        #region 写入收件人邮箱地址
        /// <summary>
        /// 写入收件人邮箱地址 ps:先获取ifram再获取input
        /// </summary>
        /// <param name="emailAddress">邮箱地址</param>
        public override JsExecuteResult WriteEmailAddress(string emailAddress) {
            //JsExecuteResult jsExecuteResult = _jsExecuteHelp.InJectGetLocationJs();
            ///收件人 toAreaCtrl class
            string addressBoxClass = "toAreaCtrl";
            Thread.Sleep(300);
            DomNameOrId = addressBoxClass;
            IFrame frame = CefWebBrowser.GetFrame("mainFrame");
            //获取收件人文本框的绝对位置
            JsExecuteResult jsExecuteResult = _jsExecuteHelp.GetDomAbsLocationById(DomNameOrId, frame);
            if (!LogJsExecuteStatus(CefWebBrowser.Address, jsExecuteResult)) {
                return jsExecuteResult;
            }
            try {
                string iframeId = "mainFrameContainer";
                DomNameOrId = iframeId;
                CefSharpWebBrowser.CefModel.DomAbsLocation iframeAbsLocation = JsonConvert.DeserializeObject<CefSharpWebBrowser.CefModel.DomAbsLocation>(_jsExecuteHelp.GetDomAbsLocationById(DomNameOrId).Result);
                CefSharpWebBrowser.CefModel.DomAbsLocation domAbsLocation = JsonConvert.DeserializeObject<CefSharpWebBrowser.CefModel.DomAbsLocation>(jsExecuteResult.Result);
                string jsString = @"document.querySelectorAll('.js_input')[1].value=''";
                CefWebBrowser.ExecuteJs(jsString, frame);
                //点击收件人文本框
                CefWebBrowser.CefMouseClick(domAbsLocation.top + iframeAbsLocation.top + 3, domAbsLocation.left + iframeAbsLocation.left + 10);
                if (!LogJsExecuteStatus(CefWebBrowser.Address, jsExecuteResult)) {
                    return jsExecuteResult;
                }
                Thread.Sleep(500);
                CefWebBrowser.CefKeyBoardInput(emailAddress);
                Thread.Sleep(500);
                //写完后再点击一次
                CefWebBrowser.CefMouseClick(domAbsLocation.top + iframeAbsLocation.top + 3, domAbsLocation.left + iframeAbsLocation.left + 40);
                return jsExecuteResult;
            } catch (System.Exception ex) {
                jsExecuteResult.Success = false;
                return jsExecuteResult;
            }
            //IFrame frame = CefWebBrowser.GetFrame("mainFrame");
            //string jsString = @"document.querySelectorAll('.js_input')[1].value='" + emailAddress + "'";
            //return CefWebBrowser.ExecuteJs(jsString, frame);
        }
        #endregion

        #region 写入邮件主题
        /// <summary>
        /// 写入邮件的主题 ps:先获取ifram再获取input
        /// </summary>
        /// <param name="emailTheme">邮件主题</param>
        public override JsExecuteResult WriteEmailTheme(string emailTheme) {
            //addr_base addr_error 错误邮箱class
            //主题 subject    id
            string themeBoxId = "subject";
            Thread.Sleep(300);
            DomNameOrId = themeBoxId;
            IFrame frame = CefWebBrowser.GetFrame("mainFrame");
            //获取收件人文本框的绝对位置
            JsExecuteResult jsExecuteResult = _jsExecuteHelp.GetDomAbsLocationById(DomNameOrId, frame);
            try {
                // iframe的Id mainFrameContainer
                string iframeId = "mainFrameContainer";
                DomNameOrId = iframeId;
                string jsString = @"document.getElementById('subject').value='" + emailTheme + "'";
                CefSharpWebBrowser.CefModel.DomAbsLocation iframeAbsLocation = JsonConvert.DeserializeObject<CefSharpWebBrowser.CefModel.DomAbsLocation>(_jsExecuteHelp.GetDomAbsLocationById(DomNameOrId).Result);
                CefSharpWebBrowser.CefModel.DomAbsLocation domAbsLocation = JsonConvert.DeserializeObject<CefSharpWebBrowser.CefModel.DomAbsLocation>(jsExecuteResult.Result);
                //点击主题文本框
                CefWebBrowser.CefMouseClick(domAbsLocation.top + iframeAbsLocation.top + 3, domAbsLocation.left + iframeAbsLocation.left + 15);
                Thread.Sleep(300);
                if (!LogJsExecuteStatus(CefWebBrowser.Address, jsExecuteResult)) {
                    return jsExecuteResult;
                }
                Thread.Sleep(500);
                CefWebBrowser.CefKeyBoardInput(emailTheme);
                return jsExecuteResult;
            } catch (System.Exception) {
                throw;
            }
            //Thread.Sleep(300);
            //DomNameOrId = themeBoxClass;
            ////点击收件人文本框
            //JsExecuteResult jsExecuteResult = _jsExecuteHelp.GetDomAbsLocationById(DomNameOrId);
            //if (!LogJsExecuteStatus(CefWebBrowser.Address, jsExecuteResult))
            //{
            //    return jsExecuteResult;
            //}
            //CefWebBrowser.CefKeyBoardInput(emailTheme);
            //return jsExecuteResult;

            //IFrame frame = CefWebBrowser.GetFrame("mainFrame");
            //string jsString = @"document.getElementById('subject').value='" + emailTheme + "'";
            //return CefWebBrowser.ExecuteJs(jsString, frame);  
        }
        #endregion

        #region 写入邮件正文
        /// <summary>
        /// 写入邮件正文
        /// </summary>
        /// <param name="body">邮件正文</param>
        public override JsExecuteResult WriteEmailBody(string emailBody) {
            //QMEditorArea   正文Id 
            string emailBodyId = "QMEditorArea";
            Thread.Sleep(300);
            DomNameOrId = emailBodyId;
            IFrame frame = CefWebBrowser.GetFrame("mainFrame");
            //点击源码
            string jsString = "document.getElementsByClassName('qmEditorHtml qmEditorBtnIconHtml qmEditorBtnIcon unselect')[0].click();";
            JsExecuteResult jsExecuteResult = CefWebBrowser.ExecuteJs(jsString, frame);
            //写入源码
            jsString = $"document.getElementsByClassName('qmEditorText')[0].value=`{emailBody}`";
            jsExecuteResult = CefWebBrowser.ExecuteJs(jsString, frame);
            //退出源码
             jsString = "document.getElementsByClassName('qmEditorBtnA qmEditorFormatBtn')[0].click();";
            jsExecuteResult = CefWebBrowser.ExecuteJs(jsString, frame);
            return jsExecuteResult;
            //获取正文文本框的绝对位置
            //JsExecuteResult jsExecuteResult = _jsExecuteHelp.GetDomAbsLocationById(DomNameOrId, frame);
            //try
            //{
            //    // iframe的Id mainFrameContainer
            //    string iframeId = "mainFrameContainer";
            //    DomNameOrId = iframeId;
            //    DomAbsLocation iframeAbsLocation = JsonConvert.DeserializeObject<DomAbsLocation>(_jsExecuteHelp.GetDomAbsLocationById(DomNameOrId).Result);
            //    DomAbsLocation domAbsLocation = JsonConvert.DeserializeObject<DomAbsLocation>(jsExecuteResult.Result);
            //    //点击正文文本框
            //    CefWebBrowser.CefMouseClick(domAbsLocation.top + iframeAbsLocation.top + 20, domAbsLocation.left + iframeAbsLocation.left + 20);
            //    if (!LogJsExecuteStatus(CefWebBrowser.Address, jsExecuteResult))
            //    {
            //        return jsExecuteResult;
            //    }
            //    Thread.Sleep(500);
            //    CefWebBrowser.CefKeyBoardInput(emailBody);
            //    return jsExecuteResult;
            //}
            //catch (System.Exception)
            //{
            //    throw;
            //}
            //IFrame frame = CefWebBrowser.GetFrame("mainFrame");
            //string jsString = @"var iframe=document.getElementsByClassName('qmEditorIfrmEditArea'); 
            //             var div=document.createElement('div');
            //             div.innerText='" + emailBody + @"';                               
            //             iframe[0].contentDocument.body.innerHTML='" + emailBody + "';";
            //return CefWebBrowser.ExecuteJs(jsString, frame);
        }
        #endregion

        #region 点击发送
        /// <summary>
        /// 点击邮件发送按键
        /// </summary>
        public override JsExecuteResult ClickSendEmail() {
            //QMconfirm_QMDialog_cancel
            IFrame frame = CefWebBrowser.GetFrame("mainFrame");
            string jsString = "btn_gray btn_space";
            _jsExecuteHelp.InJectGetLocationJs();
            JsExecuteResult jsExecuteResult= _jsExecuteHelp.GetDomAbsLocationByClass(jsString, null,frame);
            JsExecuteResult jsExecuteResult1 = _jsExecuteHelp.GetDomAbsLocationById("mainFrameContainer");
            if (jsExecuteResult.Success || jsExecuteResult1.Success)
            {
                DomAbsLocation domAbsLocation = JsonConvert.DeserializeObject<DomAbsLocation>(jsExecuteResult.Result);
                DomAbsLocation domAbsLocation1 = JsonConvert.DeserializeObject<DomAbsLocation>(jsExecuteResult1.Result);
                CefWebBrowser.CefMouseClick(domAbsLocation.top + domAbsLocation1.top + 5, domAbsLocation.left + domAbsLocation1.left + 5);
            }
            Thread.Sleep(2 * 1000);
            string cancel = "QMconfirm_QMDialog_confirm";
            if (CefWebBrowser.IsExistenceText(cancel)) {
                DomNameOrId = cancel;
                jsExecuteResult = _jsExecuteHelp.GetDomAbsLocationById(DomNameOrId);
                if (jsExecuteResult.Success)
                {
                    DomAbsLocation domAbsLocation = JsonConvert.DeserializeObject<DomAbsLocation>(jsExecuteResult.Result);
                    CefWebBrowser.CefMouseClick(domAbsLocation.top + 5, domAbsLocation.left + 5);
                }
                jsExecuteResult.Success = false;
                Thread.Sleep(1 * 1000);
            }
            //关闭错误提示框
            if (CefWebBrowser.IsExistenceText("AddressQQ_error_QMDialog") || CefWebBrowser.IsExistenceText("Address_error_QMDialog")) {
                string tipsBoxConfirmBtStr = "btn wd2";
                jsExecuteResult = _jsExecuteHelp.GetDomAbsLocationByClass(tipsBoxConfirmBtStr, null);
                if (jsExecuteResult.Success)
                {
                    DomAbsLocation domAbsLocation = JsonConvert.DeserializeObject<DomAbsLocation>(jsExecuteResult.Result);
                    CefWebBrowser.CefMouseClick(domAbsLocation.top + 3, domAbsLocation.left + 5);
                }
                jsExecuteResult.Success = false;
                Thread.Sleep(1 * 1000);
            }
            Thread.Sleep(1 * 1000);
            return jsExecuteResult;
        }
        #endregion

        public override object GetCookie() {
            return CefCookieHelper.GetCookieList("https://mail.qq.com/");
            //return CefWebBrowser.GetCookieList("https://mail.qq.com/");
        }
        public override void SetCookie(object cookie) {
            List<Cookie> cookieList = new List<Cookie>();
            cookieList = cookie as List<Cookie>;
            if (cookie != null) {
                if (cookieList == null) {
                    cookieList = JsonConvert.DeserializeObject<List<Cookie>>(cookie.ToString());
                }
                CefCookieHelper.SetCookie("https://mail.qq.com/", cookieList);
            }
        }
        /// <summary>
        /// 自动登录
        /// </summary>
        /// <param name="account">账号</param>
        /// <param name="password">密码</param>
        /// <param name="recursionCount">递归次数，默认1</param>
        /// <returns></returns>
        public override LoginState IsLogin(string account, string password, int recursionCount = 1) {
            LoginState loginState = new LoginState();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            string loginClassStr = "login_container";
            string loginsuccessIdStr = "navBarDiv";
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
                if (CefWebBrowser.GetHtmlText().Contains(loginsuccessIdStr)) {
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
            string jsframe = "login_frame";
            IFrame frame = CefWebBrowser.GetFrame(jsframe);
            JsExecuteResult jsExecuteResult = null;
            try {

                string jsString = $"document.getElementById('uin_tips').innerText='';" +
                    $"document.getElementsByClassName('inputstyle')[0].value='';" +
                    $"document.getElementsByClassName('inputstyle')[0].value='{account}'";
                CefWebBrowser.ExecuteJs(jsString, frame);
               /* string accountInputId = "uin_tips";
                string passwordInputId = "pwd_tips";
                _jsExecuteHelp.GetDomAbsLocationById(account, frame);*/
                Thread.Sleep(1000);
                jsString = $"document.getElementById('pwd_tips').innerText='';" +
                    $"document.getElementsByClassName('inputstyle password')[0].value='';" +
                    $"document.getElementsByClassName('inputstyle password')[0].value='{password}';";
                CefWebBrowser.ExecuteJs(jsString, frame);
                Thread.Sleep(1000);
                string jsGetAutomaticLogonClassStr = "document.getElementById('p_low_login_enable').className";
                JsExecuteResult jsExecuteResult2 = CefWebBrowser.ExecuteJs(jsGetAutomaticLogonClassStr, frame);
                if (jsExecuteResult2.Success) {
                    if (jsExecuteResult2.Result == "uncheck") {
                        string jsAutomaticLogonIdStr = "document.getElementById('p_low_login_enable').click();";
                        CefWebBrowser.ExecuteJs(jsAutomaticLogonIdStr, frame);
                        Thread.Sleep(1000);
                    }
                }
                //点击登录
                string jsBtString = "document.getElementById('login_button').click();";
                CefWebBrowser.ExecuteJs(jsBtString, frame);
                Thread.Sleep(1 * 1000);
            } catch (Exception ex) {

            }
            //登录超时
            stopwatch.Restart();
            LoginOutTime = 10 * 1000;
            while (true) {
                if (_StopTask) {
                    return loginState;
                }
                //账号密码错误
                string jsStringcode = "document.getElementById('err_m').innerText;";
                jsExecuteResult = CefWebBrowser.ExecuteJs(jsStringcode, frame);
                if (jsExecuteResult.Result.ToString() != "") {
                    loginState.IsLoginSuccess = false;
                    return loginState;
                }
                //验证码
                if (CefWebBrowser.GetHtmlText().Contains("tc-title middle-fontsize small-fontsize multi-line")) {
                    loginState.IsVCode = true;
                    return loginState;
                }
                //登录成功
                if (CefWebBrowser.GetHtmlText().Contains(loginsuccessIdStr)) {
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
                Thread.Sleep(100);
            }
        }
        public override bool IsNeedVerificationCode() {
            string sHtml = CefWebBrowser.GetHtmlText();
            if (sHtml.Contains(verificationCode))
                return true;
            else
                return false;
        }

        public override JsExecuteResult ClickCCList(List<string> emailStrList, out List<string> errorEmailList) {
            errorEmailList = null;
            //document.getElementById("aCC").click() <--单击抄送按钮-->
            //document.getElementsByClassName("addr_text")[2].getElementsByTagName("input")[0].value="2075383131@qq.com;3037105677@qq.com;cacode@163.com" <--添加多个人-->
            //空的
            if (emailStrList == null)
            {
                JsExecuteResult jsExecuteResult0 = new JsExecuteResult();
                jsExecuteResult0.Success = true;
                return jsExecuteResult0;
            }
            string val = "";
            string jsCCInputStr = "ccAreaCtrl";
            string valText = "document.getElementById('aCC').innerText";
            foreach (string item in emailStrList) {
                val += item + ";";
            }

            IFrame frame = CefWebBrowser.GetFrame("mainFrame");
            string sendCC = "aCC";
            JsExecuteResult jsExecuteResult1 = _jsExecuteHelp.GetDomAbsLocationById("mainFrameContainer");
            if (!CefWebBrowser.ExecuteJs(valText, frame).Result.Contains("删除"))
            {
                JsExecuteResult jsExecuteResult = _jsExecuteHelp.GetDomAbsLocationById(sendCC, frame);

                if (jsExecuteResult.Success)
                {
                    DomAbsLocation domAbsLocation = JsonConvert.DeserializeObject<DomAbsLocation>(jsExecuteResult.Result);
                    DomAbsLocation domAbsLocation1 = JsonConvert.DeserializeObject<DomAbsLocation>(jsExecuteResult1.Result);
                    CefWebBrowser.CefMouseClick(domAbsLocation.top + domAbsLocation1.top + 3, domAbsLocation.left + domAbsLocation1.left + 4);
                    Thread.Sleep(1000);
                }
            }
            JsExecuteResult jsExecuteResult2 = _jsExecuteHelp.GetDomAbsLocationById(jsCCInputStr, frame);
            try
            {
                DomAbsLocation domAbsLocation = JsonConvert.DeserializeObject<DomAbsLocation>(jsExecuteResult1.Result);
                DomAbsLocation domAbsLocation1 = JsonConvert.DeserializeObject<DomAbsLocation>(jsExecuteResult2.Result);
                CefWebBrowser.CefMouseClick(domAbsLocation.top + domAbsLocation1.top + 3, domAbsLocation.left + domAbsLocation1.left + 4);
                Thread.Sleep(1000);
            }
            catch (Exception e)
            {
                jsExecuteResult1.Success = false;
                return jsExecuteResult1;
            }
            CefWebBrowser.CefKeyBoardInput(val);
            Thread.Sleep(1000);
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
            string val = "";
            string valText = "document.getElementById('aBCC').innerText";
            string jsBCCInputStr = "bccAreaCtrl";//id
            foreach (string item in emailStrList) {
                val += item + ";";
            }
            
            IFrame frame = CefWebBrowser.GetFrame("mainFrame");
            string sendBCC = "aBCC";
            JsExecuteResult jsExecuteResult = _jsExecuteHelp.GetDomAbsLocationById(sendBCC, frame);
            JsExecuteResult jsExecuteResult1 = _jsExecuteHelp.GetDomAbsLocationById("mainFrameContainer");
            if (!CefWebBrowser.ExecuteJs(valText, frame).Result.Contains("删除"))
            {
                if (jsExecuteResult.Success)
                {
                    DomAbsLocation domAbsLocation = JsonConvert.DeserializeObject<DomAbsLocation>(jsExecuteResult.Result);
                    DomAbsLocation domAbsLocation1 = JsonConvert.DeserializeObject<DomAbsLocation>(jsExecuteResult1.Result);
                    CefWebBrowser.CefMouseClick(domAbsLocation.top + domAbsLocation1.top + 3, domAbsLocation.left + domAbsLocation1.left + 4);
                    Thread.Sleep(1000);
                }
            }
            JsExecuteResult jsExecuteResult2 = _jsExecuteHelp.GetDomAbsLocationById(jsBCCInputStr, frame);
            try
            {
                DomAbsLocation domAbsLocation1 = JsonConvert.DeserializeObject<DomAbsLocation>(jsExecuteResult1.Result);
                DomAbsLocation domAbsLocation2 = JsonConvert.DeserializeObject<DomAbsLocation>(jsExecuteResult2.Result);
                CefWebBrowser.CefMouseClick(domAbsLocation1.top + domAbsLocation2.top + 3, domAbsLocation1.left + domAbsLocation2.left + 4);
                Thread.Sleep(1000);
            }catch(Exception e)
            {
                jsExecuteResult.Success = false;
                return jsExecuteResult;
            }
            Thread.Sleep(500);
            CefWebBrowser.CefKeyBoardInput(val);
            Thread.Sleep(1000);
            return jsExecuteResult1;
        }

        /// <summary>
        /// 是否登录成功
        /// </summary>
        /// <returns></returns>
        //public bool IsLoginSuccess()
        //{
        //    Stopwatch sw = new Stopwatch();
        //    sw.Start();
        //    while (!CefWebBrowser.IsLoad)
        //    {
        //        sw.Stop();
        //        if (sw.ElapsedMilliseconds >= LoginTimeOut)
        //        {
        //            return false;
        //        }
        //        else
        //        {
        //            sw.Start();
        //        }
        //    }
        //    if (!CefWebBrowser.Address.Contains("https://mail.qq.com/cgi-bin/frame_html?"))
        //    {
        //        return false;
        //    }
        //    else
        //    {
        //        return true;
        //    }
        //}

        #region
        //#region 控件位置获取(备案)
        ///// <summary>
        ///// 获取写信按键绝对位置
        ///// </summary>
        ///// <returns></returns>
        //public string GetClickWriteEmailLocation()
        //{
        //    string jsString = @"(function(){
        //                        var d=document.getElementById('composebtn');
        //                        var top=getTop(d);
        //                        var left=getLeft(d);
        //                        var result={'top':top,'left':left};
        //                        return JSON.stringify(result);
        //                        })();";
        //    string result= CefWebBrowser.ExecuteJs(jsString);
        //    return result;
        //}
        ///// <summary>
        ///// 获取收件人控件绝对位置
        ///// </summary>
        ///// <returns></returns>
        //public string GetWriteEmailAddressLocation()
        //{
        //    IFrame frame = CefWebBrowser.GetFrame("mainFrame");
        //    string jsString = @"(function(){
        //                        var d=document.getElementsByClassName('js_input');
        //                        var top=getTop(d[1]);
        //                        var left=getLeft(d[1]);
        //                        var result={'top':top,'left':left};
        //                        return JSON.stringify(result);
        //                        })();";
        //    string result = CefWebBrowser.ExecuteJs(jsString,frame);
        //    return result;
        //}
        ///// <summary>
        ///// 获取主题控件的绝对位置
        ///// </summary>
        ///// <returns></returns>
        //public string GetWriteEmailThemeLocation()
        //{
        //    IFrame frame = CefWebBrowser.GetFrame("mainFrame");
        //    string jsString = @"(function(){
        //                        var d=document.getElementById('subject');
        //                        var top=getTop(d);
        //                        var left=getLeft(d);
        //                        var result={'top':top,'left':left};
        //                        return JSON.stringify(result);
        //                        })();";
        //    string result = CefWebBrowser.ExecuteJs(jsString, frame);
        //    return result;
        //}
        ///// <summary>
        ///// 获取正文控件iframe的绝对位置
        ///// </summary>
        ///// <returns></returns>
        //public string GetWriteEmailBodyLocation()
        //{
        //    IFrame frame = CefWebBrowser.GetFrame("mainFrame");
        //    string jsString = @"(function(){
        //                        var d=document.getElementsByClassName('qmEditorIfrmEditArea');
        //                        var top=getTop(d[0]);
        //                        var left=getLeft(d[0]);
        //                        var result={'top':top,'left':left};
        //                        return JSON.stringify(result);
        //                        })();";
        //    string result = CefWebBrowser.ExecuteJs(jsString, frame);
        //    return result;
        //}
        ///// <summary>
        ///// 获取邮件发送按键的绝对位置
        ///// </summary>
        ///// <returns></returns>
        //public string GetClickSendEmailLocation()
        //{
        //    IFrame frame = CefWebBrowser.GetFrame("mainFrame");
        //    string jsString = @"(function(){
        //                        var d=document.getElementsByName('sendbtn');
        //                        var top=getTop(d[0]);
        //                        var left=getLeft(d[0]);
        //                        var result={'top':top,'left':left};
        //                        return JSON.stringify(result);
        //                        })();";
        //    string result = CefWebBrowser.ExecuteJs(jsString, frame);
        //    return result;
        //}
        //#endregion

        //#region 模拟鼠标键盘操作(备案)
        ///// <summary>
        ///// 点击写信按键、并往html插入属性initlized=true
        ///// </summary>
        //public void ClickWriteEmail(int top, int left)
        //{
        //    CefWebBrowser.CefMouseClick(top, left);
        //}
        ///// <summary>
        ///// 下一次点击写信按键
        ///// </summary>
        //public string NextClickWriteEmail(int top, int left)
        //{
        //    string jsString = @"document.getElementById('composebtn').click()";
        //    string result = CefWebBrowser.ExecuteJs(jsString);
        //    return result;
        //}
        ///// <summary>
        ///// 写入收件人邮箱地址 ps:先获取ifram再获取input
        ///// </summary>
        ///// <param name="emailAddress">邮箱地址</param>
        //public string WriteEmailAddress(int top, int left,string emailAddress)
        //{
        //    IFrame frame = CefWebBrowser.GetFrame("mainFrame");
        //    string jsString = @"document.querySelectorAll('.js_input')[1].value='" + emailAddress + "'";
        //    string result = CefWebBrowser.ExecuteJs(jsString, frame);
        //    return result;
        //}
        ///// <summary>
        ///// 写入邮件的主题 ps:先获取ifram再获取input
        ///// </summary>
        ///// <param name="emailTheme">邮件主题</param>
        //public string WriteEmailTheme(int top, int left,string emailTheme)
        //{
        //    IFrame frame = CefWebBrowser.GetFrame("mainFrame");
        //    string jsString = @"document.getElementById('subject').value='" + emailTheme + "'";
        //    string result = CefWebBrowser.ExecuteJs(jsString, frame);
        //    return result;
        //}
        ///// <summary>
        ///// 写入邮件正文
        ///// </summary>
        ///// <param name="body">邮件正文</param>
        //public string WriteEmailBody(int top, int left,string emailBody)
        //{
        //    IFrame frame = CefWebBrowser.GetFrame("mainFrame");
        //    string jsString = @"var iframe=document.getElementsByClassName('qmEditorIfrmEditArea'); 
        //                 var div=document.createElement('div');
        //                 div.innerText='" + emailBody + @"';                               
        //                 iframe[0].contentDocument.body.innerHTML='" + emailBody + "';";
        //    string result = CefWebBrowser.ExecuteJs(jsString, frame);
        //    return result;
        //}
        ///// <summary>
        ///// 点击邮件发送按键
        ///// </summary>
        //public string ClickSendEmail(int top, int left)
        //{
        //    IFrame frame = CefWebBrowser.GetFrame("mainFrame");
        //    string jsString = @"document.getElementsByName('sendbtn')[0].click();";
        //    string result = CefWebBrowser.ExecuteJs(jsString, frame);
        //    return result;
        //}

        //#endregion

        //#region 注入可调用js代码
        ///// <summary>
        ///// 往主窗体注入获取控件绝对位置的js代码
        ///// </summary>
        ///// <returns></returns>
        //public string InJectGetLocationJs()
        //{
        //    string jsString = @"function getTop(e)
        //                     {
        //                       var offset = e.offsetTop;
        //                       if(e.offsetParent!=null) offset+=getTop(e.offsetParent);
        //                       return offset;
        //                     }
        //                     //获取元素的横坐标
        //                     function getLeft(e)
        //                     {
        //                        var offset = e.offsetLeft;
        //                        if (e.offsetParent != null) offset += getLeft(e.offsetParent);
        //                        return offset;
        //                     }";
        //    string result =CefWebBrowser.ExecuteJs(jsString);
        //    return result;
        //}
        ///// <summary>
        ///// 往Iframe里注入获取控件绝对位置的js代码
        ///// </summary>
        ///// <returns></returns>
        //public string InJectGetLocationJsForIframe()
        //{
        //    IFrame frame = CefWebBrowser.GetFrame("mainFrame");
        //    string jsString = @"function getTop(e)
        //                     {
        //                       var offset = e.offsetTop;
        //                       if(e.offsetParent!=null) offset+=getTop(e.offsetParent);
        //                       return offset;
        //                     }
        //                     //获取元素的横坐标
        //                     function getLeft(e)
        //                     {
        //                        var offset = e.offsetLeft;
        //                        if (e.offsetParent != null) offset += getLeft(e.offsetParent);
        //                        return offset;
        //                     }";
        //    string result = CefWebBrowser.ExecuteJs(jsString, frame);
        //    return result;
        //}
        //#endregion
        #endregion
    }
}
