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
            dt.Columns.Add("s_w_t", typeof(DateTime));
            dt.Columns.Add("to_delete", typeof(Boolean));
            dt.Columns.Add("s_d_t", typeof(DateTime));
            dt.Columns.Add("content", typeof(String));
            dt.Columns.Add("pid", typeof(Int32));
            dt.Columns.Add("max_cid", typeof(Int32));
            dt.Columns.Add("disk_desc", typeof(String));

            return dt;
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

        public static String DataTableFormatToString(DataTable dt, HashSet<String> noOutSet)
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
                if ((noOutSet != null) && noOutSet.Contains(dt.Columns[i].ColumnName)) continue;

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
