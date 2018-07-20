using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;

namespace MyFilm
{
    public class CommonDataTable
    {
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

        public struct SetWatchStateStruct
        {
            public int id;
            public bool is_folder;
            public bool to_watch_ex;
            public int pid;
            public int max_cid;
            public bool set_to;
            public DateTime set_time;
        }

        public struct SetDeleteStateStruct
        {
            public int id;
            public bool is_folder;
            public bool to_delete_ex;
            public int pid;
            public int max_cid;
            public bool set_to;
            public DateTime set_time;
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
                            dr[cl.ColumnName] = Convert.ToDateTime(fiDt.Rows[i][cl.ColumnName]).ToString("yyyy-MM-dd HHH:mm:ss");
                            break;
                        default:
                            throw new Exception(string.Format("未指定的列名称 {0}", cl.ColumnName));
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

            List<int> lenList = new List<int>();
            List<string> strList = new List<string>();
            List<bool> padList = new List<bool>();
            List<int> colIndex = new List<int>();

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                if (outputSet != null && !outputSet.Contains(dt.Columns[i].ColumnName))
                    continue;

                int maxLen = System.Text.Encoding.Default.GetBytes(
                    dt.Columns[i].ColumnName.ToString()).Length;
                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    // 单个英文长度为1，单个中文长度为2
                    int len = System.Text.Encoding.Default.GetBytes(dt.Rows[j][i].ToString()).Length;
                    if (maxLen < len) maxLen = len;
                }
                lenList.Add(maxLen + 2);
                strList.Add(new string(horizontal, maxLen + 2));
                padList.Add(dt.Columns[i].DataType != typeof(int));
                colIndex.Add(i);
            }

            string rstStr = string.Empty;
            string tmpStr = string.Empty;
            string spcStr = angle + string.Join(angle.ToString(), strList) + angle;

            for (int i = 0; i < colIndex.Count; i++)
            {
                if (padList[i])
                    strList[i] = " " + PadRightWhileDouble(dt.Columns[colIndex[i]].ColumnName, lenList[i] - 1);
                else
                    strList[i] = PadLeftWhileDouble(dt.Columns[colIndex[i]].ColumnName, lenList[i] - 1) + " ";
                tmpStr = vertical + string.Join(vertical.ToString(), strList) + vertical;
            }
            rstStr += (spcStr + Environment.NewLine);
            rstStr += (tmpStr + Environment.NewLine);
            rstStr += (spcStr + Environment.NewLine);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < colIndex.Count; j++)
                {
                    if (padList[j])
                        strList[j] = " " + PadRightWhileDouble(dt.Rows[i][colIndex[j]].ToString(), lenList[j] - 1);
                    else
                        strList[j] = PadLeftWhileDouble(dt.Rows[i][colIndex[j]].ToString(), lenList[j] - 1) + " ";
                }
                tmpStr = vertical + string.Join(vertical.ToString(), strList) + vertical;

                rstStr += (tmpStr + Environment.NewLine);
                if (i == dt.Rows.Count - 1)
                    rstStr += (spcStr + Environment.NewLine);
            }
            return rstStr;
        }

        /// <summary>
        /// 按单字节字符串向左填充长度
        /// </summary>
        ///<param name="input">
        ///<param name="length">
        ///<param name="paddingChar">
        /// <returns></returns>
        public static string PadLeftWhileDouble(string input, int length, char paddingChar = ' ')
        {
            var singleLength = GetSingleLength(input);
            return input.PadLeft(length - singleLength + input.Length, paddingChar);
        }
        private static int GetSingleLength(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return 0;
            }
            // 计算得到该字符串对应单字节字符串的长度
            return Regex.Replace(input, @"[^\x00-\xff]", "aa").Length;
        }
        /// <summary>
        /// 按单字节字符串向右填充长度
        /// </summary>
        ///<param name="input">
        ///<param name="length">
        ///<param name="paddingChar">
        /// <returns></returns>
        public static string PadRightWhileDouble(string input, int length, char paddingChar = ' ')
        {
            var singleLength = GetSingleLength(input);
            return input.PadRight(length - singleLength + input.Length, paddingChar);
        }
    }
}
