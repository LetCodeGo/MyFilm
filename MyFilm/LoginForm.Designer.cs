namespace MyFilm
{
    partial class LoginForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.comboBoxDataBase = new System.Windows.Forms.ComboBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.labelDataBase = new System.Windows.Forms.Label();
            this.labelPwd = new System.Windows.Forms.Label();
            this.comboBoxUser = new System.Windows.Forms.ComboBox();
            this.labelUser = new System.Windows.Forms.Label();
            this.comboBoxIP = new System.Windows.Forms.ComboBox();
            this.labelIP = new System.Windows.Forms.Label();
            this.textBoxPwd = new System.Windows.Forms.TextBox();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageMySQL = new System.Windows.Forms.TabPage();
            this.tabPageSQLite = new System.Windows.Forms.TabPage();
            this.btnSelectSQLiteDataBase = new System.Windows.Forms.Button();
            this.cbSQLiteDataBase = new System.Windows.Forms.ComboBox();
            this.tabControl.SuspendLayout();
            this.tabPageMySQL.SuspendLayout();
            this.tabPageSQLite.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBoxDataBase
            // 
            this.comboBoxDataBase.FormattingEnabled = true;
            this.comboBoxDataBase.Location = new System.Drawing.Point(181, 121);
            this.comboBoxDataBase.Name = "comboBoxDataBase";
            this.comboBoxDataBase.Size = new System.Drawing.Size(158, 20);
            this.comboBoxDataBase.TabIndex = 0;
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(147, 233);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(75, 23);
            this.btnConnect.TabIndex = 1;
            this.btnConnect.Text = "连接";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(280, 233);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // labelDataBase
            // 
            this.labelDataBase.AutoSize = true;
            this.labelDataBase.Location = new System.Drawing.Point(129, 125);
            this.labelDataBase.Name = "labelDataBase";
            this.labelDataBase.Size = new System.Drawing.Size(41, 12);
            this.labelDataBase.TabIndex = 3;
            this.labelDataBase.Text = "数据库";
            // 
            // labelPwd
            // 
            this.labelPwd.AutoSize = true;
            this.labelPwd.Location = new System.Drawing.Point(129, 92);
            this.labelPwd.Name = "labelPwd";
            this.labelPwd.Size = new System.Drawing.Size(41, 12);
            this.labelPwd.TabIndex = 5;
            this.labelPwd.Text = "密  码";
            // 
            // comboBoxUser
            // 
            this.comboBoxUser.Location = new System.Drawing.Point(181, 54);
            this.comboBoxUser.Name = "comboBoxUser";
            this.comboBoxUser.Size = new System.Drawing.Size(158, 20);
            this.comboBoxUser.TabIndex = 8;
            this.comboBoxUser.SelectedIndexChanged += new System.EventHandler(this.comboBoxUser_SelectedIndexChanged);
            // 
            // labelUser
            // 
            this.labelUser.AutoSize = true;
            this.labelUser.Location = new System.Drawing.Point(129, 59);
            this.labelUser.Name = "labelUser";
            this.labelUser.Size = new System.Drawing.Size(41, 12);
            this.labelUser.TabIndex = 7;
            this.labelUser.Text = "用  户";
            // 
            // comboBoxIP
            // 
            this.comboBoxIP.Location = new System.Drawing.Point(181, 21);
            this.comboBoxIP.Name = "comboBoxIP";
            this.comboBoxIP.Size = new System.Drawing.Size(158, 20);
            this.comboBoxIP.TabIndex = 10;
            // 
            // labelIP
            // 
            this.labelIP.AutoSize = true;
            this.labelIP.Location = new System.Drawing.Point(129, 26);
            this.labelIP.Name = "labelIP";
            this.labelIP.Size = new System.Drawing.Size(41, 12);
            this.labelIP.TabIndex = 9;
            this.labelIP.Text = "IP地址";
            // 
            // textBoxPwd
            // 
            this.textBoxPwd.Location = new System.Drawing.Point(181, 87);
            this.textBoxPwd.Name = "textBoxPwd";
            this.textBoxPwd.PasswordChar = '*';
            this.textBoxPwd.Size = new System.Drawing.Size(158, 21);
            this.textBoxPwd.TabIndex = 11;
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPageMySQL);
            this.tabControl.Controls.Add(this.tabPageSQLite);
            this.tabControl.Location = new System.Drawing.Point(12, 32);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(460, 186);
            this.tabControl.TabIndex = 12;
            // 
            // tabPageMySQL
            // 
            this.tabPageMySQL.Controls.Add(this.labelIP);
            this.tabPageMySQL.Controls.Add(this.textBoxPwd);
            this.tabPageMySQL.Controls.Add(this.comboBoxDataBase);
            this.tabPageMySQL.Controls.Add(this.comboBoxIP);
            this.tabPageMySQL.Controls.Add(this.labelDataBase);
            this.tabPageMySQL.Controls.Add(this.labelPwd);
            this.tabPageMySQL.Controls.Add(this.comboBoxUser);
            this.tabPageMySQL.Controls.Add(this.labelUser);
            this.tabPageMySQL.Location = new System.Drawing.Point(4, 22);
            this.tabPageMySQL.Name = "tabPageMySQL";
            this.tabPageMySQL.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageMySQL.Size = new System.Drawing.Size(452, 160);
            this.tabPageMySQL.TabIndex = 0;
            this.tabPageMySQL.Text = "MySQL";
            this.tabPageMySQL.UseVisualStyleBackColor = true;
            // 
            // tabPageSQLite
            // 
            this.tabPageSQLite.Controls.Add(this.btnSelectSQLiteDataBase);
            this.tabPageSQLite.Controls.Add(this.cbSQLiteDataBase);
            this.tabPageSQLite.Location = new System.Drawing.Point(4, 22);
            this.tabPageSQLite.Name = "tabPageSQLite";
            this.tabPageSQLite.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSQLite.Size = new System.Drawing.Size(452, 160);
            this.tabPageSQLite.TabIndex = 1;
            this.tabPageSQLite.Text = "SQLite";
            this.tabPageSQLite.UseVisualStyleBackColor = true;
            // 
            // btnSelectSQLiteDataBase
            // 
            this.btnSelectSQLiteDataBase.Location = new System.Drawing.Point(6, 91);
            this.btnSelectSQLiteDataBase.Name = "btnSelectSQLiteDataBase";
            this.btnSelectSQLiteDataBase.Size = new System.Drawing.Size(440, 23);
            this.btnSelectSQLiteDataBase.TabIndex = 2;
            this.btnSelectSQLiteDataBase.Text = "选择数据库文件";
            this.btnSelectSQLiteDataBase.UseVisualStyleBackColor = true;
            this.btnSelectSQLiteDataBase.Click += new System.EventHandler(this.btnSelectSQLiteDataBase_Click);
            // 
            // cbSQLiteDataBase
            // 
            this.cbSQLiteDataBase.Location = new System.Drawing.Point(6, 55);
            this.cbSQLiteDataBase.Name = "cbSQLiteDataBase";
            this.cbSQLiteDataBase.Size = new System.Drawing.Size(440, 20);
            this.cbSQLiteDataBase.TabIndex = 1;
            // 
            // LoginForm
            // 
            this.AcceptButton = this.btnConnect;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 281);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnConnect);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "登录";
            this.Load += new System.EventHandler(this.LoginForm_Load);
            this.tabControl.ResumeLayout(false);
            this.tabPageMySQL.ResumeLayout(false);
            this.tabPageMySQL.PerformLayout();
            this.tabPageSQLite.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxDataBase;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label labelDataBase;
        private System.Windows.Forms.Label labelPwd;
        private System.Windows.Forms.ComboBox comboBoxUser;
        private System.Windows.Forms.Label labelUser;
        private System.Windows.Forms.ComboBox comboBoxIP;
        private System.Windows.Forms.Label labelIP;
        private System.Windows.Forms.TextBox textBoxPwd;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPageMySQL;
        private System.Windows.Forms.TabPage tabPageSQLite;
        private System.Windows.Forms.Button btnSelectSQLiteDataBase;
        private System.Windows.Forms.ComboBox cbSQLiteDataBase;
    }
}