namespace MyFilm
{
    partial class SqlForm
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
            this.btnSearch = new System.Windows.Forms.Button();
            this.richTextBoxInfo = new System.Windows.Forms.RichTextBox();
            this.groupBoxOutput = new System.Windows.Forms.GroupBox();
            this.cbOutputCtl = new System.Windows.Forms.CheckBox();
            this.cbindex = new System.Windows.Forms.CheckBox();
            this.cbto_delete_ex = new System.Windows.Forms.CheckBox();
            this.cbto_watch_ex = new System.Windows.Forms.CheckBox();
            this.cbdisk_desc = new System.Windows.Forms.CheckBox();
            this.cbto_delete = new System.Windows.Forms.CheckBox();
            this.cbs_w_t = new System.Windows.Forms.CheckBox();
            this.cbmax_cid = new System.Windows.Forms.CheckBox();
            this.cbto_watch = new System.Windows.Forms.CheckBox();
            this.cbis_folder = new System.Windows.Forms.CheckBox();
            this.cbpid = new System.Windows.Forms.CheckBox();
            this.cbmodify_t = new System.Windows.Forms.CheckBox();
            this.cbcreate_t = new System.Windows.Forms.CheckBox();
            this.cbcontent = new System.Windows.Forms.CheckBox();
            this.cbsize = new System.Windows.Forms.CheckBox();
            this.cbpath = new System.Windows.Forms.CheckBox();
            this.cbs_d_t = new System.Windows.Forms.CheckBox();
            this.cbname = new System.Windows.Forms.CheckBox();
            this.cbid = new System.Windows.Forms.CheckBox();
            this.cbNoGrid = new System.Windows.Forms.CheckBox();
            this.textBoxSql = new MyFilm.TextBoxSql();
            this.groupBoxOutput.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSearch.Location = new System.Drawing.Point(1113, 11);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 4;
            this.btnSearch.Text = "搜索";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // richTextBoxInfo
            // 
            this.richTextBoxInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBoxInfo.Location = new System.Drawing.Point(12, 91);
            this.richTextBoxInfo.Name = "richTextBoxInfo";
            this.richTextBoxInfo.ReadOnly = true;
            this.richTextBoxInfo.Size = new System.Drawing.Size(1176, 542);
            this.richTextBoxInfo.TabIndex = 5;
            this.richTextBoxInfo.Text = "";
            this.richTextBoxInfo.WordWrap = false;
            // 
            // groupBoxOutput
            // 
            this.groupBoxOutput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxOutput.Controls.Add(this.cbOutputCtl);
            this.groupBoxOutput.Controls.Add(this.cbindex);
            this.groupBoxOutput.Controls.Add(this.cbto_delete_ex);
            this.groupBoxOutput.Controls.Add(this.cbto_watch_ex);
            this.groupBoxOutput.Controls.Add(this.cbdisk_desc);
            this.groupBoxOutput.Controls.Add(this.cbto_delete);
            this.groupBoxOutput.Controls.Add(this.cbs_w_t);
            this.groupBoxOutput.Controls.Add(this.cbmax_cid);
            this.groupBoxOutput.Controls.Add(this.cbto_watch);
            this.groupBoxOutput.Controls.Add(this.cbis_folder);
            this.groupBoxOutput.Controls.Add(this.cbpid);
            this.groupBoxOutput.Controls.Add(this.cbmodify_t);
            this.groupBoxOutput.Controls.Add(this.cbcreate_t);
            this.groupBoxOutput.Controls.Add(this.cbcontent);
            this.groupBoxOutput.Controls.Add(this.cbsize);
            this.groupBoxOutput.Controls.Add(this.cbpath);
            this.groupBoxOutput.Controls.Add(this.cbs_d_t);
            this.groupBoxOutput.Controls.Add(this.cbname);
            this.groupBoxOutput.Controls.Add(this.cbid);
            this.groupBoxOutput.Location = new System.Drawing.Point(12, 39);
            this.groupBoxOutput.Name = "groupBoxOutput";
            this.groupBoxOutput.Size = new System.Drawing.Size(1176, 46);
            this.groupBoxOutput.TabIndex = 7;
            this.groupBoxOutput.TabStop = false;
            this.groupBoxOutput.Text = "             --- 选择文本信息输出的列";
            // 
            // cbOutputCtl
            // 
            this.cbOutputCtl.AutoSize = true;
            this.cbOutputCtl.Location = new System.Drawing.Point(2, -1);
            this.cbOutputCtl.Name = "cbOutputCtl";
            this.cbOutputCtl.Size = new System.Drawing.Size(84, 16);
            this.cbOutputCtl.TabIndex = 10;
            this.cbOutputCtl.Text = "列输出控制";
            this.cbOutputCtl.UseVisualStyleBackColor = true;
            // 
            // cbindex
            // 
            this.cbindex.AutoSize = true;
            this.cbindex.Location = new System.Drawing.Point(2, 20);
            this.cbindex.Name = "cbindex";
            this.cbindex.Size = new System.Drawing.Size(54, 16);
            this.cbindex.TabIndex = 17;
            this.cbindex.Text = "index";
            this.cbindex.UseVisualStyleBackColor = true;
            // 
            // cbto_delete_ex
            // 
            this.cbto_delete_ex.AutoSize = true;
            this.cbto_delete_ex.Location = new System.Drawing.Point(764, 20);
            this.cbto_delete_ex.Name = "cbto_delete_ex";
            this.cbto_delete_ex.Size = new System.Drawing.Size(96, 16);
            this.cbto_delete_ex.TabIndex = 16;
            this.cbto_delete_ex.Text = "to_delete_ex";
            this.cbto_delete_ex.UseVisualStyleBackColor = true;
            // 
            // cbto_watch_ex
            // 
            this.cbto_watch_ex.AutoSize = true;
            this.cbto_watch_ex.Location = new System.Drawing.Point(539, 20);
            this.cbto_watch_ex.Name = "cbto_watch_ex";
            this.cbto_watch_ex.Size = new System.Drawing.Size(90, 16);
            this.cbto_watch_ex.TabIndex = 15;
            this.cbto_watch_ex.Text = "to_watch_ex";
            this.cbto_watch_ex.UseVisualStyleBackColor = true;
            // 
            // cbdisk_desc
            // 
            this.cbdisk_desc.AutoSize = true;
            this.cbdisk_desc.Location = new System.Drawing.Point(1093, 20);
            this.cbdisk_desc.Name = "cbdisk_desc";
            this.cbdisk_desc.Size = new System.Drawing.Size(78, 16);
            this.cbdisk_desc.TabIndex = 14;
            this.cbdisk_desc.Text = "disk_desc";
            this.cbdisk_desc.UseVisualStyleBackColor = true;
            // 
            // cbto_delete
            // 
            this.cbto_delete.AutoSize = true;
            this.cbto_delete.Location = new System.Drawing.Point(685, 20);
            this.cbto_delete.Name = "cbto_delete";
            this.cbto_delete.Size = new System.Drawing.Size(78, 16);
            this.cbto_delete.TabIndex = 9;
            this.cbto_delete.Text = "to_delete";
            this.cbto_delete.UseVisualStyleBackColor = true;
            // 
            // cbs_w_t
            // 
            this.cbs_w_t.AutoSize = true;
            this.cbs_w_t.Location = new System.Drawing.Point(630, 20);
            this.cbs_w_t.Name = "cbs_w_t";
            this.cbs_w_t.Size = new System.Drawing.Size(54, 16);
            this.cbs_w_t.TabIndex = 8;
            this.cbs_w_t.Text = "s_w_t";
            this.cbs_w_t.UseVisualStyleBackColor = true;
            // 
            // cbmax_cid
            // 
            this.cbmax_cid.AutoSize = true;
            this.cbmax_cid.Location = new System.Drawing.Point(1026, 20);
            this.cbmax_cid.Name = "cbmax_cid";
            this.cbmax_cid.Size = new System.Drawing.Size(66, 16);
            this.cbmax_cid.TabIndex = 13;
            this.cbmax_cid.Text = "max_cid";
            this.cbmax_cid.UseVisualStyleBackColor = true;
            // 
            // cbto_watch
            // 
            this.cbto_watch.AutoSize = true;
            this.cbto_watch.Location = new System.Drawing.Point(466, 20);
            this.cbto_watch.Name = "cbto_watch";
            this.cbto_watch.Size = new System.Drawing.Size(72, 16);
            this.cbto_watch.TabIndex = 7;
            this.cbto_watch.Text = "to_watch";
            this.cbto_watch.UseVisualStyleBackColor = true;
            // 
            // cbis_folder
            // 
            this.cbis_folder.AutoSize = true;
            this.cbis_folder.Location = new System.Drawing.Point(387, 20);
            this.cbis_folder.Name = "cbis_folder";
            this.cbis_folder.Size = new System.Drawing.Size(78, 16);
            this.cbis_folder.TabIndex = 6;
            this.cbis_folder.Text = "is_folder";
            this.cbis_folder.UseVisualStyleBackColor = true;
            // 
            // cbpid
            // 
            this.cbpid.AutoSize = true;
            this.cbpid.Location = new System.Drawing.Point(983, 20);
            this.cbpid.Name = "cbpid";
            this.cbpid.Size = new System.Drawing.Size(42, 16);
            this.cbpid.TabIndex = 12;
            this.cbpid.Text = "pid";
            this.cbpid.UseVisualStyleBackColor = true;
            // 
            // cbmodify_t
            // 
            this.cbmodify_t.AutoSize = true;
            this.cbmodify_t.Location = new System.Drawing.Point(314, 20);
            this.cbmodify_t.Name = "cbmodify_t";
            this.cbmodify_t.Size = new System.Drawing.Size(72, 16);
            this.cbmodify_t.TabIndex = 5;
            this.cbmodify_t.Text = "modify_t";
            this.cbmodify_t.UseVisualStyleBackColor = true;
            // 
            // cbcreate_t
            // 
            this.cbcreate_t.AutoSize = true;
            this.cbcreate_t.Location = new System.Drawing.Point(241, 20);
            this.cbcreate_t.Name = "cbcreate_t";
            this.cbcreate_t.Size = new System.Drawing.Size(72, 16);
            this.cbcreate_t.TabIndex = 4;
            this.cbcreate_t.Text = "create_t";
            this.cbcreate_t.UseVisualStyleBackColor = true;
            // 
            // cbcontent
            // 
            this.cbcontent.AutoSize = true;
            this.cbcontent.Location = new System.Drawing.Point(916, 20);
            this.cbcontent.Name = "cbcontent";
            this.cbcontent.Size = new System.Drawing.Size(66, 16);
            this.cbcontent.TabIndex = 11;
            this.cbcontent.Text = "content";
            this.cbcontent.UseVisualStyleBackColor = true;
            // 
            // cbsize
            // 
            this.cbsize.AutoSize = true;
            this.cbsize.Location = new System.Drawing.Point(192, 20);
            this.cbsize.Name = "cbsize";
            this.cbsize.Size = new System.Drawing.Size(48, 16);
            this.cbsize.TabIndex = 3;
            this.cbsize.Text = "size";
            this.cbsize.UseVisualStyleBackColor = true;
            // 
            // cbpath
            // 
            this.cbpath.AutoSize = true;
            this.cbpath.Location = new System.Drawing.Point(143, 20);
            this.cbpath.Name = "cbpath";
            this.cbpath.Size = new System.Drawing.Size(48, 16);
            this.cbpath.TabIndex = 2;
            this.cbpath.Text = "path";
            this.cbpath.UseVisualStyleBackColor = true;
            // 
            // cbs_d_t
            // 
            this.cbs_d_t.AutoSize = true;
            this.cbs_d_t.Location = new System.Drawing.Point(861, 20);
            this.cbs_d_t.Name = "cbs_d_t";
            this.cbs_d_t.Size = new System.Drawing.Size(54, 16);
            this.cbs_d_t.TabIndex = 10;
            this.cbs_d_t.Text = "s_d_t";
            this.cbs_d_t.UseVisualStyleBackColor = true;
            // 
            // cbname
            // 
            this.cbname.AutoSize = true;
            this.cbname.Location = new System.Drawing.Point(94, 20);
            this.cbname.Name = "cbname";
            this.cbname.Size = new System.Drawing.Size(48, 16);
            this.cbname.TabIndex = 1;
            this.cbname.Text = "name";
            this.cbname.UseVisualStyleBackColor = true;
            // 
            // cbid
            // 
            this.cbid.AutoSize = true;
            this.cbid.Location = new System.Drawing.Point(57, 20);
            this.cbid.Name = "cbid";
            this.cbid.Size = new System.Drawing.Size(36, 16);
            this.cbid.TabIndex = 0;
            this.cbid.Text = "id";
            this.cbid.UseVisualStyleBackColor = true;
            // 
            // cbNoGrid
            // 
            this.cbNoGrid.AutoSize = true;
            this.cbNoGrid.Location = new System.Drawing.Point(14, 15);
            this.cbNoGrid.Name = "cbNoGrid";
            this.cbNoGrid.Size = new System.Drawing.Size(144, 16);
            this.cbNoGrid.TabIndex = 8;
            this.cbNoGrid.Text = "不在主界面以表格显示";
            this.cbNoGrid.UseVisualStyleBackColor = true;
            this.cbNoGrid.CheckedChanged += new System.EventHandler(this.cbNoGrid_CheckedChanged);
            // 
            // textBoxSql
            // 
            this.textBoxSql.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSql.Location = new System.Drawing.Point(157, 12);
            this.textBoxSql.Name = "textBoxSql";
            this.textBoxSql.Size = new System.Drawing.Size(950, 21);
            this.textBoxSql.TabIndex = 9;
            this.textBoxSql.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxSql_KeyDown);
            // 
            // SqlForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 645);
            this.Controls.Add(this.textBoxSql);
            this.Controls.Add(this.cbNoGrid);
            this.Controls.Add(this.groupBoxOutput);
            this.Controls.Add(this.richTextBoxInfo);
            this.Controls.Add(this.btnSearch);
            this.MinimumSize = new System.Drawing.Size(1216, 684);
            this.Name = "SqlForm";
            this.ShowInTaskbar = false;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SqlForm_FormClosed);
            this.Load += new System.EventHandler(this.SqlForm_Load);
            this.groupBoxOutput.ResumeLayout(false);
            this.groupBoxOutput.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.RichTextBox richTextBoxInfo;
        private System.Windows.Forms.GroupBox groupBoxOutput;
        private System.Windows.Forms.CheckBox cbdisk_desc;
        private System.Windows.Forms.CheckBox cbto_delete;
        private System.Windows.Forms.CheckBox cbs_w_t;
        private System.Windows.Forms.CheckBox cbmax_cid;
        private System.Windows.Forms.CheckBox cbto_watch;
        private System.Windows.Forms.CheckBox cbis_folder;
        private System.Windows.Forms.CheckBox cbpid;
        private System.Windows.Forms.CheckBox cbmodify_t;
        private System.Windows.Forms.CheckBox cbcreate_t;
        private System.Windows.Forms.CheckBox cbcontent;
        private System.Windows.Forms.CheckBox cbsize;
        private System.Windows.Forms.CheckBox cbpath;
        private System.Windows.Forms.CheckBox cbs_d_t;
        private System.Windows.Forms.CheckBox cbname;
        private System.Windows.Forms.CheckBox cbid;
        private System.Windows.Forms.CheckBox cbNoGrid;
        private TextBoxSql textBoxSql;
        private System.Windows.Forms.CheckBox cbto_delete_ex;
        private System.Windows.Forms.CheckBox cbto_watch_ex;
        private System.Windows.Forms.CheckBox cbindex;
        private System.Windows.Forms.CheckBox cbOutputCtl;
    }
}