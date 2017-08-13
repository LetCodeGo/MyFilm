using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyFilm
{
    public partial class Setting : Form
    {
        // 数据库查询
        private SqlData sqlData = null;

        /// <summary>
        /// 表格关联的数据
        /// </summary>
        private DataTable gridViewData = null;

        public Setting(SqlData sqlData)
        {
            InitializeComponent();
            this.sqlData = sqlData;
        }

        private void InitGrid()
        {
            this.ColumnDisk.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            this.ColumnIndex.DataPropertyName = "index";
            this.ColumnDisk.DataPropertyName = "disk_desc";
            this.ColumnFreeSpace.DataPropertyName = "free_space";
            this.ColumnTotalSize.DataPropertyName = "total_size";
            this.ColumnCompleteScan.DataPropertyName = "complete_scan";
            this.ColumnMaxLayer.DataPropertyName = "max_layer";

            this.dataGridView.AutoGenerateColumns = false;

            foreach (DataGridViewColumn col in this.dataGridView.Columns)
                col.SortMode = DataGridViewColumnSortMode.NotSortable;

            gridViewData = ConvertDiskInfoToGrid(sqlData.GetAllDataFromDiskInfo());
            this.dataGridView.DataSource = gridViewData;
        }

        private void InitComboxLocalDisk()
        {
            this.comboBoxLocalDisk.SuspendLayout();
            this.comboBoxLocalDisk.Items.Clear();

            System.IO.DriveInfo[] drives = System.IO.DriveInfo.GetDrives();
            for (int i = 0; i < drives.Length; i++)
            {
                this.comboBoxLocalDisk.Items.Add(drives[i].Name);
            }

            this.comboBoxLocalDisk.SelectedIndex = 0;
            this.comboBoxLocalDisk.ResumeLayout();
        }

        private void Setting_Load(object sender, EventArgs e)
        {
            this.Icon = Properties.Resources.ico;
            InitComboxLocalDisk();
            InitGrid();
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
                dr[5] = completeScan ? "---" : diDt.Rows[i][5].ToString();
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

                sqlData.ScanDisk(
                    dlg.SelectedPath, diskDescribe,
                    this.checkBoxBriefScan.Checked, Convert.ToInt32(this.tbeLayer.Text));

                gridViewData = ConvertDiskInfoToGrid(sqlData.GetAllDataFromDiskInfo());
                this.dataGridView.DataSource = gridViewData;

                MessageBox.Show(String.Format("添加磁盘 {0} 完成！", diskDescribe),
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

                int deleteFilmNumber = sqlData.DeleteByDiskDescribeFromFilmInfo(diskDescribe);
                int deleteDiskNumber = sqlData.DeleteByDiskDescribeFromDiskInfo(diskDescribe);

                sqlData.ScanDisk(
                    dlg.SelectedPath, diskDescribe,
                    this.checkBoxBriefScan.Checked, Convert.ToInt32(this.tbeLayer.Text));

                gridViewData = ConvertDiskInfoToGrid(sqlData.GetAllDataFromDiskInfo());
                this.dataGridView.DataSource = gridViewData;

                MessageBox.Show(String.Format("更新磁盘 {0} 完成！", diskDescribe),
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
            int deleteFilmNumber = sqlData.DeleteByDiskDescribeFromFilmInfo(diskDescribe);
            int deleteDiskNumber = sqlData.DeleteByDiskDescribeFromDiskInfo(diskDescribe);

            gridViewData = ConvertDiskInfoToGrid(sqlData.GetAllDataFromDiskInfo());
            this.dataGridView.DataSource = gridViewData;

            MessageBox.Show(String.Format("删除磁盘 {0} 完成！", diskDescribe),
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

            int deleteNumber = sqlData.CountDeleteDataFromFilmInfo(diskDescribe);
            DataTable dt = sqlData.GetDeleteDataFromFilmInfo(0, deleteNumber, diskDescribe);
            Debug.Assert(deleteNumber == dt.Rows.Count);

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

            Helper.OpenEdit(log);
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

            int changeInFilmInfo = sqlData.UpdateDiskDescribeFromFilmInfo(diskDescribe, diskNewDescribe);
            int changeInDiskInfo = sqlData.UpdateDiskDescribeFromDiskInfo(diskDescribe, diskNewDescribe);

            gridViewData = ConvertDiskInfoToGrid(sqlData.GetAllDataFromDiskInfo());
            this.dataGridView.DataSource = gridViewData;

            MessageBox.Show(String.Format("更改磁盘描述 {0} 为 {1} 完成！", diskDescribe, diskNewDescribe),
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
                }
            }
        }

        private void checkBoxBriefScan_CheckedChanged(object sender, EventArgs e)
        {
            this.tbeLayer.Enabled = this.checkBoxBriefScan.Checked;
        }

        private void tbeLayer_MouseLeave(object sender, EventArgs e)
        {
            if (Convert.ToInt32(this.tbeLayer.Text) == 0)
                this.tbeLayer.Text = "1";
        }

        private void btnUpdateDiskFolderSize_Click(object sender, EventArgs e)
        {
            if (this.dataGridView.SelectedRows.Count != 1)
            {
                MessageBox.Show("请选中一行！", "提示", MessageBoxButtons.OK);
                return;
            }

            bool completeScan = (this.dataGridView.SelectedRows[0].Cells[4].Value.ToString() == "✔");
            if (!completeScan)
            {
                MessageBox.Show("所选磁盘需要为完全扫描！", "提示", MessageBoxButtons.OK);
                return;
            }

            String diskDescribe = this.dataGridView.SelectedRows[0].Cells[1].Value.ToString();
            sqlData.UpdateFolderSizeFromFilmInfo(diskDescribe);

            MessageBox.Show(String.Format("更新磁盘 {0} 下各文件夹大小完成！", diskDescribe),
                "提示", MessageBoxButtons.OK);
        }
    }
}
