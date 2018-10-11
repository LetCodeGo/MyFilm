using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace MyFilm
{
    public partial class ManagerForm : Form
    {
        /// <summary>
        /// 表格关联的数据
        /// </summary>
        private DataTable gridViewData = null;

        private RealOrFake4KWebDataCapture.RealOrFake4KWebDataCaptureResult
            webDataCaptureResult = null;

        private bool bCompleteScan = true;

        private bool[] controlEnableArray = null;
        public Action closeAction = null;

        private bool needReFillRamData = false;

        public ManagerForm()
        {
            InitializeComponent();
        }

        public void ThreaScanDiskResult(bool rst)
        {
            bCompleteScan = rst;
        }

        private void InitGrid()
        {
            this.ColumnDisk.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            this.ColumnIndex.DataPropertyName = "index";
            this.ColumnDisk.DataPropertyName = "disk_desc";
            this.ColumnFreeSpace.DataPropertyName = "free_space";
            this.ColumnTotalSize.DataPropertyName = "total_size";
            this.ColumnCompleteScan.DataPropertyName = "complete_scan";
            this.ColumnScanLayer.DataPropertyName = "scan_layer";

            this.dataGridView.AutoGenerateColumns = false;

            foreach (DataGridViewColumn col in this.dataGridView.Columns)
                col.SortMode = DataGridViewColumnSortMode.NotSortable;

            gridViewData = ConvertDiskInfoToGrid(SqlData.GetInstance().GetAllDataFromDiskInfo());
            this.dataGridView.DataSource = gridViewData;
        }

        private void InitComboxLocalDisk()
        {
            this.comboBoxLocalDisk.SuspendLayout();
            this.comboBoxLocalDisk.Items.Clear();

            System.IO.DriveInfo[] drives = System.IO.DriveInfo.GetDrives();
            // 是否含磁盘E
            bool eDrive = false;
            for (int i = 0; i < drives.Length; i++)
            {
                if ((!eDrive) && drives[i].Name == @"E:\") eDrive = true;
                this.comboBoxLocalDisk.Items.Add(drives[i].Name);
            }

            if (eDrive) this.comboBoxLocalDisk.SelectedItem = @"E:\";
            else this.comboBoxLocalDisk.SelectedIndex = drives.Length - 1;
            this.comboBoxLocalDisk.ResumeLayout();
        }

        private DataTable ConvertDiskInfoToGrid(DataTable diDt)
        {
            DataTable dt = CommonDataTable.GetSettingGridDataTable();

            for (int i = 0; i < diDt.Rows.Count; i++)
            {
                DataRow dr = dt.NewRow();
                dr[0] = i + 1;
                dr[1] = diDt.Rows[i][1];
                dr[2] = Helper.GetSizeString(Convert.ToInt64(diDt.Rows[i][2]));
                dr[3] = Helper.GetSizeString(Convert.ToInt64(diDt.Rows[i][3]));
                Boolean completeScan = Convert.ToBoolean(diDt.Rows[i][4]);
                dr[4] = completeScan ? "✔" : "✘";
                dr[5] = diDt.Rows[i][5];
                dt.Rows.Add(dr);
            }

            return dt;
        }

        private void btnAddDisk_Click(object sender, EventArgs e)
        {
            String diskDescribe = this.textBoxDiskDescribe.Text;
            if (String.IsNullOrWhiteSpace(diskDescribe))
            {
                MessageBox.Show("磁盘描述不能为空！", "提示", MessageBoxButtons.OK);
                return;
            }
            bool flag = false;
            for (int i = 0; i < gridViewData.Rows.Count; i++)
            {
                if (gridViewData.Rows[i]["disk_desc"].ToString() == diskDescribe)
                {
                    flag = true;
                    break;
                }
            }
            if (flag)
            {
                MessageBox.Show("已存在相同的磁盘描述！", "提示", MessageBoxButtons.OK);
                return;
            }

            FolderBrowserDialog dlg = new FolderBrowserDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                if (!dlg.SelectedPath.EndsWith(":\\"))
                {
                    MessageBox.Show("请选择磁盘根目录！", "提示", MessageBoxButtons.OK);
                    return;
                }

                bool bBriefScan = this.checkBoxBriefScan.Checked;
                int setLayer = Convert.ToInt32(this.tbeLayer.Text);

                ProgressForm progressForm = new ProgressForm();
                ThreadScanDisk threadScanDisk = new ThreadScanDisk(
                    dlg.SelectedPath, diskDescribe, this.cbScanMedia.Checked,
                    bBriefScan ? setLayer : Int32.MaxValue,
                    new ThreadScanDisk.ThreadSacnDiskCallback(ThreaScanDiskResult),
                    new ThreadScanDisk.ThreadSacnDiskProgressSetView(progressForm.SetPosAndMsg),
                    new ThreadScanDisk.ThreadSacnDiskProgressFinish(progressForm.SetFinish));

                Thread threadScan = new Thread(new ThreadStart(threadScanDisk.ScanDisk));
                threadScan.Start();
                progressForm.ShowDialog();

                this.needReFillRamData = true;

                gridViewData = ConvertDiskInfoToGrid(SqlData.GetInstance().GetAllDataFromDiskInfo());
                this.dataGridView.DataSource = gridViewData;

                string extraMsg = string.Empty;
                if (bBriefScan && bCompleteScan)
                    extraMsg = string.Format("\n设定扫描层数 {0} 足以进行完全扫描！", setLayer);
                MessageBox.Show(String.Format("添加磁盘 \'{0}\' 完成！{1}", diskDescribe, extraMsg),
                    "提示", MessageBoxButtons.OK);
            }
        }

        private void btnUpdateDisk_Click(object sender, EventArgs e)
        {
            if (this.dataGridView.SelectedRows.Count != 1)
            {
                MessageBox.Show("请选中一行！", "提示", MessageBoxButtons.OK);
                return;
            }

            String diskDescribe = this.dataGridView.SelectedRows[0].Cells[1].Value.ToString();

            FolderBrowserDialog dlg = new FolderBrowserDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                if (!dlg.SelectedPath.EndsWith(":\\"))
                {
                    MessageBox.Show("请选择磁盘根目录！", "提示", MessageBoxButtons.OK);
                    return;
                }

                int deleteFilmNumber = SqlData.GetInstance().DeleteByDiskDescribeFromFilmInfo(diskDescribe);
                int deleteDiskNumber = SqlData.GetInstance().DeleteByDiskDescribeFromDiskInfo(diskDescribe);

                bool bBriefScan = this.checkBoxBriefScan.Checked;
                int setLayer = Convert.ToInt32(this.tbeLayer.Text);

                ProgressForm progressForm = new ProgressForm();
                ThreadScanDisk threadScanDisk = new ThreadScanDisk(
                    dlg.SelectedPath, diskDescribe, this.cbScanMedia.Checked,
                    bBriefScan ? setLayer : Int32.MaxValue,
                    new ThreadScanDisk.ThreadSacnDiskCallback(ThreaScanDiskResult),
                    new ThreadScanDisk.ThreadSacnDiskProgressSetView(progressForm.SetPosAndMsg),
                    new ThreadScanDisk.ThreadSacnDiskProgressFinish(progressForm.SetFinish));

                Thread threadScan = new Thread(new ThreadStart(threadScanDisk.ScanDisk));
                threadScan.Start();
                progressForm.ShowDialog();

                this.needReFillRamData = true;

                gridViewData = ConvertDiskInfoToGrid(SqlData.GetInstance().GetAllDataFromDiskInfo());
                this.dataGridView.DataSource = gridViewData;

                string extraMsg = string.Empty;
                if (bBriefScan && bCompleteScan)
                    extraMsg = string.Format("\n设定扫描层数 {0} 足以进行完全扫描！", setLayer);
                MessageBox.Show(String.Format("更新磁盘 \'{0}\' 完成！{1}", diskDescribe, extraMsg),
                    "提示", MessageBoxButtons.OK);
            }
        }

        private void btnDeleteDisk_Click(object sender, EventArgs e)
        {
            if (this.dataGridView.SelectedRows.Count != 1)
            {
                MessageBox.Show("请选中一行！", "提示", MessageBoxButtons.OK);
                return;
            }

            String diskDescribe = this.dataGridView.SelectedRows[0].Cells[1].Value.ToString();
            int deleteFilmNumber = SqlData.GetInstance().DeleteByDiskDescribeFromFilmInfo(diskDescribe);
            int deleteDiskNumber = SqlData.GetInstance().DeleteByDiskDescribeFromDiskInfo(diskDescribe);

            this.needReFillRamData = true;

            gridViewData = ConvertDiskInfoToGrid(SqlData.GetInstance().GetAllDataFromDiskInfo());
            this.dataGridView.DataSource = gridViewData;

            MessageBox.Show(String.Format("删除磁盘 \'{0}\' 完成！", diskDescribe),
                "提示", MessageBoxButtons.OK);
        }

        private void btnMoveFolderOrFile_Click(object sender, EventArgs e)
        {
            if (this.dataGridView.SelectedRows.Count != 1)
            {
                MessageBox.Show("请选中一行！", "提示", MessageBoxButtons.OK);
                return;
            }

            String log = String.Empty;
            String logSuccess = String.Empty;
            int moveSuccess = 0;
            String logFailed = String.Empty;
            int moveFailed = 0;

            String diskDescribe = this.dataGridView.SelectedRows[0].Cells[1].Value.ToString();

            //int deleteNumber = SqlData.GetInstance().CountDeleteDataFromFilmInfo(diskDescribe);
            //DataTable dt = SqlData.GetInstance().GetDeleteDataFromFilmInfo(0, deleteNumber, diskDescribe);

            int[] idList = SqlData.GetInstance().GetDeleteDataFromFilmInfo(diskDescribe);
            DataTable dt = SqlData.GetInstance().SelectDataByIDList(idList);
            Debug.Assert(idList.Length == dt.Rows.Count);

            String localDisk = this.comboBoxLocalDisk.SelectedItem.ToString();
            String moveToFolder = Path.Combine(localDisk, "ToDelete");
            if (!Directory.Exists(moveToFolder)) Directory.CreateDirectory(moveToFolder);

            log += String.Format("查询到待删记录 {0} 条\r\n", dt.Rows.Count);
            log += String.Format("目标文件夹 {0}\r\n", moveToFolder);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                String name = dt.Rows[i]["name"].ToString();
                String path = dt.Rows[i]["path"].ToString();
                Boolean isFolder = Convert.ToBoolean(dt.Rows[i]["is_folder"]);

                String moveToPath = Path.Combine(moveToFolder, name);
                // 更改磁盘名称（扫描时的磁盘和此时的磁盘名称可能不同）
                String moveFromPath = localDisk[0] + path.Substring(1);

                if (isFolder)
                {
                    if (Directory.Exists(moveFromPath))
                    {
                        Directory.Move(moveFromPath, moveToPath);
                        moveSuccess++;
                        logSuccess += String.Format("{0} ---> {1}\r\n", moveFromPath, moveToPath);
                    }
                    else
                    {
                        moveFailed++;
                        logFailed += String.Format("{0}\r\n", moveFromPath);
                    }
                }
                else
                {
                    if (File.Exists(moveFromPath))
                    {
                        File.Move(moveFromPath, moveToPath);
                        moveSuccess++;
                        logSuccess += String.Format("{0} ---> {1}\r\n", moveFromPath, moveToPath);
                    }
                    else
                    {
                        moveFailed++;
                        logFailed += String.Format("{0}\r\n", moveFromPath);
                    }
                }
            }

            log += String.Format("\r\n成功 {0} 项：\r\n{1}\r\n失败 {2} 项：\r\n{3}",
                moveSuccess, logSuccess, moveFailed, logFailed);

            String filePath = Path.Combine(CommonString.AppDataFolder, "myfilm.temp");
            File.WriteAllText(filePath, log, System.Text.Encoding.UTF8);

            Helper.OpenEdit(filePath, log);
        }

        private void btnUpdateLocalDisk_Click(object sender, EventArgs e)
        {
            InitComboxLocalDisk();
        }

        private void btnChangeDiskDescribe_Click(object sender, EventArgs e)
        {
            if (this.dataGridView.SelectedRows.Count != 1)
            {
                MessageBox.Show("请选中一行！", "提示", MessageBoxButtons.OK);
                return;
            }

            String diskDescribe = this.dataGridView.SelectedRows[0].Cells[1].Value.ToString();
            String diskNewDescribe = this.textBoxNewDiskDescribe.Text;

            if (String.IsNullOrWhiteSpace(diskNewDescribe))
            {
                MessageBox.Show("磁盘描述不能为空！", "提示", MessageBoxButtons.OK);
                return;
            }
            bool flag = false;
            for (int i = 0; i < gridViewData.Rows.Count; i++)
            {
                if (gridViewData.Rows[i]["disk_desc"].ToString() == diskNewDescribe)
                {
                    flag = true;
                    break;
                }
            }
            if (flag)
            {
                MessageBox.Show("已存在相同的磁盘描述！", "提示", MessageBoxButtons.OK);
                return;
            }

            int changeInFilmInfo = SqlData.GetInstance().UpdateDiskDescribeFromFilmInfo(diskDescribe, diskNewDescribe);
            int changeInDiskInfo = SqlData.GetInstance().UpdateDiskDescribeFromDiskInfo(diskDescribe, diskNewDescribe);

            gridViewData = ConvertDiskInfoToGrid(SqlData.GetInstance().GetAllDataFromDiskInfo());
            this.dataGridView.DataSource = gridViewData;

            MessageBox.Show(String.Format("更改磁盘描述 \'{0}\' 为 \'{1}\' 完成！", diskDescribe, diskNewDescribe),
                "提示", MessageBoxButtons.OK);
        }

        private void dataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (sender != null)
            {
                DataGridView dgv = sender as DataGridView;
                if (dgv.SelectedRows.Count == 1)
                {
                    int index = dgv.SelectedRows[0].Index;
                    this.textBoxDiskDescribe.Text = this.gridViewData.Rows[index]["disk_desc"].ToString();

                    bool bCompleteScan = this.gridViewData.Rows[index]["complete_scan"].ToString() == "✔";
                    this.checkBoxBriefScan.Checked = !bCompleteScan;
                    this.tbeLayer.Text = this.gridViewData.Rows[index]["scan_layer"].ToString();
                    this.labelScanDepth.Enabled = !bCompleteScan;
                    this.tbeLayer.Enabled = !bCompleteScan;
                }
            }
        }

        private void checkBoxBriefScan_CheckedChanged(object sender, EventArgs e)
        {
            this.labelScanDepth.Enabled = this.checkBoxBriefScan.Checked;
            this.tbeLayer.Enabled = this.checkBoxBriefScan.Checked;
        }

        private void tbeLayer_MouseLeave(object sender, EventArgs e)
        {
            if (Convert.ToInt32(this.tbeLayer.Text) == 0)
                this.tbeLayer.Text = "1";
        }

        private void dataGridView_DataSourceChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < this.dataGridView.RowCount; i++)
            {
                if (this.dataGridView.Rows[i].Cells["ColumnCompleteScan"].Value.ToString() == "✘")
                {
                    this.dataGridView.Rows[i].Cells["ColumnCompleteScan"].Style.ForeColor =
                        System.Drawing.Color.Red;
                }
                else
                {
                    this.dataGridView.Rows[i].Cells["ColumnCompleteScan"].Style.ForeColor =
                        System.Drawing.Color.Empty;
                }
            }
        }

        private void btnUpdateROF4K_Click(object sender, EventArgs e)
        {
            WaitingForm waitingForm = new WaitingForm();
            RealOrFake4KWebDataCapture webDataCapture = new RealOrFake4KWebDataCapture(
                SetWebCaptureDataResult, waitingForm.SetFinish);
            Thread threadWebDataCapture = new Thread(new ThreadStart(webDataCapture.Update4KInfo));
            threadWebDataCapture.Start();
            waitingForm.ShowDialog();

            if (this.webDataCaptureResult.code >= 0)
                LoginForm.UpdateWebDataCaptureTimeAndSaveXml(this.webDataCaptureResult.crawlTime);
            if (this.webDataCaptureResult.code > 0) this.needReFillRamData = true;

            MessageBox.Show(this.webDataCaptureResult.strMsg);
        }

        public void SetWebCaptureDataResult(
            RealOrFake4KWebDataCapture.RealOrFake4KWebDataCaptureResult rst)
        {
            this.webDataCaptureResult = rst;
        }

        private void ManagerForm_Load(object sender, EventArgs e)
        {
            this.controlEnableArray = new bool[this.Controls.Count];

            this.Icon = Properties.Resources.ico;
            InitComboxLocalDisk();
            InitGrid();

            this.cbScanMedia.Checked = true;

            bool enabledFlag = true;
            bool mediaInfoFlag = true;
            string errMsg = "";
            mediaInfoFlag = ThreadScanDisk.MediaInfoState(ref errMsg);

            if (!mediaInfoFlag)
            {
                MessageBox.Show(string.Format("{0}\n添加和更新磁盘功能不可用", errMsg));

                enabledFlag = false;
            }

            this.btnAddDisk.Enabled = enabledFlag;
            this.btnUpdateDisk.Enabled = enabledFlag;
        }

        private void cbScanMedia_CheckedChanged(object sender, EventArgs e)
        {
            bool enabledFlag = true;

            CheckBox cb = sender as CheckBox;
            if (cb.Checked)
            {
                bool mediaInfoFlag = true;
                string errMsg = "";
                mediaInfoFlag = ThreadScanDisk.MediaInfoState(ref errMsg);

                if (!mediaInfoFlag)
                {
                    MessageBox.Show(string.Format("{0}\n添加和更新磁盘功能不可用", errMsg));

                    enabledFlag = false;
                }
            }

            this.btnAddDisk.Enabled = enabledFlag;
            this.btnUpdateDisk.Enabled = enabledFlag;
        }

        public void SetControlEnable(bool connectState)
        {
            if (connectState)
            {
                int i = 0;
                foreach (Control cl in this.Controls) cl.Enabled = controlEnableArray[i++];
            }
            else
            {
                int i = 0;
                foreach (Control cl in this.Controls)
                {
                    controlEnableArray[i++] = cl.Enabled;
                    cl.Enabled = false;
                }
            }
        }

        private void ManagerForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (needReFillRamData) SqlData.GetInstance().FillRamData();
            this.closeAction?.Invoke();
        }
    }
}
