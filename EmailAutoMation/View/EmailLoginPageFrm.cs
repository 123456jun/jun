using CefWebBrowser;
using RobotForeignWorkflow;
using RobotForeignWorkflow.AutoLogin;
using RobotForeignWorkflow.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TradeWolfRobot.EmailAutomation.Helper;
using TradeWolfRobot.EmailAutomation.Model.EmailModel;

namespace TradeWolfRobot.EmailAutomation.View
{
    public partial class EmailLoginPageFrm : Form
    {
        public CefWebBrowserControl EmailBrowser = new CefWebBrowserControl("");
        RobotForeignDal _robotForeignDal = new RobotForeignDal();
        public CefWebBrowserControl CefNoticeBrowser = null;
        public Panel _emailBrowserpanel = new Panel();
        public string EmailName;
        public string CachePath = "";

        public EmailLoginPageFrm()
        {
            InitializeComponent();
        }

        private void EmailLoginPageFrm_Load(object sender, EventArgs e)
        {
            NoticeBrowserPanel.Controls.Clear();
          
            CefNoticeBrowser = new CefWebBrowserControl(Application.StartupPath + @"\Html\page\html\alert.html")
            {
                Dock = DockStyle.Fill
                
            };
            NoticeBrowserPanel.Controls.Add(CefNoticeBrowser);
             NoticeBrowserPanel.Visible = false;
            if (CachePath.Length<=0) {
                AddEmailAccountBind(EmailName);
            }
            else
            {
                EmailShow();
            }

        }
        public void AddEmailAccountBind(string name)
        {
            if (name!=null)
            {
                EmailNameToUrl emailNameToUrl = new EmailNameToUrl();
                int emailCount = (int)_robotForeignDal.GetEmailCount(name);
                List<string> cachePath = _robotForeignDal.GetCachePath(name);
                foreach (var item in GlobalClass.GetCache())
                {
                    if (cachePath != null)
                    {

                        if (!cachePath.Contains(item))
                        {
                            LoginPage.Controls.Clear(); 
                            EmailBrowser = new CefWebBrowserControl(emailNameToUrl.EmailUrlList[name], item);
                            EmailBrowser.RegisterAsyncJsObject("aa", new SocialAutoLogin());
                            EmailBrowser.RegisterAsyncJsObject("SharpObj", new EmailManageToJs());
                            EmailBrowser.Dock = DockStyle.Fill;
                            LoginPage.Controls.Add(NoticeBrowserPanel);
                            LoginPage.Controls.Add(EmailBrowser);
                            break;
                        }
                    }
                    else
                    {

                        LoginPage.Controls.Clear();
                        EmailBrowser = new CefWebBrowserControl(emailNameToUrl.EmailUrlList[name], item);
                        EmailBrowser.RegisterAsyncJsObject("aa", new SocialAutoLogin());
                        EmailBrowser.RegisterAsyncJsObject("SharpObj", new EmailManageToJs());
                        EmailBrowser.Dock = DockStyle.Fill;
                        LoginPage.Controls.Add(NoticeBrowserPanel);
                        LoginPage.Controls.Add(EmailBrowser);
                        break;
                    }
                }
            }
            }
        public void EmailShow()
        {
            EmailNameToUrl emailNameToUrl = new EmailNameToUrl();
            LoginPage.Controls.Clear();
            EmailBrowser = new CefWebBrowserControl(emailNameToUrl.EmailUrlList[EmailName],CachePath);
            EmailBrowser.RegisterAsyncJsObject("aa", new SocialAutoLogin());
            EmailBrowser.RegisterAsyncJsObject("SharpObj", new EmailManageToJs());
            EmailBrowser.Dock = DockStyle.Fill;
            LoginPage.Controls.Add(NoticeBrowserPanel);
            LoginPage.Controls.Add(EmailBrowser);
        }
        private void LoginPage_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            NoticeBrowserPanel.Visible = false; 
        }
    }
}
