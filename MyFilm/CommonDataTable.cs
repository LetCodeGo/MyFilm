using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            dt.Columns.Add("pid", typeof(Int32));
            dt.Columns.Add("disk_desc", typeof(String));
            dt.Columns.Add("to_watch", typeof(Boolean));
            dt.Columns.Add("to_delete", typeof(Boolean));
            dt.Columns.Add("content", typeof(String));

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
            dt.Columns.Add("max_layer", typeof(Int32));

            return dt;
        }

        /// <summary>
        /// MainForm DataGridView 数据源格式
        /// </summary>
        /// <returns></returns>
        public static DataTable GetMainFormGridDataTable()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("index", typeof(Int32));
            dt.Columns.Add("id", typeof(Int32));
            dt.Columns.Add("name", typeof(String));
            dt.Columns.Add("path", typeof(String));
            dt.Columns.Add("size", typeof(String));
            dt.Columns.Add("create_t", typeof(String));
            dt.Columns.Add("modify_t", typeof(String));
            dt.Columns.Add("is_folder", typeof(Boolean));
            dt.Columns.Add("pid", typeof(Int32));
            dt.Columns.Add("disk_desc", typeof(String));
            dt.Columns.Add("to_watch", typeof(Boolean));
            dt.Columns.Add("to_delete", typeof(Boolean));
            dt.Columns.Add("content", typeof(String));

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
            dt.Columns.Add("max_layer", typeof(String));

            return dt;
        }
    }
}
