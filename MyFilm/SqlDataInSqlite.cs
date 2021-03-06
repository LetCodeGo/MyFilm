﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;

namespace MyFilm
{
    public class SqlDataInSqlite : SqlData
    {
        /// <summary>
        /// 打开数据库
        /// </summary>
        private readonly String SQLOpenCmdText = String.Empty;

        /// <summary>
        /// sqlite 数据库默认文件路径
        /// </summary>
        public static readonly String SqliteDefaultDateBasePath = Path.Combine(
            System.Windows.Forms.Application.StartupPath, "MyFilmSqlite.db");

        /// <summary>
        /// sqlite 数据库文件路径
        /// </summary>
        public readonly String SqliteDataBasePath = String.Empty;

        public SqlDataInSqlite(String sqliteDataBasePath)
        {
            this.SqliteDataBasePath = sqliteDataBasePath;
            SQLOpenCmdText = String.Format("data source = {0};", sqliteDataBasePath);
        }

        public override LoginConfig.DataBaseType GetDataBaseType()
        {
            return LoginConfig.DataBaseType.SQLITE;
        }

        public override string GetIdentString()
        {
            return string.Format("sqlite_{0}", this.SqliteDataBasePath);
        }

        /// <summary>
        /// 创建影片信息表，其中pid为其父文件夹id，disk_desc关联disk_info表
        /// +--------------+---------------+------+-----+---------+-------+
        /// | Field        | Type          | Null | Key | Default | Extra |
        /// +--------------+---------------+------+-----+---------+-------+
        /// | id           | int(11)       | NO   | PRI | NULL    |       |
        /// | name         | varchar(256)  | NO   |     | NULL    |       |
        /// | path         | varchar(1024) | NO   |     | NULL    |       |
        /// | size         | bigint(20)    | NO   | MUL | NULL    |       |
        /// | create_t     | datetime      | NO   |     | NULL    |       |
        /// | modify_t     | datetime      | NO   |     | NULL    |       |
        /// | is_folder    | tinyint(1)    | NO   | MUL | NULL    |       |
        /// | to_watch     | tinyint(1)    | NO   | MUL | NULL    |       |
        /// | to_watch_ex  | tinyint(1)    | NO   |     | NULL    |       |
        /// | s_w_t        | datetime      | NO   | MUL | NULL    |       |
        /// | to_delete    | tinyint(1)    | NO   | MUL | NULL    |       |
        /// | to_delete_ex | tinyint(1)    | NO   |     | NULL    |       |
        /// | s_d_t        | datetime      | NO   | MUL | NULL    |       |
        /// | content      | text          | NO   |     | NULL    |       |
        /// | pid          | int(11)       | NO   | MUL | NULL    |       |
        /// | max_cid      | int(11)       | NO   |     | NULL    |       |
        /// | disk_desc    | varchar(256)  | NO   | MUL | NULL    |       |
        /// +--------------+---------------+------+-----+---------+-------+
        /// </summary>
        override protected void CreateFilmInfoTable()
        {
            String cmdText = String.Format(@"create table if not exists {0} ( ", "film_info");
            cmdText += String.Format(@"{0} integer primary key, ", "id");
            cmdText += String.Format(@"{0} varchar(256) not null, ", "name");
            cmdText += String.Format(@"{0} varchar(1024) not null, ", "path");
            cmdText += String.Format(@"{0} bigint not null, ", "size");
            cmdText += String.Format(@"{0} datetime not null, ", "create_t");
            cmdText += String.Format(@"{0} datetime not null, ", "modify_t");
            cmdText += String.Format(@"{0} bool not null, ", "is_folder");
            cmdText += String.Format(@"{0} bool not null, ", "to_watch");
            cmdText += String.Format(@"{0} bool not null, ", "to_watch_ex");
            cmdText += String.Format(@"{0} datetime not null, ", "s_w_t");
            cmdText += String.Format(@"{0} bool not null, ", "to_delete");
            cmdText += String.Format(@"{0} bool not null, ", "to_delete_ex");
            cmdText += String.Format(@"{0} datetime not null, ", "s_d_t");
            cmdText += String.Format(@"{0} text not null, ", "content");
            cmdText += String.Format(@"{0} integer not null, ", "pid");
            cmdText += String.Format(@"{0} integer not null, ", "max_cid");
            cmdText += String.Format(@"{0} varchar(256) not null );", "disk_desc");

            cmdText += String.Format(@"create index IF NOT EXISTS {0}_index on {1}({0});", "size", "film_info");
            cmdText += String.Format(@"create index IF NOT EXISTS {0}_index on {1}({0});", "is_folder", "film_info");
            cmdText += String.Format(@"create index IF NOT EXISTS {0}_index on {1}({0});", "to_watch", "film_info");
            cmdText += String.Format(@"create index IF NOT EXISTS {0}_index on {1}({0});", "s_w_t", "film_info");
            cmdText += String.Format(@"create index IF NOT EXISTS {0}_index on {1}({0});", "to_delete", "film_info");
            cmdText += String.Format(@"create index IF NOT EXISTS {0}_index on {1}({0});", "s_d_t", "film_info");
            cmdText += String.Format(@"create index IF NOT EXISTS {0}_index on {1}({0});", "pid", "film_info");
            cmdText += String.Format(@"create index IF NOT EXISTS {0}_index on {1}({0});", "disk_desc", "film_info");

            ExecuteNonQueryGetAffected(cmdText, null);
        }

        /// <summary>
        /// 创建磁盘信息表，主要记录磁盘可用容量和总容量
        /// +---------------+--------------+------+-----+---------+----------------+
        /// | Field         | Type         | Null | Key | Default | Extra          |
        /// +---------------+--------------+------+-----+---------+----------------+
        /// | id            | int(11)      | NO   | PRI | NULL    | auto_increment |
        /// | disk_desc     | varchar(256) | NO   | UNI | NULL    |                |
        /// | free_space    | bigint(20)   | NO   |     | NULL    |                |
        /// | total_size    | bigint(20)   | NO   |     | NULL    |                |
        /// | complete_scan | tinyint(1)   | NO   |     | NULL    |                |
        /// | scan_layer    | int(11)      | NO   |     | NULL    |                |
        /// +---------------+--------------+------+-----+---------+----------------+
        /// </summary>
        override protected void CreateDiskInfoTable()
        {
            String cmdText = String.Format(@"create table if not exists {0} ( ", "disk_info");
            cmdText += String.Format(@"{0} integer primary key AUTOINCREMENT, ", "id");
            cmdText += String.Format(@"{0} varchar(256) unique not null, ", "disk_desc");
            cmdText += String.Format(@"{0} bigint not null, ", "free_space");
            cmdText += String.Format(@"{0} bigint not null, ", "total_size");
            cmdText += String.Format(@"{0} bool not null, ", "complete_scan");
            cmdText += String.Format(@"{0} integer not null );", "scan_layer");

            ExecuteNonQueryGetAffected(cmdText, null);
        }

        /// <summary>
        /// 创建搜索记录表，主要记录搜索关键字和时间
        /// +--------------+--------------+------+-----+---------+----------------+
        /// | Field        | Type         | Null | Key | Default | Extra          |
        /// +--------------+--------------+------+-----+---------+----------------+
        /// | id           | int(11)      | NO   | PRI | NULL    | auto_increment |
        /// | search_key   | varchar(256) | NO   |     | NULL    |                |
        /// | result_count | int(11)      | NO   |     | NULL    |                |
        /// | search_time  | datetime     | NO   |     | NULL    |                |
        /// +--------------+--------------+------+-----+---------+----------------+
        /// </summary>
        override protected void CreateSearchLogTable()
        {
            String cmdText = String.Format(@"create table if not exists {0} ( ", "search_log");
            cmdText += String.Format(@"{0} integer primary key AUTOINCREMENT, ", "id");
            cmdText += String.Format(@"{0} varchar(256) not null, ", "search_key");
            cmdText += String.Format(@"{0} integer not null, ", "result_count");
            cmdText += String.Format(@"{0} datetime not null );", "search_time");

            ExecuteNonQueryGetAffected(cmdText, null);
        }

        //public override void DeleteAllDataFormAllTable()
        //{
        //    Dictionary<String, Object> sqlParamDic = new Dictionary<string, object>();
        //    sqlParamDic.Add("@table_name", "table_name");

        //    string[] tableNames = new string[] { "film_info", "disk_info", "search_log" };
        //    foreach (string tableName in tableNames)
        //    {
        //        sqlParamDic["@table_name"] = tableName;
        //        ExecuteNonQueryGetAffected(
        //            string.Format("delete from {0};", tableName), null);
        //        ExecuteNonQueryGetAffected(
        //            "update sqlite_sequence set seq=0 where name=@table_name;", sqlParamDic);
        //    }

        //    FreeEmptySpace();
        //}

        public void FreeEmptySpace()
        {
            ExecuteNonQueryGetAffected("VACUUM;", null);
        }

        override public void InsertDataToFilmInfo(DataTable dt)
        {
            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i += 30)
                {
                    InsertDataToFilmInfo(dt, i, 30);
                }
            }
        }

        override public void InsertDataToDiskInfo(DataTable dt)
        {
            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i += 90)
                {
                    InsertDataToDiskInfo(dt, i, 90);
                }
            }
        }

        override public void InsertDataToSearchLog(DataTable dt)
        {
            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i += 120)
                {
                    InsertDataToSearchLog(dt, i, 120);
                }
            }
        }

        /// <summary>
        /// 在 film_info 中根据指定的 id列表 获取数据
        /// </summary>
        /// <param name="idList">id列表</param>
        /// <returns></returns>
        override public DataTable SelectDataByIDList(int[] idList)
        {
            if (idList == null || idList.Length == 0) return null;

            String cmdText = String.Format(
                "select * from {0} where id in ({1});",
                "film_info", String.Join(",", idList));

            DataTable dt = ExecuteReaderGetAll(cmdText, null);

            if ((!Helper.IsArrayAscended(idList)) && dt != null)
            {
                DataTable rdt = dt.Clone();
                Dictionary<int, DataRow> dic = new Dictionary<int, DataRow>();
                // id列
                int idIndex = 0;
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    if (dt.Columns[i].ColumnName == "id")
                    {
                        idIndex = i;
                        break;
                    }
                }

                foreach (DataRow dr in dt.Rows)
                {
                    dic.Add(Convert.ToInt32(dr[idIndex]), dr);
                }

                foreach (int id in idList)
                {
                    rdt.Rows.Add(dic[id].ItemArray);
                }

                return rdt;
            }

            return dt;
        }

        /// <summary>
        /// 向disk_info数据库插入或更新数据，diskDescribe唯一性
        /// </summary>
        /// <param name="diskDescribe">磁盘描述</param>
        /// <param name="freeSpace">剩余大小</param>
        /// <param name="totalSize">总大小</param>
        /// <param name="completeScan">完全扫描</param>
        /// <param name="scanLayer">扫描的层数</param>
        override public void InsertOrUpdateDataToDiskInfo(
            String diskDescribe, Int64 freeSpace, Int64 totalSize,
            Boolean completeScan, int scanLayer)
        {
            String cmdText = String.Format(
                @"insert into {0} (
                disk_desc, free_space, total_size, complete_scan, scan_layer) values(
                @disk_desc, @free_space, @total_size, @complete_scan, @scan_layer) 
                ON CONFLICT(disk_desc) DO UPDATE SET free_space = @free_space,
                total_size = @total_size, complete_scan = @complete_scan,
                scan_layer = @scan_layer;", "disk_info");

            Dictionary<String, Object> sqlParamDic = new Dictionary<string, object>();
            sqlParamDic.Add("@disk_desc", diskDescribe);
            sqlParamDic.Add("@free_space", freeSpace);
            sqlParamDic.Add("@total_size", totalSize);
            sqlParamDic.Add("@complete_scan", completeScan);
            sqlParamDic.Add("@scan_layer", scanLayer);

            ExecuteNonQueryGetAffected(cmdText, sqlParamDic);
        }

        protected override int ExecuteNonQueryGetAffected(
            String cmdText, Dictionary<String, Object> sqlParamDic = null)
        {
            int affected = 0;
            using (SQLiteConnection sqlCon = new SQLiteConnection(SQLOpenCmdText))
            {
                sqlCon.Open();
                using (SQLiteCommand sqlCmd = new SQLiteCommand(cmdText, sqlCon))
                {
                    if (sqlParamDic != null)
                    {
                        foreach (KeyValuePair<String, Object> kv in sqlParamDic)
                        {
                            sqlCmd.Parameters.AddWithValue(kv.Key, kv.Value);
                        }
                    }
                    affected = sqlCmd.ExecuteNonQuery();
                }
                sqlCon.Close();
            }

            return affected;
        }

        protected override int ExecuteScalarGetNum(
            String cmdText, Dictionary<String, Object> sqlParamDic = null)
        {
            int num = 0;
            using (SQLiteConnection sqlCon = new SQLiteConnection(SQLOpenCmdText))
            {
                sqlCon.Open();
                using (SQLiteCommand sqlCmd = new SQLiteCommand(cmdText, sqlCon))
                {
                    if (sqlParamDic != null)
                    {
                        foreach (KeyValuePair<String, Object> kv in sqlParamDic)
                        {
                            sqlCmd.Parameters.AddWithValue(kv.Key, kv.Value);
                        }
                    }
                    object obj = sqlCmd.ExecuteScalar();
                    if (obj != System.DBNull.Value) num = Convert.ToInt32(obj);
                }
                sqlCon.Close();
            }

            return num;
        }

        protected override DataTable ExecuteReaderGetAll(
            String cmdText, Dictionary<String, Object> sqlParamDic = null)
        {
            DataTable dt = null;
            using (SQLiteConnection sqlCon = new SQLiteConnection(SQLOpenCmdText))
            {
                sqlCon.Open();
                using (SQLiteCommand sqlCmd = new SQLiteCommand(cmdText, sqlCon))
                {
                    if (sqlParamDic != null)
                    {
                        foreach (KeyValuePair<String, Object> kv in sqlParamDic)
                        {
                            sqlCmd.Parameters.AddWithValue(kv.Key, kv.Value);
                        }
                    }

                    using (SQLiteDataReader sqlDataReader = sqlCmd.ExecuteReader())
                    {
                        dt = new DataTable();
                        for (int i = 0; i < sqlDataReader.FieldCount; i++)
                        {
                            dt.Columns.Add(sqlDataReader.GetName(i), sqlDataReader.GetFieldType(i));
                        }

                        while (sqlDataReader.Read())
                        {
                            DataRow dr = dt.NewRow();
                            for (int i = 0; i < dt.Columns.Count; i++)
                            {
                                dr[i] = sqlDataReader[i];
                            }
                            dt.Rows.Add(dr);
                        }
                    }
                }
                sqlCon.Close();
            }

            return dt;
        }

        protected override int[] ExecuteReaderGetIDs(
            String cmdText, Dictionary<String, Object> sqlParamDic = null)
        {
            int[] ids = null;
            using (SQLiteConnection sqlCon = new SQLiteConnection(SQLOpenCmdText))
            {
                sqlCon.Open();
                using (SQLiteCommand sqlCmd = new SQLiteCommand(cmdText, sqlCon))
                {
                    if (sqlParamDic != null)
                    {
                        foreach (KeyValuePair<String, Object> kv in sqlParamDic)
                        {
                            sqlCmd.Parameters.AddWithValue(kv.Key, kv.Value);
                        }
                    }

                    using (SQLiteDataReader sqlDataReader = sqlCmd.ExecuteReader())
                    {
                        List<int> idList = new List<int>();
                        while (sqlDataReader.Read())
                        {
                            idList.Add(Convert.ToInt32(sqlDataReader[0]));
                        }
                        ids = idList.ToArray();
                    }
                }
                sqlCon.Close();
            }

            return ids;
        }

        //public override List<String> QueryAllDataBaseNames()
        //{
        //    List<String> nameList = new List<String>();

        //    using (SQLiteConnection sqlCon = new SQLiteConnection(SQLOpenCmdText))
        //    {
        //        sqlCon.Open();
        //        using (SQLiteCommand sqlCmd = new SQLiteCommand(".databases;", sqlCon))
        //        {
        //            using (SQLiteDataReader sqlDataReader = sqlCmd.ExecuteReader())
        //            {
        //                while (sqlDataReader.Read())
        //                    nameList.Add(sqlDataReader[0].ToString());
        //            }
        //        }
        //        sqlCon.Close();
        //    }

        //    return nameList;
        //}

        //public override void CreateDataBase(String databaseName)
        //{
        //    using (SQLiteConnection sqlCon = new SQLiteConnection(SQLOpenCmdText))
        //    {
        //        sqlCon.Open();
        //        using (SQLiteCommand sqlCmd = new SQLiteCommand(
        //            String.Format("create database {0};", databaseName), sqlCon))
        //        {
        //            sqlCmd.ExecuteNonQuery();
        //        }
        //        sqlCon.Close();
        //    }
        //}

        public override void FillRamData()
        {
            String cmdText = String.Format(
                "select id, name, path, disk_desc from {0};", "film_info");

            using (SQLiteConnection sqlCon = new SQLiteConnection(SQLOpenCmdText))
            {
                sqlCon.Open();
                using (SQLiteCommand sqlCmd = new SQLiteCommand(cmdText, sqlCon))
                {
                    using (SQLiteDataReader sqlDataReader = sqlCmd.ExecuteReader())
                    {
                        ramData = new Dictionary<string, List<FileNamePathID>>();
                        int rowCount = 0;

                        while (sqlDataReader.Read())
                        {
                            rowCount++;
                            string strDiskDesc = sqlDataReader[3].ToString();

                            if (ramData.ContainsKey(strDiskDesc))
                                ramData[strDiskDesc].Add(new FileNamePathID(
                                    sqlDataReader[1].ToString().ToLower(),
                                    sqlDataReader[2].ToString().ToLower(),
                                    Convert.ToInt32(sqlDataReader[0])));
                            else
                            {
                                List<FileNamePathID> fnpiList = new List<FileNamePathID>();
                                fnpiList.Add(new FileNamePathID(
                                    sqlDataReader[1].ToString().ToLower(),
                                    sqlDataReader[2].ToString().ToLower(),
                                    Convert.ToInt32(sqlDataReader[0])));
                                ramData.Add(strDiskDesc, fnpiList);
                            }
                        }

                        ramDataCompleted = true;
                    }
                }
                sqlCon.Close();
            }
        }

        public override DataTable GetFilmInfoDatabaseTransferData()
        {
            String cmdText = String.Format("select * from {0} where disk_desc!=@disk_desc order by id;", "film_info");
            DataTable dt = CommonDataTable.GetFilmInfoDataTable();
            List<int> continuedMinList = new List<int>();
            List<int> continuedMaxList = new List<int>();
            List<int> subtractList = new List<int>();
            int continuedMin = -1, continuedMax = -1, subtract = -1, id = -1, preID = -1;

            using (SQLiteConnection sqlCon = new SQLiteConnection(SQLOpenCmdText))
            {
                sqlCon.Open();
                using (SQLiteCommand sqlCmd = new SQLiteCommand(cmdText, sqlCon))
                {
                    sqlCmd.Parameters.AddWithValue("@disk_desc", CommonString.RealOrFake4KDiskName);
                    using (SQLiteDataReader sqlDataReader = sqlCmd.ExecuteReader())
                    {
                        while (sqlDataReader.Read())
                        {
                            DataRow dr = dt.NewRow();
                            for (int i = 0; i < dt.Columns.Count; i++)
                            {
                                dr[i] = sqlDataReader[i];
                            }
                            dt.Rows.Add(dr);

                            preID = id;
                            id = Convert.ToInt32(dr["id"]);
                            if (continuedMin == -1) continuedMin = preID;

                            if (preID != -1 && id - preID > 1)
                            {
                                continuedMax = preID;
                                if (continuedMinList.Count == 0)
                                    subtract = continuedMin - 1;
                                else
                                {
                                    subtract = subtractList[subtractList.Count - 1] +
                                        continuedMin - continuedMaxList[continuedMaxList.Count - 1] - 1;
                                }
                                continuedMinList.Add(continuedMin);
                                continuedMaxList.Add(continuedMax);
                                subtractList.Add(subtract);

                                continuedMin = -1;
                            }
                        }

                        continuedMax = id;
                        if (continuedMinList.Count == 0)
                            subtract = continuedMin - 1;
                        else
                        {
                            subtract = subtractList[subtractList.Count - 1] +
                                continuedMin - continuedMaxList[continuedMaxList.Count - 1] - 1;
                        }
                        continuedMinList.Add(continuedMin);
                        continuedMaxList.Add(continuedMax);
                        subtractList.Add(subtract);
                    }
                }
                sqlCon.Close();
            }

            int[] cols = new int[3] { 0, 14, 15 };
            int startIndex = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                id = Convert.ToInt32(dt.Rows[i][0]);
                for (int j = startIndex; j < continuedMaxList.Count; j++)
                {
                    if (id <= continuedMaxList[j])
                    {
                        startIndex = j;
                        break;
                    }
                }
                foreach (int col in cols)
                {
                    int num = Convert.ToInt32(dt.Rows[i][col]);
                    if (num != -1)
                        dt.Rows[i][col] = num - subtractList[startIndex];
                }
            }

            return dt;
        }

        public override DataTable GetDiskInfoDatabaseTransferData()
        {
            String cmdText = String.Format("select * from {0} order by id;", "disk_info");
            DataTable dt = CommonDataTable.GetDiskInfoDataTable();

            using (SQLiteConnection sqlCon = new SQLiteConnection(SQLOpenCmdText))
            {
                sqlCon.Open();
                using (SQLiteCommand sqlCmd = new SQLiteCommand(cmdText, sqlCon))
                {
                    using (SQLiteDataReader sqlDataReader = sqlCmd.ExecuteReader())
                    {
                        while (sqlDataReader.Read())
                        {
                            DataRow dr = dt.NewRow();
                            for (int i = 0; i < dt.Columns.Count; i++)
                            {
                                dr[i] = sqlDataReader[i];
                            }
                            dt.Rows.Add(dr);
                        }
                    }
                }
                sqlCon.Close();
            }

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dt.Rows[i][0] = i + 1;
            }

            return dt;
        }

        public override DataTable GetSearchLogDatabaseTransferData()
        {
            String cmdText = String.Format("select * from {0} order by id;", "search_log");
            DataTable dt = CommonDataTable.GetSearchLogDataTable();

            using (SQLiteConnection sqlCon = new SQLiteConnection(SQLOpenCmdText))
            {
                sqlCon.Open();
                using (SQLiteCommand sqlCmd = new SQLiteCommand(cmdText, sqlCon))
                {
                    using (SQLiteDataReader sqlDataReader = sqlCmd.ExecuteReader())
                    {
                        while (sqlDataReader.Read())
                        {
                            DataRow dr = dt.NewRow();
                            for (int i = 0; i < dt.Columns.Count; i++)
                            {
                                dr[i] = sqlDataReader[i];
                            }
                            dt.Rows.Add(dr);
                        }
                    }
                }
                sqlCon.Close();
            }

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dt.Rows[i][0] = i + 1;
            }

            return dt;
        }

        public override int[] GetDeleteDataFromFilmInfoGroupByDisk(
            String diskDescribe = null)
        {
            String cmdText = String.Format(
                @"select group_concat(id), count(id) as id_count from {0} where 
                to_delete = 1 {1} group by disk_desc order by id_count desc;",
                "film_info", diskDescribe == null ? "" : "and disk_desc = @disk_desc");

            int[] ids = null;
            using (SQLiteConnection sqlCon = new SQLiteConnection(SQLOpenCmdText))
            {
                sqlCon.Open();
                using (SQLiteCommand sqlCmd = new SQLiteCommand(cmdText, sqlCon))
                {
                    if (diskDescribe != null)
                    {
                        sqlCmd.Parameters.AddWithValue("@disk_desc", diskDescribe);
                    }

                    using (SQLiteDataReader sqlDataReader = sqlCmd.ExecuteReader())
                    {
                        String strTemp = String.Empty;
                        while (sqlDataReader.Read())
                        {
                            strTemp += ("," + sqlDataReader[0].ToString());
                        }
                        List<String> strIdList = strTemp.Split(
                            new char[] { ',' },
                            StringSplitOptions.RemoveEmptyEntries).ToList();
                        ids = strIdList.ConvertAll(x => Convert.ToInt32(x)).ToArray();
                    }
                }
                sqlCon.Close();
            }

            return ids;
        }
    }
}
