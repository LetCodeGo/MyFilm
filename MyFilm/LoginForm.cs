﻿using System;
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

        private SqlData sqlData = null;

        /// <summary>
        /// 登陆配置文件路径
        /// </summary>
        public readonly static String LoginConfigPath = Path.Combine(
            System.Windows.Forms.Application.StartupPath, "MyFilmConfig.xml");

        public LoginForm()
        {
            InitializeComponent();

            this.ControlBox = false;
        }

        public SqlData GetSqlData() { return this.sqlData; }

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
            this.loginConfigData = LoginConfig.LoadXml(LoginConfigPath);

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
                            sqlData = new SqlDataInMySql(dbIP, dbUserName, dbPassword, dbName);
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
                sqliteDateBasePath = this.cbSQLiteDataBase.Text;
                sqlData = new SqlDataInSqlite(sqliteDateBasePath);
            }

            try
            {
                sqlData.CreateTables();

                DateTime dateTimeLast = DateTime.MinValue;

                if (databaseType == LoginConfig.DataBaseType.MYSQL)
                {
                    if (!this.loginConfigData.mysqlConfig.hostIPs.Contains(dbIP))
                        this.loginConfigData.mysqlConfig.hostIPs.Add(dbIP);

                    if (this.loginConfigData.mysqlConfig.userNameAndPassWords.FindIndex(
                        x => x.UserName == dbUserName) == -1)
                        this.loginConfigData.mysqlConfig.userNameAndPassWords.Add(
                            new LoginConfig.UserNameAndPassWord()
                            {
                                UserName = dbUserName,
                                PassWord = Helper.Encryption(dbPassword)
                            });

                    if (this.loginConfigData.mysqlConfig.dataBaseConfigs.FindIndex(
                        x => x.Name == dbName) == -1)
                        this.loginConfigData.mysqlConfig.dataBaseConfigs.Add(
                            new LoginConfig.DataBaseConfig()
                            {
                                Name = dbName,
                                WebDataCaptureTime =
                                    DateTime.MinValue.ToString("yyyy-MM-dd HHH:mm:ss")
                            });

                    this.loginConfigData.mysqlConfig.selectedUserName = dbUserName;
                    this.loginConfigData.mysqlConfig.selectedIP = dbIP;
                    this.loginConfigData.mysqlConfig.selectedDataBaseName = dbName;

                    DateTime.TryParse(
                        this.loginConfigData.mysqlConfig.SelectedDataBaseWebDataCaptureTime,
                        out dateTimeLast);
                }
                else
                {
                    if (this.loginConfigData.sqliteConfig.dataBaseConfigs.FindIndex(
                        x => x.Name == sqliteDateBasePath) == -1)
                        this.loginConfigData.sqliteConfig.dataBaseConfigs.Add(
                            new LoginConfig.DataBaseConfig()
                            {
                                Name = sqliteDateBasePath,
                                WebDataCaptureTime =
                                    DateTime.MinValue.ToString("yyyy-MM-dd HHH:mm:ss")
                            });
                    this.loginConfigData.sqliteConfig.selectedDataBasePath = sqliteDateBasePath;

                    DateTime.TryParse(
                        this.loginConfigData.sqliteConfig.SelectedDataBaseWebDataCaptureTime,
                        out dateTimeLast);
                }

                TimeSpan ts = DateTime.Now.Subtract(dateTimeLast);
                if (ts.Days > 3)
                {
                    WaitingForm waitingForm = new WaitingForm(
                        SetWebCaptureDataResult, sqlData, this.loginConfigData.crawlURL);
                    waitingForm.ShowDialog();

                    if (this.webDataCaptureResult.code >= 0)
                        dateTimeLast = this.webDataCaptureResult.crawlTime;
                    MessageBox.Show(this.webDataCaptureResult.strMsg);
                }

                if (databaseType == LoginConfig.DataBaseType.MYSQL)
                    this.loginConfigData.mysqlConfig.SelectedDataBaseWebDataCaptureTime
                        = dateTimeLast.ToString("yyyy-MM-dd HHH:mm:ss");
                else
                    this.loginConfigData.sqliteConfig.SelectedDataBaseWebDataCaptureTime
                        = dateTimeLast.ToString("yyyy-MM-dd HHH:mm:ss");

                this.loginConfigData.dataBaseType = databaseType;

                LoginConfig.SaveXml(this.loginConfigData, LoginConfigPath);
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
