using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TradeWolfRobot.EmailAutomation
{
    public class EmailBuilder
    {
        private volatile static EmailBuilder _instance = null;
        private static readonly object syc = new object();
        /// <summary>
        /// 
        /// </summary>
        private EmailBuilder() { }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static EmailBuilder CreateInstance()
        {
            if (_instance == null)
            {
                lock (syc)
                {
                    if (_instance == null)
                        _instance = new EmailBuilder();
                }
            }
            return _instance;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="AData"></param>
        /// <returns></returns>
        public EmailBase CreateEmail(string emailClassName)
        {
            //EmailBase catchInstance = Assembly.GetExecutingAssembly().CreateInstance("QqEmail") as EmailBase;
            EmailBase catchInstance = CreateInstances<EmailBase>("EmailAutomation", "TradeWolfRobot.EmailAutomation.Email", emailClassName, null);
            return catchInstance;
        }
        public EmailBase CreateEmail(string emailClassName,string cachePath)
        {
            //EmailBase catchInstance = Assembly.GetExecutingAssembly().CreateInstance("QqEmail") as EmailBase;
            object[] obj = new object[] {cachePath};
            EmailBase catchInstance = CreateInstances<EmailBase>("EmailAutomation", "TradeWolfRobot.EmailAutomation.Email", emailClassName, obj);
            return catchInstance;
        }
        private T CreateInstances<T>(string assemblyName, string nameSpace, string className, Object[] parameters)
        {
            try
            {
                string fullName = nameSpace + "." + className;//命名空间.类型名
                string path = fullName + "," + assemblyName;//命名空间.类型名,程序集
                Type o = Type.GetType(path);//加载类型
                object ect = Assembly.Load(assemblyName).CreateInstance(fullName, true,
                                BindingFlags.Default,
                                null, parameters, null, null);
                return (T)ect;//类型转换并返回
            }
            catch
            {
                throw;
            }
        }
    }
}
