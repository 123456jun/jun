using CefSharpWebBrowser.CefModel;
using System;

namespace TradeWolfRobot.EmailAutomation.Email {
    public class GetElementLocation {
        /// <summary>
        /// 获取空间绝对位置的方法
        /// </summary>
        /// <returns></returns>
        public static string function() {
            /*
//获取元素的绝对位置坐标（像对于页面左上角）
function getElementPagePosition(element){
  var actualLeft = element.offsetLeft;
  var current = element.offsetParent;
  while (current !== null){
    actualLeft += current.offsetLeft;
    current = current.offsetParent;
  }
  var actualTop = element.offsetTop;
  var current = element.offsetParent;
  while (current !== null){
    actualTop += (current.offsetTop+current.clientTop);
    current = current.offsetParent;
  }
  return {x: actualLeft, y: actualTop}
}
             */

            return @"
function getElementPagePosition(element){
  var actualLeft = element.offsetLeft;
  var current = element.offsetParent;
  while (current !== null){
    actualLeft += current.offsetLeft;
    current = current.offsetParent;
  }
  var actualTop = element.offsetTop;
  var current = element.offsetParent;
  while (current !== null){
    actualTop += (current.offsetTop+current.clientTop);
    current = current.offsetParent;
  }
  return actualLeft+':'+ actualTop;
}
";
        }

        /// <summary>
        /// 执行获取返回location绝对位置
        /// </summary>
        /// <param name="cef">cef</param>
        /// <param name="element">节点</param>
        /// <returns></returns>
        public static DomAbsLocation get(CefWebBrowser.CefWebBrowserControl cef, string element) {
            string func = function();

            func += $"getElementPagePosition({element})";

            CefWebBrowser.CefModel.JsExecuteResult jsExecuteResult = cef.ExecuteJs(func);

            string result = null;

            if (jsExecuteResult.Success) {
                result = jsExecuteResult.Result;
            }

            if (result != null) {

                string[] xy = result.Split(':');

                DomAbsLocation location = new DomAbsLocation();
                location.left = Convert.ToInt32(xy[0]);
                location.top = Convert.ToInt32(xy[1]);
                return location;
            }
            return null;

        }
    }
}
