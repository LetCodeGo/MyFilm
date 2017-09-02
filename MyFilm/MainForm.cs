﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace MyFilm
{
    public partial class MainForm : Form
    {
        /// <summary>
        /// 数据库接口类
        /// </summary>
        private SqlData sqlData = new SqlData();

        /// <summary>
        /// 总记录数
        /// </summary>
        private int totalRowCount = 0;

        /// <summary>
        /// 每页记录数
        /// </summary>
        private int pageRowCount = 30;

        /// <summary>
        /// 当前页面索引数（从0开始）
        /// </summary>
        private int currentPageIndex = 0;

        /// <summary>
        /// 分页取数据源（搜索时从数据库取，其他的从获得的datatable取）
        /// </summary>
        private enum SourceType
        {
            DATABASE_SEARCH,
            DATABASE_DELETE,
            DATABASE_WATCH,
            DATABASE_PID,
            DATATABLE_LOCAL
        }

        /// <summary>
        /// 记录最近一次执行时的条件
        /// </summary>
        public struct ActionParam
        {
            public int Pid;
            public String FolderPath;

            public String KeyWord;
            public String DiskDescribe;
        }

        private SourceType sourceType = SourceType.DATATABLE_LOCAL;
        private ActionParam actionParam = new ActionParam();

        /// <summary>
        /// 分页用的
        /// </summary>
        private DataTable sourceDataTable = null;

        /// <summary>
        /// 表格关联的数据
        /// </summary>
        private DataTable gridViewData = new DataTable();

        /// <summary>
        /// 打开的 nfo 文件所在文件夹
        /// </summary>
        private static String nfoFolder = Path.Combine(CommonString.AppDataFolder, "NFO");

        private bool heartBeatFlag = true;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.Icon = Properties.Resources.ico;
            this.Text = CommonString.DbName;

            this.notifyIcon.Icon = Properties.Resources.ico;
            this.notifyIcon.Visible = false;
            this.notifyIcon.Text = String.Format("MyFilm - {0}", CommonString.DbName);
            this.notifyIcon.ContextMenuStrip = this.contextMenuStripNotify;

            // 删除NFO文件中所有的nfo文件
            if (Directory.Exists(nfoFolder))
            {
                String[] nfoFiles = Directory.GetFiles(nfoFolder, "*.nfo", SearchOption.TopDirectoryOnly);
                nfoFiles.ToList().ForEach(filePath => File.Delete(filePath));
            }
            else Directory.CreateDirectory(nfoFolder);

            // 开启线程，接收从另一进程发送的数据
            ProcessReceiveData.ShowSearchResultAction = this.ShowSearchResult;
            Thread thread = new Thread(new ParameterizedThreadStart(ProcessReceiveData.ReceiveData));
            thread.Start(this.Handle);

            // 连接数据库创建表
            sqlData.OpenMySql();
            sqlData.CreateTables();
            // 设置 DataGridView
            SetDataGridView();
            // 获取根目录数据源
            sourceDataTable = GetDiskRootDirectoryInfo();
            totalRowCount = sourceDataTable.Rows.Count;
            InitPageCombox();
            sourceType = SourceType.DATATABLE_LOCAL;

            // 开启心跳线程
            Thread thread1 = new Thread(new ThreadStart(MySqlHeartBeat));
            thread1.Start();

            InitDiskCombox();

            // 首次显示时，若关键字为空，则显示根目录，否则显示搜索界面
            if (String.IsNullOrWhiteSpace(CommonString.WebSearchKeyWord))
                ShowDataGridViewPage(0);
            else
            {
                this.textBoxSearch.Text = CommonString.WebSearchKeyWord;
                btnSearch_Click(null, null);
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            heartBeatFlag = false;

            ProcessReceiveData.receiveExit = true;
            ProcessSendData.SendData("quit");

            sqlData.CloseMySql();
        }

        /// <summary>
        /// 设置 DataGridView 关联数据源及样式
        /// </summary>
        private void SetDataGridView()
        {
            this.ColumnName.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            this.ColumnPath.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            this.ColumnDisk.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            this.ColumnIndex.DataPropertyName = "index";
            this.ColumnName.DataPropertyName = "name";
            this.ColumnPath.DataPropertyName = "path";
            this.ColumnSize.DataPropertyName = "size";
            this.ColumnDisk.DataPropertyName = "disk_desc";
            this.ColumnModify.DataPropertyName = "modify_t";
            this.ColumnDelete.DataPropertyName = "to_delete";
            this.ColumnWatch.DataPropertyName = "to_watch";

            this.dataGridView.AutoGenerateColumns = false;
            this.dataGridView.ContextMenuStrip = this.contextMenuStrip;

            foreach (DataGridViewColumn col in this.dataGridView.Columns)
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
        }

        /// <summary>
        /// 初始化磁盘选择控件
        /// </summary>
        private void InitDiskCombox()
        {
            this.comboBoxDisk.SuspendLayout();
            this.comboBoxDisk.Items.Clear();
            this.comboBoxDisk.Items.Add("全部");
            for (int i = 0; i < sourceDataTable.Rows.Count; i++)
            {
                this.comboBoxDisk.Items.Add(sourceDataTable.Rows[i]["disk_desc"]);
            }
            this.comboBoxDisk.SelectedIndex = 0;
            this.comboBoxDisk.ResumeLayout();
        }

        /// <summary>
        /// 初始化页数选择控件
        /// </summary>
        private void InitPageCombox()
        {
            // 总页数
            int totalPageCount = CalcTotalPageCount();

            this.comboBoxPage.SuspendLayout();
            this.comboBoxPage.SelectedIndexChanged -= new System.EventHandler(this.comboBoxPage_SelectedIndexChanged);
            this.comboBoxPage.Items.Clear();
            for (int i = 0; i != totalPageCount; i++)
            {
                this.comboBoxPage.Items.Add((i + 1).ToString());
            }
            this.comboBoxPage.SelectedIndex = 0;
            this.comboBoxPage.SelectedIndexChanged += new System.EventHandler(this.comboBoxPage_SelectedIndexChanged);
            this.comboBoxPage.ResumeLayout();
        }

        private void SetPageComboxIndex()
        {
            this.comboBoxPage.SelectedIndexChanged -= new System.EventHandler(this.comboBoxPage_SelectedIndexChanged);
            this.comboBoxPage.SelectedIndex = currentPageIndex;
            this.comboBoxPage.SelectedIndexChanged += new System.EventHandler(this.comboBoxPage_SelectedIndexChanged);
        }

        /// <summary>
        /// 计算总页数
        /// </summary>
        /// <returns></returns>
        private int CalcTotalPageCount()
        {
            int totalPageCount = totalRowCount / pageRowCount + 1;
            if (totalRowCount != 0 && totalRowCount % pageRowCount == 0)
            {
                totalPageCount -= 1;
            }
            return totalPageCount;
        }

        /// <summary>
        /// 显示某一页
        /// </summary>
        /// <param name="page">从0开始</param>
        private void ShowDataGridViewPage(int page)
        {
            this.currentPageIndex = page;
            int startIndex = this.currentPageIndex * this.pageRowCount;

            // 总页数
            int totalPageCount = CalcTotalPageCount();

            if (currentPageIndex == 0)
            {
                this.btnFirstPage.Enabled = false;
                this.btnPrePage.Enabled = false;
            }
            else
            {
                this.btnFirstPage.Enabled = true;
                this.btnPrePage.Enabled = true;
            }
            if (currentPageIndex == totalPageCount - 1)
            {
                this.btnNextPage.Enabled = false;
                this.btnLastPage.Enabled = false;
            }
            else
            {
                this.btnNextPage.Enabled = true;
                this.btnLastPage.Enabled = true;
            }

            String explain1 = String.Format(
                "总共 {0} 条记录，当前第 {1} 页，共 {2} 页",
                totalRowCount, currentPageIndex + 1, totalPageCount);
            String explain2 = String.Empty;

            SetPageComboxIndex();

            switch (sourceType)
            {
                case SourceType.DATABASE_SEARCH:
                    {
                        gridViewData = ConvertFilmInfoToGrid(
                            sqlData.SearchKeyWordFromFilmInfo(
                                actionParam.KeyWord, startIndex, pageRowCount,
                                actionParam.DiskDescribe == "全部" ? null : actionParam.DiskDescribe));
                        explain2 = String.Format("在 {0} 里搜索 \'{1}\'",
                            actionParam.DiskDescribe == "全部" ? "所有磁盘" : actionParam.DiskDescribe,
                            actionParam.KeyWord);
                        break;
                    }
                case SourceType.DATABASE_DELETE:
                    {
                        gridViewData = ConvertFilmInfoToGrid(
                            sqlData.GetDeleteDataFromFilmInfo(
                                startIndex, pageRowCount,
                                actionParam.DiskDescribe == "全部" ? null : actionParam.DiskDescribe));
                        explain2 = String.Format("在 {0} 里搜索 待删",
                            actionParam.DiskDescribe == "全部" ? "所有磁盘" : actionParam.DiskDescribe);
                        break;
                    }
                case SourceType.DATABASE_WATCH:
                    {
                        gridViewData = ConvertFilmInfoToGrid(
                            sqlData.GetWatchDataFromFilmInfo(
                                startIndex, pageRowCount,
                                actionParam.DiskDescribe == "全部" ? null : actionParam.DiskDescribe));
                        explain2 = String.Format("在 {0} 里搜索 待看",
                            actionParam.DiskDescribe == "全部" ? "所有磁盘" : actionParam.DiskDescribe);
                        break;
                    }
                case SourceType.DATABASE_PID:
                    {
                        gridViewData = ConvertFilmInfoToGrid(
                            sqlData.GetDataByPidFromFilmInfo(
                                actionParam.Pid, startIndex, pageRowCount));
                        explain2 = String.Format("索引 \'{0}\'", actionParam.FolderPath);
                        break;
                    }
                case SourceType.DATATABLE_LOCAL:
                    {
                        if (sourceDataTable.Rows.Count != 0)
                            gridViewData = sourceDataTable
                                .AsEnumerable()
                                .Where((row, index) => index >= startIndex && index < startIndex + pageRowCount)
                                .CopyToDataTable();
                        explain2 = "索引 根目录";
                        break;
                    }
                default: break;
            }

            for (int i = 0; i < gridViewData.Rows.Count; i++)
                gridViewData.Rows[i]["index"] = currentPageIndex * pageRowCount + i + 1;

            String explain = String.Format("{0} （{1}）", explain1, explain2);
            Size sizeT = TextRenderer.MeasureText(explain, this.labelExplain.Font);
            int length = explain2.Length;
            while (sizeT.Width > this.labelExplain.Width && length > 0)
            {
                length--;
                explain = String.Format("{0} （{1}...）", explain1, explain2.Substring(0, length));
                sizeT = TextRenderer.MeasureText(explain, this.labelExplain.Font);
            }
            this.labelExplain.Text = explain;
            this.labelExplain.Tag = String.Format("{0}|{1}", explain1, explain2);

            this.dataGridView.DataSource = gridViewData;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            String keyWord = this.textBoxSearch.Text;
            // 为空时直接显示根目录
            if (String.IsNullOrWhiteSpace(keyWord))
            {
                btnRootDirectory_Click(null, null);
                return;
            }

            actionParam.KeyWord = keyWord;
            actionParam.DiskDescribe = this.comboBoxDisk.SelectedItem.ToString();
            sourceType = SourceType.DATABASE_SEARCH;

            totalRowCount = sqlData.CountSearchKeyWordFromFilmInfo(
                actionParam.KeyWord, actionParam.DiskDescribe == "全部" ? null : actionParam.DiskDescribe);

            if (totalRowCount == 0) this.textBoxSearch.ForeColor = Color.Red;
            else this.textBoxSearch.ForeColor = Color.Black;

            InitPageCombox();

            // 写入搜索记录
            sqlData.InsertDataToSearchLog(keyWord, totalRowCount, DateTime.Now);

            ShowDataGridViewPage(0);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            actionParam.DiskDescribe = this.comboBoxDisk.SelectedItem.ToString();
            sourceType = SourceType.DATABASE_DELETE;

            totalRowCount = sqlData.CountDeleteDataFromFilmInfo(
                actionParam.DiskDescribe == "全部" ? null : actionParam.DiskDescribe);

            InitPageCombox();

            ShowDataGridViewPage(0);
        }

        private void btnWatch_Click(object sender, EventArgs e)
        {
            actionParam.DiskDescribe = this.comboBoxDisk.SelectedItem.ToString();
            sourceType = SourceType.DATABASE_WATCH;

            totalRowCount = sqlData.CountWatchDataFromFilmInfo(
                actionParam.DiskDescribe == "全部" ? null : actionParam.DiskDescribe);

            InitPageCombox();

            ShowDataGridViewPage(0);
        }

        private void btnManager_Click(object sender, EventArgs e)
        {
            ManagerForm form = new ManagerForm(this.sqlData);
            form.ShowDialog();

            // 设置返回后显示根目录
            btnRootDirectory_Click(null, null);
        }

        private void btnUpFolder_Click(object sender, EventArgs e)
        {
            // sourceType 为 SourceType.DATABASE_PID 时，选择的行可为0（考虑场景 空文件夹）
            if (this.dataGridView.SelectedRows.Count == 1 || sourceType == SourceType.DATABASE_PID)
            {
                int pid = actionParam.Pid;
                if (this.dataGridView.SelectedRows.Count == 1)
                    pid = Convert.ToInt32(gridViewData.Rows[this.dataGridView.SelectedRows[0].Index]["pid"]);

                if (pid == -1)
                {
                    MessageBox.Show("已是顶层！", "提示", MessageBoxButtons.OK);
                    return;
                }

                DataTable dt = sqlData.GetDataByIdFromFilmInfo(pid);

                actionParam.Pid = Convert.ToInt32(dt.Rows[0]["pid"]);
                actionParam.FolderPath = Helper.GetUpFolder(dt.Rows[0]["path"].ToString());
                sourceType = SourceType.DATABASE_PID;

                totalRowCount = sqlData.CountPidFromFilmInfo(actionParam.Pid);
                int offset = sqlData.GetIdOffsetByPidFromFilmInfo(Convert.ToInt32(dt.Rows[0]["id"]), actionParam.Pid);
                Debug.Assert(offset >= 0 && offset < totalRowCount);

                InitPageCombox();

                int selectPage = offset / pageRowCount;
                int selectRow = offset % pageRowCount;

                ShowDataGridViewPage(selectPage);
                if (selectRow != 0)
                {
                    this.dataGridView.ClearSelection();
                    // 这里第0行时，不知什么原因选不上
                    this.dataGridView.CurrentCell = this.dataGridView.Rows[selectRow].Cells[0];
                }
            }
            else
            {
                MessageBox.Show("请选中一行！", "提示", MessageBoxButtons.OK);
                return;
            }
        }

        private void btnFirstPage_Click(object sender, EventArgs e)
        {
            ShowDataGridViewPage(0);
        }

        private void btnPrePage_Click(object sender, EventArgs e)
        {
            ShowDataGridViewPage(currentPageIndex - 1);
        }

        private void btnNextPage_Click(object sender, EventArgs e)
        {
            ShowDataGridViewPage(currentPageIndex + 1);
        }

        private void btnLastPage_Click(object sender, EventArgs e)
        {
            ShowDataGridViewPage(CalcTotalPageCount() - 1);
        }

        private void comboBoxPage_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowDataGridViewPage(this.comboBoxPage.SelectedIndex);
        }

        private void btnRootDirectory_Click(object sender, EventArgs e)
        {
            sourceType = SourceType.DATATABLE_LOCAL;
            sourceDataTable = GetDiskRootDirectoryInfo();
            totalRowCount = sourceDataTable.Rows.Count;

            InitPageCombox();

            ShowDataGridViewPage(0);
        }

        private void dataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            bool needLocation = false;
            int locationId = int.MinValue;

            int pid = int.MinValue;

            // 如果是文件夹就浏览文件夹下内容
            if (e.ColumnIndex == 1 && Convert.ToBoolean(gridViewData.Rows[e.RowIndex]["is_folder"]))
            {
                pid = Convert.ToInt32(gridViewData.Rows[e.RowIndex]["id"]);
                actionParam.FolderPath = Path.Combine(
                    gridViewData.Rows[e.RowIndex]["path"].ToString(),
                    gridViewData.Rows[e.RowIndex]["name"].ToString());
            }
            // 浏览此文件或文件夹所在文件夹下的全部内容
            else if (e.ColumnIndex == 2)
            {
                pid = Convert.ToInt32(gridViewData.Rows[e.RowIndex]["pid"]);
                actionParam.FolderPath = gridViewData.Rows[e.RowIndex]["path"].ToString();

                needLocation = true;
                locationId = Convert.ToInt32(gridViewData.Rows[e.RowIndex]["id"]);
            }

            if (pid != int.MinValue)
            {
                actionParam.Pid = pid;
                sourceType = SourceType.DATABASE_PID;

                totalRowCount = sqlData.CountPidFromFilmInfo(actionParam.Pid);

                int offset = 0;
                if (needLocation)
                {
                    offset = sqlData.GetIdOffsetByPidFromFilmInfo(locationId, actionParam.Pid);
                    Debug.Assert(offset >= 0 && offset < totalRowCount);
                }

                InitPageCombox();

                int selectPage = offset / pageRowCount;
                int selectRow = offset % pageRowCount;

                ShowDataGridViewPage(selectPage);
                if (selectRow != 0)
                {
                    this.dataGridView.ClearSelection();
                    // 这里第0行时，不知什么原因选不上
                    this.dataGridView.CurrentCell = this.dataGridView.Rows[selectRow].Cells[0];
                }
            }
        }

        private DataTable GetDiskRootDirectoryInfo()
        {
            DataTable dt1 = sqlData.GetAllRootDirectoryFromFilmInfo();
            DataTable dt2 = sqlData.GetAllDataFromDiskInfo();
            Debug.Assert(dt1.Rows.Count == dt2.Rows.Count);

            DataTable dt = ConvertFilmInfoToGrid(dt1);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                String diskDescribe = dt.Rows[i]["disk_desc"].ToString();
                for (int j = 0; j < dt2.Rows.Count; j++)
                {
                    if (diskDescribe.Equals(dt2.Rows[j]["disk_desc"]))
                    {
                        long freeSpace = Convert.ToInt64(dt2.Rows[j]["free_space"]);
                        long totalSize = Convert.ToInt64(dt2.Rows[j]["total_size"]);
                        dt.Rows[i]["size"] = String.Format("{0} / {1}",
                            Helper.GetSizeString(freeSpace), Helper.GetSizeString(totalSize));
                        break;
                    }
                }
            }
            return dt;
        }

        private DataTable ConvertFilmInfoToGrid(DataTable fiDt)
        {
            DataTable dt = CommonDataTable.GetMainFormGridDataTable();

            for (int i = 0; i < fiDt.Rows.Count; i++)
            {
                DataRow dr = dt.NewRow();
                for (int j = 0; j < fiDt.Columns.Count; j++)
                {
                    // path
                    if (j == 2)
                    {
                        // 用父文件夹
                        dr[j + 1] = Helper.GetUpFolder(fiDt.Rows[i][j].ToString());
                    }
                    // size
                    else if (j == 3)
                    {
                        long size = Convert.ToInt64(fiDt.Rows[i][j]);
                        if (size == -1) dr[j + 1] = "---";
                        else dr[j + 1] = Helper.GetSizeString(size);
                    }
                    else if (j == 4 || j == 5)
                        dr[j + 1] = Convert.ToDateTime(fiDt.Rows[i][j]).ToString("yyyy-MM-dd HHH:mm:ss");
                    else dr[j + 1] = fiDt.Rows[i][j];
                }
                dr[0] = i + 1;
                dt.Rows.Add(dr);
            }
            return dt;
        }

        private void contextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            // 只有选中了行才弹出菜单
            if (this.dataGridView.SelectedRows.Count < 1) e.Cancel = true;
            else if (this.dataGridView.SelectedRows.Count == 1)
            {
                int index = this.dataGridView.SelectedRows[0].Index;
                bool isDelete = Convert.ToBoolean(this.dataGridView.Rows[index].Cells["ColumnDelete"].Value);
                bool isWatch = Convert.ToBoolean(this.dataGridView.Rows[index].Cells["ColumnWatch"].Value);
                bool isFolder = Convert.ToBoolean(gridViewData.Rows[index]["is_folder"]);
                String fileName = gridViewData.Rows[index]["name"].ToString();
                bool isShowContent = (fileName.Substring(fileName.LastIndexOf('.') + 1).ToLower() == "nfo");

                this.toolStripMenuItemSetDelete.Enabled = !isDelete;
                this.toolStripMenuItemSetWatch.Enabled = !isWatch;
                this.toolStripMenuItemCancelDelete.Enabled = isDelete;
                this.toolStripMenuItemCancelWatch.Enabled = isWatch;
                this.toolStripMenuItemOpenFolder.Enabled = true;
                this.toolStripMenuItemPrintFolderTree.Enabled = isFolder;
                this.toolStripMenuItemShowContent.Enabled = isShowContent;
            }
            else
            {
                this.toolStripMenuItemSetDelete.Enabled = true;
                this.toolStripMenuItemSetWatch.Enabled = true;
                this.toolStripMenuItemCancelDelete.Enabled = true;
                this.toolStripMenuItemCancelWatch.Enabled = true;
                this.toolStripMenuItemOpenFolder.Enabled = false;
                this.toolStripMenuItemPrintFolderTree.Enabled = false;
                this.toolStripMenuItemShowContent.Enabled = false;
            }
        }

        private void toolStripMenuItemSetDelete_Click(object sender, EventArgs e)
        {
            List<Int32> idList = new List<int>();
            foreach (DataGridViewRow row in dataGridView.SelectedRows)
            {
                idList.Add(Convert.ToInt32(gridViewData.Rows[row.Index]["id"]));
                this.dataGridView.Rows[row.Index].Cells["ColumnDelete"].Value = true;
            }
            sqlData.UpdateDeleteStateFromFilmInfo(idList, true);
        }

        private void toolStripMenuItemSetWatch_Click(object sender, EventArgs e)
        {
            List<Int32> idList = new List<int>();
            foreach (DataGridViewRow row in dataGridView.SelectedRows)
            {
                idList.Add(Convert.ToInt32(gridViewData.Rows[row.Index]["id"]));
                this.dataGridView.Rows[row.Index].Cells["ColumnWatch"].Value = true;
            }
            sqlData.UpdateWatchStateFromFilmInfo(idList, true);
        }

        private void toolStripMenuItemCancelDelete_Click(object sender, EventArgs e)
        {
            List<Int32> idList = new List<int>();
            foreach (DataGridViewRow row in dataGridView.SelectedRows)
            {
                idList.Add(Convert.ToInt32(gridViewData.Rows[row.Index]["id"]));
                this.dataGridView.Rows[row.Index].Cells["ColumnDelete"].Value = false;
            }
            sqlData.UpdateDeleteStateFromFilmInfo(idList, false);
        }

        private void toolStripMenuItemCancelWatch_Click(object sender, EventArgs e)
        {
            List<Int32> idList = new List<int>();
            foreach (DataGridViewRow row in dataGridView.SelectedRows)
            {
                idList.Add(Convert.ToInt32(gridViewData.Rows[row.Index]["id"]));
                this.dataGridView.Rows[row.Index].Cells["ColumnWatch"].Value = false;
            }
            sqlData.UpdateWatchStateFromFilmInfo(idList, false);
        }

        private void toolStripMenuItemOpenFolder_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count != 1)
            {
                MessageBox.Show("请选中一行！", "提示", MessageBoxButtons.OK);
                return;
            }

            int index = dataGridView.SelectedRows[0].Index;
            bool isFolder = Convert.ToBoolean(gridViewData.Rows[index]["is_folder"]);
            String fileName = gridViewData.Rows[index]["name"].ToString();
            String upFolderPath = gridViewData.Rows[index]["path"].ToString();
            // 文件时打开其所在文件夹，文件夹时打开此文件夹
            String folderPath = upFolderPath;
            if (isFolder) folderPath = Path.Combine(upFolderPath, fileName);
            System.Diagnostics.Process.Start(folderPath);
        }

        private void toolStripMenuItemPrintFolderTree_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count != 1)
            {
                MessageBox.Show("请选中一行！", "提示", MessageBoxButtons.OK);
                return;
            }

            int selectIndex = dataGridView.SelectedRows[0].Index;
            bool isFolder = Convert.ToBoolean(gridViewData.Rows[selectIndex]["is_folder"]);
            if (!isFolder)
            {
                MessageBox.Show("请选择文件夹！", "提示", MessageBoxButtons.OK);
                return;
            }

            String strResult = String.Empty;
            PrintFolder(Convert.ToInt32(gridViewData.Rows[selectIndex]["id"]),
                gridViewData.Rows[selectIndex]["name"].ToString(), 0, "", ref strResult);

            String filePath = Path.Combine(CommonString.AppDataFolder, "myfilm.temp");
            File.WriteAllText(filePath, strResult, System.Text.Encoding.UTF8);

            Helper.OpenEdit(filePath, strResult);
        }

        private void dataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            Boolean toWatch = Convert.ToBoolean(
                this.dataGridView.Rows[e.RowIndex].Cells["ColumnWatch"].Value);
            Boolean toDelete = Convert.ToBoolean(
                this.dataGridView.Rows[e.RowIndex].Cells["ColumnDelete"].Value);

            if (toDelete && toWatch)
            {
                e.CellStyle.BackColor = Color.Yellow;
                e.CellStyle.SelectionBackColor = Color.YellowGreen;
            }
            else if (toDelete)
            {
                e.CellStyle.BackColor = Color.Red;
                e.CellStyle.SelectionBackColor = Color.PaleVioletRed;
            }
            else if (toWatch)
            {
                e.CellStyle.BackColor = Color.Green;
                e.CellStyle.SelectionBackColor = Color.LightGreen;
            }
        }

        private void tbePageRowCount_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                this.pageRowCount = Convert.ToInt32(this.tbePageRowCount.Text);
                if (this.pageRowCount <= 0)
                {
                    this.tbePageRowCount.Text = "1";
                    this.pageRowCount = 1;
                }

                InitPageCombox();
                ShowDataGridViewPage(0);

                e.Handled = false;
            }
        }

        private void textBoxSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                btnSearch_Click(null, null);
                e.Handled = false;
            }
        }

        private void labelExplain_Resize(object sender, EventArgs e)
        {
            Label lb = sender as Label;
            String[] explainArray = lb.Tag.ToString().Split(new char[] { '|' });
            Debug.Assert(explainArray.Length == 2);

            String explain = String.Format("{0} （{1}）", explainArray[0], explainArray[1]);
            Size sizeT = TextRenderer.MeasureText(explain, lb.Font);
            int length = explainArray[1].Length;
            while (sizeT.Width > lb.Width && length > 0)
            {
                length--;
                explain = String.Format("{0} （{1}...）",
                    explainArray[0], explainArray[1].Substring(0, length));
                sizeT = TextRenderer.MeasureText(explain, lb.Font);
            }
            lb.Text = explain;
        }

        private void PrintFolder(Int32 folderID, String folderName,
            Int32 depth, String prefix, ref String strResult)
        {
            // 打印当前目录
            if (depth == 0)
            {
                strResult += (prefix + folderName + Environment.NewLine);
            }
            else
            {
                strResult += (prefix.Substring(0, prefix.Length - 2) + "| " + Environment.NewLine);
                strResult += (prefix.Substring(0, prefix.Length - 2) + "|-" + folderName + Environment.NewLine);
            }

            // 打印目录下的目录信息
            DataTable childFolderDt = sqlData.GetChildFolderFromFilmInfo(folderID);
            DataTable childFileDt = sqlData.GetChildFileFromFilmInfo(folderID);

            for (int i = 0; i < childFolderDt.Rows.Count; i++)
            {
                if (i != childFolderDt.Rows.Count - 1 || childFileDt.Rows.Count != 0)
                {
                    PrintFolder(Convert.ToInt32(childFolderDt.Rows[i]["id"]),
                        childFolderDt.Rows[i]["name"].ToString(), depth + 1, prefix + "| ", ref strResult);
                }
                else
                {
                    PrintFolder(Convert.ToInt32(childFolderDt.Rows[i]["id"]),
                        childFolderDt.Rows[i]["name"].ToString(), depth + 1, prefix + "  ", ref strResult);
                }
            }

            // 打印目录下的文件信息
            for (int i = 0; i < childFileDt.Rows.Count; i++)
            {
                if (i == 0)
                {
                    strResult += (prefix + "|" + Environment.NewLine);
                }
                strResult += (prefix + "|-" +
                    String.Format("{0} [{1}]",
                    childFileDt.Rows[i]["name"].ToString(),
                    Helper.GetSizeString(Convert.ToInt64(childFileDt.Rows[i]["size"]))) + Environment.NewLine);
            }
        }

        protected override void DefWndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case Win32API.WM_SEARCH:
                    //this.textBoxSearch.Text = CommonString.WebSearchKeyWord;
                    //btnSearch_Click(null, null);
                    //// 窗口切换到最前
                    //Win32API.SwitchToThisWindow(this.Handle, true);
                    //this.TopLevel = true;
                    break;
                default:
                    base.DefWndProc(ref m);
                    break;
            }
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    btnRootDirectory_Click(null, null);
                    break;
                case Keys.F1:
                    btnDelete_Click(null, null);
                    break;
                case Keys.F2:
                    btnWatch_Click(null, null);
                    break;
                case Keys.F3:
                    btnManager_Click(null, null);
                    break;
                case Keys.F4:
                    btnSearch_Click(null, null);
                    break;
                case Keys.Space:
                    btnUpFolder_Click(null, null);
                    break;
                case Keys.Left:
                    if (this.btnPrePage.Enabled) btnPrePage_Click(null, null);
                    break;
                case Keys.Right:
                    if (this.btnNextPage.Enabled) btnNextPage_Click(null, null);
                    break;
                case Keys.Home:
                    if (this.btnFirstPage.Enabled) btnFirstPage_Click(null, null);
                    break;
                case Keys.End:
                    if (this.btnLastPage.Enabled) btnLastPage_Click(null, null);
                    break;
                default: break;
            }
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            // 判断是否最小化
            if (this.WindowState == FormWindowState.Minimized)
            {
                // 不显示在系统任务栏
                this.ShowInTaskbar = false;
                // 托盘图标可见
                notifyIcon.Visible = true;
            }
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                // 显示在系统任务栏
                this.ShowInTaskbar = true;
                // 还原窗体
                this.WindowState = FormWindowState.Normal;
                // 托盘图标隐藏
                notifyIcon.Visible = false;
            }
        }

        private void ShowSearchResult()
        {
            if (this.InvokeRequired)
                this.Invoke(new Action(ShowSearchResult));
            else
            {
                if (this.WindowState == FormWindowState.Minimized)
                {
                    // 显示在系统任务栏
                    this.ShowInTaskbar = true;
                    // 还原窗体
                    this.WindowState = FormWindowState.Normal;
                    // 托盘图标隐藏
                    notifyIcon.Visible = false;
                }

                this.textBoxSearch.Text = CommonString.WebSearchKeyWord;
                btnSearch_Click(null, null);
                // 窗口切换到最前
                Win32API.SwitchToThisWindow(this.Handle, true);
                this.TopLevel = true;
            }
        }

        private void toolStripMenuItemShowWindow_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                // 显示在系统任务栏
                this.ShowInTaskbar = true;
                // 还原窗体
                this.WindowState = FormWindowState.Normal;
                // 托盘图标隐藏
                notifyIcon.Visible = false;
            }
        }

        private void toolStripMenuItemExitWindow_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void toolStripMenuItemShowContent_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count != 1)
            {
                MessageBox.Show("请选中一行！", "提示", MessageBoxButtons.OK);
                return;
            }

            String content = gridViewData.Rows[dataGridView.SelectedRows[0].Index]["content"].ToString();

            String filePath = Path.Combine(nfoFolder,
                gridViewData.Rows[dataGridView.SelectedRows[0].Index]["name"].ToString());
            File.WriteAllText(filePath, content, System.Text.Encoding.GetEncoding("IBM437"));

            Helper.OpenEdit(filePath, content);
        }

        private void MySqlHeartBeat()
        {
            while (heartBeatFlag)
            {
                int msTime = 0;
                while (heartBeatFlag && msTime < 1800000)
                {
                    Thread.Sleep(500);
                    msTime += 500;
                }
                if (!heartBeatFlag) break;

                sqlData.CountRowsFormSearchLog();
            }
        }
    }
}
