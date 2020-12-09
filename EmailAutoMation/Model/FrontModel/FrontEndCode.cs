using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotForeignWorkflow.Model.FrontModel
{
    public class FrontEndCode
    {
        /// <summary>     
        /// -1表示开发者异常,等于0表示操作成功,1表示系统错误,2表示需要弹窗
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// 提示信息
        /// </summary>
        public string Msg { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public object Data { get; set; }
    }
}
