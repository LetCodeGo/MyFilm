using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace MyFilm
{
    public class SqlData
    {
        /// <summary>
        /// 连接MySql
        /// </summary>
        private MySqlConnection sqlCon = null;

        /// <summary>
        /// film_info id
        /// </summary>
        private int startIdGlobal = 0;

        /// <summary>
        /// 实际扫描的磁盘最大深度
        /// </summary>
        private int actualMaxScanLayer = 0;

        /// <summary>
        /// 是否为完全扫描
        /// </summary>
        private bool bCompleteScan = true;

        /// <summary>
        /// 设定的最大扫描深度
        /// </summary>
        private int setMaxScanLayer = Int32.MaxValue;

        /// <summary>
        /// 打开数据库
        /// </summary>
        public void OpenMySql()
        {
            String sqlText = String.Format(
                "server = {0}; uid = {1}; pwd = {2}; database = {3}; SslMode = none; convert zero datetime=true; allow zero datetime=true;",
                CommonString.DbIP, CommonString.DbUserName, CommonString.DbPassword, CommonString.DbName);
            sqlCon = new MySqlConnection(sqlText);
            sqlCon.Open();
        }

        /// <summary>
        /// 关闭数据库连接
        /// </summary>
        public void CloseMySql()
        {
            sqlCon.Close();
        }

        /// <summary>
        /// 创建表
        /// </summary>
        public void CreateTables()
        {
            CreateFilmInfoTable();
            CreateDiskInfoTable();
            CreateSearchLogTable();
        }

        /// <summary>
        /// 创建影片信息表，其中pid为其父文件夹id，disk_desc关联disk_info表
        /// +-----------+---------------+------+-----+---------+-------+
        /// | Field     | Type          | Null | Key | Default | Extra |
        /// +-----------+---------------+------+-----+---------+-------+
        /// | id        | int(11)       | NO   | PRI | NULL    |       |
        /// | name      | varchar(256)  | NO   |     | NULL    |       |
        /// | path      | varchar(1024) | NO   |     | NULL    |       |
        /// | size      | bigint(20)    | NO   |     | NULL    |       |
        /// | create_t  | datetime      | NO   |     | NULL    |       |
        /// | modify_t  | datetime      | NO   |     | NULL    |       |
        /// | is_folder | tinyint(1)    | NO   |     | NULL    |       |
        /// | to_watch  | tinyint(1)    | NO   |     | NULL    |       |
        /// | s_w_t     | datetime      | NO   |     | NULL    |       |
        /// | to_delete | tinyint(1)    | NO   |     | NULL    |       |
        /// | s_d_t     | datetime      | NO   |     | NULL    |       |
        /// | content   | text          | NO   |     | NULL    |       |
        /// | pid       | int(11)       | NO   |     | NULL    |       |
        /// | max_cid   | int(11)       | NO   |     | NULL    |       |
        /// | disk_desc | varchar(256)  | NO   |     | NULL    |       |
        /// +-----------+---------------+------+-----+---------+-------+
        /// </summary>
        private void CreateFilmInfoTable()
        {
            String sqlText = String.Format(@"create table if not exists {0} ( ", "film_info");
            sqlText += String.Format(@"{0} integer primary key, ", "id");
            sqlText += String.Format(@"{0} varchar(256) not null, ", "name");
            sqlText += String.Format(@"{0} varchar(1024) not null, ", "path");
            sqlText += String.Format(@"{0} bigint not null, ", "size");
            sqlText += String.Format(@"{0} datetime not null, ", "create_t");
            sqlText += String.Format(@"{0} datetime not null, ", "modify_t");
            sqlText += String.Format(@"{0} bool not null, ", "is_folder");
            sqlText += String.Format(@"{0} bool not null, ", "to_watch");
            sqlText += String.Format(@"{0} datetime not null, ", "s_w_t");
            sqlText += String.Format(@"{0} bool not null, ", "to_delete");
            sqlText += String.Format(@"{0} datetime not null, ", "s_d_t");
            sqlText += String.Format(@"{0} text not null, ", "content");
            sqlText += String.Format(@"{0} integer not null, ", "pid");
            sqlText += String.Format(@"{0} integer not null, ", "max_cid");
            sqlText += String.Format(@"{0} varchar(256) not null );", "disk_desc");

            MySqlCommand sqlCom = new MySqlCommand(sqlText, sqlCon);
            sqlCom.ExecuteNonQuery();
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
        private void CreateDiskInfoTable()
        {
            String sqlText = String.Format(@"create table if not exists {0} ( ", "disk_info");
            sqlText += String.Format(@"{0} integer primary key auto_increment, ", "id");
            sqlText += String.Format(@"{0} varchar(256) unique key not null, ", "disk_desc");
            sqlText += String.Format(@"{0} bigint not null, ", "free_space");
            sqlText += String.Format(@"{0} bigint not null, ", "total_size");
            sqlText += String.Format(@"{0} bool not null, ", "complete_scan");
            sqlText += String.Format(@"{0} integer not null );", "scan_layer");

            MySqlCommand sqlCom = new MySqlCommand(sqlText, sqlCon);
            sqlCom.ExecuteNonQuery();
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
        private void CreateSearchLogTable()
        {
            String sqlText = String.Format(@"create table if not exists {0} ( ", "search_log");
            sqlText += String.Format(@"{0} integer primary key auto_increment, ", "id");
            sqlText += String.Format(@"{0} varchar(256) not null, ", "search_key");
            sqlText += String.Format(@"{0} integer not null, ", "result_count");
            sqlText += String.Format(@"{0} datetime not null );", "search_time");

            MySqlCommand slqCom = new MySqlCommand(sqlText, sqlCon);
            slqCom.ExecuteNonQuery();
        }

        /// <summary>
        /// 扫描磁盘，更新磁盘信息和影片信息
        /// </summary>
        /// <param name="diskPath">磁盘路径</param>
        /// <param name="diskDescribe">磁盘描述</param>
        /// <param name="setScanLayer">设定的最多扫描层数</param>
        /// <returns>是否为完全扫描</returns>
        public Boolean ScanDisk(
            String diskPath, String diskDescribe, Int32 setScanLayer = Int32.MaxValue)
        {
            this.actualMaxScanLayer = 0;
            this.bCompleteScan = true;
            this.setMaxScanLayer = setScanLayer;

            string sqlStr = string.Format("select max(id) from {0};", "film_info");
            MySqlCommand sqlCom = new MySqlCommand(sqlStr, sqlCon);
            object maxIdObj = sqlCom.ExecuteScalar();
            int maxId = 0;
            if (maxIdObj != DBNull.Value) maxId = Convert.ToInt32(sqlCom.ExecuteScalar());
            int startId = maxId + 1;
            this.startIdGlobal = startId;

            DriveInfo driveInfo = new DriveInfo(diskPath);
            DataTable dt = CommonDataTable.GetFilmInfoDataTable();
            DataRow dr = dt.NewRow();
            dr["id"] = this.startIdGlobal++;
            dr["name"] = driveInfo.RootDirectory.Name;
            dr["path"] = driveInfo.RootDirectory.FullName;
            dr["size"] = -1;
            dr["create_t"] = driveInfo.RootDirectory.CreationTime;
            dr["modify_t"] = driveInfo.RootDirectory.LastWriteTime;
            dr["is_folder"] = true;
            dr["to_watch"] = false;
            dr["s_w_t"] = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
            dr["to_delete"] = false;
            dr["s_d_t"] = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
            dr["content"] = String.Empty;
            dr["pid"] = -1;
            dr["disk_desc"] = diskDescribe;
            dt.Rows.Add(dr);

            Dictionary<string, int> maxCidDic = new Dictionary<string, int>();
            maxCidDic.Add(driveInfo.RootDirectory.FullName, this.startIdGlobal - 1);

            ScanAllInFolder(driveInfo.RootDirectory, diskDescribe,
                this.startIdGlobal - 1, setScanLayer, ref dt, ref maxCidDic);

            Dictionary<int, long> sizeDic = new Dictionary<int, long>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (Convert.ToBoolean(dt.Rows[i]["is_folder"]))
                {
                    int maxCid = maxCidDic[dt.Rows[i]["path"].ToString()];
                    dt.Rows[i]["max_cid"] = maxCid;

                    if (bCompleteScan)
                    {
                        sizeDic.Add(i, 0);
                        for (int j = i + 1; j + startId <= maxCid; j++)
                        {
                            if (!Convert.ToBoolean(dt.Rows[j]["is_folder"]))
                                sizeDic[i] += Helper.CalcSpace(Convert.ToInt64(dt.Rows[j]["size"]));
                        }
                    }
                }
                else dt.Rows[i]["max_cid"] = dt.Rows[i]["id"];
            }

            // 简略扫描不计算文件夹大小
            if (bCompleteScan)
            {
                foreach (KeyValuePair<int, long> kv in sizeDic)
                    dt.Rows[kv.Key]["size"] = kv.Value;
            }

            //InsertDataToFilmInfo(dt, 0, dt.Rows.Count);
            int maxInsertRows = 1000;
            int insertTimes = dt.Rows.Count / maxInsertRows;
            if (dt.Rows.Count % maxInsertRows != 0) insertTimes += 1;
            for (int i = 0; i < insertTimes; i++)
            {
                InsertDataToFilmInfo(dt, i * maxInsertRows, maxInsertRows);
            }

            // 更新磁盘信息
            InsertOrUpdateDataToDiskInfo(
                diskDescribe, driveInfo.TotalFreeSpace, driveInfo.TotalSize,
                bCompleteScan, bCompleteScan ? actualMaxScanLayer : setScanLayer);

            return bCompleteScan;
        }

        /// <summary>
        /// 扫描文件夹内容（不包含此文件夹）
        /// </summary>
        /// <param name="directoryInfo">此文件夹信息</param>
        /// <param name="diskDescribe">磁盘描述</param>
        /// <param name="pid">此文件夹数据库id</param>
        /// <param name="setScanLayer">设定的最多扫描层数</param>
        /// <param name="dt">记录要向film_info表中插入的数据</param>
        /// <param name="maxCidDic">记录文件夹下递归的子文件夹或文件的最大id号</param>
        private void ScanAllInFolder(
            DirectoryInfo directoryInfo, String diskDescribe,
            Int32 pid, Int32 setScanLayer, ref DataTable dt,
            ref Dictionary<string, int> maxCidDic)
        {
            DirectoryInfo[] directoryInfoArray = directoryInfo.GetDirectories();
            FileInfo[] fileInfoArray = directoryInfo.GetFiles();

            Array.Sort<DirectoryInfo>(directoryInfoArray, (d1, d2) => d1.Name.CompareTo(d2.Name));
            Array.Sort<FileInfo>(fileInfoArray, (f1, f2) => f1.Name.CompareTo(f2.Name));

            if (setScanLayer <= 0)
            {
                if (directoryInfoArray.Length > 0 || fileInfoArray.Length > 0) bCompleteScan = false;
                return;
            }

            if (directoryInfoArray.Length > 0 || fileInfoArray.Length > 0)
                actualMaxScanLayer = Math.Max(actualMaxScanLayer, this.setMaxScanLayer - setScanLayer + 1);

            int i = 0;
            foreach (DirectoryInfo childDirectoryInfo in directoryInfoArray)
            {
                i++;
                if ((childDirectoryInfo.Attributes & FileAttributes.System) == FileAttributes.System) continue;

                DataRow dr = dt.NewRow();
                dr["id"] = this.startIdGlobal++;
                dr["name"] = childDirectoryInfo.Name;
                dr["path"] = childDirectoryInfo.FullName;
                dr["size"] = -1;
                dr["create_t"] = childDirectoryInfo.CreationTime;
                dr["modify_t"] = childDirectoryInfo.LastWriteTime;
                dr["is_folder"] = true;
                dr["to_watch"] = false;
                dr["s_w_t"] = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
                dr["to_delete"] = false;
                dr["s_d_t"] = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
                dr["content"] = String.Empty;
                dr["pid"] = pid;
                dr["disk_desc"] = diskDescribe;
                dt.Rows.Add(dr);

                maxCidDic.Add(childDirectoryInfo.FullName, this.startIdGlobal - 1);
                if (i == directoryInfoArray.Length)
                {
                    List<String> keyPathArray = new List<String>(maxCidDic.Keys);
                    foreach (String keyPath in keyPathArray)
                    {
                        if (childDirectoryInfo.FullName.Contains(keyPath.TrimEnd('\\') + "\\"))
                            maxCidDic[keyPath] = this.startIdGlobal - 1;
                    }
                }

                ScanAllInFolder(childDirectoryInfo, diskDescribe,
                    this.startIdGlobal - 1, setScanLayer - 1, ref dt, ref maxCidDic);
            }

            int j = 0;
            foreach (FileInfo fileInfo in fileInfoArray)
            {
                j++;
                DataRow dr = dt.NewRow();
                dr["id"] = this.startIdGlobal++;
                dr["name"] = fileInfo.Name;
                dr["path"] = fileInfo.FullName;
                dr["size"] = fileInfo.Length;
                dr["create_t"] = fileInfo.CreationTime;
                dr["modify_t"] = fileInfo.LastWriteTime;
                dr["is_folder"] = false;
                dr["to_watch"] = false;
                dr["s_w_t"] = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
                dr["to_delete"] = false;
                dr["s_d_t"] = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
                // 只读取 10KB 以下 __game_version_info__.gvi 文件内容
                if (fileInfo.Name.ToLower() == "__game_version_info__.gvi" && fileInfo.Length <= 10240)
                    dr["content"] = File.ReadAllText(fileInfo.FullName);
                else dr["content"] = String.Empty;
                dr["pid"] = pid;
                dr["disk_desc"] = diskDescribe;
                dt.Rows.Add(dr);

                if (j == fileInfoArray.Length)
                {
                    List<String> keyPathArray = new List<String>(maxCidDic.Keys);
                    foreach (String keyPath in keyPathArray)
                    {
                        if (fileInfo.FullName.Contains(keyPath.TrimEnd('\\') + "\\"))
                            maxCidDic[keyPath] = this.startIdGlobal - 1;
                    }
                }
            }
        }

        /// <summary>
        /// 根据文件夹路径和磁盘描述获取唯一数据库id，在一块磁盘下路径具有唯一性
        /// </summary>
        /// <param name="path">文件夹路径</param>
        /// <param name="diskDescribe">磁盘描述</param>
        /// <returns>id</returns>
        private Int32 SearchFilmInfoIdByPathAndDiskDescribe(String path, String diskDescribe)
        {
            string sqlStr = string.Format(
                "select id from {0} where path = @path and disk_desc = @disk_desc;", "film_info");
            MySqlCommand sqlCom = new MySqlCommand(sqlStr, sqlCon);
            sqlCom.Parameters.AddWithValue("@path", path);
            sqlCom.Parameters.AddWithValue("@disk_desc", diskDescribe);
            object obj = sqlCom.ExecuteScalar();
            if (obj == DBNull.Value) return -1;
            else return Convert.ToInt32(obj);
        }

        /// <summary>
        /// 获取最新插入数据ID（仅针对插入一条数据）
        /// </summary>
        /// <returns></returns>
        private Int32 GetLastInsertID()
        {
            string sqlStr = string.Format("select last_insert_id();");
            MySqlCommand sqlCom = new MySqlCommand(sqlStr, sqlCon);
            object obj = sqlCom.ExecuteScalar();
            if (obj == DBNull.Value) return -1;
            else return Convert.ToInt32(obj);
        }

        /// <summary>
        /// 向film_info数据库插入数据，注意dt的列为FillFilmInfoColumn生成
        /// </summary>
        /// <param name="dt">插入的数据</param>
        /// <param name="start">起始，从0开始</param>
        /// <param name="count">个数</param>
        private void InsertDataToFilmInfo(DataTable dt, int start, int count)
        {
            MySqlCommand sqlCom = new MySqlCommand();
            sqlCom.Connection = sqlCon;
            sqlCom.CommandText = String.Format(
                @"insert into {0} (
                id, name, path, size, create_t, modify_t, is_folder, to_watch, s_w_t, to_delete, s_d_t, 
                content, pid, max_cid, disk_desc) values", "film_info");

            int j = 0;
            for (int i = start; i < dt.Rows.Count && j < count; i++, j++)
            {
                sqlCom.CommandText += String.Format(
                    @"(@id{0}, @name{0}, @path{0}, @size{0}, @create_t{0}, @modify_t{0}, @is_folder{0}, 
                    @to_watch{0}, @s_w_t{0}, @to_delete{0}, @s_d_t{0}, @content{0}, @pid{0}, @max_cid{0}, @disk_desc{0}){1}",
                    i, i == dt.Rows.Count - 1 || j == count - 1 ? ";" : ",");

                sqlCom.Parameters.AddWithValue(string.Format("@id{0}", i), dt.Rows[i]["id"]);
                sqlCom.Parameters.AddWithValue(string.Format("@name{0}", i), dt.Rows[i]["name"]);
                sqlCom.Parameters.AddWithValue(string.Format("@path{0}", i), dt.Rows[i]["path"]);
                sqlCom.Parameters.AddWithValue(string.Format("@size{0}", i), dt.Rows[i]["size"]);
                sqlCom.Parameters.AddWithValue(string.Format("@create_t{0}", i), dt.Rows[i]["create_t"]);
                sqlCom.Parameters.AddWithValue(string.Format("@modify_t{0}", i), dt.Rows[i]["modify_t"]);
                sqlCom.Parameters.AddWithValue(string.Format("@is_folder{0}", i), dt.Rows[i]["is_folder"]);
                sqlCom.Parameters.AddWithValue(string.Format("@to_watch{0}", i), dt.Rows[i]["to_watch"]);
                sqlCom.Parameters.AddWithValue(string.Format("@s_w_t{0}", i), dt.Rows[i]["s_w_t"]);
                sqlCom.Parameters.AddWithValue(string.Format("@to_delete{0}", i), dt.Rows[i]["to_delete"]);
                sqlCom.Parameters.AddWithValue(string.Format("@s_d_t{0}", i), dt.Rows[i]["s_d_t"]);
                sqlCom.Parameters.AddWithValue(string.Format("@content{0}", i), dt.Rows[i]["content"]);
                sqlCom.Parameters.AddWithValue(string.Format("@pid{0}", i), dt.Rows[i]["pid"]);
                sqlCom.Parameters.AddWithValue(string.Format("@max_cid{0}", i), dt.Rows[i]["max_cid"]);
                sqlCom.Parameters.AddWithValue(string.Format("@disk_desc{0}", i), dt.Rows[i]["disk_desc"]);
            }

            int affectedRows = sqlCom.ExecuteNonQuery();
            Debug.Assert(affectedRows == j);
        }

        /// <summary>
        /// 向disk_info数据库插入或更新数据，diskDescribe唯一性
        /// </summary>
        /// <param name="diskDescribe">磁盘描述</param>
        /// <param name="freeSpace">剩余大小</param>
        /// <param name="totalSize">总大小</param>
        /// <param name="completeScan">完全扫描</param>
        /// <param name="scanLayer">扫描的层数</param>
        private void InsertOrUpdateDataToDiskInfo(
            String diskDescribe, Int64 freeSpace, Int64 totalSize,
            Boolean completeScan, Int32 scanLayer)
        {
            MySqlCommand sqlCom = new MySqlCommand();
            sqlCom.Connection = sqlCon;
            sqlCom.CommandText = String.Format(
                @"insert into {0} (disk_desc, free_space, total_size, complete_scan, scan_layer) values(
                @disk_desc, @free_space, @total_size, @complete_scan, @scan_layer) 
                on duplicate key update free_space = values(free_space),
                total_size = values(total_size), complete_scan = values(complete_scan),
                scan_layer = values(scan_layer);", "disk_info");
            sqlCom.Parameters.AddWithValue("@disk_desc", diskDescribe);
            sqlCom.Parameters.AddWithValue("@free_space", freeSpace);
            sqlCom.Parameters.AddWithValue("@total_size", totalSize);
            sqlCom.Parameters.AddWithValue("@complete_scan", completeScan);
            sqlCom.Parameters.AddWithValue("@scan_layer", scanLayer);

            int affectedRows = sqlCom.ExecuteNonQuery();
            Debug.Assert(affectedRows == 1 || affectedRows == 2);
        }

        /// <summary>
        /// 向search_log数据库插入数据
        /// </summary>
        /// <param name="searchKey">搜索关键字</param>
        /// <param name="resultCount">搜索结果条数</param>
        /// <param name="searchTime">搜索时间</param>
        public void InsertDataToSearchLog(String searchKey, Int32 resultCount, DateTime searchTime)
        {
            MySqlCommand sqlCom = new MySqlCommand();
            sqlCom.Connection = sqlCon;
            sqlCom.CommandText = String.Format(
                @"insert into {0} (search_key, result_count, search_time) values
                (@search_key, @result_count, @search_time);", "search_log");
            sqlCom.Parameters.AddWithValue("@search_key", searchKey);
            sqlCom.Parameters.AddWithValue("@result_count", resultCount);
            sqlCom.Parameters.AddWithValue("@search_time", searchTime);

            int affectedRows = sqlCom.ExecuteNonQuery();
            Debug.Assert(affectedRows == 1);
        }

        /// <summary>
        /// 计数搜索结果
        /// </summary>
        /// <param name="keyWord">搜索关键字</param>
        /// <param name="diskDescribe">null时所有磁盘，否则特定磁盘</param>
        /// <returns></returns>
        public int CountSearchKeyWordFromFilmInfo(String keyWord, String diskDescribe = null)
        {
            string sqlStr = string.Format(
                "select count(id) from {0} where name like @search and disk_desc = @disk_desc;", "film_info");
            if (diskDescribe == null)
                sqlStr = string.Format("select count(id) from {0} where name like @search;", "film_info");

            MySqlCommand sqlCom = new MySqlCommand(sqlStr, sqlCon);

            String charInString = " `~!@#$%^&*()-_=+[]{}|\\;:\'\",./<>?《》（）";
            sqlCom.Parameters.AddWithValue("@search", string.Format("%{0}%",
                Helper.Replace(keyWord.Trim(), charInString, '_')));
            if (diskDescribe != null) sqlCom.Parameters.AddWithValue("@disk_desc", diskDescribe);

            return Convert.ToInt32(sqlCom.ExecuteScalar());
        }

        /// <summary>
        /// 在film_info数据库中搜索关键字keywork
        /// </summary>
        /// <param name="keyWord">关键字</param>
        /// <param name="offset">偏移</param>
        /// <param name="rows">个数</param>
        /// <param name="diskDescribe">null时所有磁盘，否则特定磁盘</param>
        /// <returns></returns>
        public DataTable SearchKeyWordFromFilmInfo(
            String keyWord, Int32 offset, Int32 rows, String diskDescribe = null)
        {
            string sqlStr = string.Format(
                "select * from {0} where name like @search and disk_desc = @disk_desc limit {1}, {2};",
                "film_info", offset, rows);
            if (diskDescribe == null)
                sqlStr = string.Format("select * from {0} where name like @search limit {1}, {2};",
                    "film_info", offset, rows);

            MySqlCommand sqlCom = new MySqlCommand(sqlStr, sqlCon);

            String charInString = " `~!@#$%^&*()-_=+[]{}|\\;:\'\",./<>?《》（）";
            sqlCom.Parameters.AddWithValue("@search", string.Format("%{0}%",
                Helper.Replace(keyWord.Trim(), charInString, '_')));
            if (diskDescribe != null) sqlCom.Parameters.AddWithValue("@disk_desc", diskDescribe);

            DataTable dt = CommonDataTable.GetFilmInfoDataTable();

            MySqlDataReader sqlDataReader = sqlCom.ExecuteReader();
            GetDataFromSqlDataReader(ref dt, sqlDataReader);
            sqlDataReader.Close();

            return dt;
        }

        /// <summary>
        /// MySqlDataReader生成DataTable，注意是所有数据
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="sqlDataReader"></param>
        private void GetDataFromSqlDataReader(ref DataTable dt, MySqlDataReader sqlDataReader)
        {
            while (sqlDataReader.Read())
            {
                DataRow dr = dt.NewRow();
                for (int i = 0; i < dt.Columns.Count; i++) dr[i] = sqlDataReader[i];
                dt.Rows.Add(dr);
            }
        }

        /// <summary>
        /// 查看film_info表结构的详细信息
        /// </summary>
        /// <returns></returns>
        public string GetDescriptionOfFilmInfo()
        {
            //string sqlStr = string.Format("select * from information_schema.columns where table_schema = @db and table_name = @tb;");
            //MySqlCommand sqlCom = new MySqlCommand(sqlStr, sqlCon);
            //sqlCom.Parameters.AddWithValue("@db", CommonString.DbName);
            //sqlCom.Parameters.AddWithValue("@tb", "film_info");

            string sqlStr = string.Format("desc {0}", "film_info");
            MySqlCommand sqlCom = new MySqlCommand(sqlStr, sqlCon);

            MySqlDataReader sqlDataReader = sqlCom.ExecuteReader();
            DataTable dt = new DataTable();
            for (int i = 0; i < sqlDataReader.FieldCount; i++)
            {
                dt.Columns.Add(sqlDataReader.GetName(i), sqlDataReader.GetFieldType(i));
            }
            GetDataFromSqlDataReader(ref dt, sqlDataReader);
            sqlDataReader.Close();

            return CommonDataTable.DataTableFormatToString(dt);
        }

        public string GetDataBySql(String sqlStr)
        {
            SortedDictionary<int, char> dict1 = new SortedDictionary<int, char>();
            int a1 = sqlStr.IndexOf('\'', 0);
            while (a1 != -1)
            {
                dict1.Add(a1, '\'');
                a1 = sqlStr.IndexOf('\'', a1 + 1);
            }
            int a2 = sqlStr.IndexOf('"', 0);
            while (a2 != -1)
            {
                dict1.Add(a2, '"');
                a2 = sqlStr.IndexOf('"', a2 + 1);
            }

            SortedDictionary<int, int> dict2 = new SortedDictionary<int, int>();
            char tempChar = ' ';
            int tempInt = 0;
            foreach (KeyValuePair<int, char> kv in dict1)
            {
                if (tempChar == ' ') { tempChar = kv.Value; tempInt = kv.Key; }
                else
                {
                    if (tempChar == kv.Value)
                    {
                        dict2.Add(tempInt, kv.Key);
                        tempChar = ' ';
                    }
                }
            }
            if (tempChar != ' ') { return "error sql statement"; }

            int index = 0;
            int n = 0;
            string strSqlNew = string.Empty;
            foreach (KeyValuePair<int, int> kv in dict2)
            {
                strSqlNew += sqlStr.Substring(index, kv.Key - index);
                strSqlNew += string.Format("@{0}", n++);
                index = kv.Value + 1;
            }
            strSqlNew += sqlStr.Substring(index);

            MySqlCommand sqlCom = new MySqlCommand(strSqlNew, sqlCon);
            n = 0;
            foreach (KeyValuePair<int, int> kv in dict2)
            {
                sqlCom.Parameters.AddWithValue(string.Format("@{0}", n++), sqlStr.Substring(kv.Key + 1, kv.Value - kv.Key - 1));
            }

            try
            {
                DataTable dt = null;
                using (MySqlDataReader sqlDataReader = sqlCom.ExecuteReader())
                {
                    dt = new DataTable();
                    for (int i = 0; i < sqlDataReader.FieldCount; i++)
                    {
                        dt.Columns.Add(sqlDataReader.GetName(i), sqlDataReader.GetFieldType(i));
                    }
                    GetDataFromSqlDataReader(ref dt, sqlDataReader);
                }
                return CommonDataTable.DataTableFormatToString(dt);
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        /// <summary>
        /// 获取disk_info数据库所有数据
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllDataFromDiskInfo()
        {
            string sqlStr = string.Format("select * from {0};", "disk_info");
            MySqlCommand sqlCom = new MySqlCommand(sqlStr, sqlCon);

            DataTable dt = CommonDataTable.GetDiskInfoDataTable();

            MySqlDataReader sqlDataReader = sqlCom.ExecuteReader();
            GetDataFromSqlDataReader(ref dt, sqlDataReader);
            sqlDataReader.Close();

            return dt;
        }

        /// <summary>
        /// 获取所有的磁盘根目录（pid = -1）
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllRootDirectoryFromFilmInfo()
        {
            string sqlStr = string.Format("select * from {0} where pid = -1;", "film_info");
            MySqlCommand sqlCom = new MySqlCommand(sqlStr, sqlCon);

            DataTable dt = CommonDataTable.GetFilmInfoDataTable();

            MySqlDataReader sqlDataReader = sqlCom.ExecuteReader();
            GetDataFromSqlDataReader(ref dt, sqlDataReader);
            sqlDataReader.Close();

            return dt;
        }

        /// <summary>
        /// 获取指定id的数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataTable GetDataByIdFromFilmInfo(Int32 id)
        {
            string sqlStr = string.Format("select * from {0} where id = @id;", "film_info");

            MySqlCommand sqlCom = new MySqlCommand(sqlStr, sqlCon);
            sqlCom.Parameters.AddWithValue("@id", id);

            DataTable dt = CommonDataTable.GetFilmInfoDataTable();

            MySqlDataReader sqlDataReader = sqlCom.ExecuteReader();
            GetDataFromSqlDataReader(ref dt, sqlDataReader);
            sqlDataReader.Close();

            Debug.Assert(dt.Rows.Count == 1);
            return dt;
        }

        /// <summary>
        /// 计数pid
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public int CountPidFromFilmInfo(Int32 pid)
        {
            string sqlStr = string.Format("select count(id) from {0} where pid = @pid;", "film_info");

            MySqlCommand sqlCom = new MySqlCommand(sqlStr, sqlCon);
            sqlCom.Parameters.AddWithValue("@pid", pid);

            return Convert.ToInt32(sqlCom.ExecuteScalar());
        }

        /// <summary>
        /// 获取指定pid的数据
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="offset">偏移</param>
        /// <param name="rows">个数</param>
        /// <returns></returns>
        public DataTable GetDataByPidFromFilmInfo(Int32 pid, Int32 offset, Int32 rows)
        {
            string sqlStr = string.Format("select * from {0} where pid = @pid limit {1}, {2};",
                "film_info", offset, rows);

            MySqlCommand sqlCom = new MySqlCommand(sqlStr, sqlCon);
            sqlCom.Parameters.AddWithValue("@pid", pid);

            DataTable dt = CommonDataTable.GetFilmInfoDataTable();

            MySqlDataReader sqlDataReader = sqlCom.ExecuteReader();
            GetDataFromSqlDataReader(ref dt, sqlDataReader);
            sqlDataReader.Close();

            return dt;
        }

        /// <summary>
        /// 查找指定id的数据在所有相同pid数据中的偏移
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pid"></param>
        /// <returns></returns>
        public Int32 GetIdOffsetByPidFromFilmInfo(Int32 id, Int32 pid)
        {
            string sqlStr = string.Format("select id from {0} where pid = @pid;", "film_info");
            MySqlCommand sqlCom = new MySqlCommand(sqlStr, sqlCon);
            sqlCom.Parameters.AddWithValue("@pid", pid);

            MySqlDataReader reader = sqlCom.ExecuteReader();
            bool bContain = false;
            int index = 0;
            while (reader.Read())
            {
                if (reader.GetInt32(0) == id)
                {
                    bContain = true;
                    break;
                }
                index++;
            }
            reader.Close();

            if (bContain) return index;
            else return -1;
        }

        /// <summary>
        /// 计数设为待看的数据 
        /// </summary>
        /// <param name="diskDescribe">null时所有磁盘，否则特定磁盘</param>
        /// <returns></returns>
        public int CountWatchDataFromFilmInfo(String diskDescribe = null)
        {
            string sqlStr = string.Format(
                "select count(id) from {0} where to_watch = 1 and disk_desc = @disk_desc;", "film_info");
            if (diskDescribe == null)
                sqlStr = string.Format("select count(id) from {0} where to_watch = 1;", "film_info");

            MySqlCommand sqlCom = new MySqlCommand(sqlStr, sqlCon);
            if (diskDescribe != null) sqlCom.Parameters.AddWithValue("@disk_desc", diskDescribe);

            return Convert.ToInt32(sqlCom.ExecuteScalar());
        }

        /// <summary>
        /// 获取设为待看的数据
        /// </summary>
        /// <param name="offset">偏移</param>
        /// <param name="rows">个数</param>
        /// <param name="diskDescribe">null时所有磁盘，否则特定磁盘</param>
        /// <returns></returns>
        public DataTable GetWatchDataFromFilmInfo(Int32 offset, Int32 rows, String diskDescribe = null)
        {
            string sqlStr = string.Format(
                "select * from {0} where to_watch = 1 and disk_desc = @disk_desc order by s_w_t desc, id asc limit {1}, {2};",
                "film_info", offset, rows);
            if (diskDescribe == null)
                sqlStr = string.Format("select * from {0} where to_watch = 1 order by s_w_t desc, id asc limit {1}, {2};",
                    "film_info", offset, rows);

            MySqlCommand sqlCom = new MySqlCommand(sqlStr, sqlCon);
            if (diskDescribe != null) sqlCom.Parameters.AddWithValue("@disk_desc", diskDescribe);

            DataTable dt = CommonDataTable.GetFilmInfoDataTable();

            MySqlDataReader sqlDataReader = sqlCom.ExecuteReader();
            GetDataFromSqlDataReader(ref dt, sqlDataReader);
            sqlDataReader.Close();

            return dt;
        }

        /// <summary>
        /// 计数设为待删的数据
        /// </summary>
        /// <param name="diskDescribe">null时所有磁盘，否则特定磁盘</param>
        /// <returns></returns>
        public int CountDeleteDataFromFilmInfo(String diskDescribe = null)
        {
            string sqlStr = string.Format(
                "select count(id) from {0} where to_delete = 1 and disk_desc = @disk_desc;", "film_info");
            if (diskDescribe == null)
                sqlStr = string.Format("select count(id) from {0} where to_delete = 1;", "film_info");

            MySqlCommand sqlCom = new MySqlCommand(sqlStr, sqlCon);
            if (diskDescribe != null) sqlCom.Parameters.AddWithValue("@disk_desc", diskDescribe);

            return Convert.ToInt32(sqlCom.ExecuteScalar());
        }

        /// <summary>
        /// 获取设为待删的数据
        /// </summary>
        /// <param name="offset">偏移</param>
        /// <param name="rows">个数</param>
        /// <param name="diskDescribe">null时所有磁盘，否则特定磁盘</param>
        /// <returns></returns>
        public DataTable GetDeleteDataFromFilmInfo(Int32 offset, Int32 rows, String diskDescribe = null)
        {
            string sqlStr = string.Format(
                "select * from {0} where to_delete = 1 and disk_desc = @disk_desc order by s_d_t desc, id asc limit {1}, {2};",
                "film_info", offset, rows);
            if (diskDescribe == null)
                sqlStr = string.Format("select * from {0} where to_delete = 1 order by s_d_t desc, id asc limit {1}, {2};",
                    "film_info", offset, rows);

            MySqlCommand sqlCom = new MySqlCommand(sqlStr, sqlCon);
            if (diskDescribe != null) sqlCom.Parameters.AddWithValue("@disk_desc", diskDescribe);

            DataTable dt = CommonDataTable.GetFilmInfoDataTable();

            MySqlDataReader sqlDataReader = sqlCom.ExecuteReader();
            GetDataFromSqlDataReader(ref dt, sqlDataReader);
            sqlDataReader.Close();

            return dt;
        }

        /// <summary>
        /// 更新待看状态
        /// </summary>
        /// <param name="idList">待更新数据的id列表</param>
        /// <param name="toWatch">设定的待看状态</param>
        /// <returns>影响的行数</returns>
        public int UpdateWatchStateFromFilmInfo(List<Int32> idList, Boolean toWatch)
        {
            string sqlStr = "set sql_safe_updates = 0;";
            sqlStr += String.Format("update {0} set to_watch = @to_watch, s_w_t = @s_w_t where id in ({1});",
                "film_info", String.Join(",", idList));
            MySqlCommand sqlCom = new MySqlCommand(sqlStr, sqlCon);
            sqlCom.Parameters.AddWithValue("@to_watch", toWatch);
            if (toWatch) sqlCom.Parameters.AddWithValue("@s_w_t", DateTime.Now);
            else sqlCom.Parameters.AddWithValue("@s_w_t", System.Data.SqlTypes.SqlDateTime.MinValue.Value);

            return sqlCom.ExecuteNonQuery();
        }

        /// <summary>
        /// 更新待删状态
        /// </summary>
        /// <param name="idList">待更新数据的id列表</param>
        /// <param name="toDelete">设定的待删状态</param>
        /// <returns>影响的行数</returns>
        public int UpdateDeleteStateFromFilmInfo(List<Int32> idList, Boolean toDelete)
        {
            string sqlStr = "set sql_safe_updates = 0;";
            sqlStr += String.Format("update {0} set to_delete = @to_delete, s_d_t = @s_d_t where id in ({1});",
                "film_info", String.Join(",", idList));
            MySqlCommand sqlCom = new MySqlCommand(sqlStr, sqlCon);
            sqlCom.Parameters.AddWithValue("@to_delete", toDelete);
            if (toDelete) sqlCom.Parameters.AddWithValue("@s_d_t", DateTime.Now);
            else sqlCom.Parameters.AddWithValue("@s_d_t", System.Data.SqlTypes.SqlDateTime.MinValue.Value);

            return sqlCom.ExecuteNonQuery();
        }

        /// <summary>
        /// 删除给定id的数据
        /// </summary>
        /// <param name="idList">待删除数据的id列表</param>
        /// <returns>影响的行数</returns>
        public int DeleteFromFilmInfo(List<Int32> idList)
        {
            String sqlStr = String.Format("delete from {0} where id in ({1});",
                "film_info", String.Join(",", idList));
            MySqlCommand sqlCom = new MySqlCommand(sqlStr, sqlCon);

            return sqlCom.ExecuteNonQuery();
        }

        /// <summary>
        /// 从film_info中删除指定磁盘描述的数据
        /// </summary>
        /// <param name="diskDescribe"></param>
        /// <returns></returns>
        public int DeleteByDiskDescribeFromFilmInfo(String diskDescribe)
        {
            string sqlStr = "set sql_safe_updates = 0;";
            sqlStr += String.Format("delete from {0} where disk_desc = @disk_desc;",
                "film_info");
            MySqlCommand sqlCom = new MySqlCommand(sqlStr, sqlCon);
            sqlCom.Parameters.AddWithValue("@disk_desc", diskDescribe);

            return sqlCom.ExecuteNonQuery();
        }

        /// <summary>
        /// 从disk_info中删除指定磁盘描述的数据
        /// </summary>
        /// <param name="diskDescribe"></param>
        /// <returns></returns>
        public int DeleteByDiskDescribeFromDiskInfo(String diskDescribe)
        {
            string sqlStr = "set sql_safe_updates = 0;";
            sqlStr += String.Format("delete from {0} where disk_desc = @disk_desc;",
                "disk_info");
            MySqlCommand sqlCom = new MySqlCommand(sqlStr, sqlCon);
            sqlCom.Parameters.AddWithValue("@disk_desc", diskDescribe);

            return sqlCom.ExecuteNonQuery();
        }

        /// <summary>
        /// 更新film_info磁盘描述
        /// </summary>
        /// <param name="fromDiskDescribe">原</param>
        /// <param name="toDiskDescribe">新</param>
        /// <returns></returns>
        public int UpdateDiskDescribeFromFilmInfo(String fromDiskDescribe, String toDiskDescribe)
        {
            string sqlStr = "set sql_safe_updates = 0;";
            sqlStr += String.Format("update {0} set disk_desc = @t_disk_desc where disk_desc = @f_disk_desc;",
                "film_info");
            MySqlCommand sqlCom = new MySqlCommand(sqlStr, sqlCon);
            sqlCom.Parameters.AddWithValue("@t_disk_desc", toDiskDescribe);
            sqlCom.Parameters.AddWithValue("@f_disk_desc", fromDiskDescribe);

            return sqlCom.ExecuteNonQuery();
        }

        /// <summary>
        /// 更新disk_info磁盘描述
        /// </summary>
        /// <param name="fromDiskDescribe">原</param>
        /// <param name="toDiskDescribe">新</param>
        /// <returns></returns>
        public int UpdateDiskDescribeFromDiskInfo(String fromDiskDescribe, String toDiskDescribe)
        {
            string sqlStr = "set sql_safe_updates = 0;";
            sqlStr += String.Format("update {0} set disk_desc = @t_disk_desc where disk_desc = @f_disk_desc;",
                "disk_info");
            MySqlCommand sqlCom = new MySqlCommand(sqlStr, sqlCon);
            sqlCom.Parameters.AddWithValue("@t_disk_desc", toDiskDescribe);
            sqlCom.Parameters.AddWithValue("@f_disk_desc", fromDiskDescribe);

            return sqlCom.ExecuteNonQuery();
        }

        /// <summary>
        /// 获取文件夹下的文件夹
        /// </summary>
        /// <param name="folderID">文件夹ID</param>
        /// <returns></returns>
        public DataTable GetChildFolderFromFilmInfo(Int32 folderID)
        {
            string sqlStr = string.Format("select * from {0} where pid = @pid and is_folder = 1;",
                "film_info");

            MySqlCommand sqlCom = new MySqlCommand(sqlStr, sqlCon);
            sqlCom.Parameters.AddWithValue("@pid", folderID);

            DataTable dt = CommonDataTable.GetFilmInfoDataTable();

            MySqlDataReader sqlDataReader = sqlCom.ExecuteReader();
            GetDataFromSqlDataReader(ref dt, sqlDataReader);
            sqlDataReader.Close();

            return dt;
        }

        /// <summary>
        /// 获取文件夹下的文件
        /// </summary>
        /// <param name="folderID">文件夹ID</param>
        /// <returns></returns>
        public DataTable GetChildFileFromFilmInfo(Int32 folderID)
        {
            string sqlStr = string.Format("select * from {0} where pid = @pid and is_folder = 0;",
                "film_info");

            MySqlCommand sqlCom = new MySqlCommand(sqlStr, sqlCon);
            sqlCom.Parameters.AddWithValue("@pid", folderID);

            DataTable dt = CommonDataTable.GetFilmInfoDataTable();

            MySqlDataReader sqlDataReader = sqlCom.ExecuteReader();
            GetDataFromSqlDataReader(ref dt, sqlDataReader);
            sqlDataReader.Close();

            return dt;
        }

        /// <summary>
        /// 查询 search_log 表行数
        /// </summary>
        /// <returns></returns>
        public Int32 CountRowsFormSearchLog()
        {
            string sqlStr = string.Format("select count(1) from {0};", "search_log");

            MySqlCommand sqlCom = new MySqlCommand(sqlStr, sqlCon);

            return Convert.ToInt32(sqlCom.ExecuteScalar());
        }
    }
}
