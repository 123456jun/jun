using CefSharp;
using CefSharpWebBrowser.CefHelper;
using CefSharpWebBrowser.CefModel;
using CefWebBrowser;
using CefWebBrowser.CefHandler;
using CefWebBrowser.CefModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using TradeWolfRobot.EmailAutomation.Model;

namespace TradeWolfRobot.EmailAutomation.Email {
    public class AlicloudEnterpriseEmail : EmailBase {
        //提示框
        string TipsBoxClassStr1 = "e_text_link_b";
        //写信按钮
        string WriteEmailButtonClassStr = "navpane_compose e_b e_b_most";
        //收件人
        string AddresseeClassStr = "e_entry_input";
        //主题
        string emailThemeClassStr = "e_textbox_input";
        //工具栏
        string ToolbarClassStr = "e_i e_i_fs16 e_i_hover e_i_more";
        //源码
        string SourceCodeClassStr = "e_i e_i_fs16 e_i_hover e_i_code";
        //编辑源码
        string EditClassStr = "e_scroll e_scroll_auto_y";
        //退出源码
        string ExitEditClassStr = "e_editor_source_close";
        //发送按钮
        string SendAndConfirmButtonStr = "e_b e_unselectable e_inline_block e_b_middle e_b_normal";
        //提示框
        string TipsBoxClassStr2 = "e_l_wrap e_l_box_shadow e_d_wrap e_confirm_d_wrap";

        JsExecuteHelp _jsExecuteHelp;

        public AlicloudEnterpriseEmail(CefWebBrowserControl CefWebBrowser) : base(CefWebBrowser) {
            EmailName = "AlicloudEnterpriseEmail";
            _jsExecuteHelp = new JsExecuteHelp(CefWebBrowser);
        }
        public AlicloudEnterpriseEmail() : base() {
            if (CefWebBrowser == null) {
                CefWebBrowser = new CefWebBrowserControl("http://mail.hichina.com/alimail/");
            } else {
                CefWebBrowser.LoadNewUrl("http://mail.hichina.com/alimail/");
            }
            EmailName = "AlicloudEnterpriseEmail";
            _jsExecuteHelp = new JsExecuteHelp(CefWebBrowser);
        }
        public AlicloudEnterpriseEmail(string cachePath) : base(cachePath)
        {
            CefWebBrowser = new CefWebBrowserControl("http://mail.hichina.com/alimail/", cachePath);
            EmailName = "AlicloudEnterpriseEmail";
            _jsExecuteHelp = new JsExecuteHelp(CefWebBrowser);
        }
        /// <summary>
        /// 点击写信
        /// </summary>
        /// <returns></returns>
        public override JsExecuteResult ClickWriteEmail() {
            ClickCloseLabel();
            // if (CefWebBrowser.IsExistenceText(TipsBoxClassStr1))
            // {
            //     _jsExecuteHelp.ExecuteClickByClass(TipsBoxClassStr1);
            // }
            JsExecuteResult jsExecuteResult = _jsExecuteHelp.GetDomAbsLocationByClass(WriteEmailButtonClassStr);
            if (jsExecuteResult.Success)
            {
                DomAbsLocation domAbsLocation = JsonConvert.DeserializeObject<DomAbsLocation>(jsExecuteResult.Result);
                CefWebBrowser.CefMouseClick(domAbsLocation.top + 5, domAbsLocation.left + 5);
                Thread.Sleep(500);
            }
            return jsExecuteResult;
        }
        /// <summary>
        /// 收件人
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <returns></returns>

        public override JsExecuteResult WriteEmailAddress(string emailAddress) {
            List<string> addressList = new List<string>();
            while (emailAddress.IndexOf(";") != -1)
            {
                addressList.Add(emailAddress.Substring(0, emailAddress.IndexOf(";")));
                emailAddress = emailAddress.Substring(emailAddress.IndexOf(";")+1);
            }
            if (emailAddress.IndexOf(";") == -1)
            {
                addressList.Add(emailAddress);
            }
            JsExecuteResult jsExecuteResult = _jsExecuteHelp.GetDomAbsLocationByClass(AddresseeClassStr);
            try {
             
                foreach (var item in addressList)
                {
                    string jsStr = $"document.getElementsByClassName('{AddresseeClassStr}')[0].focus();";
                    CefWebBrowser.ExecuteJs(jsStr);
                    Thread.Sleep(1000);
                    CefWebBrowser.CefKeyBoardInput(item);
                    Thread.Sleep(3 * 1000);
                    CefWebBrowser.CefKeyBoardInput();
                }
                return jsExecuteResult;
            } catch (Exception ex) {
                jsExecuteResult.Success = false;
                return jsExecuteResult;
            }
        }

        /// <summary>
        /// 主题
        /// </summary>
        /// <param name="emailTheme"></param>
        /// <returns></returns>
        public override JsExecuteResult WriteEmailTheme(string emailTheme) {
            JsExecuteResult jsExecuteResult = _jsExecuteHelp.GetDomAbsLocationByClass(emailThemeClassStr,1);
            if (jsExecuteResult.Success)
            {
                DomAbsLocation domAbsLocation = JsonConvert.DeserializeObject<DomAbsLocation>(jsExecuteResult.Result);
                CefWebBrowser.CefMouseClick(domAbsLocation.top + 5, domAbsLocation.left + 5);
                Thread.Sleep(1000);
                CefWebBrowser.CefKeyBoardInput(emailTheme);
                Thread.Sleep(1000);
            }
            return jsExecuteResult;
        }
        /// <summary>
        /// 正文
        /// </summary>
        /// <param name="emailBody"></param>
        /// <returns></returns>
        public override JsExecuteResult WriteEmailBody(string emailBody) {
            //string jsString = $"document.getElementsByClassName('{ToolbarClassStr}')[0].click()";
            //CefWebBrowser.ExceuteJsAsync(jsString);
            //jsString = $"document.getElementsByClassName('{SourceCodeClassStr}')[2].click()";
            //CefWebBrowser.ExceuteJsAsync(jsString);
            //jsString = $"document.getElementsByClassName('{EditClassStr}')[6].value=`{emailBody}`";
            //CefWebBrowser.ExecuteJs(jsString);
            //jsString= $"document.getElementsByClassName('{ExitEditClassStr}')[0].click()";
            //JsExecuteResult jsExecuteResult = CefWebBrowser.ExecuteJs(jsString);
            //return jsExecuteResult;

            JsExecuteResult jsExecuteResult = _jsExecuteHelp.ExecuteClickByClass(ToolbarClassStr);
            jsExecuteResult = _jsExecuteHelp.ExecuteClickByClass(SourceCodeClassStr, 2);
            Thread.Sleep(500);
            string jsString = $"document.getElementsByClassName('{EditClassStr}')[6].value=`{emailBody}`";
            jsExecuteResult = CefWebBrowser.ExecuteJs(jsString);
            Thread.Sleep(1000);
            jsExecuteResult = _jsExecuteHelp.ExecuteClickByClass(ExitEditClassStr);
            return jsExecuteResult;
        }

        /// <summary>
        /// 发送
        /// </summary>
        /// <returns></returns>
        public override JsExecuteResult ClickSendEmail() {
            _jsExecuteHelp.InJectGetLocationJs();
            JsExecuteResult jsExecuteResult = _jsExecuteHelp.GetDomAbsLocationByClass(SendAndConfirmButtonStr);
            if (jsExecuteResult.Success)
            {
                DomAbsLocation domAbsLocation = JsonConvert.DeserializeObject<DomAbsLocation>(jsExecuteResult.Result);
                CefWebBrowser.CefMouseClick(domAbsLocation.top + 5, domAbsLocation.left + 5);
            }
            Thread.Sleep(2000);
            if (CefWebBrowser.IsExistenceText(TipsBoxClassStr2)) {
                jsExecuteResult = _jsExecuteHelp.GetDomAbsLocationByClass(SendAndConfirmButtonStr, 1);
                if (jsExecuteResult.Success)
                {
                    DomAbsLocation domAbsLocation = JsonConvert.DeserializeObject<DomAbsLocation>(jsExecuteResult.Result);
                    CefWebBrowser.CefMouseClick(domAbsLocation.top + 5, domAbsLocation.left + 5);
                    Thread.Sleep(2*1000);
                }
            }
            return jsExecuteResult;
        }
        /// <summary>
        /// 关闭新邮件
        /// </summary>
        /// <returns></returns>
        private JsExecuteResult ClickCloseLabel() {
            //所有新邮件标签
            string emailTagsClose = "ico_root_frametab_error e_i e_i_fs16 ico_error";
            //全部标签allTags
            string allTagsClose = "e_i e_i_b24 root_frametab_close_i ico_close e_i_b_hover";
            //取消草稿
            string cancel = "e_b e_unselectable e_inline_block e_b_middle e_b_least";
            string jsString = $"document.getElementsByClassName('{emailTagsClose}').length";
            JsExecuteResult jsExecuteResult = null;
            try {
                //取消离线未保存数据的提示框
                if (CefWebBrowser.IsExistenceText(TipsBoxClassStr2)) {
                    _jsExecuteHelp.ExecuteClickByClass(cancel);
                }
                //关闭新邮件
                jsExecuteResult = CefWebBrowser.ExecuteJs(jsString);
                int len1 = int.Parse(jsExecuteResult.Result);
                jsString = $"document.getElementsByClassName('{allTagsClose}').length";
                jsExecuteResult = CefWebBrowser.ExecuteJs(jsString);
                int len2 = int.Parse(jsExecuteResult.Result);
                int len = len2 - 1;
                for (int i = len2 - len1; i < len2; i++) {
                    jsString = $"document.getElementsByClassName('{allTagsClose}')[{i}].click()";
                    jsExecuteResult = CefWebBrowser.ExecuteJs(jsString);
                  
                }
                for (int i = len2 - len1; i < len2; i++) {
                    if (CefWebBrowser.IsExistenceText(TipsBoxClassStr2)) {
                        jsExecuteResult = _jsExecuteHelp.ExecuteClickByClass(cancel, len1--);
                        Thread.Sleep(1000);
                    }
                }
                return jsExecuteResult;
            } catch {
                jsExecuteResult.Success = false;
                return jsExecuteResult;
            }

        }

        public override LoginState IsLogin(string account, string password, int recursionCount = 1) {
            LoginState loginState = new LoginState();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            string loginClassStr = "login_pannel_bg";
            string loginsuccessIdStr = "root_content_wrap e_abs_bottom_full";
            string loginUsernameIdStr = "username";
            string loginPasswordIdStr = "password";
            string loginButtonId = "login_submit_btn";
            string loginErroClassStr = "text_middle login_error_text linebr";
            string loginCodeIdStr = "check_code_wrap";
            //是否为登录页面
            bool isLoginPage = false;
            IFrame frame = null;
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

            List<long> iflist = CefWebBrowser.GetFrameIdentifiersList();
            foreach (var item in iflist) {
                IFrame f = CefWebBrowser._ChromiumWebBrowser.GetBrowser().GetFrame(item);
                if (f.Url.Contains("login.htm?app_code=smartmail")) {
                    frame = f;
                }
            }
            JsExecuteResult jsExecuteResult = null;
            string jsString = $"document.getElementById('{loginUsernameIdStr}').value='';" +
                              $"document.getElementById('{loginUsernameIdStr}').value='{account}';";
            CefWebBrowser.ExecuteJs(jsString, frame);
            Thread.Sleep(1000);
            jsString = $"document.getElementById('{loginPasswordIdStr}').value='';" +
                       $"document.getElementById('{loginPasswordIdStr}').value='{password}';";
            CefWebBrowser.ExecuteJs(jsString, frame);
            Thread.Sleep(1000);
            //点击登录
            string jsBtString = $"document.getElementById('{loginButtonId}').click();";
            CefWebBrowser.ExecuteJs(jsBtString, frame);
            Thread.Sleep(2 * 1000);
            stopwatch.Restart();
            LoginOutTime = 10 * 1000;
            while (true) {
                #region
                //再次获取iframe，避免成功登录使用之前的iframe发生错误
                frame = null;
                List<long> iflist1 = CefWebBrowser.GetFrameIdentifiersList();
                foreach (var item in iflist1) {
                    IFrame f1 = CefWebBrowser._ChromiumWebBrowser.GetBrowser().GetFrame(item);
                    if (f1 != null) {
                        if (f1.Url.Contains("login.htm?app_code=smartmail")) {
                            frame = f1;
                        }
                    }
                }
                if (frame != null) {
                    jsString = $"document.getElementsByClassName('{loginErroClassStr}')[0].innerText";
                    jsExecuteResult = CefWebBrowser.ExecuteJs(jsString, frame);
                    if (jsExecuteResult.Success) {
                        jsString = $"document.getElementById('{loginCodeIdStr}').style.display";
                        jsExecuteResult = CefWebBrowser.ExecuteJs(jsString, frame);
                        //需要验证码
                        if (jsExecuteResult.Result == "block" || jsExecuteResult.Result == "") {
                            loginState.IsVCode = true;
                            return loginState;
                        }
                        //账号密码错误
                        if (jsExecuteResult.Result == "none") {
                            loginState.IsLoginSuccess = false;
                            return loginState;
                        }
                    }
                }
                #endregion
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



        public override void SetCookie(object cookie) {

        }

        public override object GetCookie() {
            return "";
        }
        public override bool IsNeedVerificationCode() {
            return false;
        }

        /// <summary>
        /// 机制：
        /// <para>获得焦点->执行脚本->回车</para>
        /// </summary>
        /// <param name="emailStrList"></param>
        /// <param name="errorEmailList"></param>
        /// <returns></returns>
        public override JsExecuteResult ClickCCList(List<string> emailStrList, out List<string> errorEmailList) {
            errorEmailList = null;
            //空的
            if (emailStrList == null)
            {
                JsExecuteResult jsExecuteResult0 = new JsExecuteResult();
                jsExecuteResult0.Success = true;
                return jsExecuteResult0;
            }
            JsExecuteResult jsExecuteResult = new JsExecuteResult();
            //阿里邮箱分步执行
            //document.getElementsByClassName("e_entry_pos_holder e_entry_input_wrap e_fs0")[1].getElementsByClassName("e_entry_input")[0].value="cacode@qq.com"
            foreach (string item in emailStrList) {
                string foc = "document.getElementsByClassName('e_entry_pos_holder e_entry_input_wrap e_fs0')[1].getElementsByClassName('e_entry_input')[0].focus()";
                jsExecuteResult= CefWebBrowser.ExecuteJs(foc);
                Thread.Sleep(1000);
                //string val = $"document.getElementsByClassName('e_entry_pos_holder e_entry_input_wrap e_fs0')[1].getElementsByClassName('e_entry_input')[0].value='{item};'";
                CefWebBrowser.CefKeyBoardInput(item);
                Thread.Sleep(1000);
                CefWebBrowser.CefKeyBoardInput();
            }
            return jsExecuteResult;
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
            string text = "document.getElementsByClassName('e_unselectable compose_rcp_toggle_right e_text_link_b')[1].innerText";
            JsExecuteResult jsExecuteResult = new JsExecuteResult();
            if (!CefWebBrowser.ExecuteJs(text).Result.Contains("删除")) {
                string click = "e_unselectable compose_rcp_toggle_right e_text_link_b";//1                                                             // CefWebBrowser.ExecuteJs(click);
              jsExecuteResult = _jsExecuteHelp.GetDomAbsLocationByClass(click, 1);
                if (jsExecuteResult.Success)
                {
                    DomAbsLocation domAbsLocation = JsonConvert.DeserializeObject<DomAbsLocation>(jsExecuteResult.Result);
                    CefWebBrowser.CefMouseClick(domAbsLocation.top + 5, domAbsLocation.left + 5);
                    Thread.Sleep(500);
                }
            }
            foreach (string item in emailStrList) {
                string foc = "document.getElementsByClassName('e_entry_pos_holder e_entry_input_wrap e_fs0')[2].getElementsByClassName('e_entry_input')[0].focus()";
                CefWebBrowser.ExecuteJs(foc);
                Thread.Sleep(1000);
                // string val = $"document.getElementsByClassName('e_entry_pos_holder e_entry_input_wrap e_fs0')[2].getElementsByClassName('e_entry_input')[0].value='{item};'";
                // CefWebBrowser.ExecuteJs(val);
                CefWebBrowser.CefKeyBoardInput(item);
                Thread.Sleep(1000);
                CefWebBrowser.CefKeyBoardInput();
            }
            return jsExecuteResult;
        }
    }
}
