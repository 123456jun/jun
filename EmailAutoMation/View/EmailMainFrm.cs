using CefSharp;
using CefSharp.WinForms;
using CefWebBrowser;
using CefWebBrowser.CefHelper;
using EmailAutoMation.Model;
using Newtonsoft.Json;
using RobotForeignWorkflow;
using RobotForeignWorkflow.AutoLogin;
using RobotForeignWorkflow.Data;
using RobotForeignWorkflow.Model.ConfigModel;
using RobotForeignWorkflow.Model.SocialPlatFormModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TradeWolfRobot.EmailAutomation.Execute;
using TradeWolfRobot.EmailAutomation.FrontEnd;
using TradeWolfRobot.EmailAutomation.Helper;
using TradeWolfRobot.EmailAutomation.Model.EmailModel;
using TradeWolfRobot.EmailAutomation.WebSocket;

namespace TradeWolfRobot.EmailAutomation.View
{
    public partial class EmailMainFrm : Form
    {
        public CefWebBrowserControl EmailBrowser=null;
        RobotForeignDal _robotForeignDal = null;
        EmailManage _emailManage = null;
        public Panel _emailBrowserpanel = new Panel();
        public EmailMainFrm()
        {
            InitializeComponent();

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {

            /* EmailLoginPageFrm emailLoginPageFrm = new EmailLoginPageFrm();
             if (GlobalClass.FormDic.ContainsKey("LoginPageFrm"))
             {
                 GlobalClass.FormDic.Remove("LoginPageFrm");
             }
             GlobalClass.FormDic.Add("LoginPageFrm", emailLoginPageFrm);
             emailLoginPageFrm.EmailName = comboBox1.Text;
             emailLoginPageFrm.ShowDialog();*/
            if (_emailManage != null)
            {
                _emailManage.IsStopLoad = true;
            }
            string emailName = comboBox1.Text.Trim();
            _emailManage = new EmailManage();
            _emailManage.IsStopLoad = false;
            int emailCount = (int)_robotForeignDal.GetEmailCount(emailName);
            if (emailCount < 3)
            {
                string emailType = comboBox1.Text.Trim();

                List<string> cachePath = _robotForeignDal.GetCachePath(emailType);
                foreach (string a in cachePath)
                {
                    Console.WriteLine(a.ToString());
                }
                foreach (var item in GlobalClass.GetCache())
                {
                    if (cachePath != null)
                    {

                        if (!cachePath.Contains(item))
                        {
                            this._emailBrowserpanel.Dispose();
                            _emailBrowserpanel = new Panel();
                            _emailBrowserpanel.Dock = DockStyle.Fill;
                            this.panel1.Controls.Add(_emailBrowserpanel);
                            _emailBrowserpanel.Controls.Clear();
                            EmailBrowser = new CefWebBrowserControl("", item);
                            EmailBrowser.RegisterAsyncJsObject("aa", new SocialAutoLogin());
                            EmailBrowser.Dock = DockStyle.Fill;
                            this._emailBrowserpanel.Controls.Add(EmailBrowser);
                            _emailManage.socialDisplay(emailType, item);

                            break;
                        }
                    }
                    else
                    {
                        this._emailBrowserpanel.Dispose();
                        _emailBrowserpanel = new Panel();
                        _emailBrowserpanel.Dock = DockStyle.Fill;
                        this.panel1.Controls.Add(_emailBrowserpanel);
                        _emailBrowserpanel.Controls.Clear();
                        EmailBrowser = new CefWebBrowserControl("", "cachePath1");
                        EmailBrowser.RegisterAsyncJsObject("aa", new SocialAutoLogin());
                        EmailBrowser.Dock = DockStyle.Fill;
                        this._emailBrowserpanel.Controls.Add(EmailBrowser);
                        _emailManage.socialDisplay(emailType, "cachePath1");
                        break;
                    }
                }
            }
            else
            {
                MessageBox.Show($"{emailName}最多{GlobalClass.EmailAcountMaxCount}个账号");
            }
        }
        private void SocialLoginFrm_Load(object sender, EventArgs e)
        {
            _emailManage = new EmailManage();
            this.listView1.Columns.Add("", 120, HorizontalAlignment.Left);
            this.listView1.Columns.Add("账号名", 120, HorizontalAlignment.Left);
            this.listView1.Columns.Add("账号", 120, HorizontalAlignment.Left);
            this.listView1.Columns.Add("状态", 120, HorizontalAlignment.Left);
            this.listView1.Columns.Add("发送上限", 120, HorizontalAlignment.Left);
            this.listView1.Columns.Add("邮箱类型", 120, HorizontalAlignment.Left);
            this.listView1.Columns.Add("", 120, HorizontalAlignment.Left);
            this.listView1.Columns.Add("", 120, HorizontalAlignment.Left);
            this.listView1.Columns[0].Width = 0;
            this.listView1.Columns[6].Width = 0;
            this.listView1.Columns[7].Width = 0;
            _robotForeignDal = new RobotForeignDal();
            GlobalClass.SocialplatFormListDic = new Dictionary<string, EmailplatForm>();
            List<EmailplatForm> allEmailInfo = _robotForeignDal.GetAllSocialPlatform();
            foreach (var emialInfo in allEmailInfo)
            {
                GlobalClass.SocialplatFormListDic.Add(emialInfo.EmailAccount+emialInfo.EmailType,emialInfo);
            }
            SetListView();
            EmailTemplateShow();
            SetAccountTb();
        }
     
        private void button2_Click(object sender, EventArgs e)
        {
            ListView.SelectedIndexCollection indexes = listView1.SelectedIndices;
            EmailAccountInfo socialplat  = new EmailAccountInfo();
            foreach (int index in indexes)
            {
                EmailManage emailManage = new EmailManage();
                socialplat.Account = this.listView1.Items[index].SubItems[2].Text;
                socialplat.EmailTypeId = int.Parse(this.listView1.Items[index].SubItems[6].Text);
              string url=  emailManage.SocialUrl[this.listView1.Items[index].SubItems[5].Text].Url;
                string cachePath= this.listView1.Items[index].SubItems[7].Text;
                RequestContextSettings requestContextSettings = new RequestContextSettings
                {
                    CachePath = cachePath,
                    PersistUserPreferences = true,
                };
                RequestContext requestContext = new RequestContext(requestContextSettings);
                ICompletionCallback callback = null;
                ICookieManager cefCookieManager = requestContext.GetCookieManager(callback);
                CefCookieHelper.GetUrlCookiesAsync(cefCookieManager, url, true);
               _robotForeignDal.DeleteEmailInfo(socialplat);
                break;
            }
            SetListView();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ListView.SelectedIndexCollection indexes = listView1.SelectedIndices;
            EmailAccountInfo emailAccountInf = new EmailAccountInfo();
            foreach (int index in indexes)
            {
                emailAccountInf.Account = this.listView1.Items[index].SubItems[2].Text;
                emailAccountInf.EmailTypeId = int.Parse(this.listView1.Items[index].SubItems[6].Text);
                emailAccountInf.SendLimit = (int)numericUpDown1.Value;
               _robotForeignDal.UpdateEmailInfo(emailAccountInf);
                break;
            }
            SetListView();

        }
        int R = 0;
        private void button4_Click(object sender, EventArgs e)
        {
            _emailBrowserpanel.Dispose();
            _emailBrowserpanel = new Panel();
            _emailBrowserpanel.Dock = DockStyle.Fill;
                this.panel1.Controls.Add(_emailBrowserpanel);
                //CefSharp.BrowserSubprocess
                ListView.SelectedIndexCollection indexes = listView1.SelectedIndices;
            EmailplatForm socialplat = new EmailplatForm();
                // emailManage = new EmailManage();
                foreach (int index in indexes)
                {
                    socialplat.EmailClassName = this.listView1.Items[index].SubItems[5].Text;
                    socialplat.CachePath = this.listView1.Items[index].SubItems[7].Text;
                _emailBrowserpanel.Controls.Clear();
                    string url = _emailManage.SocialUrl[this.listView1.Items[index].SubItems[5].Text].Url;
                    EmailBrowser = new CefWebBrowserControl("", socialplat.CachePath);
                    EmailBrowser.RegisterAsyncJsObject("aa", new SocialAutoLogin());
                    EmailBrowser.Dock = DockStyle.Fill;
                _emailBrowserpanel.Controls.Add(EmailBrowser);
                    _emailManage.socialDisplay(socialplat.EmailClassName, socialplat.CachePath);
                    break;
                }
            

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            Template template = new Template();
            template.ShowDialog();
            EmailTemplateShow();


        }
        public void EmailTemplateShow()
        {
            List<EmailTemplate> emailTemplates = null;
            emailTemplates = _robotForeignDal.GetALLemailTemplate();
            EmailTemplateBox.Items.Clear();
            int index = 0;
            foreach (var item in emailTemplates)
            {
                EmailTemplateBox.Items.Add(item.Id+"-"+item.MailTheme+"-"+item.MailBody);
                if (item.IsChoose==1)
                {
                    EmailTemplateBox.SetItemChecked(index,true);
                }
                index++;
            }
          //  EmailTemplate.SelectedIndexChanged
        }
    
        private void button6_Click(object sender, EventArgs e)
        {
              for (int i = 0; i < EmailTemplateBox.Items.Count; i++)
            {
                if (EmailTemplateBox.GetItemChecked(i))
                {
                    string templateString = EmailTemplateBox.GetItemText(EmailTemplateBox.Items[i]);
                   int index= templateString.IndexOf("-");
                    String emailTemplateId = templateString.Substring(0, index);
                    _robotForeignDal.DeletemailTemplate(emailTemplateId);
                    EmailTemplateShow();
                    continue;
                }
               
            }
        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {
            OperationConfigClass operationConfigClass = new OperationConfigClass();
            EmailConfigClass emailConfigClass = new EmailConfigClass();
            List<EmailTemplate> emailTemplateList = new List<EmailTemplate>();
            Dictionary<string, int> dic = new Dictionary<string, int>();
            List<EmailAccountInfo> emailAccountInfos = new List<EmailAccountInfo>();
            Delay delay = new Delay();
            delay.MinDelay = (int)ExecutionDelayNum1.Value;
            delay.MaxDelay = (int)ExecutionDelayNum2.Value;
            operationConfigClass.ExecutionDelay = delay;
            operationConfigClass.OperationDelay = (int)OperationDelayNum.Value;
            for (int i = 0; i < EmailTemplateBox.Items.Count; i++)
            {
                if (EmailTemplateBox.GetItemChecked(i))
                {
                    dic.Add(EmailTemplateBox.GetItemText(EmailTemplateBox.Items[i]), 1);
                    continue;
                }
                dic.Add(EmailTemplateBox.GetItemText(EmailTemplateBox.Items[i]), 0);
            }
            foreach(var item in dic)
            {
                EmailTemplate emailTemplate = new EmailTemplate();
                string item1 = item.Key;
                int indexId = item1.IndexOf("-");

                emailTemplate.Id = int.Parse(item1.Substring(0,indexId));
                if (item.Value == 1)
                {
                    emailTemplate.IsChoose = 1;
                }
                emailTemplateList.Add(emailTemplate);
            }
           // emailConfigClass.EmailTemplateContent = emailTemplateList;
            emailConfigClass.EmailswitchRate = (int)EmailswitchRateNum.Value;
          //  emailConfigClass.EmailSwitchNumber = (int)EmailSwitchNumberNum.Value;
            emailConfigClass.TemplateSwitchRate = (int)TemplateSwitchRateNum.Value;
            dic.Clear();
            for (int i = 0; i < AccountTb.Items.Count; i++)
            {
                if (AccountTb.GetItemChecked(i))
                {
                   // list.Add(AccountTb.GetItemText(AccountTb.Items[i]));
                    dic.Add(AccountTb.GetItemText(AccountTb.Items[i]),1);
                    continue;
                }
                dic.Add(AccountTb.GetItemText(AccountTb.Items[i]),0);
            }
            foreach (var item in dic)
            {
                EmailAccountInfo emailAccountInfo = new EmailAccountInfo();
                string item1 = item.Key;
                int index1 = item1.IndexOf("-");
                int index2 = item1.Substring(index1 + 1).IndexOf("-");
                emailAccountInfo.Account = item1.Substring(0, index1);
                emailAccountInfo.EmailTypeId = int.Parse(item1.Substring(index1 + 1, index2));
                if (item.Value ==  1)
                {
                    emailAccountInfo.IsChoose = 1;
                }
                emailAccountInfos.Add(emailAccountInfo);
            }
            ConfigData configData = new ConfigData()
            {
                OperationConfig = operationConfigClass,
                EmailConfig = emailConfigClass
            };
         String configDataString= JsonConvert.SerializeObject(configData);
            _robotForeignDal.UpdateEmailTemplate(emailTemplateList);
            _robotForeignDal.UpdateEmailAccountInfo(emailAccountInfos);
            _robotForeignDal.InsertAndUpdatemailConfig(configDataString);
        }
        /// <summary>
        /// /设置listView11项
        /// </summary>
        public void SetListView()
        {
            listView1.Items.Clear();
            List<EmailplatForm> socialplatForms = _robotForeignDal.GetEmailInfos();
            foreach (var emailInfo in socialplatForms)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Text = emailInfo.Id.ToString();
                lvi.SubItems.Add(emailInfo.AccountName);
                lvi.SubItems.Add(emailInfo.EmailAccount);
                if (emailInfo.Status == 1)
                {
                    lvi.SubItems.Add("正常");
                }
                else
                {
                    lvi.SubItems.Add("失效");
                }
                lvi.SubItems.Add(emailInfo.SendLimit.ToString());
                lvi.SubItems.Add(emailInfo.EmailClassName);
                lvi.SubItems.Add(emailInfo.EmailType.ToString());
                lvi.SubItems.Add(emailInfo.CachePath);
                this.listView1.Items.Add(lvi);
            }
        }
        public void SetAccountTb()
        {
            AccountTb.Items.Clear();
            List<EmailplatForm> socialplatForms = _robotForeignDal.GetEmailInfos();
            int index = 0;
            foreach (var item in socialplatForms)
            {
                AccountTb.Items.Add(item.EmailAccount + "-" + item.EmailType + "-" + item.EmailClassName);
                if (item.IsChoose == 1)
                {
                    AccountTb.SetItemChecked(index, true);
                }
                index++;
            }
        }
        public void ListCheckBoxDisplay(object sender, EventArgs e)
        {
            if (EmailTemplateBox.SelectedIndex >= 0)
            {
                string eamilTemplateText = EmailTemplateBox.SelectedItem.ToString();
                int indexId = eamilTemplateText.IndexOf("-");
                int indexMailTheme = eamilTemplateText.Substring(indexId + 1).IndexOf("-");
            }
        }
        private void SocialLoginFrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_emailManage != null)
            {
                _emailManage.IsStopLoad = true;
            }
            Thread.Sleep(1000);
            WSocket webServer = WebServerSingle.Instance;
            if (webServer != null)
            {
                try
                {
                    webServer.Stop();
                }

                catch (Exception ex)
                {
                    Console.WriteLine($"websocket停止异常" + ex);
                }
            }
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (EmailTemplateBox.SelectedIndex >= 0)
            {
                Template template = new Template();
                template.R = 1;
                string eamilTemplateText = EmailTemplateBox.SelectedItem.ToString();
                int indexId = eamilTemplateText.IndexOf("-");
                int indexMailTheme = eamilTemplateText.Substring(indexId + 1).IndexOf("-");
                template.textBox1.Text = eamilTemplateText.Substring(0,indexId);
                template.EmailTheme.Text = eamilTemplateText.Substring(indexId + 1, indexMailTheme- indexId+1);
                template.EmailBody.Text= eamilTemplateText.Substring(indexId+indexMailTheme + 2);
                template.ShowDialog();
                EmailTemplateShow();
            }

        }
       public  int Signal = 0;
        private void button9_Click(object sender, EventArgs e)
        {
            //测试//jun1904527872@163.com;jun1904527872@126.com;1904yufggf2@146.com
            if (Signal==0)
            {
                EmailSendBt.Text = "发送中";
                EmailExecute emailExecute = new EmailExecute();
                List<string> ccaddress = new List<string>() { "jun1904527872@163.com", "jun1904527872@126.com","3748939@126.com" };
                List<string> bccAddress = new List<string>() { "jun1904527872@163.com", "jun1904527872@126.com" };
                EmailInfo emailInfo = new EmailInfo()
                {
                    Addressee = "1904527872@qq.com;1904yufggf2@146.com",
                    CCAddress = ccaddress,
                    BCCAddress = bccAddress
                };
                List<EmailInfo> emailInfos = new List<EmailInfo>();
                emailInfos.Add(emailInfo);
                emailInfos.Add(emailInfo);
                emailInfos.Add(emailInfo);
                emailInfos.Add(emailInfo);
                Thread thread = new Thread(new ThreadStart(() => { 
                emailExecute.StartEmailSend(emailInfos);
                }));
                thread.IsBackground = true;
                thread.Start();

            }
            if (Signal==1)
            {
                EmailExecute._stopSend = true;
                EmailSendBt.Text = "暂停中";
            }
            if (Signal==2)
            {
               EmailExecute._stopSend = false;
               EmailExecute.AutoResetEvent.Set();
                EmailSendBt.Text = "发送中";
            }
            if (Signal == 2)
            {
                Signal = 0;
            }
            Signal++;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            EmailExecute.AutoResetEvent.Set();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            EmailExecute._stopSend = true;
        }
    }
}
