using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeWolfRobot.EmailAutomation.Model
{
    public class LoginState
    {
        /// <summary>
        /// 是否出现验证码
        /// </summary>
        public bool IsVCode { get; set; }

        /// <summary>
        /// 是否登录成功
        /// </summary>
        public bool IsLoginSuccess { get; set; }

        /// <summary>
        /// 是否登录超时
        /// </summary>
        public bool IsLoginOutTime { get; set; }

        /// <summary>
        /// 是否加载成功
        /// </summary>
        public bool IsLoadSuccess { get; set; }
    }
}
