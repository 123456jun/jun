using System;
using System.Windows.Forms;
using CefSharp;
using CefWebBrowser;
using System.Threading;
using EmailAutoMation.Model;
using WolfORC;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using TradeWolfRobot.EmailAutomation.Model;
using CefWebBrowser.CefModel;
using CefSharpWebBrowser.CefModel;
using CefSharpWebBrowser.CefHelper;
using System.Collections.Generic;

namespace TradeWolfRobot.EmailAutomation
{
    public partial class EmailForm : Form
    {
        #region
        EmailBase _EmailBase;


        //        "<div>Dear friend<br />How are you ?<br />I hope you are doing fine 。<br />Glad to hear that you're in the market for handbags. We're the MANUFAC<strong>TURER that supply handbags for CARVEL A, Liujo and River Island with high quality and pretty competitive price.</strong><br /><strong>We have aready passed the factory a</strong>udt ot Sedex. please tind the audt reports in atachment,I'm sure that our factory is good enough to meet your re<span style="color: #e03e2d;">quirement.Any comments, that will be appreciated !</span><br /><span style="color: #e03e2d;">What do you think of the style of our n</span>ew prod<span style="color: #f1c40f;">uct this time?</span><br />Would you like to send you samples？<br />Thanks &amp; Best regards,</div>
        //<div>&nbsp;</div>
        //<div>jane<br /> sales manager<br /> email: jane @fcostshop.com<br />telephone/wechat: &nbsp; +86 13729940256</div>"


        public CefWebBrowserControl CefWebBrowser { get; set; }
        public EmailForm()
        {
            InitializeComponent();
            this.FormClosed += EmailForm_FormClosed;
        }
        public CefWebBrowserControl getweb()
        {
            return CefWebBrowser;
        }
        private void EmailForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            CefWebBrowser.Dispose();
            Cef.Shutdown();
        }
        public EmailForm(string className)
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 1;
            _EmailBase = EmailBuilder.CreateInstance().CreateEmail(comboBox1.Text);
            _EmailBase.CefWebBrowser.Dock = DockStyle.Fill;
            CefWebBrowser = _EmailBase.CefWebBrowser;
            panel1.Controls.Clear();
            panel1.Controls.Add(_EmailBase.CefWebBrowser);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _EmailBase = EmailBuilder.CreateInstance().CreateEmail(comboBox1.Text);
            _EmailBase.CefWebBrowser.Dock = DockStyle.Fill;
            CefWebBrowser = _EmailBase.CefWebBrowser;
            panel1.Controls.Clear();
            panel1.Controls.Add(_EmailBase.CefWebBrowser);
        }


        private void button11_Click(object sender, EventArgs e)
        {
            _EmailBase.ClickWriteEmail();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            _EmailBase.WriteEmailAddress(AddressBox.Text);
        }

        private void button13_Click(object sender, EventArgs e)
        {
            _EmailBase.WriteEmailTheme(ThemBox.Text);
        }
        private void button14_Click(object sender, EventArgs e)
        {
            string crmUrl = "https://trace.waimaolang.cn/crlf-hyperf/mbot/";
            CrmEmailStorage crmEmailStorage = new CrmEmailStorage();
            crmEmailStorage.Email = AddressBox.Text.Trim();
            crmEmailStorage.Id = "AutoWolf_68417";
            crmEmailStorage.Uuid = Guid.NewGuid().ToString().Replace("-", "");
            crmEmailStorage.Time = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString();
            string newUrl = "Id={0}&Mailbox={1}&Uuid={2}&Time={3}";
            newUrl = string.Format(newUrl, crmEmailStorage.Id, crmEmailStorage.Email, crmEmailStorage.Uuid, crmEmailStorage.Time);
            string encryptStr = Encrypt(newUrl);
            string resultUrl = crmUrl + encryptStr + ".gif";
            string decryptStr = Decrypt(DecodeBase64(Encoding.UTF8, encryptStr));
            string img = "<img src=\"{0}{1}\" border=\"0\" width=\"0\" height=\"0\">";
            string body = BodyBox.Text + string.Format(img, crmUrl, resultUrl);
            body = BodyBox.Text;
            _EmailBase.WriteEmailBody(body);
        }


        public class CrmEmailStorage
        {
            /// <summary>
            /// id
            /// </summary>
            public string Id { get; set; }
            /// <summary>
            /// 邮箱
            /// </summary>
            public string Email { get; set; }
            /// <summary>
            /// 时间戳
            /// </summary>
            public string Time { get; set; }
            /// <summary>
            /// uuid
            /// </summary>
            public string Uuid { get; set; }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            EmailInfo emailInfo = new EmailInfo()
            {
                Addressee  = AddressBox.Text,
                Body = BodyBox.Text,
                Theme = ThemBox.Text
            };
            _EmailBase.ExecuteEmailSend(emailInfo);
            //JsExecuteResult jsExecuteResult1 = _EmailBase.ClickWriteEmail();
            //JsExecuteResult jsExecuteResult2 = _EmailBase.WriteEmailAddress(AddressBox.Text);
            //JsExecuteResult jsExecuteResult3 = _EmailBase.WriteEmailTheme(ThemBox.Text);
            //JsExecuteResult jsExecuteResult4 = _EmailBase.WriteEmailBody(BodyBox.Text);
            //JsExecuteResult jsExecuteResult5 = _EmailBase.ClickSendEmail();
        }
        #endregion

        string urlSelect = "https://trace.waimaolang.cn/Sundry/MbotSmtpTrace/index?params=XXXXXX&orderBy=XXXXX&p=1&pagesize=10;";

        private void button3_Click(object sender, EventArgs e)
        {
            _EmailBase.ClickSendEmail();
            string address = AddressBox.Text.Trim();
        }
        int count = 1500;
        private void button5_Click(object sender, EventArgs e)
        {
            List<string> data = new List<string>()
            { 
                "1737138709@qq.com", "38709@qq.com", "23638709@qq.com"
            };
            List<String> list = new List<string>();
            _EmailBase.ClickCCList(data, out list);

            _EmailBase.ClickBCCList(data, out list);
            //Thread t = new Thread(new ThreadStart(() =>
            //{
            //    count = (int)numericUpDown1.Value;
            //    ex();
            //    //string isStr = "QMVerify_QMDialog_verify_img_code";
            //    //string jsStr = "";
            //    //_EmailBase.CefWebBrowser.ExecuteJs();
            //    //WolfOrcFateadm wolfOrcFateadm = new WolfOrcFateadm();
            //    //try
            //    //{
            //    //    var result = wolfOrcFateadm.CharacterRecognition(bytes);
            //    //}
            //    //catch (Exception ex)
            //    //{
            //    //}
            //}));
            //t.Start();
        }

        public void ex()
        {

            while (true)
            {
                CefSharpWebBrowser.CefHelper.JsExecuteHelp jsExecuteHelp = new CefSharpWebBrowser.CefHelper.JsExecuteHelp(_EmailBase.CefWebBrowser);
                var d = jsExecuteHelp.GetDomAbsLocationById("QMVerify_QMDialog_changelink");
                if (!d.Success)
                {
                    Thread.Sleep(5*1000);
                    IFrame frame = CefWebBrowser.GetFrame("mainFrame");
                    string jsSt = @"document.getElementsByName('sendbtn')[0].click();";
                    _EmailBase.CefWebBrowser.ExecuteJs(jsSt, frame);
                    Thread.Sleep(3 * 1000);
                    d = jsExecuteHelp.GetDomAbsLocationById("QMVerify_QMDialog_changelink");
                    if (!d.Success)
                    {
                        ex();
                        return;
                    }
                }
                CefSharpWebBrowser.CefModel.DomAbsLocation iframeAbsLocation = JsonConvert.DeserializeObject<CefSharpWebBrowser.CefModel.DomAbsLocation>(d.Result);
                _EmailBase.CefWebBrowser.CefMouseClick(iframeAbsLocation.top + 3, iframeAbsLocation.left + 5);
                Thread.Sleep(800);
                string jsString = @"(function() {
                                var getDataFromImg = function(img) {
                                var canvas = document.createElement('canvas');
                                var context = canvas.getContext('2d');
                                canvas.width = 130; //验证码长
                                canvas.height = 53;//验证码宽
                                context.drawImage(img, 0, 0);
                                var dataURL = canvas.toDataURL('image/png');

                                return dataURL.replace(/ ^data:image\/ (png | jpg); base64,/, '');
                                }
                                //获取图片标签，在开发工具选中img标签，右键copy -> copy js xpath
                                var img = document.querySelector('#QMVerify_QMDialog_verify_img_code');
                                return getDataFromImg(img);
                                })();";
                var jsExecuteResult = CefWebBrowser.ExecuteJs(jsString);
                if (jsExecuteResult.Success == false)
                {
                    Thread.Sleep(2);
                    _EmailBase.CefWebBrowser.ExecuteJs("document.getElementById('QMVerify_QMDialog_btnCancel').click()");
                    //jsExecuteHelp.ExecuteClickById("QMVerify_QMDialog_btnCancel");
                    Thread.Sleep(2 * 1000);
                    var j = _EmailBase.CefWebBrowser.ExecuteJs("document.getElementsByClassName('btn_gray btn_space')[0].click()");
                    Thread.Sleep(2 * 1000);
                    IFrame frame = CefWebBrowser.GetFrame("mainFrame");
                    jsString = @"document.getElementsByName('sendbtn')[0].click();";
                    _EmailBase.CefWebBrowser.ExecuteJs(jsString, frame);
                    Thread.Sleep(3 * 1000);
                    //break;
                    continue;
                }
                string verificationCodeBase64 = jsExecuteResult.Result.Replace("data:image/png;base64,", "").Replace("data:image/jgp;base64,", "").Replace("data:image/jpg;base64,", "").Replace("data:image/jpeg;base64,", "");
                byte[] bytes = Convert.FromBase64String(verificationCodeBase64);
                MemoryStream memStream = new MemoryStream(bytes);
                Image mImage = Image.FromStream(memStream);
                mImage.Save(@"D:\cjzProject\外贸狼智能营销源码\mkRobotClientForeign串行版\EmailAutoMation\bin\x86\Debug\验证码图片" + $"/{count}.jpg");
                count++;
                this.Invoke(new Action(() =>
                {
                    pictureBox1.Image = mImage;
                }));
                Application.DoEvents();
            }
        }

        /// <summary>
        /// 获取qq验证码
        /// </summary>
        /// <returns></returns>
        public byte[] getEmailVerifycodeinput()
        {
            byte[] vs = new byte[1000];
            return vs;
        }
        private void button6_Click(object sender, EventArgs e)
        {
            EmailInfo emailInfo = new EmailInfo()
            {
               Addressee  = AddressBox.Text + ";" + AddressBox.Text + ";" + AddressBox.Text + ";" + AddressBox.Text + ";" + AddressBox.Text + ";" + AddressBox.Text + ";" + AddressBox.Text + ";",
                Body = BodyBox.Text,
                Theme = ThemBox.Text
            };
            button6.Enabled = false;
            stop = false;
            Thread t = new Thread(new ThreadStart(() =>
            {
                while (true)
                {
                    if (stop)
                    {
                        return;
                    }
                    _EmailBase.ExecuteInterval = 1;
                    _EmailBase.ExecuteEmailSend(emailInfo);
                    Thread.Sleep(2 * 1000);
                }
            }));
            t.Start();
        }
        bool stop = false;
        private void button7_Click(object sender, EventArgs e)
        {
            stop = true;
            button6.Enabled = true;
        }


        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="value">明文</param>
        /// <param name="key">密钥</param>
        /// <returns></returns>
        public static string Encrypt(string value, string key = "f876f0634e2d4fe8")
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            if (key == null) throw new Exception("未将对象引用设置到对象的实例。");
            if (key.Length != 16) throw new Exception("指定的密钥长度必须为16位。");
            var _keyByte = Encoding.UTF8.GetBytes(key);
            var _valueByte = Encoding.UTF8.GetBytes(value);
            using (var aes = new RijndaelManaged())
            {
                aes.Key = _keyByte;
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.PKCS7;
                var cryptoTransform = aes.CreateEncryptor();
                var resultArray = cryptoTransform.TransformFinalBlock(_valueByte, 0, _valueByte.Length);
                string result = Convert.ToBase64String(resultArray, 0, resultArray.Length);
                return Base64Encode(Encoding.UTF8, result);
            }
        }

        /// <summary>
        /// Base64加密
        /// </summary>
        /// <param name="encodeType">加密采用的编码方式</param>
        /// <param name="source">待加密的明文</param>
        /// <returns></returns>
        public static string Base64Encode(Encoding encodeType, string source)
        {
            string encode = string.Empty;
            byte[] bytes = encodeType.GetBytes(source);
            try
            {
                encode = Convert.ToBase64String(bytes);
            }
            catch
            {
                encode = source;
            }
            return encode;
        }


        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="value">密文</param>
        /// <param name="key">密钥</param>
        /// <returns></returns>
        public static string Decrypt(string value, string key = "f876f0634e2d4fe8")
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            if (key == null) throw new Exception("未将对象引用设置到对象的实例。");
            if (key.Length != 16) throw new Exception("指定的密钥长度必须为16位。");
            var _keyByte = Encoding.UTF8.GetBytes(key);
            var _valueByte = Convert.FromBase64String(value);
            using (var aes = new RijndaelManaged())
            {
                aes.Key = _keyByte;
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.PKCS7;
                var cryptoTransform = aes.CreateDecryptor();
                var resultArray = cryptoTransform.TransformFinalBlock(_valueByte, 0, _valueByte.Length);
                return Encoding.UTF8.GetString(resultArray);
            }
        }

        /// <summary>
        /// base64解密
        /// </summary>
        /// <param name="encode"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static string DecodeBase64(Encoding encode,string result)
        {
            string decode = "";
            byte[] bytes = Convert.FromBase64String(result);
            try
            {
                decode = encode.GetString(bytes);
            }
            catch
            {
                decode = result;
            }
            return decode;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ////LoginState loginState = _EmailBase.IsLogin("a15361913607@gmail.com","zxc1737138709");
            ////LoginState loginState= _EmailBase.IsLogin("a15361913607", "a1234561");
            //JsExecuteHelp _jsExecuteHelp = new JsExecuteHelp(CefWebBrowser);
            //string jsframe = "login_frame";
            //IFrame frame = CefWebBrowser.GetFrame(jsframe);
            //JsExecuteResult jsExecuteResult1 = null;
            //JsExecuteResult jsExecuteResult = _jsExecuteHelp.GetDomAbsLocationByClass("inputOuter", null, frame);
            //DomAbsLocation domAbsLocation1 = JsonConvert.DeserializeObject<DomAbsLocation>(jsExecuteResult.Result);
            //string jsStr = "login_frame";// "qqLoginCard";
            //jsExecuteResult = _jsExecuteHelp.GetDomAbsLocationById(jsStr);
            //DomAbsLocation domAbsLocation = JsonConvert.DeserializeObject<DomAbsLocation>(jsExecuteResult.Result);
            //float a = domAbsLocation.top + domAbsLocation1.top;
            //float b = domAbsLocation.left + domAbsLocation1.left;
            ////CefWebBrowser.CefMouseClick(domAbsLocation.top + domAbsLocation1.top + 5, domAbsLocation.left + domAbsLocation1.left + 15,false);
            ////CefWebBrowser.CefMouseClick(domAbsLocation.top + domAbsLocation1.top + 5, domAbsLocation.left + domAbsLocation1.left + 15, false);
            //CefWebBrowser.CefMouseClick(domAbsLocation.top + domAbsLocation1.top + 5, domAbsLocation.left + domAbsLocation1.left + 15);
            //Thread.Sleep(1000);

            //jsStr = "document.getElementById('uin_tips').innerText=''";
            //CefWebBrowser.ExecuteJs(jsStr, frame);
            //CefWebBrowser.CefKeyBoardInput("1737138709@qq.com");
            string account = textBox1.Text.Trim();
            string password = textBox2.Text.Trim();
            LoginState loginState = _EmailBase.IsLogin(account, password);
            if (loginState.IsLoginSuccess)
            {
                MessageBox.Show("登录成功");
                return;
            }

            if (loginState.IsLoginOutTime)
            {
                MessageBox.Show("登录超时");
            }
            if (loginState.IsVCode)
            {
                MessageBox.Show("出现验证码");
            }
            else
            {
                MessageBox.Show("登陆失败");
            }
           
        }
    }
}

