using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailAutoMation.Model
{
    /// <summary>
    /// 邮件信息
    /// </summary>
    public class EmailInfo
    {
        /// <summary>
        /// 收件人地址
        /// </summary>
        public string Addressee { get; set; }
        /// <summary>
        /// 抄送地址
        /// </summary>
        public List<string> CCAddress { get; set; }
        /// <summary>
        /// 密送地址
        /// </summary>
        public List<string> BCCAddress { get; set; }
        /// <summary>
        /// 主题
        /// </summary>
        public string Theme { get; set; }
        /// <summary>
        /// 正文
        /// </summary>
        public string Body { get; set; }
    }
}
