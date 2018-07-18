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
            this.textBoxSql = new System.Windows.Forms.TextBox();
            this.richTextBoxInfo = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbid = new System.Windows.Forms.CheckBox();
            this.cbname = new System.Windows.Forms.CheckBox();
            this.cbpath = new System.Windows.Forms.CheckBox();
            this.cbsize = new System.Windows.Forms.CheckBox();
            this.cbcreate_t = new System.Windows.Forms.CheckBox();
            this.cbto_delete = new System.Windows.Forms.CheckBox();
            this.cbs_w_t = new System.Windows.Forms.CheckBox();
            this.cbto_watch = new System.Windows.Forms.CheckBox();
            this.cbis_folder = new System.Windows.Forms.CheckBox();
            this.cbmodify_t = new System.Windows.Forms.CheckBox();
            this.cbdisk_desc = new System.Windows.Forms.CheckBox();
            this.cbmax_cid = new System.Windows.Forms.CheckBox();
            this.cbpid = new System.Windows.Forms.CheckBox();
            this.cbcontent = new System.Windows.Forms.CheckBox();
            this.cbs_d_t = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSearch.Location = new System.Drawing.Point(1097, 11);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 4;
            this.btnSearch.Text = "搜索";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // textBoxSql
            // 
            this.textBoxSql.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSql.Location = new System.Drawing.Point(203, 12);
            this.textBoxSql.Name = "textBoxSql";
            this.textBoxSql.Size = new System.Drawing.Size(888, 21);
            this.textBoxSql.TabIndex = 3;
            this.textBoxSql.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxSql_KeyDown);
            // 
            // richTextBoxInfo
            // 
            this.richTextBoxInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBoxInfo.Location = new System.Drawing.Point(12, 95);
            this.richTextBoxInfo.Name = "richTextBoxInfo";
            this.richTextBoxInfo.ReadOnly = true;
            this.richTextBoxInfo.Size = new System.Drawing.Size(1160, 529);
            this.richTextBoxInfo.TabIndex = 5;
            this.richTextBoxInfo.Text = "";
            this.richTextBoxInfo.WordWrap = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(185, 12);
            this.label1.TabIndex = 6;
            this.label1.Text = "SELECT * FROM film_info WHERE ";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.cbdisk_desc);
            this.groupBox1.Controls.Add(this.cbto_delete);
            this.groupBox1.Controls.Add(this.cbs_w_t);
            this.groupBox1.Controls.Add(this.cbmax_cid);
            this.groupBox1.Controls.Add(this.cbto_watch);
            this.groupBox1.Controls.Add(this.cbis_folder);
            this.groupBox1.Controls.Add(this.cbpid);
            this.groupBox1.Controls.Add(this.cbmodify_t);
            this.groupBox1.Controls.Add(this.cbcreate_t);
            this.groupBox1.Controls.Add(this.cbcontent);
            this.groupBox1.Controls.Add(this.cbsize);
            this.groupBox1.Controls.Add(this.cbpath);
            this.groupBox1.Controls.Add(this.cbs_d_t);
            this.groupBox1.Controls.Add(this.cbname);
            this.groupBox1.Controls.Add(this.cbid);
            this.groupBox1.Location = new System.Drawing.Point(12, 43);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1160, 46);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "选择输出的列";
            // 
            // cbid
            // 
            this.cbid.AutoSize = true;
            this.cbid.Location = new System.Drawing.Point(20, 19);
            this.cbid.Name = "cbid";
            this.cbid.Size = new System.Drawing.Size(36, 16);
            this.cbid.TabIndex = 0;
            this.cbid.Text = "id";
            this.cbid.UseVisualStyleBackColor = true;
            // 
            // cbname
            // 
            this.cbname.AutoSize = true;
            this.cbname.Location = new System.Drawing.Point(71, 19);
            this.cbname.Name = "cbname";
            this.cbname.Size = new System.Drawing.Size(48, 16);
            this.cbname.TabIndex = 1;
            this.cbname.Text = "name";
            this.cbname.UseVisualStyleBackColor = true;
            // 
            // cbpath
            // 
            this.cbpath.AutoSize = true;
            this.cbpath.Location = new System.Drawing.Point(134, 19);
            this.cbpath.Name = "cbpath";
            this.cbpath.Size = new System.Drawing.Size(48, 16);
            this.cbpath.TabIndex = 2;
            this.cbpath.Text = "path";
            this.cbpath.UseVisualStyleBackColor = true;
            // 
            // cbsize
            // 
            this.cbsize.AutoSize = true;
            this.cbsize.Location = new System.Drawing.Point(197, 19);
            this.cbsize.Name = "cbsize";
            this.cbsize.Size = new System.Drawing.Size(48, 16);
            this.cbsize.TabIndex = 3;
            this.cbsize.Text = "size";
            this.cbsize.UseVisualStyleBackColor = true;
            // 
            // cbcreate_t
            // 
            this.cbcreate_t.AutoSize = true;
            this.cbcreate_t.Location = new System.Drawing.Point(260, 19);
            this.cbcreate_t.Name = "cbcreate_t";
            this.cbcreate_t.Size = new System.Drawing.Size(72, 16);
            this.cbcreate_t.TabIndex = 4;
            this.cbcreate_t.Text = "create_t";
            this.cbcreate_t.UseVisualStyleBackColor = true;
            // 
            // cbto_delete
            // 
            this.cbto_delete.AutoSize = true;
            this.cbto_delete.Location = new System.Drawing.Point(683, 19);
            this.cbto_delete.Name = "cbto_delete";
            this.cbto_delete.Size = new System.Drawing.Size(78, 16);
            this.cbto_delete.TabIndex = 9;
            this.cbto_delete.Text = "to_delete";
            this.cbto_delete.UseVisualStyleBackColor = true;
            // 
            // cbs_w_t
            // 
            this.cbs_w_t.AutoSize = true;
            this.cbs_w_t.Location = new System.Drawing.Point(614, 19);
            this.cbs_w_t.Name = "cbs_w_t";
            this.cbs_w_t.Size = new System.Drawing.Size(54, 16);
            this.cbs_w_t.TabIndex = 8;
            this.cbs_w_t.Text = "s_w_t";
            this.cbs_w_t.UseVisualStyleBackColor = true;
            // 
            // cbto_watch
            // 
            this.cbto_watch.AutoSize = true;
            this.cbto_watch.Location = new System.Drawing.Point(527, 19);
            this.cbto_watch.Name = "cbto_watch";
            this.cbto_watch.Size = new System.Drawing.Size(72, 16);
            this.cbto_watch.TabIndex = 7;
            this.cbto_watch.Text = "to_watch";
            this.cbto_watch.UseVisualStyleBackColor = true;
            // 
            // cbis_folder
            // 
            this.cbis_folder.AutoSize = true;
            this.cbis_folder.Location = new System.Drawing.Point(434, 19);
            this.cbis_folder.Name = "cbis_folder";
            this.cbis_folder.Size = new System.Drawing.Size(78, 16);
            this.cbis_folder.TabIndex = 6;
            this.cbis_folder.Text = "is_folder";
            this.cbis_folder.UseVisualStyleBackColor = true;
            // 
            // cbmodify_t
            // 
            this.cbmodify_t.AutoSize = true;
            this.cbmodify_t.Location = new System.Drawing.Point(347, 19);
            this.cbmodify_t.Name = "cbmodify_t";
            this.cbmodify_t.Size = new System.Drawing.Size(72, 16);
            this.cbmodify_t.TabIndex = 5;
            this.cbmodify_t.Text = "modify_t";
            this.cbmodify_t.UseVisualStyleBackColor = true;
            // 
            // cbdisk_desc
            // 
            this.cbdisk_desc.AutoSize = true;
            this.cbdisk_desc.Location = new System.Drawing.Point(1064, 19);
            this.cbdisk_desc.Name = "cbdisk_desc";
            this.cbdisk_desc.Size = new System.Drawing.Size(78, 16);
            this.cbdisk_desc.TabIndex = 14;
            this.cbdisk_desc.Text = "disk_desc";
            this.cbdisk_desc.UseVisualStyleBackColor = true;
            // 
            // cbmax_cid
            // 
            this.cbmax_cid.AutoSize = true;
            this.cbmax_cid.Location = new System.Drawing.Point(983, 19);
            this.cbmax_cid.Name = "cbmax_cid";
            this.cbmax_cid.Size = new System.Drawing.Size(66, 16);
            this.cbmax_cid.TabIndex = 13;
            this.cbmax_cid.Text = "max_cid";
            this.cbmax_cid.UseVisualStyleBackColor = true;
            // 
            // cbpid
            // 
            this.cbpid.AutoSize = true;
            this.cbpid.Location = new System.Drawing.Point(926, 19);
            this.cbpid.Name = "cbpid";
            this.cbpid.Size = new System.Drawing.Size(42, 16);
            this.cbpid.TabIndex = 12;
            this.cbpid.Text = "pid";
            this.cbpid.UseVisualStyleBackColor = true;
            // 
            // cbcontent
            // 
            this.cbcontent.AutoSize = true;
            this.cbcontent.Location = new System.Drawing.Point(845, 19);
            this.cbcontent.Name = "cbcontent";
            this.cbcontent.Size = new System.Drawing.Size(66, 16);
            this.cbcontent.TabIndex = 11;
            this.cbcontent.Text = "content";
            this.cbcontent.UseVisualStyleBackColor = true;
            // 
            // cbs_d_t
            // 
            this.cbs_d_t.AutoSize = true;
            this.cbs_d_t.Location = new System.Drawing.Point(776, 19);
            this.cbs_d_t.Name = "cbs_d_t";
            this.cbs_d_t.Size = new System.Drawing.Size(54, 16);
            this.cbs_d_t.TabIndex = 10;
            this.cbs_d_t.Text = "s_d_t";
            this.cbs_d_t.UseVisualStyleBackColor = true;
            // 
            // SqlForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 636);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.richTextBoxInfo);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.textBoxSql);
            this.MinimumSize = new System.Drawing.Size(1200, 675);
            this.Name = "SqlForm";
            this.Text = "SqlForm";
            this.Load += new System.EventHandler(this.SqlForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.TextBox textBoxSql;
        private System.Windows.Forms.RichTextBox richTextBoxInfo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
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
    }
}