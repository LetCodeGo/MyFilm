using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace MyFilm
{
    public partial class LoginForm : Form
    {
        /// <summary>
        /// 配置文件路径
        /// </summary>
        private static String configPath = Path.Combine(
            Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData, Environment.SpecialFolderOption.Create),
            "myfilm.xml");

        [Serializable]
        [XmlRoot("DBCONFIG")]
        public class DBStruct
        {
            [XmlAttribute("DBUserName")]
            public String DBUserName;

            [XmlAttribute("DBPassword")]
            public String DBPassword;

            [XmlAttribute("DBDefaultIP")]
            public String DBDefaultIP;

            [XmlAttribute("DBDefaultName")]
            public String DBDefaultName;

            [XmlArrayItem("DBIP")]
            public List<String> DBIPs;

            [XmlArrayItem("DBName")]
            public List<String> DBNames;
        }

        private DBStruct dbStruct = null;

        public LoginForm()
        {
            InitializeComponent();

            dbStruct = new DBStruct();
            dbStruct.DBUserName = String.Empty;
            dbStruct.DBPassword = String.Empty;
            dbStruct.DBDefaultIP = String.Empty;
            dbStruct.DBDefaultName = String.Empty;
            dbStruct.DBIPs = new List<String>();
            dbStruct.DBNames = new List<String>();
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

            if (String.IsNullOrWhiteSpace(dbStruct.DBDefaultIP)) dbStruct.DBDefaultIP = "127.0.0.1";
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
            this.comboBoxIP.Items.AddRange(dbStruct.DBIPs.ToArray());
            this.comboBoxIP.Text = dbStruct.DBDefaultIP;
            this.comboBoxIP.ResumeLayout();
        }

        private void InitComboxDataBase()
        {
            this.comboBoxDataBase.SuspendLayout();
            this.comboBoxDataBase.Items.Clear();
            this.comboBoxDataBase.Items.AddRange(dbStruct.DBNames.ToArray());
            this.comboBoxDataBase.Text = dbStruct.DBDefaultName;
            this.comboBoxDataBase.ResumeLayout();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            this.Icon = Properties.Resources.ico;

            LoadXml();
            InitComboxIP();
            InitComboxDataBase();

            this.textBoxUser.Text = dbStruct.DBUserName;
            this.textBoxPwd.Text = dbStruct.DBPassword;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            CommonString.DbIP = this.comboBoxIP.Text;
            CommonString.DbUserName = this.textBoxUser.Text;
            CommonString.DbPassword = this.textBoxPwd.Text;
            CommonString.DbName = this.comboBoxDataBase.Text;

            try
            {
                SqlData sqlData = new SqlData();
                sqlData.OpenMySql();
                sqlData.CloseMySql();

                this.dbStruct.DBUserName = CommonString.DbUserName;
                this.dbStruct.DBPassword = CommonString.DbPassword;
                this.dbStruct.DBDefaultIP = CommonString.DbIP;
                this.dbStruct.DBDefaultName = CommonString.DbName;

                if (!dbStruct.DBIPs.Contains(CommonString.DbIP))
                    dbStruct.DBIPs.Add(CommonString.DbIP);
                if (!dbStruct.DBNames.Contains(CommonString.DbName))
                    dbStruct.DBNames.Add(CommonString.DbName);

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
