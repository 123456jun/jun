using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeWolfRobot.EmailAutomation.Model.FrontModel
{
    /// <summary>
    /// 显示到前端的营销数据
    /// </summary>
    public class MarketData
    {
        /// <summary>
        /// 状态
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        public string Msg { get; set; }
        /// <summary>
        /// 日志
        /// </summary>
        public string LogTxt { get; set; }
        /// <summary>
        /// 生成时间
        /// </summary>
        public string Time { get; set; } = DateTime.Now.ToString();
    }
}
