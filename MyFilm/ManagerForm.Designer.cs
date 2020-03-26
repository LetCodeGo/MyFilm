namespace MyFilm
{
    partial class ManagerForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.ColumnIndex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnDisk = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnFreeSpace = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnTotalSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnCompleteScan = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnScanLayer = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnAddDisk = new System.Windows.Forms.Button();
            this.btnUpdateDisk = new System.Windows.Forms.Button();
            this.btnDeleteDisk = new System.Windows.Forms.Button();
            this.btnMoveFolderOrFile = new System.Windows.Forms.Button();
            this.textBoxDiskDescribe = new System.Windows.Forms.TextBox();
            this.textBoxNewDiskDescribe = new System.Windows.Forms.TextBox();
            this.btnChangeDiskDescribe = new System.Windows.Forms.Button();
            this.comboBoxLocalDisk = new System.Windows.Forms.ComboBox();
            this.btnUpdateLocalDisk = new System.Windows.Forms.Button();
            this.checkBoxBriefScan = new System.Windows.Forms.CheckBox();
            this.labelScanDepth = new System.Windows.Forms.Label();
            this.btnUpdateROF4K = new System.Windows.Forms.Button();
            this.cbScanMedia = new System.Windows.Forms.CheckBox();
            this.tbeLayer = new MyFilm.TextBoxEx();
            this.btnSetEmptyFolderDelete = new System.Windows.Forms.Button();
            this.btnDatabaseCopy = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
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
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnIndex,
            this.ColumnDisk,
            this.ColumnFreeSpace,
            this.ColumnTotalSize,
            this.ColumnCompleteScan,
            this.ColumnScanLayer});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView.Location = new System.Drawing.Point(12, 12);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridView.RowHeadersVisible = false;
            this.dataGridView.RowTemplate.Height = 23;
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.Size = new System.Drawing.Size(750, 556);
            this.dataGridView.TabIndex = 0;
            this.dataGridView.DataSourceChanged += new System.EventHandler(this.dataGridView_DataSourceChanged);
            this.dataGridView.SelectionChanged += new System.EventHandler(this.dataGridView_SelectionChanged);
            // 
            // ColumnIndex
            // 
            this.ColumnIndex.FillWeight = 40F;
            this.ColumnIndex.HeaderText = "索引";
            this.ColumnIndex.MinimumWidth = 40;
            this.ColumnIndex.Name = "ColumnIndex";
            this.ColumnIndex.ReadOnly = true;
            // 
            // ColumnDisk
            // 
            this.ColumnDisk.HeaderText = "磁盘";
            this.ColumnDisk.MinimumWidth = 100;
            this.ColumnDisk.Name = "ColumnDisk";
            this.ColumnDisk.ReadOnly = true;
            // 
            // ColumnFreeSpace
            // 
            this.ColumnFreeSpace.FillWeight = 80F;
            this.ColumnFreeSpace.HeaderText = "可用空间";
            this.ColumnFreeSpace.MinimumWidth = 80;
            this.ColumnFreeSpace.Name = "ColumnFreeSpace";
            this.ColumnFreeSpace.ReadOnly = true;
            // 
            // ColumnTotalSize
            // 
            this.ColumnTotalSize.FillWeight = 60F;
            this.ColumnTotalSize.HeaderText = "总空间";
            this.ColumnTotalSize.MinimumWidth = 60;
            this.ColumnTotalSize.Name = "ColumnTotalSize";
            this.ColumnTotalSize.ReadOnly = true;
            // 
            // ColumnCompleteScan
            // 
            this.ColumnCompleteScan.FillWeight = 80F;
            this.ColumnCompleteScan.HeaderText = "完全扫描";
            this.ColumnCompleteScan.MinimumWidth = 40;
            this.ColumnCompleteScan.Name = "ColumnCompleteScan";
            this.ColumnCompleteScan.ReadOnly = true;
            this.ColumnCompleteScan.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ColumnCompleteScan.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ColumnScanLayer
            // 
            this.ColumnScanLayer.FillWeight = 80F;
            this.ColumnScanLayer.HeaderText = "扫描层数";
            this.ColumnScanLayer.MinimumWidth = 40;
            this.ColumnScanLayer.Name = "ColumnScanLayer";
            this.ColumnScanLayer.ReadOnly = true;
            // 
            // btnAddDisk
            // 
            this.btnAddDisk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddDisk.Location = new System.Drawing.Point(768, 130);
            this.btnAddDisk.Name = "btnAddDisk";
            this.btnAddDisk.Size = new System.Drawing.Size(204, 25);
            this.btnAddDisk.TabIndex = 1;
            this.btnAddDisk.Text = "添加磁盘";
            this.btnAddDisk.UseVisualStyleBackColor = true;
            this.btnAddDisk.Click += new System.EventHandler(this.btnAddDisk_Click);
            // 
            // btnUpdateDisk
            // 
            this.btnUpdateDisk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUpdateDisk.Location = new System.Drawing.Point(768, 161);
            this.btnUpdateDisk.Name = "btnUpdateDisk";
            this.btnUpdateDisk.Size = new System.Drawing.Size(204, 25);
            this.btnUpdateDisk.TabIndex = 2;
            this.btnUpdateDisk.Text = "更新磁盘";
            this.btnUpdateDisk.UseVisualStyleBackColor = true;
            this.btnUpdateDisk.Click += new System.EventHandler(this.btnUpdateDisk_Click);
            // 
            // btnDeleteDisk
            // 
            this.btnDeleteDisk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDeleteDisk.Location = new System.Drawing.Point(768, 192);
            this.btnDeleteDisk.Name = "btnDeleteDisk";
            this.btnDeleteDisk.Size = new System.Drawing.Size(204, 25);
            this.btnDeleteDisk.TabIndex = 3;
            this.btnDeleteDisk.Text = "删除磁盘";
            this.btnDeleteDisk.UseVisualStyleBackColor = true;
            this.btnDeleteDisk.Click += new System.EventHandler(this.btnDeleteDisk_Click);
            // 
            // btnMoveFolderOrFile
            // 
            this.btnMoveFolderOrFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMoveFolderOrFile.Location = new System.Drawing.Point(768, 458);
            this.btnMoveFolderOrFile.Name = "btnMoveFolderOrFile";
            this.btnMoveFolderOrFile.Size = new System.Drawing.Size(204, 25);
            this.btnMoveFolderOrFile.TabIndex = 4;
            this.btnMoveFolderOrFile.Text = "移动待删文件夹或文件";
            this.btnMoveFolderOrFile.UseVisualStyleBackColor = true;
            this.btnMoveFolderOrFile.Click += new System.EventHandler(this.btnMoveFolderOrFile_Click);
            // 
            // textBoxDiskDescribe
            // 
            this.textBoxDiskDescribe.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDiskDescribe.Location = new System.Drawing.Point(768, 12);
            this.textBoxDiskDescribe.Name = "textBoxDiskDescribe";
            this.textBoxDiskDescribe.Size = new System.Drawing.Size(204, 21);
            this.textBoxDiskDescribe.TabIndex = 6;
            // 
            // textBoxNewDiskDescribe
            // 
            this.textBoxNewDiskDescribe.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxNewDiskDescribe.Location = new System.Drawing.Point(768, 547);
            this.textBoxNewDiskDescribe.Name = "textBoxNewDiskDescribe";
            this.textBoxNewDiskDescribe.Size = new System.Drawing.Size(204, 21);
            this.textBoxNewDiskDescribe.TabIndex = 8;
            // 
            // btnChangeDiskDescribe
            // 
            this.btnChangeDiskDescribe.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnChangeDiskDescribe.Location = new System.Drawing.Point(768, 516);
            this.btnChangeDiskDescribe.Name = "btnChangeDiskDescribe";
            this.btnChangeDiskDescribe.Size = new System.Drawing.Size(204, 25);
            this.btnChangeDiskDescribe.TabIndex = 7;
            this.btnChangeDiskDescribe.Text = "更改磁盘描述";
            this.btnChangeDiskDescribe.UseVisualStyleBackColor = true;
            this.btnChangeDiskDescribe.Click += new System.EventHandler(this.btnChangeDiskDescribe_Click);
            // 
            // comboBoxLocalDisk
            // 
            this.comboBoxLocalDisk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxLocalDisk.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxLocalDisk.FormattingEnabled = true;
            this.comboBoxLocalDisk.Location = new System.Drawing.Point(886, 489);
            this.comboBoxLocalDisk.Name = "comboBoxLocalDisk";
            this.comboBoxLocalDisk.Size = new System.Drawing.Size(86, 20);
            this.comboBoxLocalDisk.TabIndex = 9;
            // 
            // btnUpdateLocalDisk
            // 
            this.btnUpdateLocalDisk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUpdateLocalDisk.Location = new System.Drawing.Point(768, 487);
            this.btnUpdateLocalDisk.Name = "btnUpdateLocalDisk";
            this.btnUpdateLocalDisk.Size = new System.Drawing.Size(112, 23);
            this.btnUpdateLocalDisk.TabIndex = 10;
            this.btnUpdateLocalDisk.Text = "刷新磁盘盘符";
            this.btnUpdateLocalDisk.UseVisualStyleBackColor = true;
            this.btnUpdateLocalDisk.Click += new System.EventHandler(this.btnUpdateLocalDisk_Click);
            // 
            // checkBoxBriefScan
            // 
            this.checkBoxBriefScan.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxBriefScan.AutoSize = true;
            this.checkBoxBriefScan.Location = new System.Drawing.Point(768, 40);
            this.checkBoxBriefScan.Name = "checkBoxBriefScan";
            this.checkBoxBriefScan.Size = new System.Drawing.Size(72, 16);
            this.checkBoxBriefScan.TabIndex = 11;
            this.checkBoxBriefScan.Text = "简略扫描";
            this.checkBoxBriefScan.UseVisualStyleBackColor = true;
            this.checkBoxBriefScan.CheckedChanged += new System.EventHandler(this.checkBoxBriefScan_CheckedChanged);
            // 
            // labelScanDepth
            // 
            this.labelScanDepth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelScanDepth.AutoSize = true;
            this.labelScanDepth.Enabled = false;
            this.labelScanDepth.Location = new System.Drawing.Point(865, 41);
            this.labelScanDepth.Name = "labelScanDepth";
            this.labelScanDepth.Size = new System.Drawing.Size(29, 12);
            this.labelScanDepth.TabIndex = 28;
            this.labelScanDepth.Text = "深度";
            // 
            // btnUpdateROF4K
            // 
            this.btnUpdateROF4K.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUpdateROF4K.Location = new System.Drawing.Point(768, 88);
            this.btnUpdateROF4K.Name = "btnUpdateROF4K";
            this.btnUpdateROF4K.Size = new System.Drawing.Size(204, 25);
            this.btnUpdateROF4K.TabIndex = 29;
            this.btnUpdateROF4K.Text = "更新 REAL_OR_FAKE_4K";
            this.btnUpdateROF4K.UseVisualStyleBackColor = true;
            this.btnUpdateROF4K.Click += new System.EventHandler(this.btnUpdateROF4K_Click);
            // 
            // cbScanMedia
            // 
            this.cbScanMedia.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbScanMedia.AutoSize = true;
            this.cbScanMedia.Location = new System.Drawing.Point(768, 64);
            this.cbScanMedia.Name = "cbScanMedia";
            this.cbScanMedia.Size = new System.Drawing.Size(192, 16);
            this.cbScanMedia.TabIndex = 30;
            this.cbScanMedia.Text = "对mkv、mp4等文件扫描媒体信息";
            this.cbScanMedia.UseVisualStyleBackColor = true;
            this.cbScanMedia.CheckedChanged += new System.EventHandler(this.cbScanMedia_CheckedChanged);
            // 
            // tbeLayer
            // 
            this.tbeLayer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbeLayer.Enabled = false;
            this.tbeLayer.Location = new System.Drawing.Point(897, 37);
            this.tbeLayer.Name = "tbeLayer";
            this.tbeLayer.Size = new System.Drawing.Size(75, 21);
            this.tbeLayer.TabIndex = 26;
            this.tbeLayer.Text = "3";
            this.tbeLayer.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbeLayer.MouseLeave += new System.EventHandler(this.tbeLayer_MouseLeave);
            // 
            // btnSetEmptyFolderDelete
            // 
            this.btnSetEmptyFolderDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSetEmptyFolderDelete.Location = new System.Drawing.Point(768, 427);
            this.btnSetEmptyFolderDelete.Name = "btnSetEmptyFolderDelete";
            this.btnSetEmptyFolderDelete.Size = new System.Drawing.Size(204, 25);
            this.btnSetEmptyFolderDelete.TabIndex = 31;
            this.btnSetEmptyFolderDelete.Text = "将空文件夹设为待删";
            this.btnSetEmptyFolderDelete.UseVisualStyleBackColor = true;
            this.btnSetEmptyFolderDelete.Click += new System.EventHandler(this.btnSetEmptyFolderDelete_Click);
            // 
            // btnDatabaseCopy
            // 
            this.btnDatabaseCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDatabaseCopy.Location = new System.Drawing.Point(768, 223);
            this.btnDatabaseCopy.Name = "btnDatabaseCopy";
            this.btnDatabaseCopy.Size = new System.Drawing.Size(204, 25);
            this.btnDatabaseCopy.TabIndex = 32;
            this.btnDatabaseCopy.Text = "复制数据库所有数据到另一数据库";
            this.btnDatabaseCopy.UseVisualStyleBackColor = true;
            this.btnDatabaseCopy.Click += new System.EventHandler(this.btnDatabaseCopy_Click);
            // 
            // ManagerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 580);
            this.Controls.Add(this.btnDatabaseCopy);
            this.Controls.Add(this.btnSetEmptyFolderDelete);
            this.Controls.Add(this.cbScanMedia);
            this.Controls.Add(this.btnUpdateROF4K);
            this.Controls.Add(this.labelScanDepth);
            this.Controls.Add(this.tbeLayer);
            this.Controls.Add(this.checkBoxBriefScan);
            this.Controls.Add(this.btnUpdateLocalDisk);
            this.Controls.Add(this.comboBoxLocalDisk);
            this.Controls.Add(this.textBoxNewDiskDescribe);
            this.Controls.Add(this.btnChangeDiskDescribe);
            this.Controls.Add(this.textBoxDiskDescribe);
            this.Controls.Add(this.btnMoveFolderOrFile);
            this.Controls.Add(this.btnDeleteDisk);
            this.Controls.Add(this.btnUpdateDisk);
            this.Controls.Add(this.btnAddDisk);
            this.Controls.Add(this.dataGridView);
            this.MinimumSize = new System.Drawing.Size(1000, 618);
            this.Name = "ManagerForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "管理";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ManagerForm_FormClosed);
            this.Load += new System.EventHandler(this.ManagerForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.Button btnAddDisk;
        private System.Windows.Forms.Button btnUpdateDisk;
        private System.Windows.Forms.Button btnDeleteDisk;
        private System.Windows.Forms.Button btnMoveFolderOrFile;
        private System.Windows.Forms.TextBox textBoxDiskDescribe;
        private System.Windows.Forms.TextBox textBoxNewDiskDescribe;
        private System.Windows.Forms.Button btnChangeDiskDescribe;
        private System.Windows.Forms.ComboBox comboBoxLocalDisk;
        private System.Windows.Forms.Button btnUpdateLocalDisk;
        private System.Windows.Forms.CheckBox checkBoxBriefScan;
        private TextBoxEx tbeLayer;
        private System.Windows.Forms.Label labelScanDepth;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnIndex;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnDisk;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnFreeSpace;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnTotalSize;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnCompleteScan;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnScanLayer;
        private System.Windows.Forms.Button btnUpdateROF4K;
        private System.Windows.Forms.CheckBox cbScanMedia;
        private System.Windows.Forms.Button btnSetEmptyFolderDelete;
        private System.Windows.Forms.Button btnDatabaseCopy;
    }
}