using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeWolfRobot.EmailAutomation.Model.EmailModel
{
    public class EmailAccountInfo
    {
        /// <summary>
        /// id 1：qq邮箱,2:163邮箱
        /// </summary>
        public int EmailTypeId { get; set; }
        /// <summary>
        /// 账号
        /// </summary>
        public string Account { get; set; }
        public string Password { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string Name { get; set; } = "";
        /// <summary>
        /// 说明
        /// </summary>
        public string Description { get; set; } = "";
        /// <summary>
        /// 是否选中账号
        /// </summary>
        public int IsChoose { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string EmailClassName { get; set; }
        /// <summary>
        /// 发送上限
        /// </summary>
        public int SendLimit { get; set; }
        /// <summary>
        /// 缓存
        /// </summary>
        public string CachePath { get; set; }
}
}
