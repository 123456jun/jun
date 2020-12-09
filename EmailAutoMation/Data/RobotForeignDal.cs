using RobotForeignWorkflow.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using TradeWolf.DAL;
using Newtonsoft.Json;
using RobotForeignWorkflow.Model.EmailModel;
using RobotForeignWorkflow.Model.ConfigModel;
using RobotForeignWorkflow.Model.SocialPlatFormModel;
using System.Text.RegularExpressions;
using System.Data.SQLite;
using System.Data.Common;
using System.Text;
using TradeWolfRobot.EmailAutomation.Model.EmailModel;

namespace RobotForeignWorkflow.Data
{
    public class RobotForeignDal
    {

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string ConnectionString { get; } = "";
        public RobotForeignDal()
        {
            string sDir = Application.StartupPath + "\\Data";
            ConnectionString = "data source=" + sDir + "\\RobotForeign.db";
        }

        public void ExecuteSql(string sqlString)
        {
            try
            {
                SQLiteHelper.ExecuteNonQuery(ConnectionString, sqlString, null);
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// 插入邮件发送记录
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        public int InsertEmailRecord(EmailSendRecord emailSendRecord)
        {
            string sSql = $"INSERT INTO emailSendRecord_tbl(emailTypeId,emailAccount,mailTemplateId,mailAddressee,mailCC,mailBCC,sendtime)"
              + $"VALUES ('{emailSendRecord.EmailTypeId}','{emailSendRecord.EmailAccount}','{emailSendRecord.MailTemplateId}','{emailSendRecord.MailAddressee}','{emailSendRecord.MailCC}','{emailSendRecord.MailBCC}','{DateTime.Now.Date}')";
           // object[] param = { emailSendRecord.EmailTypeId, emailSendRecord.EmailAccount ,emailSendRecord.MailTemplateId, emailSendRecord.MailAddressee, emailSendRecord.MailCC, emailSendRecord.MailBCC,DateTime.Now.Date };
            try
            {
                object id = SQLiteHelper.ExecuteIDResult(ConnectionString, sSql,null);
                return int.Parse(id.ToString());
            }
            catch (Exception)
            {
                throw;
            }
        }
        #region 
        /// <summary>
        /// 获取使用账号信息
        /// </summary>
        /// <returns></returns>
        public List<EmailplatForm> GetAllSocialPlatform()
        {
            string sSql = "SELECT emailAccount_tbl.Id as id,emailAccount,emailPassword,emailTypeId as emailType,emailStatus,sendLimit,isChoose,createTime,emailClassName,cachePath" +
                " FROM emailAccount_tbl LEFT JOIN emailType_tbl ON emailAccount_tbl.emailTypeId=emailType_tbl.id";
            try
            {
                using (IDataReader dr = SQLiteHelper.ExecuteReader(ConnectionString, sSql, null))
                {
                    List<EmailplatForm> socialplatForm = DBExtensions.ToList<EmailplatForm>(dr);
     /*               for (int i = 0; i < socialplatForm.Count; i++)
                    {
                        socialplatForm[i].CreateTime = Convert.ToDateTime(socialplatForm[i].CreateTime).ToString("yyyy-MM-dd");
                    }*/
                    return socialplatForm;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            finally
            {
                GC.Collect();
            }
        }
        /// <summary>
        ///获取账号数量
        /// </summary>
        /// <param name="emailName"></param>
        /// <returns></returns>
        public int GetEmailCount(string emailName)
        {
            string sql = $"SELECT COUNT(*) FROM emailAccount_tbl LEFT JOIN  emailType_tbl ON emailAccount_tbl.emailTypeId=emailType_tbl.id WHERE emailClassName='{emailName}'";
            Object obj= SQLiteHelper.ExecuteScalar(ConnectionString, sql, null);
            int count = 0;
            if (obj != null)
            {
               count = int.Parse(obj.ToString());
             
            }
            return count;
        }
        /// <summary>
        /// 获取缓存地址
        /// </summary>
        /// <param name="emailName"></param>
        /// <returns></returns>
        public List<string> GetCachePath(string emailName)
        {
            string cachePathSql = $"SELECT cachePath FROM emailAccount_tbl LEFT JOIN  emailType_tbl ON emailAccount_tbl.emailTypeId=emailType_tbl.id WHERE emailClassName='{emailName}'";
            List<string> cachePath = new List<string>();
            using (IDataReader dr = SQLiteHelper.ExecuteReader(ConnectionString, cachePathSql, null))
            {
                while (dr.Read())
                {
                    cachePath.Add(dr.GetString(0));
                }
                dr.Close();
            }
            return cachePath;
        }
        /// <summary>
        /// 获取邮箱账号信息
        /// </summary>
        /// <returns></returns>
        public List<EmailplatForm> GetEmailInfos()
        {
            string EmailInfosSql = "SELECT emailAccount,emailStatus as Status,sendLimit,isChoose as IsChoose,AccountName,accountDescription, EmailClassName,emailTypeId as emailType,cachePath" +
                " FROM emailAccount_tbl LEFT JOIN  emailType_tbl ON emailAccount_tbl.emailTypeId=emailType_tbl.id";
            List<EmailplatForm> emailInfos = new List<EmailplatForm>();
            using (IDataReader dr = SQLiteHelper.ExecuteReader(ConnectionString, EmailInfosSql, null))
            {
                emailInfos = DBExtensions.ToList<EmailplatForm>(dr);
            }
            return emailInfos;
        }
        /// <summary>
        /// 删除账号信息
        /// </summary>
        /// <param name="socialplat"></param>
        public void DeleteEmailInfo(EmailAccountInfo emailAccountInfo)
        {
            string sql = $"DELETE FROM emailAccount_tbl WHERE emailAccount='{emailAccountInfo.Account}' AND emailTypeId='{emailAccountInfo.EmailTypeId}'";
            SQLiteHelper.ExecuteNonQuery(ConnectionString, sql, null);
        }
        /// <summary>
        /// 更新账号信息
        /// </summary>
        /// <param name="socialplatForm"></param>
        public void UpdateEmailInfo(EmailAccountInfo emailAccountInfo)
        {
            string sSql = $"UPDATE emailAccount_tbl SET  SendLimit='{emailAccountInfo.SendLimit}'" +
                $"  WHERE emailAccount='{emailAccountInfo.Account}' and emailTypeId='{emailAccountInfo.EmailTypeId}' ";
            try
            {
                SQLiteHelper.ExecuteIDResult(ConnectionString, sSql, null);
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                GC.Collect();
            }
        }
        /// <summary>
        /// 账号是否选中
        /// </summary>
        /// <param name="emailAccountInfos"></param>
        public void UpdateEmailAccountInfo(List<EmailAccountInfo> emailAccountInfos)
        {
           
            foreach (var emailAccountInfo in emailAccountInfos)
            {
                string sSql = $"UPDATE emailAccount_tbl SET  isChoose='{emailAccountInfo.IsChoose}'" +
                  $"  WHERE emailAccount='{emailAccountInfo.Account}' and emailTypeId='{emailAccountInfo.EmailTypeId}' ";
                try
                {
                    SQLiteHelper.ExecuteIDResult(ConnectionString, sSql, null);
                }
                catch (Exception ex)
                {
                    throw;
                }
                finally
                {
                    GC.Collect();
                }
            }
        }
        /// <summary>
        /// 更新账号信息
        /// </summary>
        /// <param name="id"></param>
        public void UpdateSocialPlatform(EmailplatForm socialplatForm)
        {
            string accountUpdateSql = "";
            if (socialplatForm.EmailAccount != "" && socialplatForm.EmailPassword != "")
            {
                accountUpdateSql = $"emailAccount='{socialplatForm.EmailAccount}',emailPassword='{socialplatForm.EmailPassword}',";
            }
            //string sSql = $"UPDATE socialAccount_tbl SET {accountUpdateSql} status=@status,accountName=@accountName," +
            //    "accountDescription=@accountDescription,createTime=@createTime WHERE id=@id ";

            string sSql = $"UPDATE emailAccount_tbl SET {accountUpdateSql} emailStatus='{socialplatForm.Status}'," +
                $"createTime='{socialplatForm.CreateTime}' WHERE emailAccount='{socialplatForm.EmailAccount}' and emailTypeId='{socialplatForm.EmailType}' ";
           try
            {
                SQLiteHelper.ExecuteIDResult(ConnectionString, sSql, null);
           }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                GC.Collect();
            }
        }
        /// <summary>
        /// 邮箱信息插入
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="socialplatForm"></param>
        public int InsertSocialPlatform(EmailplatForm socialplatForm)
        {
            try
            {
                string sSql = "INSERT INTO emailAccount_tbl (emailAccount,emailPassword,emailTypeId,emailStatus,sendLimit,isChoose,createTime,cachePath,AccountName,accountDescription)" +
                    $" VALUES ('{socialplatForm.EmailAccount}','{socialplatForm.EmailPassword}','{socialplatForm.EmailType}'" +
                    $",'{socialplatForm.Status}','{socialplatForm.SendLimit}','{socialplatForm.IsChoose}','{socialplatForm.CreateTime}','{socialplatForm.CachePath}','{socialplatForm.AccountName}','{socialplatForm.AccountDescription}')";
                object[] objectArray = {socialplatForm.EmailAccount, socialplatForm.EmailPassword,
                    socialplatForm.EmailType, socialplatForm.Status,socialplatForm.SendLimit,socialplatForm.IsChoose, DateTime.Now.Date};
                int id = Convert.ToInt32(SQLiteHelper.ExecuteIDResult(ConnectionString, sSql, null));
                return id;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        /// <summary>
        /// 插入邮箱模板
        /// </summary>
        /// <param name="emailTemPlate"></param>
        public void InsertEmailTemplate(EmailTemplate emailTemPlate )
        {
            try
            {
                string sSql = $"INSERT INTO mailTemplate_tbl(mailTheme,mailBody,isDelete)VALUES('{emailTemPlate.MailTheme}','{emailTemPlate.MailBody}','0')";
            int id = Convert.ToInt32(SQLiteHelper.ExecuteIDResult(ConnectionString, sSql, null));
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        /// <summary>
        /// 删除邮件模板（软删除）
        /// </summary>
        /// <param name="id"></param>
        public void DeletemailTemplate(string id)
        {
            string sql = $"UPDATE  mailTemplate_tbl SET isDelete='1',isChoose='0' WHERE Id='{id}'";
            try{
                SQLiteHelper.ExecuteNonQuery(ConnectionString, sql, null);
            }
            catch
            {
                throw;
            }
        }
        public void UpdateEmailTemplate(EmailTemplate template)
        {
            string sql = $"UPDATE mailTemplate_tbl SET mailTheme='{template.MailTheme}',mailBody='{template.MailBody}' WHERE id='{template.Id}'";
            try
            {
                SQLiteHelper.ExecuteNonQuery(ConnectionString, sql, null);
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// 获取所有邮件模板
        /// </summary>
        /// <returns></returns>
        public List<EmailTemplate> GetALLemailTemplate()
        {
            
            string sql = "SELECT * FROM mailTemplate_tbl WHERE isDelete='0'";
            List<EmailTemplate> EmailTemplateInfos = new List<EmailTemplate>();
            using (IDataReader dr = SQLiteHelper.ExecuteReader(ConnectionString, sql, null))
            {
                EmailTemplateInfos = DBExtensions.ToList<EmailTemplate>(dr);
            }
            return EmailTemplateInfos;
        }
        /// <summary>
        /// 修改模板是否选中
        /// </summary>
        /// <param name="emailTemPlates"></param>
        public void UpdateEmailTemplate(List<EmailTemplate> emailTemPlates)
        {
            foreach (var emailTemPlate in emailTemPlates)
            {
                string sql = $"UPDATE  mailTemplate_tbl SET isChoose='{emailTemPlate.IsChoose}' WHERE id='{emailTemPlate.Id}'";
                try
                {
                    SQLiteHelper.ExecuteIDResult(ConnectionString, sql, null);
                }
                catch (Exception ex)
                {
                    throw;
                }
                finally
                {
                    GC.Collect();
                }
            }
        }
        /// <summary>
        /// 获取选中的邮件模板
        /// </summary>
        /// <returns></returns>
        public List<EmailTemplate> GetCheckedEmailTemplate()
        {
            string sql = "SELECT id,mailTheme,mailBody,isChoose FROM mailTemplate_tbl WHERE isChoose='1'";
            List<EmailTemplate> emailTemplateInfos = new List<EmailTemplate>();
            using (IDataReader dr = SQLiteHelper.ExecuteReader(ConnectionString, sql, null))
            {
                emailTemplateInfos = DBExtensions.ToList<EmailTemplate>(dr);
            }
            return emailTemplateInfos;
        }
        /// <summary>
        /// 获取选中的邮箱账号信息
        /// </summary>
        /// <returns></returns>
        public List<EmailAccountInfo> GetCheckedEmailAccountInfo()
        {
            string sql = "SELECT emailAccount as Account,emailpassword as password,emailTypeId,isChoose,emailClassName,sendLimit,cachePath FROM emailAccount_tbl LEFT JOIN  emailType_tbl ON emailAccount_tbl.emailTypeId=emailType_tbl.id WHERE isChoose='1'";
            List<EmailAccountInfo> emailAccountInfos = new List<EmailAccountInfo>();
            using (IDataReader dr = SQLiteHelper.ExecuteReader(ConnectionString, sql, null))
            {
                emailAccountInfos = DBExtensions.ToList<EmailAccountInfo>(dr);
            }
            return emailAccountInfos;
        }
        /// <summary>
        /// 更新或者插入邮箱设置信息
        /// </summary>
        /// <param name="configDataString"></param>
        public void InsertAndUpdatemailConfig(string configDataString)
        {
            string sql = "select count(*) from mailConfig";
            object count= SQLiteHelper.ExecuteScalar(ConnectionString, sql, null);
            if (count.ToString()!="0")
            {
                sql = $"UPDATE mailConfig SET configData='{configDataString}',createtime='{DateTime.Now.Date}' WHERE Id = '1'";
                SQLiteHelper.ExecuteNonQuery(ConnectionString, sql, null);
            }
            else
            {
                sql = $"insert into mailConfig(configData,createtime) values('{configDataString}','{DateTime.Now.Date}')";
                SQLiteHelper.ExecuteNonQuery(ConnectionString, sql, null);
            }
        }
        /// <summary>
        /// 获取邮箱配置信息
        /// </summary>
        /// <returns></returns>
        public ConfigData GetmailConfig()
        {
            string sql = "SELECT configData FROM mailConfig ";
            ConfigData defaultConfig = new ConfigData();
            using (IDataReader dr = SQLiteHelper.ExecuteReader(ConnectionString, sql, null))
            {
                while (dr.Read())
                {
                   // Console.WriteLine(dr.GetString(0));
                    defaultConfig = JsonConvert.DeserializeObject<ConfigData>(dr.GetString(0));
                }
            }
            return defaultConfig;
        }
        public Dictionary<string, int> GetCurrentDaySendCount()
        {
            string sql = $"SELECT emailAccount,emailTypeId,Count(*) as count FROM emailSendRecord_tbl WHERE sendtime='{DateTime.Now.Date}'group by emailAccount,emailTypeId";//eamilAccount
            try
            {
                using (IDataReader dr = SQLiteHelper.ExecuteReader(ConnectionString, sql, null))
                {
                    Dictionary<string, int> currentDaySendCountDic = new Dictionary<string, int>();
                    List<EmailCurrentDaySendCount> list = DBExtensions.ToList<EmailCurrentDaySendCount>(dr);
                    if (list != null)
                    {
                        foreach (var item in list)
                        {
                            currentDaySendCountDic.Add(item.emailTypeId+item.EmailAccount,item.Count);
                        }
                    }
                    return currentDaySendCountDic;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
        #endregion
        public void InsertEmailType(string emailName,string className)
        {
            string sSql = "INSERT INTO email_Type (emailName,className) VALUES (@emailName,@className)";
            object[] obj = { emailName, className };
            try
            {
                SQLiteHelper.ExecuteNonQuery(ConnectionString, sSql, obj);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}


