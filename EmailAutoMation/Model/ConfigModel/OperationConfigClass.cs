using System.Collections.Generic;
using TradeWolfRobot.EmailAutomation.Model.EmailModel;

namespace RobotForeignWorkflow.Model.ConfigModel
{
    /// <summary>
    /// 操作配置
    /// </summary>
    public class OperationConfigClass
    {
        /// <summary>
        /// 执行间隔
        /// </summary>
        public Delay ExecutionDelay { get; set; }
        /// <summary>
        /// 操作间隔
        /// </summary>
        public int OperationDelay { get; set; }
    }
   
    /// <summary>
    /// 间隔
    /// </summary>
    public class Delay
    {
        /// <summary>
        /// 最小值
        /// </summary>
        public int MinDelay { get; set; }
        /// <summary>
        /// 最大值
        /// </summary>
        public int MaxDelay { get; set; }
    }
    /// <summary>
    /// 邮箱配置
    /// </summary>
    public class EmailConfigClass
    {
        /// <summary>
        /// 模板切换率
        /// </summary>
        public int TemplateSwitchRate { get; set; }
        /// <summary>
        /// 邮箱切换率
        /// </summary>
        public int EmailswitchRate { get; set; }

       /// <summary>
       /// 同一公司发送上限
       /// </summary>
        public int CompanySendLimit { get; set; }

    }
   
  
}
