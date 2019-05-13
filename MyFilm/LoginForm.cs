using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace MyFilm
{
    public partial class LoginForm : Form
    {
        /// <summary>
        /// 配置文件路径
        /// </summary>
        private static String configPath = Path.Combine(
            CommonString.AppDataFolder, "myfilm.xml");

        private RealOrFake4KWebDataCapture.RealOrFake4KWebDataCaptureResult
            webDataCaptureResult = null;

        [XmlRoot("DataBaseConfig")]
        public class DataBaseConfig
        {
            [XmlAttribute("Name")]
            public String Name;

            [XmlAttribute("WebDataCaptureTime")]
            public String WebDataCaptureTime;
        }

        [XmlRoot("UserNameAndPassWord")]
        public class UserNameAndPassWord
        {
            [XmlAttribute("UserName")]
            public String UserName;

            [XmlAttribute("PassWord")]
            public String PassWord;
        }

        [Serializable]
        [XmlRoot("LoginConfig")]
        public class LoginConfig
        {
            [XmlAttribute("SelectedIP")]
            public String selectedIP;

            [XmlAttribute("SelectedUserName")]
            public String selectedUserName;

            [XmlAttribute("SelectedDataBaseName")]
            public String selectedDataBaseName;

            [XmlArrayItem("HostIPs")]
            public List<String> hostIPs;

            [XmlArrayItem("UserNameAndPassWords")]
            public List<UserNameAndPassWord> userNameAndPassWords;

            [XmlArrayItem("DataBaseConfigs")]
            public List<DataBaseConfig> dataBaseConfigs;

            [XmlIgnore]
            public int UserNameMaxLength
            {
                get
                {
                    int maxLength = 0;
                    userNameAndPassWords.ForEach(x =>
                    {
                        if (maxLength < x.UserName.Length)
                            maxLength = x.UserName.Length;
                    });
                    return maxLength;
                }
            }

            [XmlIgnore]
            public String SelectedPassWord
            {
                get
                {
                    UserNameAndPassWord temp = userNameAndPassWords.Find(
                        x => x.UserName == selectedUserName);
                    if (temp == null) return null;
                    else return temp.PassWord;
                }
            }

            [XmlIgnore]
            public String SelectedDataBaseWebDataCaptureTime
            {
                get
                {
                    DataBaseConfig temp = dataBaseConfigs.Find(
                        x => x.Name == selectedDataBaseName);
                    if (temp == null) return null;
                    else return temp.WebDataCaptureTime;
                }
                set
                {
                    DataBaseConfig temp = dataBaseConfigs.Find(
                        x => x.Name == selectedDataBaseName);
                    if (temp != null) temp.WebDataCaptureTime = value;
                }
            }
        }

        private static LoginConfig loginConfig = null;

        public LoginForm()
        {
            InitializeComponent();

            this.ControlBox = false;

            if (!Directory.Exists(CommonString.AppDataFolder))
                Directory.CreateDirectory(CommonString.AppDataFolder);
        }

        // 鼠标双击标题栏消息
        private const int WM_NCLBUTTONDBLCLK = 0xA3;

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                // 屏蔽双击最大化
                case WM_NCLBUTTONDBLCLK:
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            this.Icon = Properties.Resources.Film;

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(Path.Combine(CommonString.AppDataFolder, "logs", "myfilm.log"),
                rollingInterval: RollingInterval.Day)
                .CreateLogger();
            Log.Information("LoginForm load");

            LoadXml();
            InitComboxIP();
            InitComboxUser();
            InitComboxDataBase();
        }

        private static LoginConfig GetInitLoginConfig()
        {
            LoginConfig initDBStruct = new LoginConfig()
            {
                selectedIP = "127.0.0.1",
                selectedUserName = "",
                selectedDataBaseName = "",
                hostIPs = new List<string>() { "127.0.0.1" },
                userNameAndPassWords = new List<UserNameAndPassWord>(),
                dataBaseConfigs = new List<DataBaseConfig>()
            };

            return initDBStruct;
        }

        private static void LoadXml()
        {
            XmlSerializer ser = new XmlSerializer(typeof(LoginConfig));
            if (File.Exists(configPath))
            {
                using (FileStream fs = File.OpenRead(configPath))
                {
                    try { loginConfig = ser.Deserialize(fs) as LoginConfig; }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK);
                    }
                }
            }

            if (loginConfig == null) loginConfig = GetInitLoginConfig();
            else
            {
                if (loginConfig.hostIPs == null)
                    loginConfig.hostIPs = new List<string>();

                if (loginConfig.userNameAndPassWords == null)
                    loginConfig.userNameAndPassWords =
                        new List<UserNameAndPassWord>();
                else
                {
                    // 解密
                    loginConfig.userNameAndPassWords.ForEach(
                        x => x.PassWord = Decrypt(x.PassWord));
                }

                if (loginConfig.dataBaseConfigs == null)
                    loginConfig.dataBaseConfigs = new List<DataBaseConfig>();
            }

            if (String.IsNullOrWhiteSpace(loginConfig.selectedIP))
                loginConfig.selectedIP = "127.0.0.1";

            if (!loginConfig.hostIPs.Contains(loginConfig.selectedIP))
                loginConfig.hostIPs.Add(loginConfig.selectedIP);
        }

        private static void SaveXml()
        {
            LoginConfig saveLoginConfig = new LoginConfig()
            {
                selectedIP = loginConfig.selectedIP,
                selectedUserName = loginConfig.selectedUserName,
                selectedDataBaseName = loginConfig.selectedDataBaseName,
                hostIPs = loginConfig.hostIPs
                .ConvertAll<string>(
                    x => { return x; }),
                userNameAndPassWords = loginConfig.userNameAndPassWords
                .ConvertAll<UserNameAndPassWord>(
                    x =>
                    {
                        return new UserNameAndPassWord()
                        {
                            UserName = x.UserName,
                            PassWord = Encryption(x.PassWord)
                        };
                    }),
                dataBaseConfigs = loginConfig.dataBaseConfigs
                .ConvertAll<DataBaseConfig>(
                    x =>
                    {
                        return new DataBaseConfig()
                        {
                            Name = x.Name,
                            WebDataCaptureTime = x.WebDataCaptureTime
                        };
                    })
            };

            XmlSerializer ser = new XmlSerializer(typeof(LoginConfig));
            using (FileStream fs = File.Create(configPath))
            {
                ser.Serialize(fs, saveLoginConfig);
            }
        }

        public static void UpdateWebDataCaptureTimeAndSaveXml(DateTime dateTime)
        {
            DataBaseConfig temp = loginConfig.dataBaseConfigs.Find(
                x => x.Name == loginConfig.selectedDataBaseName);
            temp.WebDataCaptureTime = dateTime.ToString("yyyy-MM-dd HHH:mm:ss");
            SaveXml();
        }

        private void InitComboxIP()
        {
            this.comboBoxIP.SuspendLayout();
            this.comboBoxIP.Items.Clear();
            this.comboBoxIP.Items.AddRange(loginConfig.hostIPs.ToArray());
            this.comboBoxIP.Text = loginConfig.selectedIP;
            this.comboBoxIP.ResumeLayout();
        }

        private void InitComboxUser()
        {
            this.comboBoxUser.SuspendLayout();
            this.comboBoxUser.Items.Clear();

            int len = loginConfig.UserNameMaxLength + 2;
            if (len < 20) len = 20;

            this.comboBoxUser.Items.AddRange(
                loginConfig.userNameAndPassWords.ConvertAll<string>(
                    x =>
                    {
                        //return string.Format("{0}*****",
                        //    x.UserName.PadRight(len, ' '));
                        return x.UserName;
                    }).ToArray());
            this.comboBoxUser.Text = loginConfig.selectedUserName;

            this.comboBoxUser.ResumeLayout();
        }

        private void InitComboxDataBase()
        {
            this.comboBoxDataBase.SuspendLayout();
            this.comboBoxDataBase.Items.Clear();
            this.comboBoxDataBase.Items.AddRange(
                loginConfig.dataBaseConfigs.ConvertAll<string>(
                    x => x.Name).ToArray());
            this.comboBoxDataBase.Text = loginConfig.selectedDataBaseName;
            this.comboBoxDataBase.ResumeLayout();
        }

        public void SetWebCaptureDataResult(
            RealOrFake4KWebDataCapture.RealOrFake4KWebDataCaptureResult rst)
        {
            this.webDataCaptureResult = rst;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            CommonString.DbIP = this.comboBoxIP.Text;
            CommonString.DbUserName = this.comboBoxUser.Text;
            CommonString.DbPassword = this.textBoxPwd.Text;
            CommonString.DbName = this.comboBoxDataBase.Text;

            List<String> databaseNameList = null;
            try
            {
                databaseNameList = SqlData.QueryAllDataBaseNames();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK);
                return;
            }

            if (!databaseNameList.Contains(CommonString.DbName))
            {
                if (MessageBox.Show(String.Format("数据库 \"{0}\" 不存在，要创建吗？",
                    CommonString.DbName), "提示",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        SqlData.CreateDataBase(CommonString.DbName);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK);
                        return;
                    }
                }
                else return;
            }

            try
            {
                SqlData sqlData = SqlData.GetInstance();
                //sqlData.OpenMySql();
                sqlData.CreateTables();

                if (!loginConfig.hostIPs.Contains(CommonString.DbIP))
                    loginConfig.hostIPs.Add(CommonString.DbIP);

                if (loginConfig.userNameAndPassWords.FindIndex(
                    x => x.UserName == CommonString.DbUserName) == -1)
                    loginConfig.userNameAndPassWords.Add(
                        new UserNameAndPassWord()
                        {
                            UserName = CommonString.DbUserName,
                            PassWord = CommonString.DbPassword
                        });

                if (loginConfig.dataBaseConfigs.FindIndex(
                    x => x.Name == CommonString.DbName) == -1)
                    loginConfig.dataBaseConfigs.Add(
                        new DataBaseConfig()
                        {
                            Name = CommonString.DbName,
                            WebDataCaptureTime =
                            DateTime.MinValue.ToString("yyyy-MM-dd HHH:mm:ss")
                        });

                loginConfig.selectedUserName = CommonString.DbUserName;
                loginConfig.selectedIP = CommonString.DbIP;
                loginConfig.selectedDataBaseName = CommonString.DbName;

                DateTime dateTimeRead = DateTime.MinValue;
                DateTime.TryParse(loginConfig.SelectedDataBaseWebDataCaptureTime,
                    out dateTimeRead);

                TimeSpan ts = DateTime.Now.Subtract(dateTimeRead);
                if (ts.Days > 3)
                {
                    WaitingForm waitingForm = new WaitingForm(SetWebCaptureDataResult);
                    waitingForm.ShowDialog();

                    if (this.webDataCaptureResult.code >= 0)
                        dateTimeRead = this.webDataCaptureResult.crawlTime;
                    MessageBox.Show(this.webDataCaptureResult.strMsg);
                }

                loginConfig.SelectedDataBaseWebDataCaptureTime
                    = dateTimeRead.ToString("yyyy-MM-dd HHH:mm:ss");

                SaveXml();
                sqlData.FillRamData();

                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "LoginForm login fail");
                MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK);
            }
        }

        private void LoginForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Log.Information("LoginForm exit with login {LoginState}",
                this.DialogResult == DialogResult.OK ? "success\r\n" : "fail\r\n\r\n");
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="expressText"></param>
        /// <returns></returns>
        private static string Encryption(string expressText)
        {
            CspParameters param = new CspParameters();
            // 密匙容器的名称，保持加密解密一致才能解密成功
            param.KeyContainerName = CommonString.RSAKeyContainerName;
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(param))
            {
                // 将要加密的字符串转换为字节数组
                byte[] plainData = Encoding.Default.GetBytes(expressText);
                // 加密
                byte[] encryptdata = rsa.Encrypt(plainData, false);
                // 将加密后的字节数组转换为Base64字符串
                return Convert.ToBase64String(encryptdata);
            }
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="cipherText"></param>
        /// <returns></returns>
        private static string Decrypt(string cipherText)
        {
            CspParameters param = new CspParameters();
            param.KeyContainerName = CommonString.RSAKeyContainerName;
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(param))
            {
                byte[] encryptData = Convert.FromBase64String(cipherText);
                byte[] decryptData = rsa.Decrypt(encryptData, false);
                return Encoding.Default.GetString(decryptData);
            }
        }

        private void comboBoxUser_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedIndex = this.comboBoxUser.SelectedIndex;

            if (selectedIndex >= 0 &&
                selectedIndex < this.comboBoxUser.Items.Count)
            {
                this.textBoxPwd.Text =
                    loginConfig.userNameAndPassWords[selectedIndex].PassWord;
            }
        }
    }
}
