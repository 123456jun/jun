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
using TradeWolfRobot.EmailAutomation.Model.EmailModel;

namespace TradeWolfRobot.EmailAutomation.View
{
    public partial class Template : Form
    {
        public Template()
        {
            InitializeComponent();
        }
        //0添加1修改
        public int R = 0;
        private void button1_Click(object sender, EventArgs e)
        {
            EmailTemplate emailTemplate = null;
            RobotForeignDal _robotForeignDal = new RobotForeignDal();
            if (EmailBody.Text != "" && EmailTheme.Text != "")
            {
                if (R == 0)
                {
                    if (EmailTheme.Text != "" && EmailBody.Text != "")
                    {
                        emailTemplate = new EmailTemplate()
                        {
                            MailBody = EmailBody.Text,
                            MailTheme = EmailTheme.Text
                        };
                        _robotForeignDal.InsertEmailTemplate(emailTemplate);
                    }
                    this.Close();
                }
                else
                {
                    EmailTemplate template = new EmailTemplate();
                    template.Id = int.Parse(textBox1.Text.Trim());
                    template.MailTheme = EmailTheme.Text;
                    template.MailBody = EmailBody.Text;
                    _robotForeignDal.UpdateEmailTemplate(template);
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("别空着！好吧");
            }
        }

        private void Template_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
