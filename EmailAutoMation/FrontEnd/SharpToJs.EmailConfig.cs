using CefSharp;
using CefWebBrowser.CefHelper;
using Newtonsoft.Json;
using RobotForeignWorkflow.Model.ConfigModel;
using RobotForeignWorkflow.Model.FrontModel;
using RobotForeignWorkflow.Model.SocialPlatFormModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TradeWolfRobot.EmailAutomation.Model.EmailModel;

namespace TradeWolfRobot.EmailAutomation.FrontEnd
{
  public partial  class SharpToJs
    {
        /// <summary>
        /// 邮箱操作设置
        /// </summary>
        /// <param name="jsonString"></param>
        /// <param name="cb"></param>
        public void setEmailConfig(string jsonString, IJavascriptCallback cb)
        {
            Thread thread = new Thread(new ThreadStart(() => {
             FrontEndCode frontEndCode = new FrontEndCode();
            _robotForeignDal.InsertAndUpdatemailConfig(jsonString);
            cb.ExecuteAsync(frontEndCode);
            }));
            thread.Start();
        }
        /// <summary>
        /// 获取邮箱配置
        /// </summary>
        /// <param name="cb"></param>
        public void getEmailConfig(IJavascriptCallback cb)
        {
            Thread thread = new Thread(new ThreadStart(() => {
                ConfigData configData = _robotForeignDal.GetmailConfig();
                string jsonStr = JsonConvert.SerializeObject(configData);
                FrontEndCode frontEndCode = new FrontEndCode()
                {
                    Code = 0,
                    Msg = "",
                    Data = jsonStr
                };
                cb.ExecuteAsync(frontEndCode);
            }));
            thread.Start();
        }
        /// <summary>
        /// 设置选中的邮箱账号
        /// </summary>
        /// <param name="jsonString"></param>
        /// <param name="cb"></param>
        public void setCheckedEmailAccount(List<string> jsonString,IJavascriptCallback cb)
        {
            Thread thread = new Thread(new ThreadStart(() => {
             FrontEndCode frontEndCode = new FrontEndCode();
            List<EmailAccountInfo> emailAccountInfoList = new List<EmailAccountInfo>();
            foreach (var item in jsonString)
            {
                EmailAccountInfo emailAccountInfo = new EmailAccountInfo();
                emailAccountInfo = JsonConvert.DeserializeObject<EmailAccountInfo>(item);
                emailAccountInfoList.Add(emailAccountInfo);
            }
            _robotForeignDal.UpdateEmailAccountInfo(emailAccountInfoList);
            cb.ExecuteAsync(frontEndCode);
           }));
            thread.Start();
    }
        /// <summary>
        ///选中的模板
        /// </summary>
        /// <param name="jsonString"></param>
        /// <param name="cb"></param>
        public void setCheckedTemplate(List<string> jsonString, IJavascriptCallback cb)
        {
            Thread thread = new Thread(new ThreadStart(() => {
            FrontEndCode frontEndCode = new FrontEndCode();
            List<EmailTemplate> emailTemplateList = new List<EmailTemplate>();
            foreach (var item in jsonString)
            {
                EmailTemplate emailTemplate = new EmailTemplate();
                emailTemplate = JsonConvert.DeserializeObject<EmailTemplate>(item);
                emailTemplateList.Add(emailTemplate);
            }
            _robotForeignDal.UpdateEmailTemplate(emailTemplateList);
            cb.ExecuteAsync(frontEndCode);
            }));
            thread.Start();
        }
      
        /// <summary>
        /// 删除邮箱账号
        /// </summary>
        /// <param name="emailAccountInfo"></param>
        public void deleteEmailAccount(string jsonString, IJavascriptCallback cb)
        {
            Thread thread = new Thread(new ThreadStart(() =>
            {
                FrontEndCode frontEndCode = new FrontEndCode();
                EmailAccountInfo emailAccountInfo = JsonConvert.DeserializeObject<EmailAccountInfo>(jsonString);
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
                _robotForeignDal.DeleteEmailInfo(emailAccountInfo);
                cb.ExecuteAsync(frontEndCode);
            }));
            thread.Start();

        }
        /// <summary>
        /// 修改账号上限
        /// </summary>
        /// <param name="emailAccountInfo"></param>
        public void updateEmailAccountSendLimit(string jsonString, IJavascriptCallback cb)
        {
            Thread thread = new Thread(new ThreadStart(() =>
            {
                FrontEndCode frontEndCode = new FrontEndCode();
                EmailAccountInfo emailAccountInfo = JsonConvert.DeserializeObject<EmailAccountInfo>(jsonString);
                _robotForeignDal.UpdateEmailInfo(emailAccountInfo);
                cb.ExecuteAsync(frontEndCode);
            }));
            thread.Start();

        }
        /// <summary>
        /// 删除模板（软删除）
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cb"></param>
        public void deleteTemplate(string id, IJavascriptCallback cb)
        {
            Thread thread = new Thread(new ThreadStart(() =>
            {
                FrontEndCode frontEndCode = new FrontEndCode();
                _robotForeignDal.DeletemailTemplate(id);
                cb.ExecuteAsync(frontEndCode);
            }));
            thread.Start();
        }
        /// <summary>
        /// 修改模板
        /// </summary>
        /// <param name="jsonString"></param>
        /// <param name="cb"></param>
        public void updateTemplate(string jsonString, IJavascriptCallback cb)
        {
            Thread thread = new Thread(new ThreadStart(() =>
            {
                FrontEndCode frontEndCode = new FrontEndCode();
                EmailTemplate emailTemplate = JsonConvert.DeserializeObject<EmailTemplate>(jsonString);
                _robotForeignDal.UpdateEmailTemplate(emailTemplate);
                cb.ExecuteAsync(frontEndCode);
            }));
            thread.Start();
        }
        /// <summary>
        /// 获取所有模板
        /// </summary>
        /// <param name="cb"></param>
        public void getEmailTemplate(IJavascriptCallback cb)
        {
           List<EmailTemplate> emailTemplates= _robotForeignDal.GetALLemailTemplate();
            FrontEndCode frontEndCode = new FrontEndCode()
            {
                Code = 0,
                Msg = "",
                Data = emailTemplates
            };
            cb.ExecuteAsync(frontEndCode);
        }
        /// <summary>
        /// 获取所有邮箱账号信息
        /// </summary>
        /// <param name="cb"></param>
        public void getEmailAccount(IJavascriptCallback cb)
        {
            List<EmailplatForm> socialplatForms = _robotForeignDal.GetAllSocialPlatform();
            FrontEndCode frontEndCode = new FrontEndCode()
            {
                Code = 0,
                Msg = "",
                Data = socialplatForms
            };
            cb.ExecuteAsync(frontEndCode);
        }

        #region
      
        #endregion
    }
}
