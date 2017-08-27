using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;

namespace MyFilm
{
    public class SqlData
    {
        /// <summary>
        /// 连接MySql
        /// </summary>
        private MySqlConnection sqlCon = null;

        /// <summary>
        /// 打开数据库
        /// </summary>
        public void OpenMySql()
        {
            String sqlText = String.Format(
                "server = {0}; uid = {1}; pwd = {2}; database = {3};",
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
        /// +-----------+---------------+------+-----+---------+----------------+
        /// | Field     | Type          | Null | Key | Default | Extra          |
        /// +-----------+---------------+------+-----+---------+----------------+
        /// | id        | int(11)       | NO   | PRI | NULL    | auto_increment |
        /// | name      | varchar(256)  | NO   |     | NULL    |                |
        /// | path      | varchar(1024) | NO   |     | NULL    |                |
        /// | size      | bigint(20)    | NO   |     | NULL    |                |
        /// | create_t  | datetime      | NO   |     | NULL    |                |
        /// | modify_t  | datetime      | NO   |     | NULL    |                |
        /// | is_folder | tinyint(1)    | NO   |     | NULL    |                |
        /// | pid       | int(11)       | NO   |     | NULL    |                |
        /// | disk_desc | varchar(256)  | NO   |     | NULL    |                |
        /// | to_watch  | tinyint(1)    | NO   |     | NULL    |                |
        /// | to_delete | tinyint(1)    | NO   |     | NULL    |                |
        /// | content   | text          | NO   |     | NULL    |                |
        /// +-----------+---------------+------+-----+---------+----------------+
        /// </summary>
        private void CreateFilmInfoTable()
        {
            String sqlText = String.Format(@"create table if not exists {0} ( ", "film_info");
            sqlText += String.Format(@"{0} integer primary key auto_increment, ", "id");
            sqlText += String.Format(@"{0} varchar(256) not null, ", "name");
            sqlText += String.Format(@"{0} varchar(1024) not null, ", "path");
            sqlText += String.Format(@"{0} bigint not null, ", "size");
            sqlText += String.Format(@"{0} datetime not null, ", "create_t");
            sqlText += String.Format(@"{0} datetime not null, ", "modify_t");
            sqlText += String.Format(@"{0} bool not null, ", "is_folder");
            sqlText += String.Format(@"{0} integer not null, ", "pid");
            sqlText += String.Format(@"{0} varchar(256) not null, ", "disk_desc");
            sqlText += String.Format(@"{0} bool not null, ", "to_watch");
            sqlText += String.Format(@"{0} bool not null, ", "to_delete");
            sqlText += String.Format(@"{0} text not null );", "content");

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
        /// | max_layer     | int(11)      | NO   |     | NULL    |                |
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
            sqlText += String.Format(@"{0} integer not null );", "max_layer");

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
        /// <param name="brifeScan">简略扫描</param>
        /// <param name="maxLayer">最多扫描层数，brifeScan为true时起作用</param>
        public void ScanDisk(
            String diskPath, String diskDescribe, Boolean brifeScan, Int32 maxLayer = Int32.MaxValue)
        {
            DriveInfo driveInfo = new DriveInfo(diskPath);

            if (!brifeScan) maxLayer = Int32.MaxValue;
            // 更新磁盘信息
            InsertOrUpdateDataToDiskInfo(
                diskDescribe, driveInfo.TotalFreeSpace, driveInfo.TotalSize, brifeScan, maxLayer);

            DataTable dt = CommonDataTable.GetFilmInfoDataTable();

            DataRow dr = dt.NewRow();
            dr[1] = driveInfo.RootDirectory.Name;
            dr[2] = driveInfo.RootDirectory.FullName;
            dr[3] = -1;
            dr[4] = driveInfo.RootDirectory.CreationTime;
            dr[5] = driveInfo.RootDirectory.LastWriteTime;
            dr[6] = true;
            dr[7] = -1;
            dr[8] = diskDescribe;
            dr[9] = false;
            dr[10] = false;
            dr[11] = String.Empty;
            dt.Rows.Add(dr);

            InsertDataToFilmInfo(dt);

            // 获取新插入的文件夹的id，该文件夹下的子文件夹或文件的pid即为此值
            //Int32 pid = SearchFilmInfoIdByPathAndDiskDescribe(driveInfo.RootDirectory.FullName, diskDescribe);
            Int32 pid = GetLastInsertID();
            ScanAllInFolder(driveInfo.RootDirectory, diskDescribe, pid, maxLayer);

            if (!brifeScan) UpdateFolderSizeFromFilmInfo(diskDescribe);
        }

        /// <summary>
        /// 扫描文件夹内容（不包含此文件夹）
        /// </summary>
        /// <param name="directoryInfo">此文件夹信息</param>
        /// <param name="diskDescribe">磁盘描述</param>
        /// <param name="pid">此文件夹数据库id</param>
        /// <param name="maxLayer">最多扫描层数</param>
        private void ScanAllInFolder(
            DirectoryInfo directoryInfo, String diskDescribe, Int32 pid, Int32 maxLayer)
        {
            if (maxLayer <= 0) return;

            foreach (DirectoryInfo childDirectoryInfo in directoryInfo.GetDirectories())
            {
                if ((childDirectoryInfo.Attributes & FileAttributes.System) == FileAttributes.System) continue;

                DataTable dt = CommonDataTable.GetFilmInfoDataTable();

                DataRow dr = dt.NewRow();
                dr[1] = childDirectoryInfo.Name;
                dr[2] = childDirectoryInfo.FullName;
                dr[3] = -1;
                dr[4] = childDirectoryInfo.CreationTime;
                dr[5] = childDirectoryInfo.LastWriteTime;
                dr[6] = true;
                dr[7] = pid;
                dr[8] = diskDescribe;
                dr[9] = false;
                dr[10] = false;
                dr[11] = String.Empty;
                dt.Rows.Add(dr);

                InsertDataToFilmInfo(dt);

                //Int32 cpid = SearchFilmInfoIdByPathAndDiskDescribe(childDirectoryInfo.FullName, diskDescribe);
                Int32 cpid = GetLastInsertID();
                ScanAllInFolder(childDirectoryInfo, diskDescribe, cpid, maxLayer - 1);
            }

            FileInfo[] fileInfoArray = directoryInfo.GetFiles();
            if (fileInfoArray.Length > 0)
            {
                DataTable dt = CommonDataTable.GetFilmInfoDataTable();

                foreach (FileInfo fileInfo in fileInfoArray)
                {
                    DataRow dr = dt.NewRow();
                    dr[1] = fileInfo.Name;
                    dr[2] = fileInfo.FullName;
                    dr[3] = fileInfo.Length;
                    dr[4] = fileInfo.CreationTime;
                    dr[5] = fileInfo.LastWriteTime;
                    dr[6] = false;
                    dr[7] = pid;
                    dr[8] = diskDescribe;
                    dr[9] = false;
                    dr[10] = false;
                    // 只读取 30KB 以下 nfo 文件内容
                    if (fileInfo.Extension.ToLower() == ".nfo" && fileInfo.Length <= 30720)
                        dr[11] = File.ReadAllText(fileInfo.FullName, System.Text.Encoding.GetEncoding("IBM437"));
                    else dr[11] = String.Empty;
                    dt.Rows.Add(dr);
                }

                InsertDataToFilmInfo(dt);
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
        /// <param name="dt"></param>
        private void InsertDataToFilmInfo(DataTable dt)
        {
            MySqlCommand sqlCom = new MySqlCommand();
            sqlCom.Connection = sqlCon;
            sqlCom.CommandText = String.Format(
                @"insert into {0} (
                name, path, size, create_t, modify_t, is_folder, pid, disk_desc, to_watch, to_delete, content)
                values", "film_info");

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                sqlCom.CommandText += String.Format(
                    @"(@name{0}, @path{0}, @size{0}, @create_t{0}, @modify_t{0}, @is_folder{0}, @pid{0}, 
                    @disk_desc{0},@to_watch{0}, @to_delete{0}, @content{0}){1}",
                    i, i == dt.Rows.Count - 1 ? ";" : ",");

                sqlCom.Parameters.AddWithValue(string.Format("@name{0}", i), dt.Rows[i][1]);
                sqlCom.Parameters.AddWithValue(string.Format("@path{0}", i), dt.Rows[i][2]);
                sqlCom.Parameters.AddWithValue(string.Format("@size{0}", i), dt.Rows[i][3]);
                sqlCom.Parameters.AddWithValue(string.Format("@create_t{0}", i), dt.Rows[i][4]);
                sqlCom.Parameters.AddWithValue(string.Format("@modify_t{0}", i), dt.Rows[i][5]);
                sqlCom.Parameters.AddWithValue(string.Format("@is_folder{0}", i), dt.Rows[i][6]);
                sqlCom.Parameters.AddWithValue(string.Format("@pid{0}", i), dt.Rows[i][7]);
                sqlCom.Parameters.AddWithValue(string.Format("@disk_desc{0}", i), dt.Rows[i][8]);
                sqlCom.Parameters.AddWithValue(string.Format("@to_watch{0}", i), dt.Rows[i][9]);
                sqlCom.Parameters.AddWithValue(string.Format("@to_delete{0}", i), dt.Rows[i][10]);
                sqlCom.Parameters.AddWithValue(string.Format("@content{0}", i), dt.Rows[i][11]);
            }

            int affectedRows = sqlCom.ExecuteNonQuery();
            Debug.Assert(affectedRows == dt.Rows.Count);
        }

        /// <summary>
        /// 向disk_info数据库插入或更新数据，diskDescribe唯一性
        /// </summary>
        /// <param name="diskDescribe">磁盘描述</param>
        /// <param name="freeSpace">剩余大小</param>
        /// <param name="totalSize">总大小</param>
        /// <param name="brifeScan">简略扫描</param>
        /// <param name="maxLayer">brifeScan为true时起作用</param>
        private void InsertOrUpdateDataToDiskInfo(
            String diskDescribe, Int64 freeSpace, Int64 totalSize,
            Boolean brifeScan, Int32 maxLayer = Int32.MaxValue)
        {
            if (!brifeScan) maxLayer = Int32.MaxValue;

            MySqlCommand sqlCom = new MySqlCommand();
            sqlCom.Connection = sqlCon;
            sqlCom.CommandText = String.Format(
                @"insert into {0} (disk_desc, free_space, total_size, complete_scan, max_layer) values(
                @disk_desc, @free_space, @total_size, @complete_scan, @max_layer) 
                on duplicate key update free_space = values(free_space),
                total_size = values(total_size), complete_scan = values(complete_scan),
                max_layer = values(max_layer);", "disk_info");
            sqlCom.Parameters.AddWithValue("@disk_desc", diskDescribe);
            sqlCom.Parameters.AddWithValue("@free_space", freeSpace);
            sqlCom.Parameters.AddWithValue("@total_size", totalSize);
            sqlCom.Parameters.AddWithValue("@complete_scan", !brifeScan);
            sqlCom.Parameters.AddWithValue("@max_layer", maxLayer);

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
            sqlCom.Parameters.AddWithValue("@search", string.Format("%{0}%", keyWord));
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
            List<Int32> idList = new List<Int32>();
            while (reader.Read()) idList.Add(reader.GetInt32(0));
            reader.Close();

            return idList.IndexOf(id);
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
                "select * from {0} where to_watch = 1 and disk_desc = @disk_desc limit {1}, {2};",
                "film_info", offset, rows);
            if (diskDescribe == null)
                sqlStr = string.Format("select * from {0} where to_watch = 1 limit {1}, {2};",
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
                "select * from {0} where to_delete = 1 and disk_desc = @disk_desc limit {1}, {2};",
                "film_info", offset, rows);
            if (diskDescribe == null)
                sqlStr = string.Format("select * from {0} where to_delete = 1 limit {1}, {2};",
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
            sqlStr += String.Format("update {0} set to_watch = @to_watch where id in ({1});",
                "film_info", String.Join(",", idList));
            MySqlCommand sqlCom = new MySqlCommand(sqlStr, sqlCon);
            sqlCom.Parameters.AddWithValue("@to_watch", toWatch);

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
            sqlStr += String.Format("update {0} set to_delete = @to_delete where id in ({1});",
                "film_info", String.Join(",", idList));
            MySqlCommand sqlCom = new MySqlCommand(sqlStr, sqlCon);
            sqlCom.Parameters.AddWithValue("@to_delete", toDelete);

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
        /// 更新film_info中diskDescribe磁盘下各文件夹大小
        /// </summary>
        /// <param name="diskDescribe"></param>
        public void UpdateFolderSizeFromFilmInfo(String diskDescribe)
        {
            String sqlAllFolder = String.Format(
                "select id from {0} where is_folder = 1 and disk_desc = @disk_desc;", "film_info");
            MySqlCommand sqlCom1 = new MySqlCommand(sqlAllFolder, sqlCon);
            sqlCom1.Parameters.AddWithValue("@disk_desc", diskDescribe);
            MySqlDataReader reader1 = sqlCom1.ExecuteReader();

            List<Int32> idList = new List<Int32>();
            while (reader1.Read())
            {
                idList.Add(reader1.GetInt32(0));
            }
            reader1.Close();

            foreach (Int32 folderID in idList)
            {
                Int32 maxChildID = GetMaxChildID(folderID);

                String sqlAllFile = String.Format(
                    "select sum(size) from {0} where is_folder = 0 and id between @minID and @maxID;",
                    "film_info");
                MySqlCommand sqlCom3 = new MySqlCommand(sqlAllFile, sqlCon);
                sqlCom3.Parameters.AddWithValue("@minID", folderID);
                sqlCom3.Parameters.AddWithValue("@maxID", maxChildID);
                object folderSizeObj = sqlCom3.ExecuteScalar();
                long folderSize = 0;
                if (folderSizeObj != DBNull.Value) folderSize = Convert.ToInt64(folderSizeObj);

                String sqlUpdateFolderSize = String.Format(
                    "update {0} set size = @size where id = @id;", "film_info");
                MySqlCommand sqlCom4 = new MySqlCommand(sqlUpdateFolderSize, sqlCon);
                sqlCom4.Parameters.AddWithValue("@size", folderSize);
                sqlCom4.Parameters.AddWithValue("@id", folderID);
                sqlCom4.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 获取文件夹下文件或文件夹的最大ID
        /// </summary>
        /// <param name="folderID">文件夹ID</param>
        /// <returns></returns>
        private Int32 GetMaxChildID(Int32 folderID)
        {
            String sqlMaxChildID = String.Format(
                    "select id, is_folder from {0} where pid = @pid order by id desc limit 1;",
                    "film_info");
            MySqlCommand sqlCom = new MySqlCommand(sqlMaxChildID, sqlCon);
            sqlCom.Parameters.AddWithValue("@pid", folderID);

            MySqlDataReader reader = sqlCom.ExecuteReader();
            bool hasRows = reader.Read();

            Int32 maxID = -1;
            bool isFolder = true;
            if (hasRows)
            {
                maxID = reader.GetInt32(0);
                isFolder = reader.GetBoolean(1);
            }
            reader.Close();

            if (hasRows)
            {
                if (isFolder) return GetMaxChildID(maxID);
                // 最大ID项为文件
                else return maxID;
            }
            // 文件夹为空
            else return folderID;
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
    }
}
