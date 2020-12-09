using CefSharp;
using CefWebBrowser;
using CefWebBrowser.CefHelper;
using CefWebBrowser.CefModel;
using Newtonsoft.Json;
using RobotForeignWorkflow;
using RobotForeignWorkflow.AutoLogin;
using RobotForeignWorkflow.Data;
using RobotForeignWorkflow.Model.FrontModel;
using RobotForeignWorkflow.Model.SocialPlatFormModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TradeWolfRobot.EmailAutomation.Model.EmailModel;
using TradeWolfRobot.EmailAutomation.View;
namespace TradeWolfRobot.EmailAutomation.Helper
{
   public  class EmailManage
    {
        public RobotForeignWorkflow.AutoLogin.LoginHandler _loginHandler = null;
        public Dictionary<string, EmailAccountData> SocialUrl;
        RobotForeignDal _robotForeignDal;
        EmailMainFrm _socialLoginFrm;
        SocialType socialType = null;
        int _sendLimit = 200;
        /// <summary>
        /// 登录
        /// </summary>
        public CefWebBrowserControl LoginBrowser;
        /// <summary>
        /// 是否停止加载
        /// </summary>
       public bool IsStopLoad;
        public EmailManage()
        {
           
            _loginHandler = new RobotForeignWorkflow.AutoLogin.LoginHandler(null);
            _socialLoginFrm = GlobalClass.FormDic["EmailLoginFrm"] as EmailMainFrm;
            _robotForeignDal = new RobotForeignDal();
            // SocialMainBrowser = _socialLoginFrm.MainWebBrowserControl;
            EmailAccountData socialAccountData = new EmailAccountData();
            socialType = new SocialType();
            SocialUrl = new Dictionary<string, EmailAccountData>
            {
                { socialType.Netease163EnterpriseEmail, SetSocialDefaultData("https://qiye.163.com/login", "正在检测163企业邮箱账号的绑定状态,登录时请勾选自动登录", "163企业邮箱账号绑定成功", "https://qiye.163.com") },
                { socialType.NetEase163Email, SetSocialDefaultData("https://mail.163.com", "正在检测163邮箱账号的绑定状态,登录时请勾选自动登录", "163邮箱账号绑定成功", "https://mail.163.com") },
                { socialType.NetEase126Email, SetSocialDefaultData("https://mail.126.com", "正在检测126邮箱账号的绑定状态,登录时请勾选自动登录", "126邮箱账号绑定成功", "https://mail.126.com") },
                { socialType.NetEaseYeahEmail, SetSocialDefaultData("https://www.yeah.net", "正在检测Yeah邮箱账号的绑定状态,登录时请勾选自动登录", "Yeah邮箱账号绑定成功", "https://www.yeah.net") },
                { socialType.QqEmail, SetSocialDefaultData("https://mail.qq.com", "正在检测qq邮箱账号的绑定状态,登录时请勾选自动登录", "qq邮箱账号绑定成功", "https://mail.qq.com") },
                { socialType.TencentEnterpriseEmail, SetSocialDefaultData("https://exmail.qq.com/login", "正在检测腾讯企业邮箱账号的绑定状态,登录时请勾选自动登录", "腾讯企业邮箱账号绑定成功", "https://exmail.qq.com") },
                { socialType.G_email, SetSocialDefaultData("https://accounts.google.com/signin/v2/identifier?flowName=GlifWebSignIn&flowEntry=ServiceLogin", "正在检测Gmail邮箱账号的绑定状态", "Gmail邮箱账号绑定成功", "https://accounts.google.com") },
                { socialType.AlicloudEnterpriseEmail, SetSocialDefaultData("http://mail.hichina.com/", "正在检测阿里企业邮箱账号的绑定状态", "阿里企业邮箱账号绑定成功", "http://mail.hichina.com") }
            };
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="loginMsg"></param>
        /// <param name="loginSuccessMsg"></param>
        /// <param name="deleteCookieUrl"></param>
        /// <returns></returns>
        private EmailAccountData SetSocialDefaultData(string url, string loginMsg, string loginSuccessMsg, string deleteCookieUrl)
        {
            EmailAccountData socialAccountData = new EmailAccountData()
            {
                LoginSuccessMsg = loginSuccessMsg,
                LoginMsg = loginMsg,
                Url = url,
                DeleteCookieUrl = deleteCookieUrl,
            };
            return socialAccountData;
        }
        /// <summary>
        /// 加载登录页面
        /// </summary>
        /// <param name="name"></param>
        /// <param name="cb"></param>
        public void socialDisplay(string name,string cachePath)
        {
            _loginHandler.isBreakCycle = true;
            DateTime starttime = DateTime.Now;
            IsStopLoad = true;
            Thread.Sleep(1000);
            _loginHandler.isBreakCycle = false;
            _socialLoginFrm.EmailBrowser.LoadNewUrl("");
            IsStopLoad = false;
            Thread t = new Thread(new ThreadStart(() =>
            {
                if (IsStopLoad)
                {
                    return; 
                }
                string msg = SocialUrl[name].LoginMsg;
                string url = SocialUrl[name].Url;
                LoginBrowser = _socialLoginFrm.EmailBrowser;
                LoginBrowser.LoadNewUrl(url);
                FrontEndCode frontEndCode = IsLoadSuccess(starttime);
                if (IsStopLoad)
                {
                    return;
                }
                if (frontEndCode.Code == -1)
                {
                    return;
                }
                else
                {
                    _loginHandler.CefWebBrowser = LoginBrowser;
                    if (IsStopLoad)
                    {
                        return;
                    }
                    IsLoginSuccess(LoginBrowser, name, cachePath);
                }
            }));
            t.IsBackground = true;
            t.Start();
        }

        public FrontEndCode IsLoadSuccess(DateTime starttime)
        {
            while (true)
            {
                FrontEndCode frontEndCode = new FrontEndCode();
                //结束这个线程
                if (IsStopLoad)
                {
                    return frontEndCode;
                }
                //超过两分钟未加载完成则视为加载失败
                if ((DateTime.Now - starttime).TotalMinutes > 1)
                {
                    frontEndCode.Code = -1;
                    frontEndCode.Msg = $"加载超时,请重新加载或者检查网络";
                    Application.DoEvents();
                    return frontEndCode;
                }
                if (LoginBrowser.CanExceuteJs)
                {
                    if (LoginBrowser.Address == "")
                    {
                        Application.DoEvents();
                        frontEndCode.Code = -1;
                        frontEndCode.Msg = $"页面加载失败,请重新加载或者检查代理网络";
                        return frontEndCode;
                    }
                return frontEndCode;
            }
                Thread.Sleep(50);
            }
            }
        /// <summary>
        /// 是否登录成功 循环检测浏览器的Url，通过Url判断用户是否登录
        /// </summary>
        /// <param name="cefWebBrowser">浏览器</param>
        private void IsLoginSuccess(CefWebBrowserControl cefWebBrowser, string name,string cachePath)
        {
            Thread t = new Thread(new ThreadStart(() =>
            {
                _loginHandler.injectionJs(name);
            }));
            t.Start();
            Thread.Sleep(100);
            bool isLoad = false;
            string msg = "";
            EmailAccountInfo socialplatAccountInfo = new EmailAccountInfo();
            EmailMainFrm socialLoginFrm = _socialLoginFrm;//GlobalClass.FormDic["socialLoginFrm"] as SocialLoginFrm;
            while (true)
            {
                if (IsStopLoad)
                {
                    _loginHandler.isBreakCycle = true;
                    return;
                }
                string html = cefWebBrowser.GetHtmlText();
                string address = cefWebBrowser.Address;
               
                 if (name == socialType.NetEase163Email)
                {
                    if (address.Contains("mail.163.com/js6/main.jsp?"))
                    {
                        isLoad = true;
                        Thread.Sleep(1000);
                        socialplatAccountInfo = Get163EmailAccount(cefWebBrowser);
                        socialplatAccountInfo.EmailTypeId = 3;
                        socialplatAccountInfo.EmailClassName = "NetEase163Email";
                    }
                }
                else if (name == socialType.QqEmail)
                {
                    if (address.Contains("mail.qq.com/cgi-bin/frame_html?"))
                    {
                        isLoad = true;
                        Thread.Sleep(1000);
                        socialplatAccountInfo = GetqqEmailAccount(cefWebBrowser);
                        socialplatAccountInfo.EmailTypeId = 1;
                        socialplatAccountInfo.EmailClassName = "QqEmail";
                    }
                }
                else if (name == socialType.NetEaseYeahEmail)
                {
                    if (address.Contains("mail.yeah.net/js6/main.jsp?"))
                    {
                        isLoad = true;
                        Thread.Sleep(1000);
                        socialplatAccountInfo = Get163EmailAccount(cefWebBrowser);
                        socialplatAccountInfo.EmailTypeId = 4;
                        socialplatAccountInfo.EmailClassName = "NetEaseYeahEmail";

                    }
                }
                else if (name == socialType.NetEase126Email)
                {
                    if (address.Contains("mail.126.com/js6/main.jsp?"))
                    {
                        isLoad = true;
                        Thread.Sleep(1000);
                        socialplatAccountInfo = Get163EmailAccount(cefWebBrowser);
                        socialplatAccountInfo.EmailTypeId = 2;
                        socialplatAccountInfo.EmailClassName = "NetEase126Email";

                    }
                }
                else if (name == socialType.TencentEnterpriseEmail)
                {
                    if (address.Contains("exmail.qq.com/cgi-bin/frame_html?"))
                    {
                        isLoad = true;
                        Thread.Sleep(1000);
                        socialplatAccountInfo = GetqqEmailAccount(cefWebBrowser);
                        socialplatAccountInfo.EmailTypeId = 5;
                        socialplatAccountInfo.EmailClassName = "TencentEnterpriseEmail";

                    }
                }
                else if (name == socialType.G_email)
                {
                    if (address.Contains("google.com/?utm_source="))
                    {
                        isLoad = true;
                        Thread.Sleep(1000);
                        socialplatAccountInfo = new EmailAccountInfo();
                        socialplatAccountInfo.EmailTypeId = 6;
                        socialplatAccountInfo.EmailClassName = "G_email";

                    }
                }
                else if (name == socialType.Netease163EnterpriseEmail)
                {
                    if (html.Contains("sH0 nui-scroll"))
                    {
                        isLoad = true;
                        Thread.Sleep(1000);
                        socialplatAccountInfo = new EmailAccountInfo();
                        socialplatAccountInfo.EmailTypeId = 7;
                        socialplatAccountInfo.EmailClassName = "Netease163EnterpriseEmail";

                    }
                }
                else if (name == socialType.AlicloudEnterpriseEmail)
                {
                    if (html.Contains("root_content_wrap e_abs_bottom_full"))
                    {
                        isLoad = true;
                        Thread.Sleep(1000);
                        socialplatAccountInfo = new EmailAccountInfo();
                        socialplatAccountInfo.EmailTypeId = 8;
                        socialplatAccountInfo.EmailClassName = "AlicloudEnterpriseEmail";

                    }
                }
                if (IsStopLoad)
                {
                    _loginHandler.isBreakCycle = true;
                    return;
                }
                if (isLoad)
                {
                    _loginHandler.isBreakCycle = true;
                    lock (GlobalClass.SocailAccountDic)
                    {
                        if (GlobalClass.SocailAccountDic.ContainsKey(name))
                        {
                            var data = GlobalClass.SocailAccountDic[name];
                            socialplatAccountInfo.Account = data.Account;
                            socialplatAccountInfo.Password = data.PassWord;
                        }
                    }
                    socialplatAccountInfo.CachePath = cachePath;
                    SetEmailplatFormData(socialplatAccountInfo);
                    return;
                }
                Thread.Sleep(50);
            }
        }
        private void SetEmailplatFormData(EmailAccountInfo socialplatAccountInfo)
        {
            lock (GlobalClass.SocialplatFormListDic)
            {
                if (GlobalClass.SocialplatFormListDic.ContainsKey(socialplatAccountInfo.Account + socialplatAccountInfo.EmailTypeId))
                {
                    //删除相同邮箱账号缓存
                    if (GlobalClass.SocialplatFormListDic[socialplatAccountInfo.Account + socialplatAccountInfo.EmailTypeId].CachePath != socialplatAccountInfo.CachePath)
                    {
                        DeleteCache(socialplatAccountInfo);
                        return;
                    }
                    UpdateEmailplatFormData(socialplatAccountInfo);
                }
                else
                {
                    if(socialplatAccountInfo.Account!="" && socialplatAccountInfo.Account!=null)
                        InsertEmailplatFormData(socialplatAccountInfo);
                }
             }
        }

        private void DeleteCache(EmailAccountInfo emailAccountInfo)
        {
            RequestContextSettings requestContextSettings = new RequestContextSettings
            {
                CachePath = emailAccountInfo.CachePath,
                PersistUserPreferences = true,
            };
            RequestContext requestContext = new RequestContext(requestContextSettings);
            ICompletionCallback callback = null;
            ICookieManager cefCookieManager = requestContext.GetCookieManager(callback);
            EmailNameToUrl emailUrl = new EmailNameToUrl();
            string url = emailUrl.EmailUrlList[emailAccountInfo.EmailClassName];
            CefCookieHelper.GetUrlCookiesAsync(cefCookieManager, url, true);
        }
        private void InsertEmailplatFormData(EmailAccountInfo socialplatAccountInfo)
        {
            EmailplatForm socialplatForm = new EmailplatForm();
            socialplatForm.AccountName = socialplatAccountInfo.Name;
            socialplatForm.AccountDescription = socialplatAccountInfo.Description;
            socialplatForm.Status = 1;
            socialplatForm.CreateTime = DateTime.Now.ToString("yyyy-MM-dd");
            socialplatForm.EmailAccount = socialplatAccountInfo.Account;
            socialplatForm.EmailPassword = socialplatAccountInfo.Password;
            socialplatForm.SendLimit = _sendLimit;
            socialplatForm.IsChoose = 0;
            socialplatForm.EmailType =socialplatAccountInfo.EmailTypeId;
            socialplatForm.CachePath = socialplatAccountInfo.CachePath;
            int id = _robotForeignDal.InsertSocialPlatform(socialplatForm);
            GlobalClass.SocialplatFormListDic.Add(socialplatAccountInfo.Account+socialplatAccountInfo.EmailTypeId, socialplatForm);
        }

        private void UpdateEmailplatFormData(EmailAccountInfo socialplatAccountInfo)
        {
            EmailplatForm socialplatForm = new EmailplatForm();
            socialplatForm.Status = 1;
            socialplatForm.CreateTime = DateTime.Now.ToString("yyyy-MM-dd");
            socialplatForm.EmailAccount = socialplatAccountInfo.Account;
            socialplatForm.EmailPassword = socialplatAccountInfo.Password;
            socialplatForm.EmailType = socialplatAccountInfo.EmailTypeId;
            _robotForeignDal.UpdateSocialPlatform(socialplatForm);
            GlobalClass.SocialplatFormListDic[socialplatAccountInfo.Account+socialplatAccountInfo.EmailTypeId] = socialplatForm;
        }
     

        private EmailAccountInfo GetqqEmailAccount(CefWebBrowserControl cefWebBrowser)
        {
            string userInfoId = "useralias";
            string descriptionId = "useraddr";
            string jsStr = $"(function(){{" +
                $"var name= document.getElementById('{userInfoId}').innerText;" +
                $"var description= document.getElementById('{descriptionId}').innerText;" +
                $"var jsonobject={{'name':name,'description':description}};" +
                $"return JSON.stringify(jsonobject)" +
                $"}}" +
                $")()";
            JsExecuteResult jsExecuteResult = cefWebBrowser.ExecuteJs(jsStr);
            if (jsExecuteResult.Success)
            {
                EmailAccountInfo socialplatAccountInfo = JsonConvert.DeserializeObject<EmailAccountInfo>(jsExecuteResult.Result);
                return socialplatAccountInfo;
            }
            else
            {
                return new EmailAccountInfo();
            }
        }

        private EmailAccountInfo Get163EmailAccount(CefWebBrowserControl cefWebBrowser)
        {
            string userInfoClass = "js-component-component gWel-greet-trueName";
            //string picInfo = "nav-settings__member-photo EntityPhoto-circle-4 ghost-person ember-view";
            string descriptionId = "spnUid";
            string jsStr = $"(function(){{" +
                $"var name= document.getElementsByClassName('{userInfoClass}')[0].innerText;" +
                $"var description= document.getElementById('{descriptionId}').innerText;" +
                $"var jsonobject={{'name':name,'description':description}};" +
                $"return JSON.stringify(jsonobject)" +
                $"}}" +
                $")()";
            JsExecuteResult jsExecuteResult = cefWebBrowser.ExecuteJs(jsStr);
            if (jsExecuteResult.Success)
            {
                EmailAccountInfo socialplatAccountInfo = JsonConvert.DeserializeObject<EmailAccountInfo>(jsExecuteResult.Result);
                return socialplatAccountInfo;
            }
            else
            {
                return new EmailAccountInfo();
            }
        }
    }
}
