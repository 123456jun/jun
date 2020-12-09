using CefSharp;
using RobotForeignWorkflow;
using RobotForeignWorkflow.Data;
using RobotForeignWorkflow.Model.FrontModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeWolfRobot.EmailAutomation.View;

namespace TradeWolfRobot.EmailAutomation.FrontEnd
{
  public  class EmailLoginFrmToJs
    {
        RobotForeignDal _robotForeignDal = new RobotForeignDal();
        EmailLoginPageFrm EmailLoginPageFrm = null;
        public EmailLoginFrmToJs()
        {
            EmailLoginPageFrm = GlobalClass.FormDic["LoginPageFrm"] as EmailLoginPageFrm;
        }
        /// <summary>
        /// 添加邮箱账号
        /// </summary>
        /// <param name="name"></param>
        /// <param name="cb"></param>
        public void emailLoginPageFrmShow(string name,IJavascriptCallback cb)
        {
            FrontEndCode frontEndCode = null;
            List<string> cachePath = _robotForeignDal.GetCachePath(name);
            if (cachePath.Count >= 3)
            {
                frontEndCode = new FrontEndCode()
                {
                    Msg = $"相同邮箱账号最多绑定{GlobalClass.EmailAcountMaxCount}个"
                };
                cb.ExecuteAsync(frontEndCode);
            }
            EmailLoginPageFrm.Invoke(new Action(() => {
                EmailLoginPageFrm.EmailName = name;
                EmailLoginPageFrm.ShowDialog();
            }));
            cb.ExecuteAsync(new FrontEndCode());
        }
        /// <summary>
        /// 显示邮箱账号
        /// </summary>
        /// <param name="name"></param>
        /// <param name="cachePath"></param>
        /// <param name="cb"></param>
        public void emailLoginPageFrmShow(string name,string cachePath, IJavascriptCallback cb)
        {
            EmailLoginPageFrm.Invoke(new Action(() => {
                EmailLoginPageFrm.EmailName = name;
                EmailLoginPageFrm.CachePath = cachePath;
                EmailLoginPageFrm.ShowDialog();
            }));
            cb.ExecuteAsync(new FrontEndCode());
        }
    }
}
