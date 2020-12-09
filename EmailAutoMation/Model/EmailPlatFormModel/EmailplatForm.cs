using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotForeignWorkflow.Model.SocialPlatFormModel
{
    /// <summary>
    /// 邮箱账号状态信息
    /// </summary>
    public class EmailplatForm
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        ///  1:qq邮箱   2:126邮箱 3:163邮箱 4:网易yeah邮箱 5:腾讯企业邮箱 6谷歌邮箱 7:163企业邮箱 8:阿里云邮箱
        /// </summary>
        public int EmailType { get; set; }
        /// <summary>
        /// 邮箱名称
        /// </summary>
        public string EmailClassName { get; set; }
        /// <summary>
        /// <summary>
        /// 状态 0:未绑定 1:已绑定  2:绑定失效
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 绑定时间
        /// </summary>
        public string CreateTime { get; set; }
        /// <summary>
        /// 账号名称
        /// </summary>
        public string AccountName { get; set; }
        /// <summary>
        /// 账号说明
        /// </summary>
        public string AccountDescription { get; set; }
        /// <summary>
        /// 账号
        /// </summary>
        public string EmailAccount { get; set; } = "";
        /// <summary>
        /// 密码
        /// </summary>
        public string EmailPassword { get; set; } = "";
        /// <summary>
        /// 执行时间
        /// </summary>
        public int ExecuteTime { get; set; }
        /// <summary>
        /// 发送上限
        /// </summary>
        public int SendLimit { get; set; }

        /// <summary>
        /// 是否勾选
        /// </summary>
        public int IsChoose { get; set; }
        public string CachePath { get; set; }
    }
}
