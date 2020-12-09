using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeWolfRobot.EmailAutomation.WebSocket
{
    /// <summary>
    /// webserverSocket单例
    /// </summary>
    public class WebServerSingle
    {
        private static WSocket _WebServer = null;
        private static readonly object sync = new object();
        public static WSocket Instance
        {
            get
            {
                if (_WebServer == null)
                {
                    lock (sync)
                    {
                        if (_WebServer == null)
                        {
                            _WebServer = new WSocket();
                        }
                    };
                }
                return _WebServer;
            }
        }
    }
}
