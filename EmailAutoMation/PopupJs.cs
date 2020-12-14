using CefWebBrowser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace RobotForeignWorkflow
{
    public class PopupJs
    {
        /// <summary>
        /// 弹窗//66666999
        /// </summary>
        /// <param name="cefWebBrowser"></param>
        /// <param name="popupStatus"></param>
        /// <param name="content"></param>
        public static void ExcuextJsPopup(CefWebBrowserControl cefWebBrowser, PopupStatus popupStatus, string content, bool? closable = null)
        {
            if (closable == null)
            {
                closable = true;
            }
            string type = Enum.Parse(typeof(PopupStatus), popupStatus.ToString()).ToString();
            string jsStr = $"doConfigureAlert({closable.ToString().ToLower()},{{title:'{content}',description:'',closable:true,center:false,'close-text':'','show-icon':true,effect:'light',type:'{type}'}});";
            //执行js 方法
            cefWebBrowser.ExceuteJsAsync(jsStr);
        }
    }
}
