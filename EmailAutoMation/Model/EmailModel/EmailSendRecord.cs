using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotForeignWorkflow.Model.EmailModel
{
    /// <summary>
    /// 发送记录
    /// </summary>
    public class EmailSendRecord
    {
        /// <summary>
        /// 邮箱类型id
        /// </summary>
        public int EmailTypeId { get; set; }
        /// <summary>
        /// 邮箱账号
        /// </summary>
        public string EmailAccount { get; set; }
        /// <summary>
        /// 模板类型id
        /// </summary>
        public int MailTemplateId { get; set; }
        /// <summary>
        /// 收件人
        /// </summary>
        public string MailAddressee { get; set; }
        /// <summary>
        /// 抄送
        /// </summary>
        public List<string> MailCC { get; set; } 
        /// <summary>
        /// 密送
        /// </summary>
        public List<string> MailBCC { get; set;}
        /// <summary>
        /// 发送时间
        /// </summary>
        public DateTime SendTime { get; set; }
    }
}
