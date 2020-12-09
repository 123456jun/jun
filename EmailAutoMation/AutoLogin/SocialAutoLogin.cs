using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CefSharp;
using CefWebBrowser;
using CefWebBrowser.CefModel;
using Newtonsoft.Json;
using RobotForeignWorkflow.Model.SocialPlatFormModel;

namespace RobotForeignWorkflow.AutoLogin
{
    public class SocialAutoLogin
    {
        SocialType socialType;
        Dictionary<string, AccountData> SocailAccountDic;

        public SocialAutoLogin()
        {
            SocailAccountDic = GlobalClass.SocailAccountDic;
            socialType = new SocialType();
        }
        /// <summary>
        /// 网易126，163，yeah邮箱登录成功获取登录用户信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        public void getneteaselogin(string socailName, string user, string password)
        {
            AccountData accountData = new AccountData()
            {
                Account = user,
                PassWord = password,
            };
            lock (SocailAccountDic)
            {
                if (SocailAccountDic.Keys.Contains(socailName))
                {
                    SocailAccountDic[socailName] = accountData;
                }
                else
                {
                    SocailAccountDic.Add(socailName, accountData);
                }
            }
        }
        /// <summary>
        /// 阿里云邮箱登陆成功获取用户登录信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        public void alicloudEnterpriseEmailLogin(string user, string password)
        {
            //2.登录成功保存账号与密码
            AccountData accountData = new AccountData()
            {
                Account = user,
                PassWord = password

            };
            lock (SocailAccountDic)
            {
                if (SocailAccountDic.Keys.Contains(socialType.AlicloudEnterpriseEmail))
                {
                    SocailAccountDic[socialType.AlicloudEnterpriseEmail] = accountData;
                }
                else
                {
                    SocailAccountDic.Add(socialType.AlicloudEnterpriseEmail, accountData);
                }
            }
        }


        /// <summary>
        ///QQ邮箱登录成功获取用户登录信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        public void register(string user, string password)
        {
            AccountData accountData = new AccountData()
            {
                Account = user,
                PassWord = password,
            };
            lock (SocailAccountDic)
            {
                if (SocailAccountDic.Keys.Contains(socialType.QqEmail))
                {
                    SocailAccountDic[socialType.QqEmail] = accountData;
                }
                else
                {
                    SocailAccountDic.Add(socialType.QqEmail, accountData);
                }
            }
        }
        /// <summary>
        /// 网易企业邮箱登录成功获取登录用户信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        public void getneteaseenterpriselogin(string user, string password)
        {
            AccountData accountData = new AccountData()
            {
                Account = user,
                PassWord = password,
            };
            lock (SocailAccountDic)
            {
                if (SocailAccountDic.Keys.Contains(socialType.Netease163EnterpriseEmail))
                {
                    SocailAccountDic[socialType.Netease163EnterpriseEmail] = accountData;
                }
                else
                {
                    SocailAccountDic.Add(socialType.Netease163EnterpriseEmail, accountData);
                }
            }
        }
        /// <summary>
        /// 腾讯企业邮箱登陆成功获取用户登录信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        public void gettencentuser(string user, string password)
        {

            AccountData accountData = new AccountData()
            {
                Account = user,
                PassWord = password,
            };
            lock (SocailAccountDic)
            {
                if (SocailAccountDic.Keys.Contains(socialType.TencentEnterpriseEmail))
                {
                    SocailAccountDic[socialType.TencentEnterpriseEmail] = accountData;
                }
                else
                {
                    SocailAccountDic.Add(socialType.TencentEnterpriseEmail, accountData);
                }
            }

        }
     

        public void getg_emaillogin(string user, string password)
        {
            AccountData accountData = new AccountData()
            {
                Account = user,
                PassWord = password,
            };
            lock (SocailAccountDic)
            {
                if (SocailAccountDic.Keys.Contains(socialType.G_email))
                {
                    SocailAccountDic[socialType.G_email] = accountData;
                }
                else
                {
                    SocailAccountDic.Add(socialType.G_email, accountData);
                }
            }
        }
    }
}
