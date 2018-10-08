using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using static MyFilm.CommonDataTable;

namespace MyFilm
{
    public partial class MainForm : Form
    {
        /// <summary>
        /// 总记录数
        /// </summary>
        private int totalRowCount = 0;

        /// <summary>
        /// 每页记录数
        /// </summary>
        private int pageRowCount = 20;

        /// <summary>
        /// 当前页面索引数（从0开始）
        /// </summary>
        private int currentPageIndex = 0;

        private string comboxDiskDefaultString = "全部";
        private bool connectState = true;
        private bool[] controlEnableArray = null;

        private enum ActionType
        {
            ACTION_DISK_ROOT,
            ACTION_KEY_WORD_SEARCH,
            ACTION_DELETE_SEARCH,
            ACTION_WATCH_SEARCH,
            ACTION_SQL_QUERY,
            ACTION_FOLDER_UP,
            ACTION_FOLDER_DOWN
        }

        private ActionType actionType = ActionType.ACTION_DISK_ROOT;
        private int folderDownID = int.MinValue;

        /// <summary>
        /// 查询的结果（id列表）
        /// </summary>
        private int[] idList = null;

        /// <summary>
        /// 磁盘根目录表
        /// </summary>
        private DataTable diskRootDataTable = null;

        /// <summary>
        /// 显示的查询信息
        /// </summary>
        private String queryInfo = String.Empty;

        /// <summary>
        /// 表格关联的数据
        /// </summary>
        private DataTable gridViewData = null;

        /// <summary>
        /// 打开的 nfo 文件所在文件夹
        /// </summary>
        private static String nfoFolder = Path.Combine(CommonString.AppDataFolder, "NFO");

        /// <summary>
        /// 定时发送心跳包
        /// </summary>
        private bool heartBeatFlag = true;

        private Action<DataTable> UpdateSqlFormRichTextBoxActionByDataTable = null;
        private Action<String> UpdateSqlFormRichTextBoxActionByString = null;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.controlEnableArray = new bool[this.Controls.Count];

            this.Icon = Properties.Resources.ico;
            this.Text = String.Format("{0}@{1} [MyFilm v{2}]",
                CommonString.DbName, CommonString.DbIP, Application.ProductVersion);
            this.tbePageRowCount.Text = this.pageRowCount.ToString();

            this.notifyIcon.Icon = Properties.Resources.ico;
            this.notifyIcon.Visible = false;
            this.notifyIcon.Text = String.Format("{0}@{1}", CommonString.DbName, CommonString.DbIP);
            this.notifyIcon.ContextMenuStrip = this.contextMenuStripNotify;

            // 删除NFO文件夹中所有的nfo文件
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

            // 获取根目录数据源
            diskRootDataTable = GetDiskRootDirectoryInfo();
            totalRowCount = diskRootDataTable.Rows.Count;

            this.comboxDiskDefaultString = string.Format("全部（共 {0} 磁盘）", totalRowCount);

            // 开启心跳线程
            Thread thread1 = new Thread(new ThreadStart(MySqlHeartBeat));
            thread1.Start();

            InitPageCombox();
            InitDiskCombox();
            InitComboxMapDisk();
            SetGridView();

            // 首次显示时，若关键字为空，则显示根目录，否则显示搜索界面
            if (String.IsNullOrWhiteSpace(CommonString.WebSearchKeyWord))
            {
                queryInfo = "索引 根目录";
                actionType = ActionType.ACTION_DISK_ROOT;
                ShowDataGridViewPage(0);
            }
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

            SqlData.GetInstance().CloseMySql();
        }

        private void InitComboxMapDisk()
        {
            this.comboBoxMapDisk.SuspendLayout();
            this.comboBoxMapDisk.Items.Clear();

            System.IO.DriveInfo[] drives = System.IO.DriveInfo.GetDrives();
            // 是否含磁盘E
            bool eDrive = false;
            for (int i = 0; i < drives.Length; i++)
            {
                if ((!eDrive) && drives[i].Name == @"E:\") eDrive = true;
                this.comboBoxMapDisk.Items.Add(drives[i].Name);
            }

            if (eDrive) this.comboBoxMapDisk.SelectedItem = @"E:\";
            else this.comboBoxMapDisk.SelectedIndex = drives.Length - 1;
            this.comboBoxMapDisk.ResumeLayout();
        }

        private void SetGridView()
        {
            this.dataGridView.Columns.Clear();

            string[] defaultCols = new string[] {
                "index", "name", "path", "size", "modify_t" , "disk_desc", "to_watch_ex", "to_delete_ex"};

            foreach (string strCol in defaultCols)
            {
                DataGridViewColumn dgvCl = null;

                if (strCol == "to_watch_ex" || strCol == "to_delete_ex")
                {
                    dgvCl = new DataGridViewCheckBoxColumn();
                    dgvCl.ReadOnly = false;
                    dgvCl.Visible = false;
                }
                else
                {
                    dgvCl = new DataGridViewTextBoxColumn();
                    dgvCl.ReadOnly = true;
                    dgvCl.Visible = true;
                }

                dgvCl.DataPropertyName = strCol;
                dgvCl.Name = strCol;
                dgvCl.SortMode = DataGridViewColumnSortMode.NotSortable;

                switch (strCol)
                {
                    case "index":
                        dgvCl.FillWeight = 30F;
                        dgvCl.HeaderText = "索引";
                        dgvCl.MinimumWidth = 30;
                        break;
                    case "name":
                        dgvCl.HeaderText = "名称";
                        dgvCl.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                        break;
                    case "path":
                        dgvCl.HeaderText = "路径";
                        dgvCl.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                        break;
                    case "size":
                        dgvCl.FillWeight = 50F;
                        dgvCl.HeaderText = "大小";
                        dgvCl.MinimumWidth = 50;
                        dgvCl.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        break;
                    case "disk_desc":
                        dgvCl.FillWeight = 60F;
                        dgvCl.HeaderText = "磁盘";
                        dgvCl.MinimumWidth = 60;
                        dgvCl.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                        break;
                    case "modify_t":
                        dgvCl.FillWeight = 50F;
                        dgvCl.HeaderText = "修改日期";
                        dgvCl.MinimumWidth = 50;
                        break;
                    default: break;
                }

                this.dataGridView.Columns.Add(dgvCl);
            }

            this.dataGridView.AutoGenerateColumns = false;
            this.dataGridView.ContextMenuStrip = this.contextMenuStrip;
        }

        /// <summary>
        /// 初始化磁盘选择控件
        /// </summary>
        private void InitDiskCombox()
        {
            this.comboBoxDisk.SuspendLayout();
            this.comboBoxDisk.Items.Clear();
            this.comboBoxDisk.Items.Add(comboxDiskDefaultString);
            for (int i = 0; i < diskRootDataTable.Rows.Count; i++)
            {
                this.comboBoxDisk.Items.Add(diskRootDataTable.Rows[i]["disk_desc"]);
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

            SetPageComboxIndex();

            if (actionType == ActionType.ACTION_DISK_ROOT)
            {
                if (diskRootDataTable.Rows.Count != 0)
                    gridViewData = diskRootDataTable
                        .AsEnumerable()
                        .Where((row, index) => index >= startIndex && index < startIndex + pageRowCount)
                        .CopyToDataTable();
                else gridViewData = diskRootDataTable.Clone();
            }
            else
            {
                DataTable dt = SqlData.GetInstance().SelectDataByIDList(
                        Helper.ArraySlice(idList, startIndex, pageRowCount));
                if (dt != null && dt.Rows.Count > 0)
                    gridViewData = CommonDataTable.ConvertFilmInfoToGrid(dt);
                else
                    gridViewData = diskRootDataTable.Clone();
            }

            for (int i = 0; i < gridViewData.Rows.Count; i++)
                gridViewData.Rows[i]["index"] = currentPageIndex * pageRowCount + i + 1;

            String explain = String.Format("{0} （{1}）", explain1, queryInfo);
            Size sizeT = TextRenderer.MeasureText(explain, this.labelExplain.Font);
            int lenHigh = queryInfo.Length;
            int lenLow = 0;
            int lenActual = lenHigh;
            if (sizeT.Width > this.labelExplain.Width)
            {
                while (lenLow < lenHigh - 1)
                {
                    lenActual = lenLow + (lenHigh - lenLow) / 2;
                    explain = String.Format("{0} （{1}...）", explain1,
                        queryInfo.Substring(0, lenActual));
                    sizeT = TextRenderer.MeasureText(explain, this.labelExplain.Font);

                    if (sizeT.Width == this.labelExplain.Width) break;
                    else if (sizeT.Width > this.labelExplain.Width) lenHigh = lenActual;
                    else lenLow = lenActual;
                }
            }
            if (sizeT.Width > this.labelExplain.Width)
                explain = String.Format("{0} （{1}...）", explain1,
                        queryInfo.Substring(0, lenActual - 1));
            this.labelExplain.Text = explain;
            this.labelExplain.Tag = String.Format("{0}|{1}", explain1, queryInfo);

            this.dataGridView.DataSource = gridViewData;

            UpdateSqlFormRichTextBoxActionByString?.Invoke(String.Format(
                "mysql> {0} ({1})", queryInfo, explain1));
            UpdateSqlFormRichTextBoxActionByDataTable?.Invoke(gridViewData);

            // 不是在 SqlForm 查询的话，将主界面设为焦点
            if (this.actionType != ActionType.ACTION_SQL_QUERY) this.Focus();
        }

        private void SqlQuery(string cmdText)
        {
            String errMsg = String.Empty;
            int[] newIDList = SqlData.GetInstance().SelectIDBySqlText(cmdText, ref errMsg);

            if (newIDList != null)
            {
                queryInfo = cmdText;
                actionType = ActionType.ACTION_SQL_QUERY;

                idList = newIDList;
                totalRowCount = idList.Length;

                InitPageCombox();
                ShowDataGridViewPage(0);
            }
            else
            {
                // 出错了不改变界面，还是显示原来的
                UpdateSqlFormRichTextBoxActionByString?.Invoke(
                    string.Format("mysql> {0}", cmdText) +
                    Environment.NewLine + errMsg + Environment.NewLine);
            }
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

            String diskDescribe = this.comboBoxDisk.SelectedItem.ToString();

            queryInfo = String.Format("在 {0} 里搜索 \'{1}\'",
                            diskDescribe == comboxDiskDefaultString ? "所有磁盘" : diskDescribe,
                            keyWord);
            actionType = ActionType.ACTION_KEY_WORD_SEARCH;

            idList = SqlData.GetInstance().SearchKeyWordFromFilmInfo(
                keyWord, diskDescribe == comboxDiskDefaultString ? null : diskDescribe);
            totalRowCount = (idList == null ? 0 : idList.Length);

            if (totalRowCount == 0) this.textBoxSearch.ForeColor = Color.Red;
            else this.textBoxSearch.ForeColor = Color.Black;

            InitPageCombox();

            // 写入搜索记录
            SqlData.GetInstance().InsertDataToSearchLog(keyWord, totalRowCount, DateTime.Now);

            ShowDataGridViewPage(0);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            String diskDescribe = this.comboBoxDisk.SelectedItem.ToString();
            queryInfo = String.Format("在 {0} 里搜索 待删（结果以设置时间倒序）",
                            diskDescribe == comboxDiskDefaultString ? "所有磁盘" : diskDescribe);
            actionType = ActionType.ACTION_DELETE_SEARCH;

            idList = SqlData.GetInstance().GetDeleteDataFromFilmInfo(
                diskDescribe == comboxDiskDefaultString ? null : diskDescribe);
            totalRowCount = idList.Length;

            InitPageCombox();

            ShowDataGridViewPage(0);
        }

        private void btnDeleteOrderByDisk_Click(object sender, EventArgs e)
        {
            String diskDescribe = this.comboBoxDisk.SelectedItem.ToString();
            queryInfo = String.Format("在 {0} 里搜索 待删（结果以磁盘分组）",
                            diskDescribe == comboxDiskDefaultString ? "所有磁盘" : diskDescribe);
            actionType = ActionType.ACTION_DELETE_SEARCH;

            idList = SqlData.GetInstance().GetDeleteDataFromFilmInfoGroupByDisk(
                diskDescribe == comboxDiskDefaultString ? null : diskDescribe);
            totalRowCount = idList.Length;

            InitPageCombox();

            ShowDataGridViewPage(0);
        }

        private void btnWatch_Click(object sender, EventArgs e)
        {

            String diskDescribe = this.comboBoxDisk.SelectedItem.ToString();
            queryInfo = String.Format("在 {0} 里搜索 待看",
                            diskDescribe == comboxDiskDefaultString ? "所有磁盘" : diskDescribe);
            actionType = ActionType.ACTION_WATCH_SEARCH;

            idList = SqlData.GetInstance().GetWatchDataFromFilmInfo(
                diskDescribe == comboxDiskDefaultString ? null : diskDescribe);
            totalRowCount = idList.Length;

            InitPageCombox();

            ShowDataGridViewPage(0);
        }

        private void btnManager_Click(object sender, EventArgs e)
        {
            ManagerForm form = new ManagerForm();
            form.ShowDialog();

            // 设置返回后显示根目录
            ReLoadDiskRootDataAndShow();
        }

        private void btnUpFolder_Click(object sender, EventArgs e)
        {
            // 考虑场景 空文件夹
            if (this.dataGridView.SelectedRows.Count == 1 || actionType == ActionType.ACTION_FOLDER_DOWN)
            {
                int pid = folderDownID;
                if (this.dataGridView.SelectedRows.Count == 1)
                    pid = Convert.ToInt32(gridViewData.Rows[this.dataGridView.SelectedRows[0].Index]["pid"]);

                if (pid == -1)
                {
                    MessageBox.Show("已是顶层！", "提示", MessageBoxButtons.OK);
                    return;
                }

                DataTable dt = SqlData.GetInstance().GetDataByIdFromFilmInfo(pid);

                int folderUpID = Convert.ToInt32(dt.Rows[0]["pid"]);
                actionType = ActionType.ACTION_FOLDER_UP;

                queryInfo = String.Format("索引 \'{0}\'", Helper.GetUpFolder(dt.Rows[0]["path"].ToString()));

                idList = SqlData.GetInstance().GetDataByPidFromFilmInfo(folderUpID);
                totalRowCount = idList.Length;
                int offset = Array.IndexOf(idList, pid);
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
            ReLoadDiskRootDataAndShow();
        }

        private void ReLoadDiskRootDataAndShow()
        {
            queryInfo = "索引 根目录";
            actionType = ActionType.ACTION_DISK_ROOT;

            diskRootDataTable = GetDiskRootDirectoryInfo();

            totalRowCount = diskRootDataTable.Rows.Count;

            this.comboxDiskDefaultString = string.Format("全部（共 {0} 磁盘）", totalRowCount);

            InitPageCombox();
            InitDiskCombox();
            ShowDataGridViewPage(0);
        }

        private void dataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //if (Convert.ToInt32(gridViewData.Rows[e.RowIndex]["pid"]) != -1 &&
            //    gridViewData.Rows[e.RowIndex]["disk_desc"].ToString() ==
            //    CommonString.RealOrFake4KDiskName)
            //    return;

            bool needLocation = false;
            int locationId = int.MinValue;

            int pid = int.MinValue;

            // 如果是文件夹就浏览文件夹下内容
            if (dataGridView.Columns[e.ColumnIndex].DataPropertyName == "name" &&
                Convert.ToBoolean(gridViewData.Rows[e.RowIndex]["is_folder"]))
            {
                pid = Convert.ToInt32(gridViewData.Rows[e.RowIndex]["id"]);
                queryInfo = String.Format("索引 \'{0}\'", Path.Combine(
                    gridViewData.Rows[e.RowIndex]["path"].ToString(),
                    gridViewData.Rows[e.RowIndex]["name"].ToString()));

            }
            // 浏览此文件或文件夹所在文件夹下的全部内容，会自动跳转到当前的选中行
            else if (dataGridView.Columns[e.ColumnIndex].DataPropertyName == "path")
            {
                pid = Convert.ToInt32(gridViewData.Rows[e.RowIndex]["pid"]);
                queryInfo = String.Format("索引 \'{0}\'",
                    gridViewData.Rows[e.RowIndex]["path"].ToString());


                needLocation = true;
                locationId = Convert.ToInt32(gridViewData.Rows[e.RowIndex]["id"]);
            }

            if (pid != int.MinValue)
            {
                actionType = ActionType.ACTION_FOLDER_DOWN;
                folderDownID = pid;

                idList = SqlData.GetInstance().GetDataByPidFromFilmInfo(pid);
                totalRowCount = idList.Length;

                int offset = 0;
                if (needLocation)
                {
                    offset = Array.IndexOf(idList, locationId);
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
            DataTable dt1 = SqlData.GetInstance().GetAllRootDirectoryFromFilmInfo();
            DataTable dt2 = SqlData.GetInstance().GetAllDataFromDiskInfo();
            Debug.Assert(dt1.Rows.Count == dt2.Rows.Count || dt1.Rows.Count == dt2.Rows.Count + 1);

            DataTable dt = CommonDataTable.ConvertFilmInfoToGrid(dt1);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                String diskDescribe = dt.Rows[i]["disk_desc"].ToString();
                if (diskDescribe == CommonString.RealOrFake4KDiskName) continue;

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



        private void contextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            // 只有选中了行才弹出菜单
            if (this.dataGridView.SelectedRows.Count < 1) e.Cancel = true;
            else if (this.dataGridView.SelectedRows.Count == 1)
            {
                int index = this.dataGridView.SelectedRows[0].Index;
                bool isDelete = Convert.ToBoolean(this.dataGridView.Rows[index].Cells["to_delete_ex"].Value);
                bool isWatch = Convert.ToBoolean(this.dataGridView.Rows[index].Cells["to_watch_ex"].Value);
                bool isFolder = Convert.ToBoolean(gridViewData.Rows[index]["is_folder"]);
                String fileName = gridViewData.Rows[index]["name"].ToString();
                bool isShowContent = ((fileName.ToLower() == "__game_version_info__.gvi") ||
                    CommonString.MediaExts.Contains(
                        fileName.Substring(fileName.LastIndexOf('.')).ToLower()));
                bool isOpenFolder =
                    this.dataGridView.Rows[index].Cells["disk_desc"].Value.ToString() !=
                    CommonString.RealOrFake4KDiskName;

                this.toolStripMenuItemSetDelete.Enabled = !isDelete;
                this.toolStripMenuItemSetWatch.Enabled = !isWatch;
                this.toolStripMenuItemCancelDelete.Enabled = isDelete;
                this.toolStripMenuItemCancelWatch.Enabled = isWatch;
                this.toolStripMenuItemOpenFolder.Enabled = isOpenFolder;
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
            List<SetStateStruct> setStateStructList = new List<SetStateStruct>();
            foreach (DataGridViewRow row in dataGridView.SelectedRows)
            {
                if (gridViewData.Rows[row.Index]["disk_desc"].ToString() ==
                    CommonString.RealOrFake4KDiskName)
                    continue;

                SetStateStruct SetStateStruct = new SetStateStruct();
                SetStateStruct.name = Convert.ToString(gridViewData.Rows[row.Index]["name"]);
                SetStateStruct.id = Convert.ToInt32(gridViewData.Rows[row.Index]["id"]);
                SetStateStruct.is_folder = Convert.ToBoolean(gridViewData.Rows[row.Index]["is_folder"]);
                SetStateStruct.to_delete_ex = Convert.ToBoolean(gridViewData.Rows[row.Index]["to_delete_ex"]);
                SetStateStruct.pid = Convert.ToInt32(gridViewData.Rows[row.Index]["pid"]);
                SetStateStruct.max_cid = Convert.ToInt32(gridViewData.Rows[row.Index]["max_cid"]);

                setStateStructList.Add(SetStateStruct);

                this.dataGridView.Rows[row.Index].Cells["to_delete_ex"].Value = true;
            }
            SqlData.GetInstance().UpdateWatchOrDeleteStateFromFilmInfo(false, setStateStructList, DateTime.Now, true);
        }

        private void toolStripMenuItemSetWatch_Click(object sender, EventArgs e)
        {
            List<SetStateStruct> setStateStructList = new List<SetStateStruct>();
            foreach (DataGridViewRow row in dataGridView.SelectedRows)
            {
                if (gridViewData.Rows[row.Index]["disk_desc"].ToString() ==
                    CommonString.RealOrFake4KDiskName)
                    continue;

                SetStateStruct SetStateStruct = new SetStateStruct();
                SetStateStruct.name = Convert.ToString(gridViewData.Rows[row.Index]["name"]);
                SetStateStruct.id = Convert.ToInt32(gridViewData.Rows[row.Index]["id"]);
                SetStateStruct.is_folder = Convert.ToBoolean(gridViewData.Rows[row.Index]["is_folder"]);
                SetStateStruct.to_watch_ex = Convert.ToBoolean(gridViewData.Rows[row.Index]["to_watch_ex"]);
                SetStateStruct.pid = Convert.ToInt32(gridViewData.Rows[row.Index]["pid"]);
                SetStateStruct.max_cid = Convert.ToInt32(gridViewData.Rows[row.Index]["max_cid"]);

                setStateStructList.Add(SetStateStruct);

                this.dataGridView.Rows[row.Index].Cells["to_watch_ex"].Value = true;
            }
            SqlData.GetInstance().UpdateWatchOrDeleteStateFromFilmInfo(true, setStateStructList, DateTime.Now, true);
        }

        private void toolStripMenuItemCancelDelete_Click(object sender, EventArgs e)
        {
            List<SetStateStruct> setStateStructList = new List<SetStateStruct>();
            foreach (DataGridViewRow row in dataGridView.SelectedRows)
            {
                if (gridViewData.Rows[row.Index]["disk_desc"].ToString() ==
                    CommonString.RealOrFake4KDiskName)
                    continue;

                SetStateStruct SetStateStruct = new SetStateStruct();
                SetStateStruct.name = Convert.ToString(gridViewData.Rows[row.Index]["name"]);
                SetStateStruct.id = Convert.ToInt32(gridViewData.Rows[row.Index]["id"]);
                SetStateStruct.is_folder = Convert.ToBoolean(gridViewData.Rows[row.Index]["is_folder"]);
                SetStateStruct.to_delete_ex = Convert.ToBoolean(gridViewData.Rows[row.Index]["to_delete_ex"]);
                SetStateStruct.pid = Convert.ToInt32(gridViewData.Rows[row.Index]["pid"]);
                SetStateStruct.max_cid = Convert.ToInt32(gridViewData.Rows[row.Index]["max_cid"]);

                setStateStructList.Add(SetStateStruct);

                this.dataGridView.Rows[row.Index].Cells["to_delete_ex"].Value = false;
            }
            SqlData.GetInstance().UpdateWatchOrDeleteStateFromFilmInfo(false, setStateStructList,
                System.Data.SqlTypes.SqlDateTime.MinValue.Value, false);
        }

        private void toolStripMenuItemCancelWatch_Click(object sender, EventArgs e)
        {
            List<SetStateStruct> setStateStructList = new List<SetStateStruct>();
            foreach (DataGridViewRow row in dataGridView.SelectedRows)
            {
                if (gridViewData.Rows[row.Index]["disk_desc"].ToString() ==
                    CommonString.RealOrFake4KDiskName)
                    continue;

                SetStateStruct SetStateStruct = new SetStateStruct();
                SetStateStruct.name = Convert.ToString(gridViewData.Rows[row.Index]["name"]);
                SetStateStruct.id = Convert.ToInt32(gridViewData.Rows[row.Index]["id"]);
                SetStateStruct.is_folder = Convert.ToBoolean(gridViewData.Rows[row.Index]["is_folder"]);
                SetStateStruct.to_watch_ex = Convert.ToBoolean(gridViewData.Rows[row.Index]["to_watch_ex"]);
                SetStateStruct.pid = Convert.ToInt32(gridViewData.Rows[row.Index]["pid"]);
                SetStateStruct.max_cid = Convert.ToInt32(gridViewData.Rows[row.Index]["max_cid"]);

                setStateStructList.Add(SetStateStruct);

                this.dataGridView.Rows[row.Index].Cells["to_watch_ex"].Value = false;
            }
            SqlData.GetInstance().UpdateWatchOrDeleteStateFromFilmInfo(true, setStateStructList,
                System.Data.SqlTypes.SqlDateTime.MinValue.Value, false);
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
            // 打开位置实际磁盘
            folderPath = this.comboBoxMapDisk.SelectedItem.ToString()[0] + folderPath.Substring(1);

            try
            {
                System.Diagnostics.Process.Start(folderPath);
            }
            catch
            {
                MessageBox.Show(string.Format("系统找不到指定的文件夹。{0}{1}",
                    Environment.NewLine, folderPath));
            }
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
                this.dataGridView.Rows[e.RowIndex].Cells["to_watch_ex"].Value);
            Boolean toDelete = Convert.ToBoolean(
                this.dataGridView.Rows[e.RowIndex].Cells["to_delete_ex"].Value);

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
            DataTable childFolderDt = SqlData.GetInstance().GetChildFolderFromFilmInfo(folderID);
            DataTable childFileDt = SqlData.GetInstance().GetChildFileFromFilmInfo(folderID);

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

        private void SqlFormColsed()
        {
            this.UpdateSqlFormRichTextBoxActionByDataTable = null;
            this.UpdateSqlFormRichTextBoxActionByString = null;
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Q)
            {
                SqlForm form = new SqlForm();
                form.SqlQueryAction = this.SqlQuery;
                form.SqlFormColsedAction = this.SqlFormColsed;
                this.UpdateSqlFormRichTextBoxActionByDataTable = form.UpdateRichTextBox;
                this.UpdateSqlFormRichTextBoxActionByString = form.UpdateRichTextBox;
                form.Show();
            }
            //switch (e.KeyCode)
            //{
            //    case Keys.Escape:
            //        btnRootDirectory_Click(null, null);
            //        break;
            //    case Keys.F1:
            //        btnDelete_Click(null, null);
            //        break;
            //    case Keys.F2:
            //        btnWatch_Click(null, null);
            //        break;
            //    case Keys.F3:
            //        btnManager_Click(null, null);
            //        break;
            //    case Keys.F4:
            //        btnSearch_Click(null, null);
            //        break;
            //    case Keys.Space:
            //        btnUpFolder_Click(null, null);
            //        break;
            //    case Keys.Left:
            //        if (this.btnPrePage.Enabled) btnPrePage_Click(null, null);
            //        break;
            //    case Keys.Right:
            //        if (this.btnNextPage.Enabled) btnNextPage_Click(null, null);
            //        break;
            //    case Keys.Home:
            //        if (this.btnFirstPage.Enabled) btnFirstPage_Click(null, null);
            //        break;
            //    case Keys.End:
            //        if (this.btnLastPage.Enabled) btnLastPage_Click(null, null);
            //        break;
            //    default: break;
            //}
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

                // 命令行搜索时为全盘
                this.comboBoxDisk.SelectedIndex = 0;
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
            String fileName = gridViewData.Rows[dataGridView.SelectedRows[0].Index]["name"].ToString();
            String strExt = ".content";
            if (CommonString.MediaExts.Contains(
                fileName.Substring(fileName.LastIndexOf('.')).ToLower()))
                strExt = ".mediainfo";

            String filePath = Path.Combine(nfoFolder, fileName + strExt);
            File.WriteAllText(filePath, content, System.Text.Encoding.UTF8);

            Helper.OpenEdit(filePath, content);
        }

        private void MySqlHeartBeat()
        {
            bool exitFlag = false;
            bool connectStatePrevious = connectState;
            // 30分钟查询一次，10分钟检测一次网卡
            int timeInterval = 1800000;
            int flags = 0;

            while (heartBeatFlag)
            {
                int msTime = 0;
                while (heartBeatFlag && msTime < timeInterval)
                {
                    Thread.Sleep(500);
                    msTime += 500;
                }
                if (!heartBeatFlag) break;

                connectStatePrevious = connectState;
                if (connectState)
                {
                    try
                    {
                        SqlData.GetInstance().CountRowsFromSearchLog();
                    }
                    // 电脑睡眠时，网卡睡眠
                    catch
                    {
                        connectState = false;
                        timeInterval = 600000;
                    }
                }
                else
                {
                    if (connectState = Win32API.InternetGetConnectedState(ref flags, 0))
                    {
                        try
                        {
                            SqlData.GetInstance().OpenMySql();
                            timeInterval = 1800000;
                        }
                        catch (Exception ex)
                        {
                            // 依然打开失败，退出
                            MessageBox.Show(string.Format("{0}\n{1}",
                                DateTime.Now.ToString("yyyy-MM-dd HHH:mm:ss"), ex.Message));
                            heartBeatFlag = false;
                            exitFlag = true;
                        }
                    }
                }

                if (heartBeatFlag && (connectStatePrevious != connectState))
                {
                    MethodInvoker mi = new MethodInvoker(ChangeControlEnable);
                    this.BeginInvoke(mi);
                }
            }

            if (exitFlag) this.Close();
        }

        private void ChangeControlEnable()
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

        private void btnRefreshMapdisk_Click(object sender, EventArgs e)
        {
            InitComboxMapDisk();
        }
    }
}
