using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace MyFilm
{
    public class CommonDataTable
    {
        /// <summary>
        /// 最多输出的列（最小设为 5）
        /// </summary>
        private static int MaxOutputRow = 50;

        /// <summary>
        /// 获取film_info数据库对应的DataTable
        /// </summary>
        /// <returns></returns>
        public static DataTable GetFilmInfoDataTable()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("id", typeof(Int32));
            dt.Columns.Add("name", typeof(String));
            dt.Columns.Add("path", typeof(String));
            dt.Columns.Add("size", typeof(Int64));
            dt.Columns.Add("create_t", typeof(DateTime));
            dt.Columns.Add("modify_t", typeof(DateTime));
            dt.Columns.Add("is_folder", typeof(Boolean));
            dt.Columns.Add("to_watch", typeof(Boolean));
            dt.Columns.Add("to_watch_ex", typeof(Boolean));
            dt.Columns.Add("s_w_t", typeof(DateTime));
            dt.Columns.Add("to_delete", typeof(Boolean));
            // 对一个文件夹，如果设为待删，在主界面点击待删时
            // 此文件夹会出现(to_delete=true)，而此文件夹的子文件或子文件夹不会出现(to_delete=false)
            // 但此文件夹或子文件、文件夹都会变红(to_delete_ex=true)
            // 待看同理
            dt.Columns.Add("to_delete_ex", typeof(Boolean));
            dt.Columns.Add("s_d_t", typeof(DateTime));
            dt.Columns.Add("content", typeof(String));
            dt.Columns.Add("pid", typeof(Int32));
            dt.Columns.Add("max_cid", typeof(Int32));
            dt.Columns.Add("disk_desc", typeof(String));

            return dt;
        }

        public class CancelIDList : IEnumerable
        {
            private List<TreeSetState> cancelIDList = null;

            public CancelIDList()
            {
                this.cancelIDList = new List<TreeSetState>();
            }

            public TreeSetState this[int i]
            {
                get { return this.cancelIDList[i]; }
                set { this.cancelIDList[i] = value; }
            }

            public int Count
            {
                get { return this.cancelIDList.Count; }
            }

            public List<int> SelectID()
            {
                return cancelIDList.Select(x => x.id).ToList();
            }

            public List<int> SelectMaxcid()
            {
                return cancelIDList.Select(x => x.max_cid).ToList();
            }

            public IEnumerator GetEnumerator()
            {
                int i = 0;
                while (i < this.cancelIDList.Count) yield return this[i++];
            }
        }

        public class TreeSetState
        {
            public string name;
            public int id;
            public int max_cid;
            public List<TreeSetState> cancelIDList;

            public void Add(TreeSetState nodeSetState)
            {
                foreach (TreeSetState _nodeSetState in this.cancelIDList)
                {
                    if (_nodeSetState.id == nodeSetState.id)
                    {
                        _nodeSetState.cancelIDList.AddRange(nodeSetState.cancelIDList);
                        return;
                    }
                    else if (_nodeSetState.id > nodeSetState.id &&
                        _nodeSetState.id <= nodeSetState.max_cid)
                    {
                        _nodeSetState.Add(nodeSetState);
                        return;
                    }
                }

                this.cancelIDList.Add(nodeSetState);
            }
        }

        public struct SetStateStruct
        {
            public string name;
            public int id;
            public bool is_folder;
            public bool to_watch_ex;
            public bool to_delete_ex;
            public int pid;
            public int max_cid;
        }

        /// <summary>
        /// 获取disk_info数据库对应的DataTable
        /// </summary>
        /// <returns></returns>
        public static DataTable GetDiskInfoDataTable()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("id", typeof(Int32));
            dt.Columns.Add("disk_desc", typeof(String));
            dt.Columns.Add("free_space", typeof(Int64));
            dt.Columns.Add("total_size", typeof(Int64));
            dt.Columns.Add("complete_scan", typeof(Boolean));
            dt.Columns.Add("scan_layer", typeof(Int32));

            return dt;
        }

        /// <summary>
        /// MainForm DataGridView 数据源格式
        /// </summary>
        /// <returns></returns>
        public static DataTable GetMainFormGridDataTable(DataTable filmInfoDt)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("index", typeof(Int32));

            foreach (DataColumn cl in filmInfoDt.Columns)
            {
                dt.Columns.Add(cl.ColumnName,
                    cl.ColumnName == "size" || cl.DataType == typeof(DateTime) ?
                    typeof(String) : cl.DataType);
            }

            return dt;
        }

        public static DataTable ConvertFilmInfoToGrid(DataTable fiDt)
        {
            DataTable dt = GetMainFormGridDataTable(fiDt);
            for (int i = 0; i < fiDt.Rows.Count; i++)
            {
                DataRow dr = dt.NewRow();
                dr["index"] = i + 1;

                foreach (DataColumn cl in fiDt.Columns)
                {
                    switch (cl.ColumnName)
                    {
                        case "id":
                        case "name":
                        case "is_folder":
                        case "to_watch":
                        case "to_watch_ex":
                        case "to_delete":
                        case "to_delete_ex":
                        case "content":
                        case "pid":
                        case "max_cid":
                        case "disk_desc":
                            dr[cl.ColumnName] = fiDt.Rows[i][cl.ColumnName];
                            break;
                        // 用父文件夹
                        case "path":
                            dr["path"] = Helper.GetUpFolder(fiDt.Rows[i]["path"].ToString());
                            break;
                        case "size":
                            long size = Convert.ToInt64(fiDt.Rows[i]["size"]);
                            if (size == -1) dr["size"] = "------";
                            else dr["size"] = Helper.GetSizeString(size);
                            break;
                        case "create_t":
                        case "modify_t":
                        case "s_w_t":
                        case "s_d_t":
                            dr[cl.ColumnName] = Convert.ToDateTime(
                                fiDt.Rows[i][cl.ColumnName]).ToString("yyyy-MM-dd HHH:mm:ss");
                            break;
                        default:
                            dr[cl.ColumnName] = fiDt.Rows[i][cl.ColumnName];
                            break;
                    }
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }

        /// <summary>
        /// Setting DataGridView 数据源格式
        /// </summary>
        /// <returns></returns>
        public static DataTable GetSettingGridDataTable()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("index", typeof(Int32));
            dt.Columns.Add("disk_desc", typeof(String));
            dt.Columns.Add("free_space", typeof(String));
            dt.Columns.Add("total_size", typeof(String));
            dt.Columns.Add("complete_scan", typeof(String));
            dt.Columns.Add("scan_layer", typeof(Int32));

            return dt;
        }

        /// <summary>
        /// DataTable以文本输出
        /// </summary>
        /// <param name="dt">数据源</param>
        /// <param name="outputSet">要输出的字段，为 null 时输出全部</param>
        /// <returns></returns>
        public static String DataTableFormatToString(DataTable dt, HashSet<String> outputSet)
        {
            char angle = '+';
            char horizontal = '-';
            char vertical = '|';
            char star = '*';

            // 每列的最大长度
            List<int> lenList = new List<int>();
            // 每列的字符串
            List<string> strList = new List<string>();
            // 每列是否右对齐
            List<bool> padList = new List<bool>();
            // dt 实际会输出的列索引
            List<int> colIndex = new List<int>();
            // 是否会折叠输出
            bool omitOutput = dt.Rows.Count > MaxOutputRow;
            // 0-(tempRowCount-1) 正常输出，tempRowCount-(MaxOutputRow-2)输出*
            // (MaxOutputRow-2) 输出 dt 最后一行
            int tempRowCount = omitOutput ? MaxOutputRow - 4 : dt.Rows.Count;

            // 记录字符串转换为单字节与本身长度差
            List<List<int>> lll = new List<List<int>>();

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                if (outputSet != null && !outputSet.Contains(dt.Columns[i].ColumnName))
                    continue;

                List<int> ll = new List<int>();

                String strTemp = dt.Columns[i].ColumnName;
                int maxLen = System.Text.UTF8Encoding.Default.GetBytes(strTemp).Length;
                ll.Add(maxLen - strTemp.Length);

                for (int j = 0; j < tempRowCount; j++)
                {
                    strTemp = dt.Rows[j][i].ToString();
                    int len = System.Text.UTF8Encoding.Default.GetBytes(strTemp).Length;
                    ll.Add(len - strTemp.Length);
                    if (maxLen < len) maxLen = len;
                }
                if (omitOutput)
                {
                    // 最后一行
                    strTemp = dt.Rows[dt.Rows.Count - 1][i].ToString();
                    int len = System.Text.UTF8Encoding.Default.GetBytes(strTemp).Length;
                    ll.Add(len - strTemp.Length);
                    if (maxLen < len) maxLen = len;
                }

                lll.Add(ll);
                lenList.Add(maxLen + 2);
                strList.Add(new string(horizontal, maxLen + 2));
                padList.Add(dt.Columns[i].DataType != typeof(int));
                colIndex.Add(i);
            }

            string rstStr = string.Empty;
            string tmpStr = string.Empty;
            string spcStr = angle + string.Join(angle.ToString(), strList) + angle;

            // 列标题
            for (int i = 0; i < colIndex.Count; i++)
            {
                if (padList[i])
                    strList[i] = " " + dt.Columns[colIndex[i]].ColumnName.PadRight(
                        lenList[i] - 1 - lll[i][0]);
                else
                    strList[i] = dt.Columns[colIndex[i]].ColumnName.PadLeft(
                        lenList[i] - 1 - lll[i][0]) + " ";
                tmpStr = vertical + string.Join(vertical.ToString(), strList) + vertical;
            }
            rstStr += (spcStr + Environment.NewLine);
            rstStr += (tmpStr + Environment.NewLine);
            rstStr += (spcStr + Environment.NewLine);

            // 列数据
            for (int i = 0, nl = 1; i < tempRowCount || (omitOutput && i < MaxOutputRow);
                i++, nl++)
            {
                // 星号输出
                bool starOutput = (omitOutput && i >= tempRowCount && i < (MaxOutputRow - 1));
                // 折叠输出最后一行为 dt 最后一行
                if (omitOutput && (i == MaxOutputRow - 1))
                {
                    i = dt.Rows.Count - 1;
                    nl = lll[0].Count - 1;
                }

                for (int j = 0; j < colIndex.Count; j++)
                {
                    if (starOutput)
                    {
                        if (j == 0 && dt.Columns[colIndex[j]].ColumnName == "index")
                        {
                            strList[j] = new string(star, i == MaxOutputRow - 2 ?
                                dt.Rows[dt.Rows.Count - 1]["index"].ToString().Length :
                                dt.Rows[i]["index"].ToString().Length).PadLeft(
                                lenList[j] - 1) + " ";
                        }
                        else strList[j] = " " + new string(star, lenList[j] - 2) + " ";
                    }
                    else
                    {
                        if (padList[j])
                            strList[j] = " " + dt.Rows[i][colIndex[j]].ToString().PadRight(
                                lenList[j] - 1 - lll[j][nl]);
                        else
                            strList[j] = dt.Rows[i][colIndex[j]].ToString().PadLeft(
                                lenList[j] - 1 - lll[j][nl]) + " ";
                    }
                }
                tmpStr = vertical + string.Join(vertical.ToString(), strList) + vertical;

                rstStr += (tmpStr + Environment.NewLine);
                if (i == dt.Rows.Count - 1)
                    rstStr += (spcStr + Environment.NewLine);
            }

            return rstStr;
        }
    }
}
