using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace MyFilm
{
    public partial class LoginForm : Form
    {
        private LoginConfig.LoginConfigData loginConfigData = null;

        private RealOrFake4KWebDataCapture.RealOrFake4KWebDataCaptureResult
            webDataCaptureResult = null;

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
            this.loginConfigData = LoginConfig.LoadXml(CommonString.LoginConfigPath);

            InitComboxIP();
            InitComboxUser();

            var uap = this.loginConfigData.mysqlConfig.userNameAndPassWords.Find(
                x => x.UserName == this.comboBoxUser.Text);
            if (uap != null) this.textBoxPwd.Text = Helper.Decrypt(uap.PassWord);

            InitComboxDataBase();

            this.cbSQLiteDataBase.Items.Clear();
            this.loginConfigData.sqliteConfig.dataBaseConfigs.ForEach(
                x => this.cbSQLiteDataBase.Items.Add(x.Name));
            this.cbSQLiteDataBase.Text =
                this.loginConfigData.sqliteConfig.selectedDataBasePath;

            this.tabControl.SelectedIndex =
                (this.loginConfigData.dataBaseType == LoginConfig.DataBaseType.MYSQL ? 0 : 1);
        }

        private void InitComboxIP()
        {
            this.comboBoxIP.SuspendLayout();
            this.comboBoxIP.Items.Clear();
            this.comboBoxIP.Items.AddRange(
                this.loginConfigData.mysqlConfig.hostIPs.ToArray());
            this.comboBoxIP.Text = this.loginConfigData.mysqlConfig.selectedIP;
            this.comboBoxIP.ResumeLayout();
        }

        private void InitComboxUser()
        {
            this.comboBoxUser.SuspendLayout();
            this.comboBoxUser.SelectedIndexChanged -= comboBoxUser_SelectedIndexChanged;
            this.comboBoxUser.Items.Clear();
            this.loginConfigData.mysqlConfig.userNameAndPassWords.ForEach(
                x => this.comboBoxUser.Items.Add(x.UserName));
            this.comboBoxUser.Text = this.loginConfigData.mysqlConfig.selectedUserName;
            this.comboBoxUser.SelectedIndexChanged += comboBoxUser_SelectedIndexChanged;
            this.comboBoxUser.ResumeLayout();
        }

        private void InitComboxDataBase()
        {
            this.comboBoxDataBase.SuspendLayout();
            this.comboBoxDataBase.Items.Clear();
            this.loginConfigData.mysqlConfig.dataBaseConfigs.ForEach(
                x => this.comboBoxDataBase.Items.Add(x.Name));
            this.comboBoxDataBase.Text =
                this.loginConfigData.mysqlConfig.selectedDataBaseName;
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
                    Helper.Decrypt(this.loginConfigData.mysqlConfig
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
            CommonString.DataBaseType =
                this.tabControl.SelectedIndex == 0 ?
                LoginConfig.DataBaseType.MYSQL :
                LoginConfig.DataBaseType.SQLITE;

            if (CommonString.DataBaseType == LoginConfig.DataBaseType.MYSQL)
            {
                CommonString.DbIP = this.comboBoxIP.Text;
                CommonString.DbUserName = this.comboBoxUser.Text;
                CommonString.DbPassword = this.textBoxPwd.Text;
                CommonString.DbName = this.comboBoxDataBase.Text;

                List<String> databaseNameList = null;
                try
                {
                    databaseNameList = SqlData.GetSqlData().QueryAllDataBaseNames();
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
                            SqlData.GetSqlData().CreateDataBase(CommonString.DbName);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK);
                            return;
                        }
                    }
                    else return;
                }
            }
            else
            {
                CommonString.SqliteDateBasePath = this.cbSQLiteDataBase.Text;
            }

            try
            {
                SqlData sqlData = SqlData.GetSqlData();
                sqlData.CreateTables();

                DateTime dateTimeLast = DateTime.MinValue;

                if (CommonString.DataBaseType == LoginConfig.DataBaseType.MYSQL)
                {
                    if (!this.loginConfigData.mysqlConfig.hostIPs.Contains(CommonString.DbIP))
                        this.loginConfigData.mysqlConfig.hostIPs.Add(CommonString.DbIP);

                    if (this.loginConfigData.mysqlConfig.userNameAndPassWords.FindIndex(
                        x => x.UserName == CommonString.DbUserName) == -1)
                        this.loginConfigData.mysqlConfig.userNameAndPassWords.Add(
                            new LoginConfig.UserNameAndPassWord()
                            {
                                UserName = CommonString.DbUserName,
                                PassWord = Helper.Encryption(CommonString.DbPassword)
                            });

                    if (this.loginConfigData.mysqlConfig.dataBaseConfigs.FindIndex(
                        x => x.Name == CommonString.DbName) == -1)
                        this.loginConfigData.mysqlConfig.dataBaseConfigs.Add(
                            new LoginConfig.DataBaseConfig()
                            {
                                Name = CommonString.DbName,
                                WebDataCaptureTime =
                                    DateTime.MinValue.ToString("yyyy-MM-dd HHH:mm:ss")
                            });

                    this.loginConfigData.mysqlConfig.selectedUserName =
                        CommonString.DbUserName;
                    this.loginConfigData.mysqlConfig.selectedIP =
                        CommonString.DbIP;
                    this.loginConfigData.mysqlConfig.selectedDataBaseName =
                        CommonString.DbName;

                    DateTime.TryParse(
                        this.loginConfigData.mysqlConfig.SelectedDataBaseWebDataCaptureTime,
                        out dateTimeLast);
                }
                else
                {
                    if (this.loginConfigData.sqliteConfig.dataBaseConfigs.FindIndex(
                        x => x.Name == CommonString.SqliteDateBasePath) == -1)
                        this.loginConfigData.sqliteConfig.dataBaseConfigs.Add(
                            new LoginConfig.DataBaseConfig()
                            {
                                Name = CommonString.SqliteDateBasePath,
                                WebDataCaptureTime =
                                    DateTime.MinValue.ToString("yyyy-MM-dd HHH:mm:ss")
                            });
                    this.loginConfigData.sqliteConfig.selectedDataBasePath =
                        CommonString.SqliteDateBasePath;

                    DateTime.TryParse(
                        this.loginConfigData.sqliteConfig.SelectedDataBaseWebDataCaptureTime,
                        out dateTimeLast);
                }

                TimeSpan ts = DateTime.Now.Subtract(dateTimeLast);
                if (ts.Days > 3)
                {
                    WaitingForm waitingForm = new WaitingForm(SetWebCaptureDataResult);
                    waitingForm.ShowDialog();

                    if (this.webDataCaptureResult.code >= 0)
                        dateTimeLast = this.webDataCaptureResult.crawlTime;
                    MessageBox.Show(this.webDataCaptureResult.strMsg);
                }

                if (CommonString.DataBaseType == LoginConfig.DataBaseType.MYSQL)
                    this.loginConfigData.mysqlConfig.SelectedDataBaseWebDataCaptureTime
                        = dateTimeLast.ToString("yyyy-MM-dd HHH:mm:ss");
                else
                    this.loginConfigData.sqliteConfig.SelectedDataBaseWebDataCaptureTime
                        = dateTimeLast.ToString("yyyy-MM-dd HHH:mm:ss");

                this.loginConfigData.dataBaseType = CommonString.DataBaseType;

                LoginConfig.SaveXml(this.loginConfigData, CommonString.LoginConfigPath);
                sqlData.FillRamData();

                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK);
            }
        }
    }
}
