using RobotForeignWorkflow.Model.SocialPlatFormModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeWolfRobot.EmailAutomation.Model.EmailModel
{
    public class EmailNameToUrl
    {
        SocialType socialType = null;
        public Dictionary<string, string> EmailUrlList = null;
        public EmailNameToUrl()
        {
            socialType = new SocialType();
            EmailUrlList = new Dictionary<string, string>()
                {
                { socialType.Netease163EnterpriseEmail, "https://qiye.163.com/login" },
                { socialType.NetEase163Email, "https://mail.163.com" },
                { socialType.NetEase126Email, "https://mail.126.com" },
                { socialType.NetEaseYeahEmail,"https://www.yeah.net"},
                { socialType.QqEmail, "https://mail.qq.com"},
                { socialType.TencentEnterpriseEmail, "https://exmail.qq.com/login" },
                { socialType.G_email, "https://accounts.google.com/signin/v2/identifier?flowName=GlifWebSignIn&flowEntry=ServiceLogin" },
                { socialType.AlicloudEnterpriseEmail, "http://mail.hichina.com/" }
            };
        }

    }
}
