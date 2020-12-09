using RobotForeignWorkflow;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TradeWolfRobot.EmailAutomation.View;

namespace TradeWolfRobot.EmailAutomation
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            EmailMainFrm socialLoginFrm = new EmailMainFrm();
            if (GlobalClass.FormDic.ContainsKey("EmailLoginFrm"))
            {
                GlobalClass.FormDic.Remove("EmailLoginFrm");
            }
            GlobalClass.FormDic.Add("EmailLoginFrm", socialLoginFrm);
            socialLoginFrm.ShowDialog();

        }
    }
}
