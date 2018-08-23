using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using static MyFilm.CommonDataTable;

namespace MyFilm
{
    public class SqlData
    {
        /// <summary>
        /// 连接MySql
        /// </summary>
        private MySqlConnection sqlConnection = null;

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
        private int setMaxScanLayer = int.MaxValue;

        /// <summary>
        /// 搜索关键字分隔符
        /// </summary>
        private static readonly char[] searchKeyWordSplitter =
            " `~!@#$%^&*()-_=+[]{}|\\;:\'\",./<>?《》（），。？；：’“【】、—￥·".ToCharArray();

        /// <summary>
        /// 打开数据库
        /// </summary>
        public void OpenMySql()
        {
            String cmdText = String.Format(
                "server = {0}; uid = {1}; pwd = {2}; database = {3}; SslMode = none; convert zero datetime=true; allow zero datetime=true;",
                CommonString.DbIP, CommonString.DbUserName, CommonString.DbPassword, CommonString.DbName);
            sqlConnection = new MySqlConnection(cmdText);
            sqlConnection.Open();
        }

        /// <summary>
        /// 关闭数据库连接
        /// </summary>
        public void CloseMySql()
        {
            sqlConnection.Close();
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
        private void CreateFilmInfoTable()
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
            cmdText += String.Format(@"{0} varchar(256) not null, ", "disk_desc");

            cmdText += String.Format(@"index {0}_index({0}), ", "size");
            cmdText += String.Format(@"index {0}_index({0}), ", "is_folder");
            cmdText += String.Format(@"index {0}_index({0}), ", "to_watch");
            cmdText += String.Format(@"index {0}_index({0}), ", "s_w_t");
            cmdText += String.Format(@"index {0}_index({0}), ", "to_delete");
            cmdText += String.Format(@"index {0}_index({0}), ", "s_d_t");
            cmdText += String.Format(@"index {0}_index({0}), ", "pid");
            cmdText += String.Format(@"index {0}_index({0}) );", "disk_desc");

            MySqlCommand sqlCmd = new MySqlCommand(cmdText, sqlConnection);
            sqlCmd.ExecuteNonQuery();
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
            String cmdText = String.Format(@"create table if not exists {0} ( ", "disk_info");
            cmdText += String.Format(@"{0} integer primary key auto_increment, ", "id");
            cmdText += String.Format(@"{0} varchar(256) unique key not null, ", "disk_desc");
            cmdText += String.Format(@"{0} bigint not null, ", "free_space");
            cmdText += String.Format(@"{0} bigint not null, ", "total_size");
            cmdText += String.Format(@"{0} bool not null, ", "complete_scan");
            cmdText += String.Format(@"{0} integer not null );", "scan_layer");

            MySqlCommand sqlCmd = new MySqlCommand(cmdText, sqlConnection);
            sqlCmd.ExecuteNonQuery();
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
            String cmdText = String.Format(@"create table if not exists {0} ( ", "search_log");
            cmdText += String.Format(@"{0} integer primary key auto_increment, ", "id");
            cmdText += String.Format(@"{0} varchar(256) not null, ", "search_key");
            cmdText += String.Format(@"{0} integer not null, ", "result_count");
            cmdText += String.Format(@"{0} datetime not null );", "search_time");

            MySqlCommand slqCom = new MySqlCommand(cmdText, sqlConnection);
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
            String diskPath, String diskDescribe, int setScanLayer = int.MaxValue)
        {
            this.actualMaxScanLayer = 0;
            this.bCompleteScan = true;
            this.setMaxScanLayer = setScanLayer;

            String cmdText = String.Format("select max(id) from {0};", "film_info");
            MySqlCommand sqlCmd = new MySqlCommand(cmdText, sqlConnection);
            object maxIdObj = sqlCmd.ExecuteScalar();
            int maxId = 0;
            if (maxIdObj != DBNull.Value) maxId = Convert.ToInt32(sqlCmd.ExecuteScalar());
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
            dr["to_watch_ex"] = false;
            dr["s_w_t"] = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
            dr["to_delete"] = false;
            dr["to_delete_ex"] = false;
            dr["s_d_t"] = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
            dr["content"] = String.Empty;
            dr["pid"] = -1;
            dr["disk_desc"] = diskDescribe;
            dt.Rows.Add(dr);

            Dictionary<String, int> maxCidDic = new Dictionary<String, int>();
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
            int pid, int setScanLayer, ref DataTable dt,
            ref Dictionary<String, int> maxCidDic)
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
                dr["to_watch_ex"] = false;
                dr["s_w_t"] = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
                dr["to_delete"] = false;
                dr["to_delete_ex"] = false;
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
                dr["to_watch_ex"] = false;
                dr["s_w_t"] = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
                dr["to_delete"] = false;
                dr["to_delete_ex"] = false;
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
        /// 向film_info数据库插入数据，注意dt的列为FillFilmInfoColumn生成
        /// </summary>
        /// <param name="dt">插入的数据</param>
        /// <param name="start">起始，从0开始</param>
        /// <param name="count">个数</param>
        private void InsertDataToFilmInfo(DataTable dt, int start, int count)
        {
            MySqlCommand sqlCmd = new MySqlCommand();
            sqlCmd.Connection = sqlConnection;
            sqlCmd.CommandText = String.Format(
                @"insert into {0} (
                id, name, path, size, create_t, modify_t, is_folder, to_watch, to_watch_ex, s_w_t, 
                to_delete, to_delete_ex, s_d_t, content, pid, max_cid, disk_desc) values",
                "film_info");

            int j = 0;
            for (int i = start; i < dt.Rows.Count && j < count; i++, j++)
            {
                sqlCmd.CommandText += String.Format(
                    @"(@id{0}, @name{0}, @path{0}, @size{0}, @create_t{0}, @modify_t{0}, @is_folder{0}, 
                    @to_watch{0}, @to_watch_ex{0}, @s_w_t{0}, @to_delete{0}, @to_delete_ex{0}, 
                    @s_d_t{0}, @content{0}, @pid{0}, @max_cid{0}, @disk_desc{0}){1}",
                    i, i == dt.Rows.Count - 1 || j == count - 1 ? ";" : ",");

                sqlCmd.Parameters.AddWithValue(String.Format("@id{0}", i), dt.Rows[i]["id"]);
                sqlCmd.Parameters.AddWithValue(String.Format("@name{0}", i), dt.Rows[i]["name"]);
                sqlCmd.Parameters.AddWithValue(String.Format("@path{0}", i), dt.Rows[i]["path"]);
                sqlCmd.Parameters.AddWithValue(String.Format("@size{0}", i), dt.Rows[i]["size"]);
                sqlCmd.Parameters.AddWithValue(String.Format("@create_t{0}", i), dt.Rows[i]["create_t"]);
                sqlCmd.Parameters.AddWithValue(String.Format("@modify_t{0}", i), dt.Rows[i]["modify_t"]);
                sqlCmd.Parameters.AddWithValue(String.Format("@is_folder{0}", i), dt.Rows[i]["is_folder"]);
                sqlCmd.Parameters.AddWithValue(String.Format("@to_watch{0}", i), dt.Rows[i]["to_watch"]);
                sqlCmd.Parameters.AddWithValue(String.Format("@to_watch_ex{0}", i), dt.Rows[i]["to_watch_ex"]);
                sqlCmd.Parameters.AddWithValue(String.Format("@s_w_t{0}", i), dt.Rows[i]["s_w_t"]);
                sqlCmd.Parameters.AddWithValue(String.Format("@to_delete{0}", i), dt.Rows[i]["to_delete"]);
                sqlCmd.Parameters.AddWithValue(String.Format("@to_delete_ex{0}", i), dt.Rows[i]["to_delete_ex"]);
                sqlCmd.Parameters.AddWithValue(String.Format("@s_d_t{0}", i), dt.Rows[i]["s_d_t"]);
                sqlCmd.Parameters.AddWithValue(String.Format("@content{0}", i), dt.Rows[i]["content"]);
                sqlCmd.Parameters.AddWithValue(String.Format("@pid{0}", i), dt.Rows[i]["pid"]);
                sqlCmd.Parameters.AddWithValue(String.Format("@max_cid{0}", i), dt.Rows[i]["max_cid"]);
                sqlCmd.Parameters.AddWithValue(String.Format("@disk_desc{0}", i), dt.Rows[i]["disk_desc"]);
            }

            int affectedRows = sqlCmd.ExecuteNonQuery();
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
            Boolean completeScan, int scanLayer)
        {
            MySqlCommand sqlCmd = new MySqlCommand();
            sqlCmd.Connection = sqlConnection;
            sqlCmd.CommandText = String.Format(
                @"insert into {0} (disk_desc, free_space, total_size, complete_scan, scan_layer) values(
                @disk_desc, @free_space, @total_size, @complete_scan, @scan_layer) 
                on duplicate key update free_space = values(free_space),
                total_size = values(total_size), complete_scan = values(complete_scan),
                scan_layer = values(scan_layer);", "disk_info");
            sqlCmd.Parameters.AddWithValue("@disk_desc", diskDescribe);
            sqlCmd.Parameters.AddWithValue("@free_space", freeSpace);
            sqlCmd.Parameters.AddWithValue("@total_size", totalSize);
            sqlCmd.Parameters.AddWithValue("@complete_scan", completeScan);
            sqlCmd.Parameters.AddWithValue("@scan_layer", scanLayer);

            int affectedRows = sqlCmd.ExecuteNonQuery();
            Debug.Assert(affectedRows == 1 || affectedRows == 2);
        }

        /// <summary>
        /// 向search_log数据库插入数据
        /// </summary>
        /// <param name="searchKey">搜索关键字</param>
        /// <param name="resultCount">搜索结果条数</param>
        /// <param name="searchTime">搜索时间</param>
        public void InsertDataToSearchLog(String searchKey, int resultCount, DateTime searchTime)
        {
            MySqlCommand sqlCmd = new MySqlCommand();
            sqlCmd.Connection = sqlConnection;
            sqlCmd.CommandText = String.Format(
                @"insert into {0} (search_key, result_count, search_time) values
                (@search_key, @result_count, @search_time);", "search_log");
            sqlCmd.Parameters.AddWithValue("@search_key", searchKey);
            sqlCmd.Parameters.AddWithValue("@result_count", resultCount);
            sqlCmd.Parameters.AddWithValue("@search_time", searchTime);

            int affectedRows = sqlCmd.ExecuteNonQuery();
            Debug.Assert(affectedRows == 1);
        }

        /// <summary>
        /// 执行 sqlCmd.ExecuteReader 并获取所有数据
        /// </summary>
        /// <param name="sqlCom"></param>
        /// <param name="errMsg">错误信息</param>
        /// <returns>出错返回 null</returns>
        private DataTable SqlComExecuteReaderGetAllData(MySqlCommand sqlCmd, ref String errMsg)
        {
            try
            {
                using (MySqlDataReader sqlDataReader = sqlCmd.ExecuteReader())
                {
                    DataTable dt = new DataTable();
                    for (int i = 0; i < sqlDataReader.FieldCount; i++)
                    {
                        dt.Columns.Add(sqlDataReader.GetName(i),
                            sqlDataReader.GetFieldType(i) == typeof(MySql.Data.Types.MySqlDateTime) ?
                            typeof(DateTime) : sqlDataReader.GetFieldType(i));
                    }

                    while (sqlDataReader.Read())
                    {
                        DataRow dr = dt.NewRow();
                        for (int i = 0; i < dt.Columns.Count; i++) dr[i] = sqlDataReader[i];
                        dt.Rows.Add(dr);
                    }
                    return dt;
                }
            }
            catch (Exception e)
            {
                errMsg = e.Message;
                return null;
            }
        }

        /// <summary>
        /// 执行 sqlCmd.ExecuteReader（只返回id列） 并获取 ID
        /// </summary>
        /// <param name="sqlCom"></param>
        /// <param name="errMsg">错误信息</param>
        /// <returns>出错返回 null</returns>
        private int[] SqlComExecuteReaderGetID(MySqlCommand sqlCmd, ref String errMsg)
        {
            try
            {
                using (MySqlDataReader sqlDataReader = sqlCmd.ExecuteReader())
                {
                    List<int> idList = new List<int>();
                    while (sqlDataReader.Read()) idList.Add(Convert.ToInt32(sqlDataReader[0]));
                    return idList.ToArray();
                }
            }
            catch (Exception e)
            {
                errMsg = e.Message;
                return null;
            }
        }

        /// <summary>
        /// 在 film_info 中根据指定的 id列表 获取数据
        /// </summary>
        /// <param name="idList">id列表</param>
        /// <returns></returns>
        public DataTable SelectDataByIDList(int[] idList)
        {
            if (idList == null || idList.Length == 0) return null;

            String cmdText = String.Format(
                "select * from {0} where id in ({1}) order by field(id, {1});",
                "film_info", String.Join(",", idList));

            MySqlCommand sqlCmd = new MySqlCommand(cmdText, sqlConnection);
            String errMsg = String.Empty;

            return SqlComExecuteReaderGetAllData(sqlCmd, ref errMsg);
        }

        /// <summary>
        /// 搜索 film_info 表
        /// </summary>
        /// <param name="keyWord">搜索关键字</param>
        /// <param name="diskDescribe">null时所有磁盘，否则特定磁盘</param>
        /// <returns></returns>
        public int[] SearchKeyWordFromFilmInfo(String keyWord, String diskDescribe = null)
        {
            String[] keyWords = keyWord.Split(searchKeyWordSplitter,
                StringSplitOptions.RemoveEmptyEntries);
            if (keyWords.Length == 0) return null;

            String strTemp = String.Empty;
            for (int i = 0; i < keyWords.Length; i++)
            {
                strTemp += String.Format(" locate(@{0}, name)>0 {1}",
                    i, i == keyWords.Length - 1 ? "" : "and");
            }
            String cmdText = String.Format(
                "select id from {0} where {1} {2} order by id;", "film_info",
                strTemp, diskDescribe == null ? "" : "and disk_desc = @disk_desc");

            MySqlCommand sqlCmd = new MySqlCommand(cmdText, sqlConnection);

            for (int i = 0; i < keyWords.Length; i++)
                sqlCmd.Parameters.AddWithValue(String.Format("@{0}", i), keyWords[i]);
            if (diskDescribe != null) sqlCmd.Parameters.AddWithValue("@disk_desc", diskDescribe);

            String errMsg = String.Empty;

            return SqlComExecuteReaderGetID(sqlCmd, ref errMsg);
        }

        /// <summary>
        /// 查看film_info表结构的详细信息
        /// </summary>
        /// <returns></returns>
        public DataTable GetDescriptionOfFilmInfo()
        {
            String cmdText = String.Format("desc {0}", "film_info");
            MySqlCommand sqlCmd = new MySqlCommand(cmdText, sqlConnection);

            String errMsg = String.Empty;
            return SqlComExecuteReaderGetAllData(sqlCmd, ref errMsg);
        }

        /// <summary>
        /// 根据 sql 语句生成 MySqlCommand 对象
        /// </summary>
        /// <param name="cmdText">sql 语句</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns>sql 语句解析出错时返回 null</returns>
        private MySqlCommand DealWithSqlQueryText(String cmdText, ref String errMsg)
        {
            SortedDictionary<int, char> dict1 = new SortedDictionary<int, char>();
            int a1 = cmdText.IndexOf('\'', 0);
            while (a1 != -1)
            {
                dict1.Add(a1, '\'');
                a1 = cmdText.IndexOf('\'', a1 + 1);
            }
            int a2 = cmdText.IndexOf('"', 0);
            while (a2 != -1)
            {
                dict1.Add(a2, '"');
                a2 = cmdText.IndexOf('"', a2 + 1);
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
            if (tempChar != ' ') { errMsg = "error sql statement"; return null; }

            int index = 0;
            int n = 0;
            String cmdTextActual = String.Empty;
            foreach (KeyValuePair<int, int> kv in dict2)
            {
                cmdTextActual += cmdText.Substring(index, kv.Key - index);
                cmdTextActual += String.Format("@{0}", n++);
                index = kv.Value + 1;
            }
            cmdTextActual += cmdText.Substring(index);

            MySqlCommand sqlCmd = new MySqlCommand(cmdTextActual, sqlConnection);
            n = 0;
            foreach (KeyValuePair<int, int> kv in dict2)
            {
                sqlCmd.Parameters.AddWithValue(String.Format("@{0}", n++),
                    cmdText.Substring(kv.Key + 1, kv.Value - kv.Key - 1));
            }

            return sqlCmd;
        }

        /// <summary>
        /// 用 sql 语句查询所有列数据
        /// </summary>
        /// <param name="cmdText">sql 语句</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns>sql 语句解析出错时返回 null</returns>
        public DataTable SelectAllDataBySqlText(String cmdText, ref String errMsg)
        {
            MySqlCommand sqlCmd = DealWithSqlQueryText(cmdText, ref errMsg);

            if (sqlCmd == null) return null;
            else return SqlComExecuteReaderGetAllData(sqlCmd, ref errMsg);
        }

        /// <summary>
        /// 用 sql 查询 id 列（语句必须以 SELECT * FROM 开头）
        /// </summary>
        /// <param name="sqlStr">sql 语句</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns>sql 语句解析出错时返回 null</returns>
        public int[] SelectIDBySqlText(String cmdText, ref String errMsg)
        {
            String prefix = "SELECT * FROM";
            if (!cmdText.StartsWith(prefix))
            {
                errMsg = String.Format("语句必须以 {0} 开头", prefix);
                return null;
            }

            MySqlCommand sqlCmd = DealWithSqlQueryText(
                "SELECT id FROM" + cmdText.Substring(prefix.Length), ref errMsg);

            if (sqlCmd == null) return null;
            else return SqlComExecuteReaderGetID(sqlCmd, ref errMsg);
        }

        /// <summary>
        /// 获取disk_info数据库所有数据
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllDataFromDiskInfo()
        {
            String cmdText = String.Format("select * from {0};", "disk_info");
            MySqlCommand sqlCmd = new MySqlCommand(cmdText, sqlConnection);
            String errMsg = String.Empty;

            return SqlComExecuteReaderGetAllData(sqlCmd, ref errMsg);
        }

        /// <summary>
        /// 获取所有的磁盘根目录（pid = -1）
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllRootDirectoryFromFilmInfo()
        {
            String cmdText = String.Format("select * from {0} where pid = -1;", "film_info");
            MySqlCommand sqlCmd = new MySqlCommand(cmdText, sqlConnection);
            String errMsg = String.Empty;

            return SqlComExecuteReaderGetAllData(sqlCmd, ref errMsg);
        }

        /// <summary>
        /// 获取指定id的数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataTable GetDataByIdFromFilmInfo(int id)
        {
            String cmdText = String.Format("select * from {0} where id = @id;", "film_info");

            MySqlCommand sqlCmd = new MySqlCommand(cmdText, sqlConnection);
            sqlCmd.Parameters.AddWithValue("@id", id);
            String errMsg = String.Empty;

            return SqlComExecuteReaderGetAllData(sqlCmd, ref errMsg);
        }

        /// <summary>
        /// 获取指定pid的数据
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public int[] GetDataByPidFromFilmInfo(int pid)
        {
            String cmdText = String.Format("select id from {0} where pid = @pid order by id;",
                "film_info");

            MySqlCommand sqlCmd = new MySqlCommand(cmdText, sqlConnection);
            sqlCmd.Parameters.AddWithValue("@pid", pid);
            String errMsg = String.Empty;

            return SqlComExecuteReaderGetID(sqlCmd, ref errMsg);
        }

        /// <summary>
        /// 获取设为待看的数据
        /// </summary>
        /// <param name="diskDescribe">null时所有磁盘，否则特定磁盘</param>
        /// <returns></returns>
        public int[] GetWatchDataFromFilmInfo(String diskDescribe = null)
        {
            String cmdText = String.Format(
                "select id from {0} where to_watch = 1 {1} order by s_w_t desc, id asc;",
                "film_info", diskDescribe == null ? "" : "and disk_desc = @disk_desc");
            String errMsg = String.Empty;

            MySqlCommand sqlCmd = new MySqlCommand(cmdText, sqlConnection);
            if (diskDescribe != null) sqlCmd.Parameters.AddWithValue("@disk_desc", diskDescribe);

            return SqlComExecuteReaderGetID(sqlCmd, ref errMsg);
        }

        /// <summary>
        /// 获取设为待删的数据
        /// </summary>
        /// <param name="diskDescribe">null时所有磁盘，否则特定磁盘</param>
        /// <returns></returns>
        public int[] GetDeleteDataFromFilmInfo(String diskDescribe = null)
        {
            String cmdText = String.Format(
                "select id from {0} where to_delete = 1 {1} order by s_d_t desc, id asc;",
                "film_info", diskDescribe == null ? "" : "and disk_desc = @disk_desc");
            String errMsg = String.Empty;

            MySqlCommand sqlCmd = new MySqlCommand(cmdText, sqlConnection);
            if (diskDescribe != null) sqlCmd.Parameters.AddWithValue("@disk_desc", diskDescribe);

            return SqlComExecuteReaderGetID(sqlCmd, ref errMsg);
        }

        public int[] GetDeleteDataFromFilmInfoGroupByDisk(String diskDescribe = null)
        {
            String cmdText = String.Format(
                @"select group_concat(id order by id), count(id) as id_count from {0} where 
                to_delete = 1 {1} group by disk_desc order by id_count desc;",
                "film_info", diskDescribe == null ? "" : "and disk_desc = @disk_desc");
            String errMsg = String.Empty;

            MySqlCommand sqlCmd = new MySqlCommand(cmdText, sqlConnection);
            if (diskDescribe != null) sqlCmd.Parameters.AddWithValue("@disk_desc", diskDescribe);

            try
            {
                using (MySqlDataReader sqlDataReader = sqlCmd.ExecuteReader())
                {
                    String strTemp = String.Empty;
                    while (sqlDataReader.Read()) strTemp += ("," + sqlDataReader[0].ToString());
                    List<String> strIdList = strTemp.Split(
                        new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    return strIdList.ConvertAll(x => Convert.ToInt32(x)).ToArray();
                }
            }
            catch (Exception e)
            {
                errMsg = e.Message;
                return null;
            }
        }

        public void UpdateWatchOrDeleteStateFromFilmInfo(bool isWatch,
            List<SetStateStruct> setStateStructList, DateTime setTime, bool setTo)
        {
            if (setStateStructList == null || setStateStructList.Count == 0) return;
            // 用 pid 排序（最顶层的文件夹在前面）
            setStateStructList.Sort((x, y) => x.pid.CompareTo(y.pid));

            MySqlCommand sqlCmd = new MySqlCommand();

            int pi = 0;
            string k1 = isWatch ? "to_watch" : "to_delete";
            string k2 = isWatch ? "to_watch_ex" : "to_delete_ex";
            string k3 = isWatch ? "s_w_t" : "s_d_t";

            List<string> c1 = new List<string>();
            List<string> c2 = new List<string>();
            List<string> c3 = new List<string>();
            List<string> c4 = new List<string>();
            List<string> s1 = new List<string>();

            // 记录已被处理过的
            List<int> dealedIDList = new List<int>();

            if (setTo)
            {
                for (int i = 0; i < setStateStructList.Count; i++)
                {
                    if ((isWatch ? setStateStructList[i].to_watch_ex : setStateStructList[i].to_delete_ex) ||
                        dealedIDList.Contains(setStateStructList[i].id))
                        continue;

                    // 树形结构，只向下走，不管上面
                    c1.Add(String.Format("(id={0})", setStateStructList[i].id));
                    c3.Add(String.Format("(id>={0} and id<={1})",
                        setStateStructList[i].id, setStateStructList[i].max_cid));
                    s1.Add(String.Format("when id>={0} and id<={1} then @{2}",
                        setStateStructList[i].id, setStateStructList[i].max_cid, pi));

                    if (setStateStructList[i].is_folder)
                    {
                        c2.Add(String.Format("(id>{0} and id<={1})",
                            setStateStructList[i].id, setStateStructList[i].max_cid));

                        for (int j = i + 1; j < setStateStructList.Count; j++)
                        {
                            // 如果在当前文件夹中
                            if (setStateStructList[j].id > setStateStructList[i].id &&
                                setStateStructList[j].id <= setStateStructList[i].max_cid)
                            {
                                dealedIDList.Add(setStateStructList[j].id);
                            }
                        }
                    }

                    sqlCmd.Parameters.AddWithValue(String.Format("@{0}", pi++), setTime);
                }
            }
            else
            {
                List<TreeSetState> nodeTreeSetStateList = new List<TreeSetState>();

                for (int i = 0; i < setStateStructList.Count; i++)
                {
                    if ((!(isWatch ? setStateStructList[i].to_watch_ex : setStateStructList[i].to_delete_ex)) ||
                        dealedIDList.Contains(setStateStructList[i].id))
                        continue;

                    int id = setStateStructList[i].id;
                    int pid = setStateStructList[i].pid;
                    DateTime setTimeWithPositive = setTime;

                    // 记录文件信息
                    TreeSetState nodeSetState = new TreeSetState();
                    nodeSetState.name = setStateStructList[i].name;
                    nodeSetState.id = id;
                    nodeSetState.max_cid = setStateStructList[i].max_cid;

                    // 向上 查询父文件夹信息
                    while (pid != -1)
                    {
                        DataTable pdt = GetDataByIdFromFilmInfo(pid);
                        // 如果父文件夹是待看待删
                        if (pdt != null && pdt.Rows.Count == 1 &&
                            Convert.ToBoolean(pdt.Rows[0][k2]))
                        {
                            id = Convert.ToInt32(pdt.Rows[0]["id"]);
                            pid = Convert.ToInt32(pdt.Rows[0]["pid"]);
                            setTimeWithPositive = Convert.ToDateTime(pdt.Rows[0][k3]);

                            TreeSetState upNodeSetState = new TreeSetState();
                            upNodeSetState.name = Convert.ToString(pdt.Rows[0]["name"]);
                            upNodeSetState.id = id;
                            upNodeSetState.max_cid = Convert.ToInt32(pdt.Rows[0]["max_cid"]);
                            upNodeSetState.cancelIDList = new List<TreeSetState>();
                            upNodeSetState.Add(nodeSetState);

                            nodeSetState = upNodeSetState;
                        }
                        else break;
                    }

                    if (setTimeWithPositive != setTime)
                    {
                        sqlCmd.Parameters.AddWithValue(String.Format("@{0}", pi++),
                            setTimeWithPositive);
                    }

                    for (int j = i + 1; j < setStateStructList.Count; j++)
                    {
                        // 如果在当前文件夹中
                        if (setStateStructList[j].id > nodeSetState.id &&
                            setStateStructList[j].id <= nodeSetState.max_cid)
                        {
                            dealedIDList.Add(setStateStructList[j].id);

                            int _id = setStateStructList[j].id;
                            int _pid = setStateStructList[j].pid;

                            TreeSetState _nodeSetState = new TreeSetState();
                            _nodeSetState.name = setStateStructList[j].name;
                            _nodeSetState.id = _id;
                            _nodeSetState.max_cid = setStateStructList[j].max_cid;

                            while (_pid != nodeSetState.id)
                            {
                                DataTable _pdt = GetDataByIdFromFilmInfo(_pid);
                                Debug.Assert(_pdt != null && _pdt.Rows.Count == 1);

                                _id = Convert.ToInt32(_pdt.Rows[0]["id"]);
                                _pid = Convert.ToInt32(_pdt.Rows[0]["pid"]);

                                TreeSetState _upNodeSetState = new TreeSetState();
                                _upNodeSetState.name = Convert.ToString(_pdt.Rows[0]["name"]);
                                _upNodeSetState.id = _id;
                                _upNodeSetState.max_cid = Convert.ToInt32(_pdt.Rows[0]["max_cid"]);
                                _upNodeSetState.cancelIDList = new List<TreeSetState>();
                                _upNodeSetState.Add(_nodeSetState);

                                _nodeSetState = _upNodeSetState;
                            }

                            nodeSetState.Add(_nodeSetState);
                        }
                    }

                    nodeTreeSetStateList.Add(nodeSetState);
                }

                sqlCmd.Parameters.AddWithValue("@cancel", setTime);

                pi = 0;
                foreach (TreeSetState nodeSetState in nodeTreeSetStateList)
                {
                    List<string> tc3 = new List<string>();

                    GenerateConditionString(nodeSetState, ref c1, ref c2, ref tc3, ref c4);

                    if (tc3.Count > 0)
                        s1.Add(String.Format("when {0} then @{1}", string.Join(" or ", tc3), pi++));

                    c3.AddRange(tc3);
                }
                s1.Add(String.Format("when {0} then @cancel", string.Join(" or ", c4)));
            }

            if (c1.Count > 0 || c2.Count > 0)
            {
                string str1 = c1.Count == 0 ? " " :
                    string.Format(" when {0} then 1 ", string.Join(" or ", c1));
                string str2 = c2.Count == 0 ? " " :
                    string.Format(" when {0} then 0 ", string.Join(" or ", c2));
                string str3 = c3.Count == 0 ? " " :
                    string.Format(" when {0} then 1 ", string.Join(" or ", c3));
                string str4 = c4.Count == 0 ? " " :
                    string.Format(" when {0} then 0 ", string.Join(" or ", c4));

                sqlCmd.Connection = sqlConnection;
                sqlCmd.CommandText = String.Format(@"update {0} set 
                    {6} = (case {1} {2} else {6} end),
                    {7} = (case {3} {4} else {7} end),
                    {8} = (case {5} else {8} end);",
                    "film_info", str1, str2, str3, str4, string.Join(" ", s1), k1, k2, k3);

                sqlCmd.ExecuteNonQuery();
            }
        }

        private void GenerateConditionString(TreeSetState nodeSetState, ref List<string> c1,
            ref List<string> c2, ref List<string> c3, ref List<string> c4)
        {
            if (nodeSetState == null) return;

            if (nodeSetState.cancelIDList == null)
            {
                c2.Add(String.Format("(pid={0} or id={0})", nodeSetState.id));
                c4.Add(String.Format("(id>={0} and id<={1})", nodeSetState.id, nodeSetState.max_cid));
            }
            else
            {
                // 不可能为空
                Debug.Assert(nodeSetState.cancelIDList.Count > 0);

                List<int> idList = nodeSetState.cancelIDList.Select(x => x.id).ToList();
                List<int> maxcidList = nodeSetState.cancelIDList.Select(x => x.max_cid).ToList();
                idList.Sort();
                maxcidList.Sort();

                string strC3Range = GenerateRangeString(nodeSetState.id,
                    nodeSetState.max_cid, idList, maxcidList);

                string strTemp = String.Join(",", idList);

                c1.Add(String.Format("(pid={0} and id not in ({1}))", nodeSetState.id, strTemp));
                c2.Add(String.Format("(id={0})", nodeSetState.id));
                if (!string.IsNullOrEmpty(strC3Range)) c3.Add(String.Format("({0})", strC3Range));
                c4.Add(String.Format("(id={0})", nodeSetState.id));

                foreach (TreeSetState _nodeSetState in nodeSetState.cancelIDList)
                    GenerateConditionString(_nodeSetState, ref c1, ref c2, ref c3, ref c4);
            }
        }

        private string GenerateRangeString(int id, int max_cid, List<int> idList, List<int> maxcidList)
        {
            Debug.Assert(idList != null && maxcidList != null && idList.Count == maxcidList.Count);
            Debug.Assert(id <= idList[0] && max_cid >= maxcidList[maxcidList.Count - 1]);

            List<string> strList = new List<string>();

            int pos = id;
            for (int i = 0; i < idList.Count; i++)
            {
                if (idList[i] > pos) strList.Add(string.Format("(id>{0} and id<{1})", pos, idList[i]));
                pos = maxcidList[i];
            }
            if (pos < max_cid) strList.Add(string.Format("(id>{0} and id<{1})", pos, max_cid));

            return string.Join(" or ", strList);
        }

        /// <summary>
        /// 删除给定id的数据
        /// </summary>
        /// <param name="idList">待删除数据的id列表</param>
        /// <returns>影响的行数</returns>
        public int DeleteFromFilmInfo(List<int> idList)
        {
            String cmdText = String.Format("delete from {0} where id in ({1});",
                "film_info", String.Join(",", idList));
            MySqlCommand sqlCmd = new MySqlCommand(cmdText, sqlConnection);

            return sqlCmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 从film_info中删除指定磁盘描述的数据
        /// </summary>
        /// <param name="diskDescribe"></param>
        /// <returns></returns>
        public int DeleteByDiskDescribeFromFilmInfo(String diskDescribe)
        {
            String cmdText = "set sql_safe_updates = 0;";
            cmdText += String.Format("delete from {0} where disk_desc = @disk_desc;",
                "film_info");
            MySqlCommand sqlCmd = new MySqlCommand(cmdText, sqlConnection);
            sqlCmd.Parameters.AddWithValue("@disk_desc", diskDescribe);

            return sqlCmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 从disk_info中删除指定磁盘描述的数据
        /// </summary>
        /// <param name="diskDescribe"></param>
        /// <returns></returns>
        public int DeleteByDiskDescribeFromDiskInfo(String diskDescribe)
        {
            String cmdText = "set sql_safe_updates = 0;";
            cmdText += String.Format("delete from {0} where disk_desc = @disk_desc;",
                "disk_info");
            MySqlCommand sqlCmd = new MySqlCommand(cmdText, sqlConnection);
            sqlCmd.Parameters.AddWithValue("@disk_desc", diskDescribe);

            return sqlCmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 更新film_info磁盘描述
        /// </summary>
        /// <param name="fromDiskDescribe">原</param>
        /// <param name="toDiskDescribe">新</param>
        /// <returns></returns>
        public int UpdateDiskDescribeFromFilmInfo(String fromDiskDescribe, String toDiskDescribe)
        {
            String cmdText = "set sql_safe_updates = 0;";
            cmdText += String.Format("update {0} set disk_desc = @t_disk_desc where disk_desc = @f_disk_desc;",
                "film_info");
            MySqlCommand sqlCmd = new MySqlCommand(cmdText, sqlConnection);
            sqlCmd.Parameters.AddWithValue("@t_disk_desc", toDiskDescribe);
            sqlCmd.Parameters.AddWithValue("@f_disk_desc", fromDiskDescribe);

            return sqlCmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 更新disk_info磁盘描述
        /// </summary>
        /// <param name="fromDiskDescribe">原</param>
        /// <param name="toDiskDescribe">新</param>
        /// <returns></returns>
        public int UpdateDiskDescribeFromDiskInfo(String fromDiskDescribe, String toDiskDescribe)
        {
            String cmdText = "set sql_safe_updates = 0;";
            cmdText += String.Format("update {0} set disk_desc = @t_disk_desc where disk_desc = @f_disk_desc;",
                "disk_info");
            MySqlCommand sqlCmd = new MySqlCommand(cmdText, sqlConnection);
            sqlCmd.Parameters.AddWithValue("@t_disk_desc", toDiskDescribe);
            sqlCmd.Parameters.AddWithValue("@f_disk_desc", fromDiskDescribe);

            return sqlCmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 获取文件夹下的文件夹
        /// </summary>
        /// <param name="folderID">文件夹ID</param>
        /// <returns></returns>
        public DataTable GetChildFolderFromFilmInfo(int folderID)
        {
            String cmdText = String.Format("select * from {0} where pid = @pid and is_folder = 1;",
                "film_info");
            String errMsg = String.Empty;

            MySqlCommand sqlCmd = new MySqlCommand(cmdText, sqlConnection);
            sqlCmd.Parameters.AddWithValue("@pid", folderID);

            return SqlComExecuteReaderGetAllData(sqlCmd, ref errMsg);
        }

        /// <summary>
        /// 获取文件夹下的文件
        /// </summary>
        /// <param name="folderID">文件夹ID</param>
        /// <returns></returns>
        public DataTable GetChildFileFromFilmInfo(int folderID)
        {
            String cmdText = String.Format("select * from {0} where pid = @pid and is_folder = 0;",
                "film_info");
            String errMsg = String.Empty;

            MySqlCommand sqlCmd = new MySqlCommand(cmdText, sqlConnection);
            sqlCmd.Parameters.AddWithValue("@pid", folderID);

            return SqlComExecuteReaderGetAllData(sqlCmd, ref errMsg);
        }

        /// <summary>
        /// 查询 search_log 表行数
        /// </summary>
        /// <returns></returns>
        public int CountRowsFormSearchLog()
        {
            String cmdText = String.Format("select count(*) from {0};", "search_log");

            MySqlCommand sqlCmd = new MySqlCommand(cmdText, sqlConnection);

            return Convert.ToInt32(sqlCmd.ExecuteScalar());
        }
    }
}
