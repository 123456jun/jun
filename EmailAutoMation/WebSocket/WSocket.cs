using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace TradeWolfRobot.EmailAutomation.WebSocket
{
    public class WSocket
    {
        public WebSocketServer _wsServer;
        //发送消息的事件
        public delegate bool SendHandle(string messageData);
        public static event SendHandle SendEvent;
        //List<WebSocketServiceHost> webSocketServiceHosts;
        public WSocket()
        {
            _wsServer = new WebSocketServer("ws://127.0.0.1:9999");
            _wsServer.AddWebSocketService<WsRobotForeign>("/wsRobot");
            WebSocketServiceManager webSocketServiceManager = _wsServer.WebSocketServices;
            //webSocketServiceHosts = webSocketServiceManager.Hosts.ToList();
        }
        public void Start()
        {
            _wsServer.Start();
        }
        public void Stop()
        {
            _wsServer.Stop();
        }
        public bool SendData(string MessageData)
        {
            try
            {
              //  List<WebSocketServiceHost> webSocketServiceHosts;
               
                //第一种写法
                //webSocketServiceHosts[0].Sessions.Broadcast(MessageData);
                //var vs = webSocketServiceHosts[0].Sessions.ActiveIDs;
                //string ID = "";
                //foreach (var item in vs)
                //{
                //    ID = item.ToString();
                //    Console.WriteLine(ID);
                //}
                //return true;
                //第二种写法
                if (SendEvent != null)
                {
                    bool ss = SendEvent(MessageData);
                    if (ss == false)
                    {
                        Console.WriteLine("发送失败");
                    }
                    return ss;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
                //throw;
            }
        }
    }
    /// <summary>
    /// WebSocket处理类
    /// </summary>
    public class WsRobotForeign : WebSocketBehavior
    {
        //string ID = "";
        public WsRobotForeign()
        {
            WSocket.SendEvent += WebServer_SendEvent;
        }
        protected override void OnClose(CloseEventArgs e)
        {
            //websocket连接断开
            //LogHelp.info($"websocket断开: Code:{e.Code},Reason:{e.Reason},WasClean:{e.WasClean}");
            Console.WriteLine("连接断开：当前连接人数" + Sessions.Count);
        }
        /// <summary>
        /// 打开连接的事件
        /// </summary>
        protected override void OnOpen()
        {
            //var vs = Sessions.ActiveIDs;
            //foreach (var item in vs)
            //{
            //    ID = item.ToString();
            //    Console.WriteLine(ID);
            //}
            //LogHelp.info("websocket正常连接");
            Console.WriteLine("连接成功：当前连接人数" + Sessions.Count);
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="MessageData"></param>
        private bool WebServer_SendEvent(string MessageData)
        {
            try
            {
                if (State == WebSocketState.Open)
                {
                    if (Sessions.Count > 0)
                    {
                        //Sessions.SendTo(MessageData, ID);
                        //同步发送
                        Send(MessageData);
                        //异步发送
                        //SendAsync(MessageData, null);
                        return true;
                    }
                }
                else
                {
                    //LogUtil.error($"WebSocket的状态:{State}");
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        /// <summary>
        /// 收到消息事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMessage(MessageEventArgs e)
        {
           // WSType websocketSendType = new WSType
            //{
            //    Type = "Heartbeat",
            //    Data = e.Data
            //};
            //try
            //{
                //心跳数据
                //Send(JsonConvert.SerializeObject(websocketSendType));
            //}
            //catch (Exception ex)
           // {
           //     Console.WriteLine(ex.Message);
           // }
        }
        /// <summary>
        /// 错误触发的事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnError(WebSocketSharp.ErrorEventArgs e)
        {
            //LogHelp.info($"websocket错误:{e.Message}");
            //Console.WriteLine("连接错误");
        }
    }
}
