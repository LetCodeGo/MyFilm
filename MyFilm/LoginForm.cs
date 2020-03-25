using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace MyFilm
{
    public partial class LoginForm : Form
    {
        public enum LoginType
        {
            Normal,
            DataBaseDataCopy
        }

        private RealOrFake4KWebDataCapture.RealOrFake4KWebDataCaptureResult
            webDataCaptureResult = null;

        private LoginType loginType = LoginType.Normal;
        /// <summary>
        /// 通过新参数创建的数据库连接
        /// </summary>
        private SqlData generatedSqlData = null;

        /// <summary>
        /// 登陆配置文件路径
        /// </summary>
        public readonly static String LoginConfigPath = Path.Combine(
            System.Windows.Forms.Application.StartupPath, "MyFilmConfig.xml");

        public LoginForm(LoginType loginType = LoginType.Normal)
        {
            InitializeComponent();

            this.loginType = loginType;
            if (this.loginType == LoginType.Normal) this.Text = "连接数据库";
            else if (this.loginType == LoginType.DataBaseDataCopy)
            {
                this.Text = "选择要复制的数据库";
                this.btnSetting.Enabled = false;
            }
        }

        public SqlData GetGeneratedSqlData() { return this.generatedSqlData; }

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
            CommonString.LoginConfigData = LoginConfig.LoadXml(LoginConfigPath);

            InitComboxIP();
            InitComboxUser();

            var uap = CommonString.LoginConfigData.mysqlConfig.userNameAndPassWords.Find(
                x => x.UserName == this.comboBoxUser.Text);
            if (uap != null) this.textBoxPwd.Text = Helper.Decrypt(uap.PassWord);

            InitComboxDataBase();

            this.cbSQLiteDataBase.Items.Clear();
            CommonString.LoginConfigData.sqliteConfig.dataBaseConfigs.ForEach(
                x => this.cbSQLiteDataBase.Items.Add(x.Name));
            this.cbSQLiteDataBase.Text =
                CommonString.LoginConfigData.sqliteConfig.selectedDataBasePath;

            this.tabControl.SelectedIndex =
                (CommonString.LoginConfigData.dataBaseType == LoginConfig.DataBaseType.MYSQL ? 0 : 1);
        }

        private void InitComboxIP()
        {
            this.comboBoxIP.SuspendLayout();
            this.comboBoxIP.Items.Clear();
            this.comboBoxIP.Items.AddRange(
                CommonString.LoginConfigData.mysqlConfig.hostIPs.ToArray());
            this.comboBoxIP.Text = CommonString.LoginConfigData.mysqlConfig.selectedIP;
            this.comboBoxIP.ResumeLayout();
        }

        private void InitComboxUser()
        {
            this.comboBoxUser.SuspendLayout();
            this.comboBoxUser.SelectedIndexChanged -= comboBoxUser_SelectedIndexChanged;
            this.comboBoxUser.Items.Clear();
            CommonString.LoginConfigData.mysqlConfig.userNameAndPassWords.ForEach(
                x => this.comboBoxUser.Items.Add(x.UserName));
            this.comboBoxUser.Text = CommonString.LoginConfigData.mysqlConfig.selectedUserName;
            this.comboBoxUser.SelectedIndexChanged += comboBoxUser_SelectedIndexChanged;
            this.comboBoxUser.ResumeLayout();
        }

        private void InitComboxDataBase()
        {
            this.comboBoxDataBase.SuspendLayout();
            this.comboBoxDataBase.Items.Clear();
            CommonString.LoginConfigData.mysqlConfig.dataBaseConfigs.ForEach(
                x => this.comboBoxDataBase.Items.Add(x.Name));
            this.comboBoxDataBase.Text =
                CommonString.LoginConfigData.mysqlConfig.selectedDataBaseName;
            this.comboBoxDataBase.ResumeLayout();
        }

        public void SetWebCaptureDataResult(
            RealOrFake4KWebDataCapture.RealOrFake4KWebDataCaptureResult rst)
        {
            this.webDataCaptureResult = rst;
        }

        private void comboBoxUser_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedIndex = this.comboBoxUser.SelectedIndex;

            if (selectedIndex >= 0 &&
                selectedIndex < this.comboBoxUser.Items.Count)
            {
                this.textBoxPwd.Text =
                    Helper.Decrypt(CommonString.LoginConfigData.mysqlConfig
                    .userNameAndPassWords[selectedIndex].PassWord);
            }
        }

        private void btnSelectSQLiteDataBase_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog
            {
                Filter = "db file|*.db",
                Multiselect = false
            };
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                this.cbSQLiteDataBase.Text = dlg.FileName;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            LoginConfig.DataBaseType databaseType =
                this.tabControl.SelectedIndex == 0 ?
                LoginConfig.DataBaseType.MYSQL :
                LoginConfig.DataBaseType.SQLITE;

            String dbIP = String.Empty;
            String dbUserName = String.Empty;
            String dbPassword = String.Empty;
            String dbName = String.Empty;
            String sqliteDateBasePath = String.Empty;

            if (databaseType == LoginConfig.DataBaseType.MYSQL)
            {
                dbIP = this.comboBoxIP.Text;
                dbUserName = this.comboBoxUser.Text;
                dbPassword = this.textBoxPwd.Text;
                dbName = this.comboBoxDataBase.Text;

                List<String> databaseNameList = null;
                try
                {
                    databaseNameList = SqlDataInMySql.QueryAllDataBaseNames(
                        dbIP, dbUserName, dbPassword);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK);
                    return;
                }

                if (!databaseNameList.Contains(dbName))
                {
                    if (MessageBox.Show(String.Format("数据库 \"{0}\" 不存在，要创建吗？",
                        dbName), "提示",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        try
                        {
                            SqlDataInMySql.CreateDataBase(dbIP, dbUserName, dbPassword, dbName);
                            generatedSqlData = new SqlDataInMySql(dbIP, dbUserName, dbPassword, dbName);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK);
                            return;
                        }
                    }
                    else return;
                }
                else if (this.loginType == LoginType.DataBaseDataCopy)
                {
                    if (MessageBox.Show(String.Format(
                        "数据库 \"{0}\" 已存在，此操作会覆盖该数据库相同的表，仍然要复制到该数据库吗？",
                        dbName), "提示",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        generatedSqlData = new SqlDataInMySql(dbIP, dbUserName, dbPassword, dbName);
                    }
                    else return;
                }
                else
                {
                    generatedSqlData = new SqlDataInMySql(dbIP, dbUserName, dbPassword, dbName);
                }
            }
            else
            {
                sqliteDateBasePath = this.cbSQLiteDataBase.Text;
                generatedSqlData = new SqlDataInSqlite(sqliteDateBasePath);

                if (this.loginType == LoginType.DataBaseDataCopy && File.Exists(sqliteDateBasePath))
                {
                    if (MessageBox.Show(String.Format(
                        "数据库 \"{0}\"\n已存在，此操作会覆盖该数据库相同的表，仍然要复制到该数据库吗？",
                        sqliteDateBasePath), "提示",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    {
                        return;
                    }
                }
            }

            try
            {
                generatedSqlData.CreateTables();

                DateTime dateTimeLast = DateTime.MinValue;

                if (databaseType == LoginConfig.DataBaseType.MYSQL)
                {
                    if (!CommonString.LoginConfigData.mysqlConfig.hostIPs.Contains(dbIP))
                        CommonString.LoginConfigData.mysqlConfig.hostIPs.Add(dbIP);

                    if (CommonString.LoginConfigData.mysqlConfig.userNameAndPassWords.FindIndex(
                        x => x.UserName == dbUserName) == -1)
                        CommonString.LoginConfigData.mysqlConfig.userNameAndPassWords.Add(
                            new LoginConfig.UserNameAndPassWord()
                            {
                                UserName = dbUserName,
                                PassWord = Helper.Encryption(dbPassword)
                            });

                    if (CommonString.LoginConfigData.mysqlConfig.dataBaseConfigs.FindIndex(
                        x => x.Name == dbName) == -1)
                        CommonString.LoginConfigData.mysqlConfig.dataBaseConfigs.Add(
                            new LoginConfig.DataBaseConfig()
                            {
                                Name = dbName,
                                WebDataCaptureTime =
                                    DateTime.MinValue.ToString("yyyy-MM-dd HHH:mm:ss")
                            });

                    CommonString.LoginConfigData.mysqlConfig.selectedUserName = dbUserName;
                    CommonString.LoginConfigData.mysqlConfig.selectedIP = dbIP;
                    CommonString.LoginConfigData.mysqlConfig.selectedDataBaseName = dbName;

                    DateTime.TryParse(
                        CommonString.LoginConfigData.mysqlConfig.SelectedDataBaseWebDataCaptureTime,
                        out dateTimeLast);
                }
                else
                {
                    if (CommonString.LoginConfigData.sqliteConfig.dataBaseConfigs.FindIndex(
                        x => x.Name == sqliteDateBasePath) == -1)
                        CommonString.LoginConfigData.sqliteConfig.dataBaseConfigs.Add(
                            new LoginConfig.DataBaseConfig()
                            {
                                Name = sqliteDateBasePath,
                                WebDataCaptureTime =
                                    DateTime.MinValue.ToString("yyyy-MM-dd HHH:mm:ss")
                            });
                    CommonString.LoginConfigData.sqliteConfig.selectedDataBasePath = sqliteDateBasePath;

                    DateTime.TryParse(
                        CommonString.LoginConfigData.sqliteConfig.SelectedDataBaseWebDataCaptureTime,
                        out dateTimeLast);
                }

                if (this.loginType == LoginType.Normal &&
                    CommonString.LoginConfigData.crawlConfig.IsCrawl)
                {
                    TimeSpan ts = DateTime.Now.Subtract(dateTimeLast);
                    if (ts.Days >= CommonString.LoginConfigData.crawlConfig.IntervalDays)
                    {
                        WaitingForm waitingForm = new WaitingForm(
                            SetWebCaptureDataResult, generatedSqlData,
                            CommonString.LoginConfigData.crawlConfig.CrawlURL);
                        waitingForm.ShowDialog();

                        if (this.webDataCaptureResult.code >= 0)
                            dateTimeLast = this.webDataCaptureResult.crawlTime;
                        MessageBox.Show(this.webDataCaptureResult.strMsg);
                    }
                }

                if (databaseType == LoginConfig.DataBaseType.MYSQL)
                    CommonString.LoginConfigData.mysqlConfig.SelectedDataBaseWebDataCaptureTime
                        = dateTimeLast.ToString("yyyy-MM-dd HHH:mm:ss");
                else
                    CommonString.LoginConfigData.sqliteConfig.SelectedDataBaseWebDataCaptureTime
                        = dateTimeLast.ToString("yyyy-MM-dd HHH:mm:ss");

                CommonString.LoginConfigData.dataBaseType = databaseType;

                LoginConfig.SaveXml(CommonString.LoginConfigData, LoginConfigPath);

                if (this.loginType == LoginType.Normal) generatedSqlData.FillRamData();

                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK);
            }
        }

        private void SettingFormApply(LoginConfig.CrawlConfig crawlConfig,
            LoginConfig.WebServerConfig webServerConfig)
        {
            CommonString.LoginConfigData.crawlConfig = crawlConfig;
            CommonString.LoginConfigData.webServerConfig = webServerConfig;
        }

        private void btnSetting_Click(object sender, EventArgs e)
        {
            SettingForm form = new SettingForm(
                CommonString.LoginConfigData.crawlConfig,
                CommonString.LoginConfigData.webServerConfig);
            form.SettingFormApplyAction += this.SettingFormApply;
            form.ShowDialog();
        }
    }
}
