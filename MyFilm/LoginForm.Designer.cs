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
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.labelDataBase = new System.Windows.Forms.Label();
            this.labelPwd = new System.Windows.Forms.Label();
            this.textBoxPwd = new System.Windows.Forms.TextBox();
            this.textBoxUser = new System.Windows.Forms.TextBox();
            this.labelUser = new System.Windows.Forms.Label();
            this.labelProcessCommct = new System.Windows.Forms.Label();
            this.comboBoxProcessCommct = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // comboBoxDataBase
            // 
            this.comboBoxDataBase.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDataBase.FormattingEnabled = true;
            this.comboBoxDataBase.Items.AddRange(new object[] {
            "myfilm",
            "test"});
            this.comboBoxDataBase.Location = new System.Drawing.Point(237, 126);
            this.comboBoxDataBase.Name = "comboBoxDataBase";
            this.comboBoxDataBase.Size = new System.Drawing.Size(121, 20);
            this.comboBoxDataBase.TabIndex = 0;
            this.comboBoxDataBase.SelectedIndexChanged += new System.EventHandler(this.comboBoxDataBase_SelectedIndexChanged);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(127, 216);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(87, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "确定";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(271, 216);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(87, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // labelDataBase
            // 
            this.labelDataBase.AutoSize = true;
            this.labelDataBase.Location = new System.Drawing.Point(125, 130);
            this.labelDataBase.Name = "labelDataBase";
            this.labelDataBase.Size = new System.Drawing.Size(89, 12);
            this.labelDataBase.TabIndex = 3;
            this.labelDataBase.Text = "数    据    库";
            // 
            // labelPwd
            // 
            this.labelPwd.AutoSize = true;
            this.labelPwd.Location = new System.Drawing.Point(125, 88);
            this.labelPwd.Name = "labelPwd";
            this.labelPwd.Size = new System.Drawing.Size(89, 12);
            this.labelPwd.TabIndex = 5;
            this.labelPwd.Text = "密          码";
            // 
            // textBoxPwd
            // 
            this.textBoxPwd.Location = new System.Drawing.Point(237, 84);
            this.textBoxPwd.Name = "textBoxPwd";
            this.textBoxPwd.Size = new System.Drawing.Size(121, 21);
            this.textBoxPwd.TabIndex = 6;
            this.textBoxPwd.Text = "123456";
            // 
            // textBoxUser
            // 
            this.textBoxUser.Location = new System.Drawing.Point(237, 42);
            this.textBoxUser.Name = "textBoxUser";
            this.textBoxUser.Size = new System.Drawing.Size(121, 21);
            this.textBoxUser.TabIndex = 8;
            this.textBoxUser.Text = "root";
            // 
            // labelUser
            // 
            this.labelUser.AutoSize = true;
            this.labelUser.Location = new System.Drawing.Point(125, 46);
            this.labelUser.Name = "labelUser";
            this.labelUser.Size = new System.Drawing.Size(89, 12);
            this.labelUser.TabIndex = 7;
            this.labelUser.Text = "用          户";
            // 
            // labelProcessCommct
            // 
            this.labelProcessCommct.AutoSize = true;
            this.labelProcessCommct.Location = new System.Drawing.Point(125, 171);
            this.labelProcessCommct.Name = "labelProcessCommct";
            this.labelProcessCommct.Size = new System.Drawing.Size(89, 12);
            this.labelProcessCommct.TabIndex = 10;
            this.labelProcessCommct.Text = "进程间通信方式";
            // 
            // comboBoxProcessCommct
            // 
            this.comboBoxProcessCommct.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxProcessCommct.FormattingEnabled = true;
            this.comboBoxProcessCommct.Items.AddRange(new object[] {
            "PIPE",
            "共享内存",
            "TCP"});
            this.comboBoxProcessCommct.Location = new System.Drawing.Point(237, 167);
            this.comboBoxProcessCommct.Name = "comboBoxProcessCommct";
            this.comboBoxProcessCommct.Size = new System.Drawing.Size(121, 20);
            this.comboBoxProcessCommct.TabIndex = 9;
            this.comboBoxProcessCommct.SelectedIndexChanged += new System.EventHandler(this.comboBoxProcessCommct_SelectedIndexChanged);
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 281);
            this.Controls.Add(this.labelProcessCommct);
            this.Controls.Add(this.comboBoxProcessCommct);
            this.Controls.Add(this.textBoxUser);
            this.Controls.Add(this.labelUser);
            this.Controls.Add(this.textBoxPwd);
            this.Controls.Add(this.labelPwd);
            this.Controls.Add(this.labelDataBase);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.comboBoxDataBase);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "登录";
            this.Load += new System.EventHandler(this.LoginForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxDataBase;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label labelDataBase;
        private System.Windows.Forms.Label labelPwd;
        private System.Windows.Forms.TextBox textBoxPwd;
        private System.Windows.Forms.TextBox textBoxUser;
        private System.Windows.Forms.Label labelUser;
        private System.Windows.Forms.Label labelProcessCommct;
        private System.Windows.Forms.ComboBox comboBoxProcessCommct;
    }
}