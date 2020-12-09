using CefSharp;
using CefSharp.Web;
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

namespace TradeWolfRobot.EmailAutomation.Email
{

    public class TencentEnterpriseEmail : EmailBase
    {
        JsExecuteHelp _jsExecuteHelp;
        public TencentEnterpriseEmail(CefWebBrowserControl CefWebBrowser) : base(CefWebBrowser)
        {
            EmailName = "exmail";
            _jsExecuteHelp = new JsExecuteHelp(CefWebBrowser);
        }
        public TencentEnterpriseEmail() : base()
        {

            if (CefWebBrowser == null)
            {
                CefWebBrowser = new CefWebBrowserControl("https://exmail.qq.com/login");
            }
            else
            {
                CefWebBrowser.LoadNewUrl("https://exmail.qq.com/login");
            }
            EmailName = "exmail";
            _jsExecuteHelp = new JsExecuteHelp(CefWebBrowser);
        }
        /// <summary>
        /// 点击发送
        /// </summary>
        /// <returns></returns>
        public override JsExecuteResult ClickSendEmail()
        {
            //点击发送按钮
            IFrame frame = CefWebBrowser.GetFrame("mainFrame");
            string jsString = "document.getElementsByClassName('compose_btn_text')[0].click();";
            JsExecuteResult jsExecuteResult = CefWebBrowser.ExecuteJs(jsString, frame);
            Thread.Sleep(1000);
            //错误信息或无主题点击确认
            if (CefWebBrowser.GetHtmlText().Contains("qm_dialog"))
            {
                jsString = "document.getElementsByClassName('qm_dialog')[0].getElementsByClassName('btn_blue btn_input')[0].click();";
                jsExecuteResult = CefWebBrowser.ExecuteJs(jsString);
                jsExecuteResult.Success = false;
                Thread.Sleep(300);
            }
            if (CefWebBrowser.GetHtmlText().Contains("QMconfirm_s"))
            {
                jsString = "document.getElementsByClassName('btn_blue btn_input')[0].click();";
                jsExecuteResult = CefWebBrowser.ExecuteJs(jsString);

            }
            return jsExecuteResult;
        }
        /// <summary>
        /// 点击写信
        /// </summary>
        /// <returns></returns>
        public override JsExecuteResult ClickWriteEmail()
        {
            JsExecuteResult jsExecuteResult = new JsExecuteResult();
            string writeEmailButtonId = "composebtn";
            DomNameOrId = writeEmailButtonId;
            jsExecuteResult = _jsExecuteHelp.GetDomAbsLocationById(DomNameOrId);
            if (!LogJsExecuteStatus(CefWebBrowser.Address, jsExecuteResult))
            {
                return jsExecuteResult;
            }
            try
            {
                DomAbsLocation domAbsLocation = JsonConvert.DeserializeObject<DomAbsLocation>(jsExecuteResult.Result);
                //点击写信按键
                CefWebBrowser.CefMouseClick(domAbsLocation.top + 5, domAbsLocation.left + 5);
                Thread.Sleep(1000);
                string cancel = "qm_dialog nofoot";//取消草稿
                if (CefWebBrowser.IsExistenceText(cancel))
                {
                    DomNameOrId = "document.getElementsByClassName('qm_dialog nofoot ')[0].getElementsByClassName('btn_gray btn_input')[2].click();";//点击取消草稿
                    jsExecuteResult = CefWebBrowser.ExecuteJs(DomNameOrId);
                    Thread.Sleep(500);
                }
                return jsExecuteResult;
            }
            catch
            {
                jsExecuteResult.Success = false;
                return jsExecuteResult;
            }

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
            string loginClassStr = "loginWrap";
            string loginsuccessIdStr = "navBarDiv";
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
                if (CefWebBrowser.GetHtmlText().Contains(loginsuccessIdStr))
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

            //页面加载失败 重新加载
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
            //点击用账号登录
            string jsAccountLoginStr = "document.getElementsByClassName('js_show_pwd_panel')[0].click();";
            CefWebBrowser.ExecuteJs(jsAccountLoginStr);
            string jsUserAndPasswordClasStr = "login_scan_panel_pwd_input";
            JsExecuteResult jsExecuteResult = _jsExecuteHelp.GetDomAbsLocationByClass(jsUserAndPasswordClasStr);
            try
            {
                string jsUserString = $"document.getElementsByClassName('{jsUserAndPasswordClasStr}')[0].value='';document.getElementsByClassName('{jsUserAndPasswordClasStr}')[0].value='{account}'";
                CefWebBrowser.ExecuteJs(jsUserString);
                Thread.Sleep(1000);
                string jsPasswordString = $"document.getElementsByClassName('{jsUserAndPasswordClasStr}')[1].value='';document.getElementsByClassName('{jsUserAndPasswordClasStr}')[1].value='{password}'";
                CefWebBrowser.ExecuteJs(jsPasswordString);
                Thread.Sleep(1000);
                //自动登录checkbox框
                string jsCheckboxIdStr = "auto_login_in_five_days_pwd";
                string jsGetAutomaticLogonClassStr = $"document.getElementById('{jsCheckboxIdStr}').checked";
                string jsAutomaticLogonStr = $"document.getElementById('{jsCheckboxIdStr}').click();";
                JsExecuteResult jsExecuteResult2 = CefWebBrowser.ExecuteJs(jsGetAutomaticLogonClassStr);
                if (jsExecuteResult2.Success)
                {
                    if (jsExecuteResult2.Result.Contains("False"))
                    {

                        CefWebBrowser.ExecuteJs(jsAutomaticLogonStr);
                        Thread.Sleep(1000);
                    }
                }
                Thread.Sleep(1000);
                //点击登录
                string jsLoginBtString = "document.getElementsByClassName('login_btn')[1].click();";
                CefWebBrowser.ExecuteJs(jsLoginBtString);                               
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


            stopwatch.Restart();
            LoginOutTime = 10 * 1000;
            while (true)
            {
                if (_StopTask)
                {
                    return loginState;
                }
                //登录成功
                if (CefWebBrowser.GetHtmlText().Contains(loginsuccessIdStr))
                {
                    stopwatch.Stop();
                    loginState.IsLoginSuccess = true;
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
        }

        public override bool IsNeedVerificationCode()
        {
            return false;
        }

        public override void SetCookie(object cookie)
        {
          
        }
        /// <summary>
        /// 写入收件人
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <returns></returns>
        public override JsExecuteResult WriteEmailAddress(string emailAddress)
        {
            string addressBoxClass = "toAreaCtrl";
            Thread.Sleep(300);
            DomNameOrId = addressBoxClass;
            IFrame frame = CefWebBrowser.GetFrame("mainFrame");
            //获取收件人的绝对位置
            JsExecuteResult jsExecuteResult = _jsExecuteHelp.GetDomAbsLocationById(DomNameOrId, frame);
            if (!LogJsExecuteStatus(CefWebBrowser.Address, jsExecuteResult))
            {
                return jsExecuteResult;
            }
            try
            {
                string iframeId = "mainFrame";
                DomNameOrId = iframeId;
                DomAbsLocation iframeAbsLocation = JsonConvert.DeserializeObject<DomAbsLocation>(_jsExecuteHelp.GetDomAbsLocationById(DomNameOrId).Result);
                DomAbsLocation domAbsLocation = JsonConvert.DeserializeObject<DomAbsLocation>(jsExecuteResult.Result);
                //点击收件人文本框
                CefWebBrowser.CefMouseClick(domAbsLocation.top + iframeAbsLocation.top + 3, domAbsLocation.left + iframeAbsLocation.left + 10);
                if (!LogJsExecuteStatus(CefWebBrowser.Address, jsExecuteResult))
                {
                    return jsExecuteResult;
                }
                Thread.Sleep(500);
                //对收件人文本框输入信息
                CefWebBrowser.CefKeyBoardInput(emailAddress);
                Thread.Sleep(500);
                //再次点击收件人文本框
                CefWebBrowser.CefMouseClick(domAbsLocation.top + iframeAbsLocation.top + 3, domAbsLocation.left + iframeAbsLocation.left + 40);
                return jsExecuteResult;
            }
            catch (Exception ex)
            {
                jsExecuteResult.Success = false;
                return jsExecuteResult;
            }
        }
        /// <summary>
        /// 写入正文
        /// </summary>
        /// <param name="emailBody"></param>
        /// <returns></returns>
        public override JsExecuteResult WriteEmailBody(string emailBody)
        {
            //进入源码
            IFrame frame = CefWebBrowser.GetFrame("mainFrame");
            string jsString = "document.getElementsByClassName('QMEditorToolBarItem')[16].click()";
            JsExecuteResult jsExecuteResult = CefWebBrowser.ExecuteJs(jsString, frame);
            //写入正文信息
            jsString = $"document.getElementsByClassName('QMEditorText')[0].value=`{emailBody}`";
            jsExecuteResult = CefWebBrowser.ExecuteJs(jsString, frame);
            //退出源码
            jsString = "document.getElementsByClassName('QMEditorBtnA QMEditorFormatBtn')[0].click();";
            jsExecuteResult = CefWebBrowser.ExecuteJs(jsString, frame);
            return jsExecuteResult;


        }
        /// <summary>
        /// 写入主题
        /// </summary>
        /// <param name="emailTheme"></param>
        /// <returns></returns>
        public override JsExecuteResult WriteEmailTheme(string emailTheme)
        {


            string jsString = "subject";
            DomNameOrId = jsString;
            Thread.Sleep(300);
            IFrame frame = CefWebBrowser.GetFrame("mainFrame");
            //获取主题文本框的绝对位置
            JsExecuteResult jsExecuteResult = _jsExecuteHelp.GetDomAbsLocationById(DomNameOrId, frame);
            try
            {

                string iframeId = "mainFrameContainer";
                DomNameOrId = iframeId;
                DomAbsLocation iframeAbsLocation = JsonConvert.DeserializeObject<DomAbsLocation>(_jsExecuteHelp.GetDomAbsLocationById(DomNameOrId).Result);
                DomAbsLocation domAbsLocation = JsonConvert.DeserializeObject<DomAbsLocation>(jsExecuteResult.Result);
                //点击主题文本框
                CefWebBrowser.CefMouseClick(domAbsLocation.top + iframeAbsLocation.top + 3, domAbsLocation.left + iframeAbsLocation.left + 15);
                Thread.Sleep(300);
                if (!LogJsExecuteStatus(CefWebBrowser.Address, jsExecuteResult))
                {
                    return jsExecuteResult;
                }
                Thread.Sleep(500);
                //对主题文本框输入信息
                CefWebBrowser.CefKeyBoardInput(emailTheme);
                return jsExecuteResult;

            }
            catch
            {
                jsExecuteResult.Success = false;
                return jsExecuteResult;
            }
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
