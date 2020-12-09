using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotForeignWorkflow.Model.EmailModel
{
    /// <summary>
    /// 邮箱类型
    /// </summary>
    public class EmailType
    {
        public int Id { get; set; }

        public string EmailName { get; set; }

        public string ClassName { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
