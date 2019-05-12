namespace MyFilm
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.comboBoxDisk = new System.Windows.Forms.ComboBox();
            this.textBoxSearch = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnWatch = new System.Windows.Forms.Button();
            this.btnManager = new System.Windows.Forms.Button();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.labelExplain = new System.Windows.Forms.Label();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemSetDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemCancelDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemSetWatch = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemCancelWatch = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemOpenFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemPrintFolderTree = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemShowContent = new System.Windows.Forms.ToolStripMenuItem();
            this.btnRootDirectory = new System.Windows.Forms.Button();
            this.comboBoxPage = new System.Windows.Forms.ComboBox();
            this.btnLastPage = new System.Windows.Forms.Button();
            this.btnNextPage = new System.Windows.Forms.Button();
            this.btnPrePage = new System.Windows.Forms.Button();
            this.btnFirstPage = new System.Windows.Forms.Button();
            this.btnUpFolder = new System.Windows.Forms.Button();
            this.labelPageRowCount = new System.Windows.Forms.Label();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStripNotify = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemShowWindow = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemExitWindow = new System.Windows.Forms.ToolStripMenuItem();
            this.btnDeleteOrderByDisk = new System.Windows.Forms.Button();
            this.tbePageRowCount = new MyFilm.TextBoxEx();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.contextMenuStrip.SuspendLayout();
            this.contextMenuStripNotify.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBoxDisk
            // 
            this.comboBoxDisk.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDisk.FormattingEnabled = true;
            this.comboBoxDisk.Location = new System.Drawing.Point(12, 12);
            this.comboBoxDisk.Name = "comboBoxDisk";
            this.comboBoxDisk.Size = new System.Drawing.Size(231, 20);
            this.comboBoxDisk.TabIndex = 0;
            // 
            // textBoxSearch
            // 
            this.textBoxSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSearch.Location = new System.Drawing.Point(249, 12);
            this.textBoxSearch.Name = "textBoxSearch";
            this.textBoxSearch.Size = new System.Drawing.Size(533, 21);
            this.textBoxSearch.TabIndex = 1;
            this.textBoxSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxSearch_KeyDown);
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSearch.Location = new System.Drawing.Point(788, 11);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 2;
            this.btnSearch.Text = "搜索";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDelete.Location = new System.Drawing.Point(865, 11);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 3;
            this.btnDelete.Text = "待删(时间)";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnWatch
            // 
            this.btnWatch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnWatch.Location = new System.Drawing.Point(1019, 11);
            this.btnWatch.Name = "btnWatch";
            this.btnWatch.Size = new System.Drawing.Size(75, 23);
            this.btnWatch.TabIndex = 4;
            this.btnWatch.Text = "待看";
            this.btnWatch.UseVisualStyleBackColor = true;
            this.btnWatch.Click += new System.EventHandler(this.btnWatch_Click);
            // 
            // btnManager
            // 
            this.btnManager.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnManager.Location = new System.Drawing.Point(1097, 11);
            this.btnManager.Name = "btnManager";
            this.btnManager.Size = new System.Drawing.Size(75, 23);
            this.btnManager.TabIndex = 5;
            this.btnManager.Text = "管理";
            this.btnManager.UseVisualStyleBackColor = true;
            this.btnManager.Click += new System.EventHandler(this.btnManager_Click);
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridView.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView.Location = new System.Drawing.Point(12, 78);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.RowHeadersVisible = false;
            this.dataGridView.RowTemplate.Height = 23;
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.Size = new System.Drawing.Size(1160, 513);
            this.dataGridView.TabIndex = 6;
            this.dataGridView.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellDoubleClick);
            this.dataGridView.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dataGridView_CellFormatting);
            // 
            // labelExplain
            // 
            this.labelExplain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelExplain.Location = new System.Drawing.Point(12, 55);
            this.labelExplain.Name = "labelExplain";
            this.labelExplain.Size = new System.Drawing.Size(980, 12);
            this.labelExplain.TabIndex = 7;
            this.labelExplain.Text = "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX";
            this.labelExplain.Resize += new System.EventHandler(this.labelExplain_Resize);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemSetDelete,
            this.toolStripMenuItemCancelDelete,
            this.toolStripSeparator1,
            this.toolStripMenuItemSetWatch,
            this.toolStripMenuItemCancelWatch,
            this.toolStripSeparator2,
            this.toolStripMenuItemOpenFolder,
            this.toolStripComboBox,
            this.toolStripSeparator3,
            this.toolStripMenuItemPrintFolderTree,
            this.toolStripSeparator4,
            this.toolStripMenuItemShowContent});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(149, 211);
            this.contextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Opening);
            // 
            // toolStripMenuItemSetDelete
            // 
            this.toolStripMenuItemSetDelete.Name = "toolStripMenuItemSetDelete";
            this.toolStripMenuItemSetDelete.Size = new System.Drawing.Size(148, 22);
            this.toolStripMenuItemSetDelete.Text = "设为待删";
            this.toolStripMenuItemSetDelete.Click += new System.EventHandler(this.toolStripMenuItemSetDelete_Click);
            // 
            // toolStripMenuItemCancelDelete
            // 
            this.toolStripMenuItemCancelDelete.Name = "toolStripMenuItemCancelDelete";
            this.toolStripMenuItemCancelDelete.Size = new System.Drawing.Size(148, 22);
            this.toolStripMenuItemCancelDelete.Text = "取消待删";
            this.toolStripMenuItemCancelDelete.Click += new System.EventHandler(this.toolStripMenuItemCancelDelete_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(145, 6);
            // 
            // toolStripMenuItemSetWatch
            // 
            this.toolStripMenuItemSetWatch.Name = "toolStripMenuItemSetWatch";
            this.toolStripMenuItemSetWatch.Size = new System.Drawing.Size(148, 22);
            this.toolStripMenuItemSetWatch.Text = "设为待看";
            this.toolStripMenuItemSetWatch.Click += new System.EventHandler(this.toolStripMenuItemSetWatch_Click);
            // 
            // toolStripMenuItemCancelWatch
            // 
            this.toolStripMenuItemCancelWatch.Name = "toolStripMenuItemCancelWatch";
            this.toolStripMenuItemCancelWatch.Size = new System.Drawing.Size(148, 22);
            this.toolStripMenuItemCancelWatch.Text = "取消待看";
            this.toolStripMenuItemCancelWatch.Click += new System.EventHandler(this.toolStripMenuItemCancelWatch_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(145, 6);
            // 
            // toolStripMenuItemOpenFolder
            // 
            this.toolStripMenuItemOpenFolder.Name = "toolStripMenuItemOpenFolder";
            this.toolStripMenuItemOpenFolder.Size = new System.Drawing.Size(148, 22);
            this.toolStripMenuItemOpenFolder.Text = "打开位置";
            this.toolStripMenuItemOpenFolder.Click += new System.EventHandler(this.toolStripMenuItemOpenFolder_Click);
            // 
            // toolStripComboBox
            // 
            this.toolStripComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolStripComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.toolStripComboBox.Name = "toolStripComboBox";
            this.toolStripComboBox.Size = new System.Drawing.Size(75, 25);
            this.toolStripComboBox.ToolTipText = "设置打开位置实际所在磁盘";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(145, 6);
            // 
            // toolStripMenuItemPrintFolderTree
            // 
            this.toolStripMenuItemPrintFolderTree.Name = "toolStripMenuItemPrintFolderTree";
            this.toolStripMenuItemPrintFolderTree.Size = new System.Drawing.Size(148, 22);
            this.toolStripMenuItemPrintFolderTree.Text = "打印目录树";
            this.toolStripMenuItemPrintFolderTree.Click += new System.EventHandler(this.toolStripMenuItemPrintFolderTree_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(145, 6);
            // 
            // toolStripMenuItemShowContent
            // 
            this.toolStripMenuItemShowContent.Name = "toolStripMenuItemShowContent";
            this.toolStripMenuItemShowContent.Size = new System.Drawing.Size(148, 22);
            this.toolStripMenuItemShowContent.Text = "查看文件内容";
            this.toolStripMenuItemShowContent.Click += new System.EventHandler(this.toolStripMenuItemShowContent_Click);
            // 
            // btnRootDirectory
            // 
            this.btnRootDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRootDirectory.Location = new System.Drawing.Point(196, 601);
            this.btnRootDirectory.Name = "btnRootDirectory";
            this.btnRootDirectory.Size = new System.Drawing.Size(75, 23);
            this.btnRootDirectory.TabIndex = 23;
            this.btnRootDirectory.Text = "根目录";
            this.btnRootDirectory.UseVisualStyleBackColor = true;
            this.btnRootDirectory.Click += new System.EventHandler(this.btnRootDirectory_Click);
            // 
            // comboBoxPage
            // 
            this.comboBoxPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxPage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxPage.FormattingEnabled = true;
            this.comboBoxPage.Location = new System.Drawing.Point(1097, 603);
            this.comboBoxPage.Name = "comboBoxPage";
            this.comboBoxPage.Size = new System.Drawing.Size(75, 20);
            this.comboBoxPage.TabIndex = 22;
            this.comboBoxPage.SelectedIndexChanged += new System.EventHandler(this.comboBoxPage_SelectedIndexChanged);
            // 
            // btnLastPage
            // 
            this.btnLastPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLastPage.Location = new System.Drawing.Point(1016, 601);
            this.btnLastPage.Name = "btnLastPage";
            this.btnLastPage.Size = new System.Drawing.Size(75, 23);
            this.btnLastPage.TabIndex = 21;
            this.btnLastPage.Text = "末页";
            this.btnLastPage.UseVisualStyleBackColor = true;
            this.btnLastPage.Click += new System.EventHandler(this.btnLastPage_Click);
            // 
            // btnNextPage
            // 
            this.btnNextPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNextPage.Location = new System.Drawing.Point(935, 601);
            this.btnNextPage.Name = "btnNextPage";
            this.btnNextPage.Size = new System.Drawing.Size(75, 23);
            this.btnNextPage.TabIndex = 20;
            this.btnNextPage.Text = "下一页";
            this.btnNextPage.UseVisualStyleBackColor = true;
            this.btnNextPage.Click += new System.EventHandler(this.btnNextPage_Click);
            // 
            // btnPrePage
            // 
            this.btnPrePage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPrePage.Location = new System.Drawing.Point(854, 601);
            this.btnPrePage.Name = "btnPrePage";
            this.btnPrePage.Size = new System.Drawing.Size(75, 23);
            this.btnPrePage.TabIndex = 19;
            this.btnPrePage.Text = "上一页";
            this.btnPrePage.UseVisualStyleBackColor = true;
            this.btnPrePage.Click += new System.EventHandler(this.btnPrePage_Click);
            // 
            // btnFirstPage
            // 
            this.btnFirstPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFirstPage.Location = new System.Drawing.Point(773, 601);
            this.btnFirstPage.Name = "btnFirstPage";
            this.btnFirstPage.Size = new System.Drawing.Size(75, 23);
            this.btnFirstPage.TabIndex = 18;
            this.btnFirstPage.Text = "首页";
            this.btnFirstPage.UseVisualStyleBackColor = true;
            this.btnFirstPage.Click += new System.EventHandler(this.btnFirstPage_Click);
            // 
            // btnUpFolder
            // 
            this.btnUpFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnUpFolder.Location = new System.Drawing.Point(12, 601);
            this.btnUpFolder.Name = "btnUpFolder";
            this.btnUpFolder.Size = new System.Drawing.Size(178, 23);
            this.btnUpFolder.TabIndex = 17;
            this.btnUpFolder.Text = "上一目录...";
            this.btnUpFolder.UseVisualStyleBackColor = true;
            this.btnUpFolder.Click += new System.EventHandler(this.btnUpFolder_Click);
            // 
            // labelPageRowCount
            // 
            this.labelPageRowCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelPageRowCount.AutoSize = true;
            this.labelPageRowCount.Location = new System.Drawing.Point(1018, 55);
            this.labelPageRowCount.Name = "labelPageRowCount";
            this.labelPageRowCount.Size = new System.Drawing.Size(77, 12);
            this.labelPageRowCount.TabIndex = 24;
            this.labelPageRowCount.Text = "每页记录条数";
            // 
            // notifyIcon
            // 
            this.notifyIcon.Text = "notifyIcon";
            this.notifyIcon.Visible = true;
            this.notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseDoubleClick);
            // 
            // contextMenuStripNotify
            // 
            this.contextMenuStripNotify.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemShowWindow,
            this.toolStripMenuItemExitWindow});
            this.contextMenuStripNotify.Name = "contextMenuStripNotify";
            this.contextMenuStripNotify.Size = new System.Drawing.Size(101, 48);
            // 
            // toolStripMenuItemShowWindow
            // 
            this.toolStripMenuItemShowWindow.Name = "toolStripMenuItemShowWindow";
            this.toolStripMenuItemShowWindow.Size = new System.Drawing.Size(100, 22);
            this.toolStripMenuItemShowWindow.Text = "显示";
            this.toolStripMenuItemShowWindow.Click += new System.EventHandler(this.toolStripMenuItemShowWindow_Click);
            // 
            // toolStripMenuItemExitWindow
            // 
            this.toolStripMenuItemExitWindow.Name = "toolStripMenuItemExitWindow";
            this.toolStripMenuItemExitWindow.Size = new System.Drawing.Size(100, 22);
            this.toolStripMenuItemExitWindow.Text = "退出";
            this.toolStripMenuItemExitWindow.Click += new System.EventHandler(this.toolStripMenuItemExitWindow_Click);
            // 
            // btnDeleteOrderByDisk
            // 
            this.btnDeleteOrderByDisk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDeleteOrderByDisk.Location = new System.Drawing.Point(942, 11);
            this.btnDeleteOrderByDisk.Name = "btnDeleteOrderByDisk";
            this.btnDeleteOrderByDisk.Size = new System.Drawing.Size(75, 23);
            this.btnDeleteOrderByDisk.TabIndex = 29;
            this.btnDeleteOrderByDisk.Text = "待删(磁盘)";
            this.btnDeleteOrderByDisk.UseVisualStyleBackColor = true;
            this.btnDeleteOrderByDisk.Click += new System.EventHandler(this.btnDeleteOrderByDisk_Click);
            // 
            // tbePageRowCount
            // 
            this.tbePageRowCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbePageRowCount.Location = new System.Drawing.Point(1097, 51);
            this.tbePageRowCount.Name = "tbePageRowCount";
            this.tbePageRowCount.Size = new System.Drawing.Size(75, 21);
            this.tbePageRowCount.TabIndex = 25;
            this.tbePageRowCount.Text = "20";
            this.tbePageRowCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbePageRowCount.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbePageRowCount_KeyDown);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 636);
            this.Controls.Add(this.btnDeleteOrderByDisk);
            this.Controls.Add(this.tbePageRowCount);
            this.Controls.Add(this.labelPageRowCount);
            this.Controls.Add(this.btnRootDirectory);
            this.Controls.Add(this.comboBoxPage);
            this.Controls.Add(this.btnLastPage);
            this.Controls.Add(this.btnNextPage);
            this.Controls.Add(this.btnPrePage);
            this.Controls.Add(this.btnFirstPage);
            this.Controls.Add(this.btnUpFolder);
            this.Controls.Add(this.labelExplain);
            this.Controls.Add(this.dataGridView);
            this.Controls.Add(this.btnManager);
            this.Controls.Add(this.btnWatch);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.textBoxSearch);
            this.Controls.Add(this.comboBoxDisk);
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(1200, 675);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "文件";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.SizeChanged += new System.EventHandler(this.MainForm_SizeChanged);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.contextMenuStrip.ResumeLayout(false);
            this.contextMenuStripNotify.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxDisk;
        private System.Windows.Forms.TextBox textBoxSearch;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnWatch;
        private System.Windows.Forms.Button btnManager;
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.Label labelExplain;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSetDelete;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSetWatch;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemCancelDelete;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemCancelWatch;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemOpenFolder;
        private System.Windows.Forms.Button btnRootDirectory;
        private System.Windows.Forms.ComboBox comboBoxPage;
        private System.Windows.Forms.Button btnLastPage;
        private System.Windows.Forms.Button btnNextPage;
        private System.Windows.Forms.Button btnPrePage;
        private System.Windows.Forms.Button btnFirstPage;
        private System.Windows.Forms.Button btnUpFolder;
        private System.Windows.Forms.Label labelPageRowCount;
        private TextBoxEx tbePageRowCount;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemPrintFolderTree;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripNotify;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemShowWindow;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemExitWindow;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemShowContent;
        private System.Windows.Forms.Button btnDeleteOrderByDisk;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBox;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
    }
}

