namespace MyFilm
{
    partial class SettingForm
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tbIntervalDays = new MyFilm.TextBoxEx();
            this.labelIntervalDays = new System.Windows.Forms.Label();
            this.tbCrawlAddr = new System.Windows.Forms.TextBox();
            this.labelCrawlAddr = new System.Windows.Forms.Label();
            this.cbIsCrawl = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tbRowsPerPage = new MyFilm.TextBoxEx();
            this.label2 = new System.Windows.Forms.Label();
            this.tbPort = new MyFilm.TextBoxEx();
            this.label1 = new System.Windows.Forms.Label();
            this.cbStartWebServer = new System.Windows.Forms.CheckBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tbIntervalDays);
            this.groupBox1.Controls.Add(this.labelIntervalDays);
            this.groupBox1.Controls.Add(this.tbCrawlAddr);
            this.groupBox1.Controls.Add(this.labelCrawlAddr);
            this.groupBox1.Controls.Add(this.cbIsCrawl);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(460, 105);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "抓取 RealOrFake4K 信息设置 ";
            // 
            // tbIntervalDays
            // 
            this.tbIntervalDays.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbIntervalDays.Location = new System.Drawing.Point(279, 44);
            this.tbIntervalDays.Name = "tbIntervalDays";
            this.tbIntervalDays.Size = new System.Drawing.Size(175, 21);
            this.tbIntervalDays.TabIndex = 32;
            this.tbIntervalDays.Text = "10";
            this.tbIntervalDays.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // labelIntervalDays
            // 
            this.labelIntervalDays.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelIntervalDays.AutoSize = true;
            this.labelIntervalDays.Location = new System.Drawing.Point(4, 48);
            this.labelIntervalDays.Name = "labelIntervalDays";
            this.labelIntervalDays.Size = new System.Drawing.Size(269, 12);
            this.labelIntervalDays.TabIndex = 31;
            this.labelIntervalDays.Text = "每次登录时自动抓取 RealOrFake4K 信息间隔天数";
            // 
            // tbCrawlAddr
            // 
            this.tbCrawlAddr.Location = new System.Drawing.Point(58, 71);
            this.tbCrawlAddr.Name = "tbCrawlAddr";
            this.tbCrawlAddr.Size = new System.Drawing.Size(396, 21);
            this.tbCrawlAddr.TabIndex = 30;
            // 
            // labelCrawlAddr
            // 
            this.labelCrawlAddr.AutoSize = true;
            this.labelCrawlAddr.Location = new System.Drawing.Point(4, 76);
            this.labelCrawlAddr.Name = "labelCrawlAddr";
            this.labelCrawlAddr.Size = new System.Drawing.Size(53, 12);
            this.labelCrawlAddr.TabIndex = 29;
            this.labelCrawlAddr.Text = "爬取网址";
            // 
            // cbIsCrawl
            // 
            this.cbIsCrawl.AutoSize = true;
            this.cbIsCrawl.Checked = true;
            this.cbIsCrawl.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbIsCrawl.Location = new System.Drawing.Point(6, 20);
            this.cbIsCrawl.Name = "cbIsCrawl";
            this.cbIsCrawl.Size = new System.Drawing.Size(240, 16);
            this.cbIsCrawl.TabIndex = 28;
            this.cbIsCrawl.Text = "每次登录时自动抓取 RealOrFake4K 信息";
            this.cbIsCrawl.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.tbRowsPerPage);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.tbPort);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.cbStartWebServer);
            this.groupBox2.Location = new System.Drawing.Point(12, 123);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(460, 102);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Web Server 设置";
            // 
            // tbRowsPerPage
            // 
            this.tbRowsPerPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbRowsPerPage.Location = new System.Drawing.Point(87, 70);
            this.tbRowsPerPage.Name = "tbRowsPerPage";
            this.tbRowsPerPage.Size = new System.Drawing.Size(93, 21);
            this.tbRowsPerPage.TabIndex = 36;
            this.tbRowsPerPage.Text = "20";
            this.tbRowsPerPage.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 74);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 12);
            this.label2.TabIndex = 35;
            this.label2.Text = "每页显示行数";
            // 
            // tbPort
            // 
            this.tbPort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbPort.Location = new System.Drawing.Point(87, 43);
            this.tbPort.Name = "tbPort";
            this.tbPort.Size = new System.Drawing.Size(93, 21);
            this.tbPort.TabIndex = 34;
            this.tbPort.Text = "5555";
            this.tbPort.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 47);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 33;
            this.label1.Text = "端口号";
            // 
            // cbStartWebServer
            // 
            this.cbStartWebServer.AutoSize = true;
            this.cbStartWebServer.Checked = true;
            this.cbStartWebServer.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbStartWebServer.Location = new System.Drawing.Point(6, 20);
            this.cbStartWebServer.Name = "cbStartWebServer";
            this.cbStartWebServer.Size = new System.Drawing.Size(114, 16);
            this.cbStartWebServer.TabIndex = 29;
            this.cbStartWebServer.Text = "开启 Web Server";
            this.cbStartWebServer.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(304, 231);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "确定";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(211, 231);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 3;
            this.btnApply.Text = "应用";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(397, 231);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // SettingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 262);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "设置";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private TextBoxEx tbIntervalDays;
        private System.Windows.Forms.Label labelIntervalDays;
        private System.Windows.Forms.TextBox tbCrawlAddr;
        private System.Windows.Forms.Label labelCrawlAddr;
        private System.Windows.Forms.CheckBox cbIsCrawl;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox cbStartWebServer;
        private TextBoxEx tbRowsPerPage;
        private System.Windows.Forms.Label label2;
        private TextBoxEx tbPort;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnCancel;
    }
}