using CefSharp;
using CefSharpWebBrowser.CefHelper;
using CefSharpWebBrowser.CefModel;
using CefWebBrowser;
using CefWebBrowser.CefModel;
using Newtonsoft.Json;
using RobotForeignWorkflow.Model.SocialPlatFormModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using TradeWolfRobot.EmailAutomation.Model;

namespace RobotForeignWorkflow.AutoLogin
{
    /// <summary>
    /// 注入email登录点击事件
    /// </summary>
    public class LoginHandler
    {
        /// <summary>
        /// 是否结束任务
        /// </summary>
        public bool _stopTask = false;
        /// <summary>
        /// 是否跳出循环
        /// </summary>
        public bool isBreakCycle = false;

        public CefWebBrowserControl CefWebBrowser { get; set; }
        private JsExecuteHelp _jsExecuteHelp = null;
        SocialType socialType = null;

        /// <summary>
        /// 是否跳出
        /// </summary>
        private bool _isBreak { get { return _stopTask || isBreakCycle; } }

        public LoginHandler(CefWebBrowserControl CefWebBrowser)
        {
            this.CefWebBrowser = CefWebBrowser;
            _jsExecuteHelp = new JsExecuteHelp(CefWebBrowser);
            socialType = new SocialType();
        }

        /// <summary>
        /// js注入
        /// </summary>
        /// <param name="socailName"></param>
        public void injectionJs(string socailName)
        {
           
           if (socailName == socialType.Netease163EnterpriseEmail)
            {
                NeteaseEnterpriseInjectionClick();
            }
            else if (socailName == socialType.NetEase163Email || socailName == socialType.NetEaseYeahEmail || socailName == socialType.NetEase126Email)
            {
                NeteaseInjectionClick(socailName);
            }
            else if (socailName == socialType.QqEmail)
            {
                QqEamilInjectionClick();
            }
            else if (socailName == socialType.TencentEnterpriseEmail)
            {
                TencentInjectionClick();
            }
            else if (socailName == socialType.G_email)
            {
                G_EmailInjectionClick();
            }
            else if (socailName == socialType.AlicloudEnterpriseEmail)
            {
                AlicloudEnterpriseEmailInjectionClick();
            }
        }

        /// <summary>
        /// 阿里云登录注入事件
        /// </summary>
        public void AlicloudEnterpriseEmailInjectionClick()
        {
            string loginClassStr = "login_pannel_bg";
            IFrame frame = GetIFramByUrl(CefWebBrowser, "login.htm?app_code=smartmail");
            if (frame == null)
            {
                return;
            }
            string jsString = @"document.getElementById('login_submit_btn').onclick=function(){
                                var user =document.getElementById('username').value;
                                var password=document.getElementById('password').value;
                                aa.alicloudEnterpriseEmailLogin(user,password);
                                }";
            JsExecuteResult jsExecuteResult1 = CefWebBrowser.ExecuteJs(jsString, frame);
            while (!jsExecuteResult1.Success)
            {
                if (_isBreak)
                {
                    return;
                }
                string html = CefWebBrowser.GetHtmlText();
                if (html.Contains(loginClassStr))
                {
                    jsExecuteResult1 = CefWebBrowser.ExecuteJs(jsString, frame);
                }
                Thread.Sleep(500);
            }
        }

        /// <summary>
        /// 获取Iframe 163，yeah，126
        /// </summary>
        /// <returns></returns>
        private IFrame GetFrame(DateTime? starttime = null)
        {
            if (starttime == null)
            {
                starttime = DateTime.Now;
            }
            //大于两分钟跳出
            if ((DateTime.Now - (DateTime)starttime).TotalMinutes >= 2)
            {
                return null;
            }
            if (_isBreak)
            {
                return null;
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
                if (f.Url.Contains("passport.126.com"))
                {
                    frame = f;
                }
                if (f.Url.Contains("passport-v6.yeah.net"))
                {
                    frame = f;
                }
            }
            if (frame == null)
            {
                Thread.Sleep(500);
                frame = GetFrame(starttime);
            }
            return frame;
        }

        /// <summary>
        /// 网易126，163，yeah邮箱注入登录点击事件
        /// </summary>
        private void NeteaseInjectionClick(string socailName)
        {
            IFrame frame = GetFrame();
            if (frame == null)
            {
                return;
            }
            string loginClickStr = "dologin";
            string accountInputStr = "j-inputtext dlemail j-nameforslide";
            string passwordInputStr = "j-inputtext dlpwd";
            string iframEntry = "cnt-box-parent";
            string jsStr = @"document.getElementById('" + loginClickStr + "').onclick=function(){" +
                           @"var user=document.getElementsByClassName('j-inputtext dlemail j-nameforslide')[0].value;
                           var password=document.getElementsByClassName('j-inputtext dlpwd')[0].value;
                           aa.getneteaselogin('" + socailName + "',user,password);}";
            string jsStrfunction = @"function(e){
                                     if(e.keyCode ==13){
                                     var user=document.getElementsByClassName('j-inputtext dlemail j-nameforslide')[0].value;
                                     var password=document.getElementsByClassName('j-inputtext dlpwd')[0].value;
                                     aa.getneteaselogin('" + socailName + "',user,password);}}";
            string jsuserStr = $"document.getElementsByClassName('{accountInputStr}')[0].onkeydown ={jsStrfunction}";
            string jsPasswordStr = $"document.getElementsByClassName('{passwordInputStr}')[0].onkeydown ={jsStrfunction}";
            string jsLoginStr = $"document.getElementById('{iframEntry}').onkeydown ={jsStrfunction}";
            JsExecuteResult jsExecuteResult1 = CefWebBrowser.ExecuteJs(jsStr, frame);
            Thread.Sleep(500);
            JsExecuteResult jsExecuteResult2 = CefWebBrowser.ExecuteJs(jsuserStr, frame);
            JsExecuteResult jsExecuteResult3 = CefWebBrowser.ExecuteJs(jsPasswordStr, frame);
            JsExecuteResult jsExecuteResult4 = CefWebBrowser.ExecuteJs(jsLoginStr, frame);
            while (!(jsExecuteResult1.Success && jsExecuteResult2.Success && jsExecuteResult3.Success && jsExecuteResult4.Success))
            {
                if (_isBreak)
                {
                    return;
                }
                string html = CefWebBrowser.GetHtmlText();
                if (!jsExecuteResult1.Success && html.Contains(loginClickStr))
                {
                    jsExecuteResult1 = CefWebBrowser.ExecuteJs(jsStr, frame);
                }
                if (!jsExecuteResult2.Success && html.Contains(accountInputStr))
                {
                    jsExecuteResult2 = CefWebBrowser.ExecuteJs(jsuserStr, frame);
                }
                if (!jsExecuteResult3.Success && html.Contains(passwordInputStr))
                {
                    jsExecuteResult3 = CefWebBrowser.ExecuteJs(jsPasswordStr, frame);
                }
                if (!jsExecuteResult4.Success && html.Contains(iframEntry))
                {
                    jsExecuteResult4 = CefWebBrowser.ExecuteJs(jsLoginStr, frame);
                }
                Thread.Sleep(500);
            }
        }

        /// <summary>
        /// 网易企业邮箱注入登录点击事件
        /// </summary>
        private void NeteaseEnterpriseInjectionClick()
        {
            string loginClick = "w-button w-button-account js-loginbtn";
            string jsString = @"document.getElementsByClassName('" + loginClick + "')[0].onclick=function(){" +
                              @"var user=  document.getElementsByClassName('ipt js-value js-username')[0].value;
                              var password=  document.getElementsByClassName('ipt js-value js-pwd')[0].value;
                              aa.getneteaseenterpriselogin(user,password);
                              }";
            JsExecuteResult jsExecuteResult1 = CefWebBrowser.ExecuteJs(jsString);
            while (!jsExecuteResult1.Success)
            {
                if (_isBreak)
                {
                    return;
                }
                if (CefWebBrowser.GetHtmlText().Contains(loginClick))
                    jsExecuteResult1 = CefWebBrowser.ExecuteJs(jsString);
                Thread.Sleep(500);
            }
        }
        private IFrame GetIFramByName(CefWebBrowserControl cefWebBrowserControl, string ifname)
        {
            IFrame frame = CefWebBrowser.GetFrame(ifname);
            while (frame==null)
            {
                if (_isBreak)
                {
                    return null;
                }
                if (frame == null)
                {
                    frame = CefWebBrowser.GetFrame(ifname);
                }
                Thread.Sleep(500);
            }
            return frame;
        }

        private IFrame GetIFramByUrl(CefWebBrowserControl cefWebBrowserControl, string url)
        {

           
            IFrame frame = null;

            while (true)
            {
                if (_isBreak)
                {
                    return null;
                }
                if (frame == null)
                {
                    List<long> iflist = CefWebBrowser.GetFrameIdentifiersList();
                    foreach (var item in iflist)
                    {
                        if (_isBreak)
                        {
                            return null;
                        }
                        IFrame f = CefWebBrowser._ChromiumWebBrowser.GetBrowser().GetFrame(item);
                        if (f != null)
                        {
                            if (f.Url.Contains(url))
                            {
                                frame = f;
                                return frame;
                            }
                        }
                    }
                }
                Thread.Sleep(500);
            }
        }


        /// <summary>
        /// QQ邮箱注入登录点击事件
        /// </summary>
        private void QqEamilInjectionClick()
        {
            string jsframe = "login_frame";
            IFrame frame = GetIFramByName(CefWebBrowser, jsframe);
            if (frame == null)
            {
                return;
            }
            string loginStr = "\"login_button\"";
            string jsString = @"document.getElementsByClassName('btn')[0].onclick=function(){
                                      var user=document.getElementsByClassName('inputstyle')[0].value;
                                      var password=document.getElementsByClassName('inputstyle')[1].value;
                                      aa.register(user,password);}";
            JsExecuteResult jsExecuteResult1 = CefWebBrowser.ExecuteJs(jsString, frame);
            while (!jsExecuteResult1.Success)
            {
                if (_isBreak)
                {
                    return;
                }
                if (CefWebBrowser.GetHtmlText().Contains(loginStr))
                {
                    jsExecuteResult1 = CefWebBrowser.ExecuteJs(jsString, frame);
                }
                Thread.Sleep(500);
            }
        }

        /// <summary>
        /// 腾讯企业邮箱注入登录点击事件
        /// </summary>
        private void TencentInjectionClick()
        {
            string loginClickStr = "login_btn";
            string jsString = @"document.getElementsByClassName('" + loginClickStr + "')[1].onclick=function(){" +
                              @"var user =document.getElementsByClassName('login_scan_panel_pwd_input')[0].value;
                                var password=document.getElementsByClassName('login_scan_panel_pwd_input')[1].value;
                                aa.gettencentuser(user,password);
                                }";
            JsExecuteResult jsExecuteResult1 = CefWebBrowser.ExecuteJs(jsString);
            while (!jsExecuteResult1.Success)
            {
                if (_isBreak)
                {
                    return;
                }
                jsExecuteResult1 = CefWebBrowser.ExecuteJs(jsString);
                Thread.Sleep(500);
            }
        }
        /// <summary>
        /// G_Email注入登录点击事件
        /// </summary>
        private void G_EmailInjectionClick()
        {
            string loginClickStr = "VfPpkd-RLmnJb";
            string jsString = @"document.getElementsByClassName('" + loginClickStr + "')[0].onclick=function(){" +
                                @"var password=document.getElementsByClassName('whsOnd zHQkBf')[0].value;
                                var user=document.getElementsByClassName('KTeGk')[0].innerText;
                                aa.getg_emaillogin(user,password);
                                }";
            string jsStr = @"function(e){
                                if(e.keyCode ==13){
                                var password=document.getElementsByClassName('whsOnd zHQkBf')[0].value;
                                var user=document.getElementsByClassName('KTeGk')[0].innerText;
                                aa.getg_emaillogin(user,password);}}";
            string passwordInputStr = "KTeGk";
            string jsPasswordStr = $"document.getElementsByClassName('{passwordInputStr}')[0].onkeydown ={jsStr} ";
            JsExecuteResult jsExecuteResult1 = CefWebBrowser.ExecuteJs(jsString);
            JsExecuteResult jsExecuteResult2 = CefWebBrowser.ExecuteJs(jsPasswordStr);
            while (!(jsExecuteResult1.Success && jsExecuteResult2.Success))
            {
                if (_isBreak)
                {
                    return;
                }
                string html = CefWebBrowser.GetHtmlText();
                if (!jsExecuteResult1.Success && html.Contains(loginClickStr))
                {
                    jsExecuteResult1 = CefWebBrowser.ExecuteJs(jsString);
                }
                if (!jsExecuteResult2.Success && html.Contains(passwordInputStr))
                {
                    jsExecuteResult2 = CefWebBrowser.ExecuteJs(jsPasswordStr);
                }
                Thread.Sleep(500);
            }
        }

        public LoginState G_EmailLogin(string account, string password)
        {
            //地址栏
            string jsaddress = "window.document.location.href";
            LoginState loginState = new LoginState();
            if (CefWebBrowser.ExecuteJs(jsaddress).Result.Contains("mail.google.com"))
            {
                string jsUserAndPasswordClassStr = "whsOnd zHQkBf";
                string jsIsUserString = "document.getElementsByClassName('wLBAL')[0].innerText";
                JsExecuteResult jsExecuteResult2 = CefWebBrowser.ExecuteJs(jsIsUserString);
                if (jsExecuteResult2.Success)
                {
                    try
                    {
                        //存在用户账号
                        string jsString = "document.getElementsByClassName('BHzsHc')[0].click();";
                        CefWebBrowser.ExecuteJs(jsString);
                        Thread.Sleep(1000);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                //鼠标模拟点击用户账号
                _jsExecuteHelp.InJectGetLocationJs();
                JsExecuteResult jsExecuteResult = _jsExecuteHelp.GetDomAbsLocationByClass(jsUserAndPasswordClassStr);
                try
                {
                    DomAbsLocation domAbsLocation = JsonConvert.DeserializeObject<DomAbsLocation>(jsExecuteResult.Result);
                    CefWebBrowser.CefMouseClick(domAbsLocation.top + 5, domAbsLocation.left + 15);
                    Thread.Sleep(1000);
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
                    if (jsExecuteResult1.Success)
                    {
                        //用户账号错误
                        if (!(jsExecuteResult1.Result == ""))
                        {
                            loginState.IsLoginSuccess = false;
                            return loginState;

                        }
                    }
                    //鼠标模拟点击用户密码框
                    CefWebBrowser.CefMouseClick(domAbsLocation.top + 5, domAbsLocation.left + 15);
                    Thread.Sleep(1000);
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
                    if (jsExecuteResult1.Success)
                    {
                        //密码错误
                        if (!(jsExecuteResult1.Result == ""))
                        {
                            loginState.IsLoginSuccess = false;
                            return loginState;
                        }
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    loginState.IsLoginSuccess = false;
                    return loginState;
                }
                Stopwatch sw1 = new Stopwatch();
                sw1.Start();
                while (true)
                {
                    //是否登陆成功
                    Thread.Sleep(1000);
                    string address = CefWebBrowser.ExecuteJs(jsaddress).Result;
                    if (CefWebBrowser.ExecuteJs(jsaddress).Result.Contains("mail.google.com/mail/u/0"))
                    {
                        sw1.Stop();
                        loginState.IsLoginSuccess = true;
                        return loginState;
                    }
                    //超时
                    if (sw1.ElapsedMilliseconds >= 30 * 1000)
                    {
                        sw1.Stop();
                        loginState.IsLoginSuccess = false;
                        return loginState;
                    }
                    Thread.Sleep(50);
                }
            }
            else
            {
                loginState.IsLoginSuccess = false;
                return loginState;
            }
        }
        /// <summary>
        /// 网易126，163，yeah邮箱模拟登录
        /// </summary>
        public LoginState NeteaseLogin(string account, string password)
        {
            //地址栏
            string jsaddress = "window.document.location.href";
            JsExecuteResult jsExecuteResult1 = CefWebBrowser.ExecuteJs(jsaddress);
            LoginState loginState = new LoginState();
            //是否在登录界面
            if (jsExecuteResult1.Result.Contains("mail.163.com") || jsExecuteResult1.Result.Contains("mail.126.com") || jsExecuteResult1.Result.Contains("yeah.net"))
            {
                IFrame frame = GetFrame();
                //获取账号绝对位置
                string jsString = "document.getElementsByClassName('j-inputtext dlemail j-nameforslide')[0].value='';document.getElementsByClassName('j-inputtext dlemail j-nameforslide')[0].value='" + account + "'";
                string jsStr = "j-inputtext dlemail j-nameforslide";
                _jsExecuteHelp.InJectGetLocationJs();
                JsExecuteResult jsExecuteResult = _jsExecuteHelp.GetDomAbsLocationByClass(jsStr, null, frame);
                try
                {
                    DomAbsLocation domAbsLocation1 = JsonConvert.DeserializeObject<DomAbsLocation>(jsExecuteResult.Result);
                    jsStr = "loginDiv";
                    jsExecuteResult = _jsExecuteHelp.GetDomAbsLocationById(jsStr);
                    DomAbsLocation domAbsLocation = JsonConvert.DeserializeObject<DomAbsLocation>(jsExecuteResult.Result);
                    CefWebBrowser.CefMouseClick(domAbsLocation.top + domAbsLocation1.top + 5, domAbsLocation.left + domAbsLocation1.left + 15);
                    CefWebBrowser.ExecuteJs(jsString, frame);
                    Thread.Sleep(1000);
                    //获取密码绝对位置
                    jsStr = "j-inputtext dlpwd";
                    _jsExecuteHelp.InJectGetLocationJs();
                    jsExecuteResult = _jsExecuteHelp.GetDomAbsLocationByClass(jsStr, null, frame);
                    DomAbsLocation domAbsLocation2 = JsonConvert.DeserializeObject<DomAbsLocation>(jsExecuteResult.Result);
                    CefWebBrowser.CefMouseClick(domAbsLocation.top + domAbsLocation2.top + 5, domAbsLocation.left + domAbsLocation2.left + 15);
                    CefWebBrowser.CefKeyBoardInput(password);
                    Thread.Sleep(1000);
                    string jsGetAutomaticLogonClassStr = "document.getElementsByClassName('b-unlogn j-unlogn')[0].getElementsByTagName('span')[0].className";
                    JsExecuteResult jsExecuteResult2 = CefWebBrowser.ExecuteJs(jsGetAutomaticLogonClassStr, frame);
                    if (jsExecuteResult2.Success)
                    {
                        if (jsExecuteResult2.Result == "u-checkbox tabfocus")
                        {
                            string jsAutomaticLogonStr = "document.getElementById('un-login').click();";
                            CefWebBrowser.ExecuteJs(jsAutomaticLogonStr, frame);
                            Thread.Sleep(1000);
                        }
                    }
                    //点击登录
                    jsString = "document.getElementById('dologin').click();";
                    CefWebBrowser.ExecuteJs(jsString, frame);
                    Thread.Sleep(5 * 1000);
                    jsStr = "document.getElementsByClassName('yidun_tips__text yidun-fallback__tip')[0].innerText";//document.getElementsByClassName('yidun_tips')[0]
                    jsExecuteResult = CefWebBrowser.ExecuteJs(jsStr, frame);
                    if (jsExecuteResult.Result == "点此进行验证")
                    {
                        loginState.IsVCode = true;
                        return loginState;
                    }
                }

                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                //登录超时
                Stopwatch sw1 = new Stopwatch();
                sw1.Start();
                while (true)
                {

                    //登录成功
                    if (CefWebBrowser.ExecuteJs(jsaddress).Result.Contains("mail.126.com/js6/main.jsp"))
                    {
                        sw1.Stop();
                        loginState.IsLoginSuccess = true;
                        return loginState;
                    }
                    //账号或密码错误
                    jsStr = "document.getElementById('nerror').innerText;";
                    jsExecuteResult = CefWebBrowser.ExecuteJs(jsStr, frame);
                    if (jsExecuteResult.Success)
                    {
                        if (jsExecuteResult.Result.Contains("帐号或密码错误"))
                        {
                            sw1.Stop();
                            loginState.IsLoginSuccess = false;
                            return loginState;
                        }
                    }
                    //超时
                    if (sw1.ElapsedMilliseconds >= 30 * 1000)
                    {
                        sw1.Stop();
                        loginState.IsLoginSuccess = false;
                        return loginState;
                    }
                    Thread.Sleep(500);
                }
            }
            else
            {
                loginState.IsLoginSuccess = false;
                return loginState;
            }
        }
        /// <summary>
        /// 模拟QQ邮箱登录
        /// </summary>
        public LoginState QqLogin(string account, string password)
        {
            //地址栏
            string jsaddress = "window.document.location.href";
            JsExecuteResult jsExecuteResult0 = CefWebBrowser.ExecuteJs(jsaddress);
            LoginState loginState = new LoginState();
            //是否在登录界面
            if (jsExecuteResult0.Result.Contains("mail.qq.com"))
            {
                JsExecuteResult jsExecuteResult1 = new JsExecuteResult();
                string jsframe = "login_frame";
                IFrame frame = CefWebBrowser.GetFrame(jsframe);
                //弹出提示信息
                //获取账号绝对位置
                _jsExecuteHelp.InJectGetLocationJs();
                JsExecuteResult jsExecuteResult = _jsExecuteHelp.GetDomAbsLocationByClass("inputOuter", null, frame);
                try
                {
                    DomAbsLocation domAbsLocation1 = JsonConvert.DeserializeObject<DomAbsLocation>(jsExecuteResult.Result);
                    string jsStr = "qqLoginCard";
                    jsExecuteResult = _jsExecuteHelp.GetDomAbsLocationById(jsStr);
                    DomAbsLocation domAbsLocation = JsonConvert.DeserializeObject<DomAbsLocation>(jsExecuteResult.Result);
                    float a = domAbsLocation.top + domAbsLocation1.top;
                    float b = domAbsLocation.left + domAbsLocation1.left;
                    CefWebBrowser.CefMouseClick(domAbsLocation.top + domAbsLocation1.top + 5, domAbsLocation.left + domAbsLocation1.left + 15);
                    Thread.Sleep(1000);
                    string jsString = $"document.getElementsByClassName('inputstyle')[0].value='';document.getElementsByClassName('inputstyle')[0].value='{account}'";
                    CefWebBrowser.ExecuteJs(jsString, frame);
                    Thread.Sleep(1000);
                    //获取密码绝对位置
                    jsExecuteResult = _jsExecuteHelp.GetDomAbsLocationById("p", frame);
                    DomAbsLocation domAbsLocation2 = JsonConvert.DeserializeObject<DomAbsLocation>(jsExecuteResult.Result);
                    CefWebBrowser.CefMouseClick(domAbsLocation.top + domAbsLocation2.top + 5, domAbsLocation.left + domAbsLocation2.left + 15);
                    Thread.Sleep(1000);
                    jsString = $"document.getElementsByClassName('inputstyle password')[0].value='';document.getElementsByClassName('inputstyle password')[0].value='{password}';";
                    CefWebBrowser.ExecuteJs(jsString, frame);
                    Thread.Sleep(1000);
                    string jsGetAutomaticLogonClassStr = "document.getElementById('p_low_login_enable').className";
                    JsExecuteResult jsExecuteResult2 = CefWebBrowser.ExecuteJs(jsGetAutomaticLogonClassStr, frame);
                    if (jsExecuteResult2.Success)
                    {
                        if (jsExecuteResult2.Result == "uncheck")
                        {
                            string jsAutomaticLogonIdStr = "document.getElementById('p_low_login_enable').click();";
                            CefWebBrowser.ExecuteJs(jsAutomaticLogonIdStr, frame);
                            Thread.Sleep(1000);
                        }
                    }
                    //点击登录
                    jsString = "document.getElementById('login_button').click();";
                    CefWebBrowser.ExecuteJs(jsString, frame);
                    Thread.Sleep(1 * 1000);
                    string jsStringcode = "document.getElementsByClassName('err_m')[0].innerText";
                    IFrame frame1 = null;
                    jsExecuteResult1 = CefWebBrowser.ExecuteJs(jsStringcode, frame);
                    Thread.Sleep(5 * 1000);
                    //是否有验证界面的信息
                    List<long> iflist = CefWebBrowser.GetFrameIdentifiersList();
                    foreach (var item in iflist)
                    {
                        IFrame f = CefWebBrowser._ChromiumWebBrowser.GetBrowser().GetFrame(item);
                        if (f.Url.Contains("cap_union_new_show"))
                        {
                            frame1 = f;
                        }
                    }
                    jsString = "document.getElementsByClassName('tc-title middle-fontsize small-fontsize multi-line')[0].innerText";
                    jsExecuteResult = CefWebBrowser.ExecuteJs(jsString, frame1);
                    if (jsExecuteResult1.Success)
                    {
                        if (jsExecuteResult.Result != null || jsExecuteResult1.Result.Contains("验证码") || jsExecuteResult1.Result.Contains("verification code"))
                        {
                            loginState.IsVCode = true;
                            return loginState;
                        }
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                //登录超时
                Stopwatch sw1 = new Stopwatch();
                sw1.Start();
                while (true)
                {
                    if (CefWebBrowser.ExecuteJs(jsaddress).Result.Contains("mail.qq.com/cgi-bin/frame_html"))
                    {
                        sw1.Stop();
                        loginState.IsLoginSuccess = true;
                        return loginState;
                    }
                    //账号或者密码错误
                    if (jsExecuteResult1.Success)
                    {
                        if (jsExecuteResult1.Result == "你输入的帐号或密码不正确，请重新输入。")
                        {
                            sw1.Stop();
                            loginState.IsLoginSuccess = false;
                            return loginState;
                        }
                    }
                    if (sw1.ElapsedMilliseconds >= 30 * 1000)
                    {
                        sw1.Stop();
                        loginState.IsLoginSuccess = false;
                        return loginState;
                    }
                }
            }
            else
            {
                loginState.IsLoginSuccess = false;
                return loginState;
            }
        }
        /// <summary>
        /// 网易企业邮箱模拟登录
        /// </summary>
        public LoginState NeteaseEnterpriseLogin(string account, string password)
        {
            //地址栏
            string jsaddress = "window.document.location.href";
            JsExecuteResult jsExecuteResult0 = CefWebBrowser.ExecuteJs(jsaddress);
            LoginState loginState = new LoginState();
            //是否在登录界面
            if (jsExecuteResult0.Result.Contains("qiye.163.com"))
            {
                string jsLoginBlock = "document.getElementById('loginBlock').className";
                JsExecuteResult jsExecuteResult1 = CefWebBrowser.ExecuteJs(jsLoginBlock);
                if (jsExecuteResult1.Success)
                {
                    if (jsExecuteResult1.Result.Contains("show-qrocde-panel"))
                    {
                        string jsPasswordLogonStr = "document.getElementById('switchNormalCtrl').click();";
                        CefWebBrowser.ExecuteJs(jsPasswordLogonStr);
                        Thread.Sleep(1000);
                    }
                }
                JsExecuteResult jsExecuteResult = CefWebBrowser.ExecuteJs(jsaddress);
                string jsUserInputString = "ipt js-value js-username";
                //获取账号绝对位置
                _jsExecuteHelp.InJectGetLocationJs();
                jsExecuteResult = _jsExecuteHelp.GetDomAbsLocationByClass(jsUserInputString);
                try
                {
                    DomAbsLocation domAbsLocation = JsonConvert.DeserializeObject<DomAbsLocation>(jsExecuteResult.Result);
                    CefWebBrowser.CefMouseClick(domAbsLocation.top + 5, domAbsLocation.left + 15);
                    Thread.Sleep(1000);
                    string jsUserString = $"document.getElementsByClassName('{jsUserInputString}')[0].value='';document.getElementsByClassName('{jsUserInputString}')[0].value='{account}'";
                    CefWebBrowser.ExecuteJs(jsUserString);
                    Thread.Sleep(1000);
                    //获取密码绝对位置
                    string jsPasswordInputString = "ipt js-value js-pwd";
                    _jsExecuteHelp.InJectGetLocationJs();
                    jsExecuteResult = _jsExecuteHelp.GetDomAbsLocationByClass(jsPasswordInputString);
                    DomAbsLocation domAbsLocation1 = JsonConvert.DeserializeObject<DomAbsLocation>(jsExecuteResult.Result);
                    CefWebBrowser.CefMouseClick(domAbsLocation1.top + 5, domAbsLocation1.left + 15);
                    string jsPasswordString = $"document.getElementsByClassName('{jsPasswordInputString}')[0].value='';document.getElementsByClassName('{jsPasswordInputString}')[0].value='{password}'";
                    CefWebBrowser.ExecuteJs(jsPasswordString);
                    // CefWebBrowser.CefKeyBoardInput(userInformation[1]);
                    Thread.Sleep(1000);
                    //自动登录checkbox框
                    string jsAutomaticLogonStr = "document.getElementsByClassName('logincheck js-logincheck')[0].getElementsByTagName('span')[0].click();";
                    string jsGetAutomaticLogonClassStr = "document.getElementsByClassName('logincheck js-logincheck')[0].getElementsByTagName('span')[0].className";
                    JsExecuteResult jsExecuteResult2 = CefWebBrowser.ExecuteJs(jsGetAutomaticLogonClassStr);
                    if (jsExecuteResult2.Success)
                    {
                        if (!jsExecuteResult2.Result.Contains("icon-checkbox-checked"))
                        {

                            CefWebBrowser.ExecuteJs(jsAutomaticLogonStr);
                            Thread.Sleep(1000);
                        }
                    }
                    //点击登录
                    string jsLoginBtString = "document.getElementsByClassName('w-button w-button-account js-loginbtn')[0].click();";
                    CefWebBrowser.ExecuteJs(jsLoginBtString);
                    Thread.Sleep(2 * 1000);
                    //提示用户
                    string jsString = "document.getElementsByClassName('m-verifycode')[0].style['display']";
                    jsExecuteResult = CefWebBrowser.ExecuteJs(jsString);
                    if (jsExecuteResult.Result == "block")
                    {
                        loginState.IsVCode = true;
                        return loginState;
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                //登录超时
                Stopwatch sw1 = new Stopwatch();
                sw1.Start();
                while (true)
                {

                    if (CefWebBrowser.ExecuteJs(jsaddress).Result.Contains("mailhz.qiye.163.com/js6/main.jsp"))
                    {
                        sw1.Stop();
                        loginState.IsLoginSuccess = true;
                        return loginState;
                    }
                    string jsloginerrorClassStr = "document.getElementsByClassName('loginerror')[0].innerText";
                    jsExecuteResult = CefWebBrowser.ExecuteJs(jsloginerrorClassStr);
                    if (jsExecuteResult.Success)
                    {
                        if (jsExecuteResult.Result.Contains("邮箱帐号和密码不匹配") || jsExecuteResult.Result.Contains("Email account and password do not match"))
                        {
                            sw1.Stop();
                            loginState.IsLoginSuccess = false;
                            return loginState;

                        }
                    }
                    if (sw1.ElapsedMilliseconds >= 30 * 1000)
                    {
                        sw1.Stop();
                        loginState.IsLoginSuccess = false;
                        return loginState;
                    }

                }
            }
            else
            {
                loginState.IsLoginSuccess = false;
                return loginState;
            }
        }
        /// <summary>
        /// 腾讯企业邮箱模拟登录
        /// </summary>
        public LoginState TencentLogin(string account, string password)
        {
            //地址栏
            string jsaddress = "window.document.location.href";
            JsExecuteResult jsExecuteResult1 = CefWebBrowser.ExecuteJs(jsaddress);
            LoginState loginState = new LoginState();
            //是否在登录界面
            if (jsExecuteResult1.Result.Contains("exmail.qq.com"))
            {

                //点击用账号登录
                string jsAccountLoginStr = "document.getElementsByClassName('js_show_pwd_panel')[0].click();";
                CefWebBrowser.ExecuteJs(jsAccountLoginStr);
                string jsUserAndPasswordClasStr = "login_scan_panel_pwd_input";
                //获取账号绝对位置
                _jsExecuteHelp.InJectGetLocationJs();
                JsExecuteResult jsExecuteResult = _jsExecuteHelp.GetDomAbsLocationByClass(jsUserAndPasswordClasStr);
                try
                {
                    DomAbsLocation domAbsLocation = JsonConvert.DeserializeObject<DomAbsLocation>(jsExecuteResult.Result);
                    CefWebBrowser.CefMouseClick(domAbsLocation.top + 5, domAbsLocation.left + 15);
                    Thread.Sleep(1000);
                    string jsUserString = $"document.getElementsByClassName('{jsUserAndPasswordClasStr}')[0].value='';document.getElementsByClassName('{jsUserAndPasswordClasStr}')[0].value='{account}'";
                    CefWebBrowser.ExecuteJs(jsUserString);
                    Thread.Sleep(1000);
                    //获取密码绝对位置
                    _jsExecuteHelp.InJectGetLocationJs();
                    jsExecuteResult = _jsExecuteHelp.GetDomAbsLocationByClass(jsUserAndPasswordClasStr, 1);
                    DomAbsLocation domAbsLocation1 = JsonConvert.DeserializeObject<DomAbsLocation>(jsExecuteResult.Result);
                    CefWebBrowser.CefMouseClick(domAbsLocation1.top + 5, domAbsLocation1.left + 15);
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
                    //点击登录
                    string jsLoginBtString = "document.getElementsByClassName('login_btn')[1].click();";
                    CefWebBrowser.ExecuteJs(jsLoginBtString);
                    Thread.Sleep(10 * 1000);
                    string jsString = "document.getElementsByClassName('signup_item')[6].style['display']";
                    jsExecuteResult = CefWebBrowser.ExecuteJs(jsString);
                    if (jsExecuteResult.Result == "block")
                    {
                        loginState.IsVCode = true;
                        return loginState;
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                //登录超时
                Stopwatch sw1 = new Stopwatch();
                sw1.Start();
                while (true)
                {

                    if (CefWebBrowser.ExecuteJs(jsaddress).Result.Contains("exmail.qq.com/cgi-bin/frame_html"))
                    {
                        sw1.Stop();
                        loginState.IsLoginSuccess = true;
                        return loginState;
                    }
                    //账号或者密码错误
                    string jsloginerrorClassStr = "document.getElementsByClassName('signup_item_content_tips_error')[4].innerText";
                    jsExecuteResult = CefWebBrowser.ExecuteJs(jsloginerrorClassStr);
                    if (jsExecuteResult.Success)
                    {
                        if (jsExecuteResult.Result.Contains("你填写的帐号或密码不正确"))
                        {
                            sw1.Stop();
                            loginState.IsLoginSuccess = false;
                            return loginState;
                        }
                    }
                    if (sw1.ElapsedMilliseconds >= 30 * 1000)
                    {
                        sw1.Stop();
                        loginState.IsLoginSuccess = false;
                        return loginState;
                    }

                }
            }
            else
            {
                loginState.IsLoginSuccess = false;
                return loginState;
            }
        }
    }
}
