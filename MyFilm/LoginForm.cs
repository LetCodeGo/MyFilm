using System;
using System.Collections.Generic;
using System.IO;
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

        [Serializable]
        [XmlRoot("DBCONFIG")]
        public class DBStruct
        {
            [XmlElement("DefaultDataBaseIP")]
            public String DefaultDataBaseIP;

            [XmlElement("DefaultDataBaseUserName")]
            public String DefaultDataBaseUserName;

            [XmlElement("DefaultDataBasePassWord")]
            public String DefaultDataBasePassWord;

            [XmlElement("DefaultDataBaseName")]
            public String DefaultDataBaseName;

            [XmlArrayItem("DataBaseIP")]
            public List<String> DataBaseIPs;

            [XmlArrayItem("DataBaseUserName")]
            public List<String> DataBaseUserNames;

            [XmlArrayItem("DataBasePassWord")]
            public List<String> DataBasePassWords;

            [XmlArrayItem("DataBaseName")]
            public List<String> DataBaseNames;

            [XmlElement("WebDataCaptureTime")]
            public String WebDataCaptureTime;
        }

        private DBStruct dbStruct = null;

        public LoginForm()
        {
            InitializeComponent();

            dbStruct = new DBStruct();
            dbStruct.DefaultDataBaseIP = String.Empty;
            dbStruct.DefaultDataBaseUserName = String.Empty;
            dbStruct.DefaultDataBasePassWord = String.Empty;
            dbStruct.DefaultDataBaseName = String.Empty;
            dbStruct.DataBaseIPs = new List<String>();
            dbStruct.DataBaseUserNames = new List<String>();
            dbStruct.DataBasePassWords = new List<String>();
            dbStruct.DataBaseNames = new List<String>();
            dbStruct.WebDataCaptureTime = DateTime.MinValue.ToString("yyyy-MM-dd HHH:mm:ss");

            if (!Directory.Exists(CommonString.AppDataFolder))
                Directory.CreateDirectory(CommonString.AppDataFolder);
        }

        private void LoadXml()
        {
            XmlSerializer ser = new XmlSerializer(typeof(DBStruct));
            if (File.Exists(configPath))
            {
                using (FileStream fs = File.OpenRead(configPath))
                {
                    try { dbStruct = ser.Deserialize(fs) as DBStruct; }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK);
                    }
                }
            }

            if (String.IsNullOrWhiteSpace(dbStruct.DefaultDataBaseIP))
                dbStruct.DefaultDataBaseIP = "127.0.0.1";
        }

        private void SaveXml()
        {
            XmlSerializer ser = new XmlSerializer(typeof(DBStruct));
            using (FileStream fs = File.Create(configPath))
            {
                ser.Serialize(fs, dbStruct);
            }
        }

        private void InitComboxIP()
        {
            this.comboBoxIP.SuspendLayout();
            this.comboBoxIP.Items.Clear();
            this.comboBoxIP.Items.AddRange(dbStruct.DataBaseIPs.ToArray());
            this.comboBoxIP.Text = dbStruct.DefaultDataBaseIP;
            this.comboBoxIP.ResumeLayout();
        }

        private void InitComboxUser()
        {
            this.comboBoxUser.SuspendLayout();
            this.comboBoxUser.Items.Clear();
            this.comboBoxUser.Items.AddRange(dbStruct.DataBaseUserNames.ToArray());
            this.comboBoxUser.Text = dbStruct.DefaultDataBaseUserName;
            this.comboBoxUser.ResumeLayout();
        }

        private void InitComboxPwd()
        {
            this.comboBoxPwd.SuspendLayout();
            this.comboBoxPwd.Items.Clear();
            this.comboBoxPwd.Items.AddRange(dbStruct.DataBasePassWords.ToArray());
            this.comboBoxPwd.Text = dbStruct.DefaultDataBasePassWord;
            this.comboBoxPwd.ResumeLayout();
        }

        private void InitComboxDataBase()
        {
            this.comboBoxDataBase.SuspendLayout();
            this.comboBoxDataBase.Items.Clear();
            this.comboBoxDataBase.Items.AddRange(dbStruct.DataBaseNames.ToArray());
            this.comboBoxDataBase.Text = dbStruct.DefaultDataBaseName;
            this.comboBoxDataBase.ResumeLayout();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            this.Icon = Properties.Resources.ico;

            LoadXml();
            InitComboxIP();
            InitComboxUser();
            InitComboxPwd();
            InitComboxDataBase();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            CommonString.DbIP = this.comboBoxIP.Text;
            CommonString.DbUserName = this.comboBoxUser.Text;
            CommonString.DbPassword = this.comboBoxPwd.Text;
            CommonString.DbName = this.comboBoxDataBase.Text;

            try
            {
                SqlData sqlData = SqlData.GetInstance();
                sqlData.OpenMySql();
                sqlData.CreateTables();

                DateTime dateTime = DateTime.MinValue;
                if (this.dbStruct.WebDataCaptureTime != null)
                    dateTime = DateTime.Parse(this.dbStruct.WebDataCaptureTime);
                DateTime dateTimeNow = DateTime.Now;
                TimeSpan ts = dateTimeNow.Subtract(dateTime);
                if (ts.Days > 1)
                {
                    string errMsg = "";
                    List<string> infoList = RealOrFake4KWebDataCapture.CrawlData(ref errMsg);
                    if (infoList == null)
                    {
                        MessageBox.Show(string.Format("从网址\n{0}\n抓取数据失败\n{1}",
                            RealOrFake4KWebDataCapture.webPageAddress, errMsg));
                    }
                    else if (infoList.Count > 0) sqlData.Update4KInfo(infoList);
                }

                this.dbStruct.DefaultDataBaseUserName = CommonString.DbUserName;
                this.dbStruct.DefaultDataBasePassWord = CommonString.DbPassword;
                this.dbStruct.DefaultDataBaseIP = CommonString.DbIP;
                this.dbStruct.DefaultDataBaseName = CommonString.DbName;

                if (!dbStruct.DataBaseIPs.Contains(CommonString.DbIP))
                    dbStruct.DataBaseIPs.Add(CommonString.DbIP);
                if (!dbStruct.DataBaseUserNames.Contains(CommonString.DbUserName))
                    dbStruct.DataBaseUserNames.Add(CommonString.DbUserName);
                if (!dbStruct.DataBasePassWords.Contains(CommonString.DbPassword))
                    dbStruct.DataBasePassWords.Add(CommonString.DbPassword);
                if (!dbStruct.DataBaseNames.Contains(CommonString.DbName))
                    dbStruct.DataBaseNames.Add(CommonString.DbName);

                dbStruct.WebDataCaptureTime = dateTimeNow.ToString("yyyy-MM-dd HHH:mm:ss");

                SaveXml();

                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK);
            }
        }
    }
}
