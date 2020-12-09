using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeWolfRobot.EmailAutomation.Model.FrontModel;

namespace TradeWolfRobot.EmailAutomation.WebSocket
{
    public class WSSendData
    {
       public  WSocket _webServer;
        WSLogTxtData _wBLogTxtData;
        public WSSendData()
        {
            _webServer = WebServerSingle.Instance;
            _wBLogTxtData = new WSLogTxtData();

        }
        /// <summary>
        /// 发送日志
        /// </summary>
        /// <param name="logTxt"></param>
        /// <param name="msg"></param>
        public void WebSocketSendLog(string logTxt)
        {
            _wBLogTxtData.LogTxt = logTxt;
            _wBLogTxtData.TxtCreateTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            WSType websocketSendType = new WSType
            {
                Type = "InsertLog",
                Data = _wBLogTxtData
            };
            _webServer.SendData(JsonConvert.SerializeObject(websocketSendType));
        }
        /// <summary>
        /// 发送营销信息
        /// </summary>
        /// <param name="txt">信息</param>
        /// <param name="code">0 或 -1</param>
        /// <param name="type">1(邮箱) 2(LinkedIn) 3(Facebook) 4(Ins) 5(Twitter)</param>
        public void SendMarketInfo(string txt, int code)
        {
            MarketData marketData = new MarketData();
            marketData.LogTxt = txt;
            marketData.Code = code;
            marketData.Time = DateTime.Now.ToString();
            WSType websocketSendType = new WSType
            {
                Type = "MarketData",
                Data = marketData
            };
            _webServer.SendData(JsonConvert.SerializeObject(websocketSendType));
        }

    }
    public class WSType
    {
        public string Type { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public object Data { get; set; }
    }
    /// <summary>
    /// 日志数据
    /// </summary>
    public class WSLogTxtData
    {
        public string LogTxt { get; set; }

        public string TxtCreateTime { get; set; }
    }

}
