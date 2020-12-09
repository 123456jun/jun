using CefWebBrowser.CefModel;
using EmailAutoMation.Model;
using RobotForeignWorkflow;
using RobotForeignWorkflow.Data;
using RobotForeignWorkflow.Model.ConfigModel;
using RobotForeignWorkflow.Model.EmailModel;
using RobotForeignWorkflow.Model.FrontModel;
using RobotForeignWorkflow.Model.SocialPlatFormModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TradeWolfRobot.EmailAutomation.Model;
using TradeWolfRobot.EmailAutomation.Model.EmailModel;
using TradeWolfRobot.EmailAutomation.View;
using TradeWolfRobot.EmailAutomation.WebSocket;

namespace TradeWolfRobot.EmailAutomation.Execute
{
    public class EmailExecute
    {
        EmailMainFrm _socialLoginFrm = GlobalClass.FormDic["EmailLoginFrm"] as EmailMainFrm;
        OperationConfigClass _oprationConfig;
        RobotForeignDal _robotForeignDal;
        bool _stopTask;
        EmailConfigClass _emailConfig;
        EmailInfo _emailInfo;
        string EmailClassName = "";
        ConfigData _configData;
        WSSendData _wsSendData;
        Dictionary<int, EmailBase> _emailBaseDic = new Dictionary<int, EmailBase>();
        /// <summary>
        /// 暂停发送
        /// </summary>
        public static bool _stopSend;
        /// <summary>
        /// 邮箱账号的发送数量
        /// </summary>
        Dictionary<string, int> _emailAccountSendCountDic;
        /// <summary>
        /// 选中的邮件模板
        /// </summary>
        List<EmailTemplate> _checkedEmailTemplate = null;
        /// <summary>
        /// 选中的邮箱账号
        /// </summary>
        List<EmailAccountInfo> _checkedEmailAccountInfo;
        //获取选中的邮箱
        HashSet<string> _emailNameList;
        public delegate void SendLimitHandle();
        public delegate void LoginInvalidHandle();
        public event SendLimitHandle _sendLimitEvent;
        public event LoginInvalidHandle _loginInvalidEvent;
        public static AutoResetEvent AutoResetEvent = new AutoResetEvent(false);
        //QqEmail NetEase126Email NetEase163Email NetEaseYeahEmail TencentEnterpriseEmail G_email Netease163EnterpriseEmail AlicloudEnterpriseEmail
        public EmailExecute()
        {
            _wsSendData = new WSSendData();
            _wsSendData._webServer.Start();
            _emailInfo = new EmailInfo();
            _robotForeignDal = new RobotForeignDal();
            _emailAccountSendCountDic = new Dictionary<string, int>();
            _emailAccountSendCountDic = _robotForeignDal.GetCurrentDaySendCount();
            _configData = _robotForeignDal.GetmailConfig();
            _oprationConfig = _configData.OperationConfig;
            _emailConfig = _configData.EmailConfig;
            _checkedEmailTemplate = _robotForeignDal.GetCheckedEmailTemplate();
            _checkedEmailAccountInfo = _robotForeignDal.GetCheckedEmailAccountInfo();
            _emailNameList = new HashSet<string>(_checkedEmailAccountInfo.Select(x => x.EmailClassName).ToList());
        }
        /// <summary>
        /// 开始发送
        /// </summary>
        /// <param name="addressList"></param>
        public FrontEndCode StartEmailSend(List<EmailInfo> addressList)
        {
            FrontEndCode frontEndCode = new FrontEndCode();
            if (GlobalClass.IsStop)
            {
                return frontEndCode;
            }
            if (_emailNameList == null || _emailNameList.Count == 0)
            {
                //  Console.WriteLine("无选中邮箱账号");
                _wsSendData.WebSocketSendLog("无选中邮箱账号，请勾选邮箱账号");
                _socialLoginFrm.Invoke(new Action(() =>
                {
                    _socialLoginFrm.EmailSendBt.Text = "测试";
                    _socialLoginFrm.Signal = 0;
                }));
                frontEndCode.Code = -1;
                frontEndCode.Data = "无选中邮箱账号，请勾选邮箱账号";
                return frontEndCode;
            }
            if (addressList.Count == 0)
            {
                frontEndCode.Msg = "无可发送邮件地址";
                return frontEndCode;
            }
            //是否可以执行邮箱发送
            bool canExecute = false;
            Dictionary<int, EmailBase> emailDic = new Dictionary<int, EmailBase>();
            string key = "";
            //相同邮箱的账号数
            int sameEamilTypeAccountCount = 0;
            //循环发送不同邮箱
            foreach (var emilName in _emailNameList)
            {
                //模板索引
                int templateSwitchIndex = 0;
                //邮箱账号切换的索引
                int emailSwitchIndex = 0;
                //模板切换频率(如:每发送3封切换模板)
                int templateSwitchRate = 0;
                //邮箱账号切换率
                int emailSwitchRate = 0;
                if (_stopSend == true)
                {
                    AutoResetEvent.WaitOne();
                }
                _emailBaseDic.Clear();
                EmailClassName = emilName;
                EmailAccountInfo[] accountInfos = (from r in _checkedEmailAccountInfo where r.EmailClassName == EmailClassName select r).ToArray();
                sameEamilTypeAccountCount = accountInfos.Length;
                for (int i = 0; i < sameEamilTypeAccountCount; i++)
                {
                    EmailBase emailBase = EmailBuilder.CreateInstance().CreateEmail(EmailClassName, accountInfos[i].CachePath);
                    emailBase.CefWebBrowser.Dock = DockStyle.Fill;
                    emailBase.CefWebBrowser.Enabled = true;
                    //执行间隔
                    Delay executionDelay = _oprationConfig.ExecutionDelay;
                    Random random = new Random();
                    int operatDelay = random.Next(executionDelay.MinDelay, executionDelay.MaxDelay);
                    emailBase.ExecuteInterval = operatDelay;
                    _emailBaseDic.Add(i, emailBase);
                }

                //
                foreach (var address in addressList)
                {
                    if (GlobalClass.IsStop)
                    {
                        return frontEndCode;
                    }
                    if (_stopSend == true)
                    {
                        AutoResetEvent.WaitOne();
                    }
                    //邮箱账号发送上限
                    int count = sameEamilTypeAccountCount;
                    for (int i = 0; i < count; i++)
                    {

                        string indexStr = accountInfos[i].EmailTypeId + accountInfos[i].Account;
                        if (!_emailAccountSendCountDic.ContainsKey(indexStr))
                        {
                            _emailAccountSendCountDic[indexStr] = 0;
                        }
                        if (accountInfos[i].SendLimit <= _emailAccountSendCountDic[indexStr])
                        {
                            for (int j = i; j < count; j++)
                            {
                                if (_emailBaseDic.ContainsKey(j + 1))
                                {
                                    accountInfos[i] = accountInfos[i + 1];
                                    _emailBaseDic[j] = _emailBaseDic[j + 1];
                                }
                            }
                            _emailBaseDic.Remove(_emailBaseDic.Count - 1);
                            sameEamilTypeAccountCount--;
                            emailSwitchRate = 0;
                            _wsSendData.SendMarketInfo($"{EmailClassName}:{accountInfos[i].Account}今日邮件发送已达上限", -1);
                            // _sendLimitEvent.Invoke();
                        }
                        if (sameEamilTypeAccountCount == 0)
                        {
                            break;
                        }
                    }
                    //同邮箱账号切换
                    if (sameEamilTypeAccountCount == 0)
                    {
                        // Console.WriteLine($"{EmailClassName}所有账号都上限了");
                        //  _sendLimitEvent.Invoke();
                        break;
                    }

                    if (emailSwitchRate >= _emailConfig.EmailswitchRate || emailSwitchRate == 0)
                    {

                        if (emailSwitchIndex >= sameEamilTypeAccountCount)
                        {
                            emailSwitchIndex = 0;
                        }
                        _socialLoginFrm.Invoke(new Action(() =>
                        {
                            _socialLoginFrm._emailBrowserpanel.Dispose();
                            _socialLoginFrm._emailBrowserpanel = new Panel();
                            _socialLoginFrm._emailBrowserpanel.Dock = DockStyle.Fill;
                            _socialLoginFrm._emailBrowserpanel.Enabled = false;
                            _socialLoginFrm._emailBrowserpanel.Controls.Add(_emailBaseDic[emailSwitchIndex].CefWebBrowser);
                            _socialLoginFrm.panel1.Controls.Add(_socialLoginFrm._emailBrowserpanel);
                        }));
                        Thread.Sleep(3 * 1000);
                        //是否登录成功
                        LoginState loginState = _emailBaseDic[emailSwitchIndex].IsLogin(accountInfos[emailSwitchIndex].Account, accountInfos[emailSwitchIndex].Password);
                        if (loginState.IsLoginSuccess)
                        {
                            canExecute = true;
                            if (_stopTask)
                            {
                                return frontEndCode;
                            }
                            //3秒等待页面加载完毕
                            Thread.Sleep(3 * 1000);
                        }
                        else
                        {
                            if (_stopTask)
                            {
                                return frontEndCode;
                            }
                            if (loginState.IsLoginOutTime)
                            {
                                _wsSendData.WebSocketSendLog($"{accountInfos[emailSwitchIndex].EmailClassName + accountInfos[emailSwitchIndex].Account}加载超时,请检查网络");
                                emailSwitchRate = 0;
                                continue;
                            }
                            else if (loginState.IsVCode)
                            {
                                _wsSendData.WebSocketSendLog($"自动登录出现验证码,请重新授权登录");
                                emailSwitchRate = 0;
                                continue;

                            }
                            else
                            {
                                Console.WriteLine($"登录失效,请检查授权状态");
                                //  _loginInvalidEvent.Invoke();
                                emailSwitchRate = 0;

                            }
                            //登录失效改状态
                            EmailplatForm socialplatForm = new EmailplatForm();
                            socialplatForm.Status = 2;
                            socialplatForm.EmailAccount = accountInfos[emailSwitchIndex].Account;
                            socialplatForm.EmailType = accountInfos[emailSwitchIndex].EmailTypeId;
                            _robotForeignDal.UpdateSocialPlatform(socialplatForm);
                        }
                        key = accountInfos[emailSwitchIndex].EmailTypeId + accountInfos[emailSwitchIndex].Account;
                        if (!_emailAccountSendCountDic.ContainsKey(key))
                        {
                            _emailAccountSendCountDic.Add(key, 0);
                        }
                        emailSwitchRate = 0;
                        emailSwitchIndex++;

                    }
                    if (canExecute)
                    {
                        if (_stopSend == true)
                        {
                            AutoResetEvent.WaitOne();
                        }
                        if (templateSwitchRate >= _emailConfig.TemplateSwitchRate || templateSwitchRate == 0)
                        {
                            if (templateSwitchIndex >= _checkedEmailTemplate.Count)
                            {
                                templateSwitchIndex = 0;
                            }
                            _emailInfo.Addressee = address.Addressee;
                            _emailInfo.CCAddress = address.CCAddress;
                            _emailInfo.BCCAddress = address.BCCAddress;
                            _emailInfo.Theme = _checkedEmailTemplate[templateSwitchIndex].MailTheme;
                            _emailInfo.Body = _checkedEmailTemplate[templateSwitchIndex].MailBody;
                            templateSwitchIndex++;
                            templateSwitchRate = 0;
                        }
                        JsExecuteResult result = EmailSend(_emailBaseDic[emailSwitchIndex - 1], _emailInfo);
                        if (result.Success)
                        {
                            if (_emailBaseDic[emailSwitchIndex - 1].IsNeedVerificationCode())
                            {
                                Console.WriteLine("需要验证码");
                                continue;
                            }
                            EmailSendRecord emailSendRecord = new EmailSendRecord()
                            {
                                EmailTypeId = accountInfos[emailSwitchIndex - 1].EmailTypeId,
                                EmailAccount = accountInfos[emailSwitchIndex - 1].Account,
                                MailAddressee = _emailInfo.Addressee,
                                MailTemplateId = _checkedEmailTemplate[templateSwitchIndex - 1].Id,
                                MailBCC = address.BCCAddress,
                                MailCC = address.CCAddress
                            };

                            _wsSendData.WebSocketSendLog("发送成功");
                            _emailAccountSendCountDic[key]++;
                            _robotForeignDal.InsertEmailRecord(emailSendRecord);
                        }
                        else
                        {
                            _wsSendData.WebSocketSendLog("发送失败");
                        }
                        emailSwitchRate++;
                        templateSwitchRate++;
                        //只有一个无需切换
                        if (sameEamilTypeAccountCount == 1)
                        {
                            emailSwitchRate = -1;
                        }
                        //发送邮箱操作间隔
                        Thread.Sleep(_oprationConfig.OperationDelay * 1000);
                    }
                }
            }
            _socialLoginFrm.Invoke(new Action(() =>
            {
                _socialLoginFrm.EmailSendBt.Text = "测试";
                _socialLoginFrm.Signal = 0;
            }));
            return frontEndCode;
        }

        /// <summary>
        /// 邮箱发送
        /// </summary>
        /// <param name="emailBase"></param>
        /// <param name="emailInfo"></param>
        /// <returns></returns>
        public JsExecuteResult EmailSend(EmailBase emailBase, EmailInfo emailInfo)
        {
            JsExecuteResult emailSendResult = emailBase.ExecuteEmailSend(emailInfo);
            return emailSendResult;
        }
        /// <summary>
        /// 停止任务
        /// </summary>
        public void StopTask()
        {
            _stopTask = true;
            if (_emailBaseDic != null)
            {
                for (int i = 0; i < _emailBaseDic.Count; i++)
                {
                    if (_emailBaseDic[i].CefWebBrowser != null)
                    {
                        _emailBaseDic[i].CefWebBrowser.StopWhile = true;
                        _emailBaseDic[i].StopTask();
                    }
                }
            }
        }
    }
}
