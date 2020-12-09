using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeWolfRobot.EmailAutomation.Model.EmailModel
{
    /// <summary>
    /// 邮箱账号当天的已发送量
    /// </summary>
    public class EmailCurrentDaySendCount
    {
        public string EmailAccount { get; set; }
        public int emailTypeId { get; set; }
        public int Count { get; set; }
    }
}
