using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using static MyFilm.CommonDataTable;

namespace MyFilm
{
    public abstract class SqlData
    {
        /// <summary>
        /// 内存中存储数据库文件结构
        /// </summary>
        protected class FileNamePathID
        {
            // 文件名
            public string FileName;
            // 文件路径
            public string FilePath;
            // 文件在数据库中 id
            public int FileDataBaseID;

            public FileNamePathID(string fileName, string filePath, int fileDataBaseID)
            {
                this.FileName = fileName;
                this.FilePath = filePath;
                this.FileDataBaseID = fileDataBaseID;
            }

            public bool IsFileNameContainStrings(string[] strs)
            {
                bool isContain = true;

                foreach (string str in strs)
                {
                    if (!this.FileName.Contains(str)) { isContain = false; break; }
                }

                return isContain;
            }
        }

        /// <summary>
        /// 搜索关键字分隔符
        /// </summary>
        private static readonly char[] searchKeyWordSplitter =
            " `~!@#$%^&*()-_=+[]{}|\\;:\'\",./<>?《》（），。？；：’“【】、—￥·"
            .ToCharArray();

        /// <summary>
        /// 在程序打开或修改数据库时加载全部数据库到内存
        /// </summary>
        protected Dictionary<string, List<FileNamePathID>> ramData = null;
        protected bool ramDataCompleted = false;

        public static SqlData GetSqlData()
        {
            if (CommonString.DataBaseType == LoginConfig.DataBaseType.MYSQL)
                return SqlDataInMySql.GetInstance();
            else return SqlDataInSqlite.GetInstance();
        }

        protected abstract int ExecuteNonQueryGetAffected(
            String cmdText, Dictionary<String, Object> sqlParamDic = null);
        protected abstract int ExecuteScalarGetNum(
            String cmdText, Dictionary<String, Object> sqlParamDic = null);
        protected abstract DataTable ExecuteReaderGetAll(
            String cmdText, Dictionary<String, Object> sqlParamDic = null);
        protected abstract int[] ExecuteReaderGetIDs(
            String cmdText, Dictionary<String, Object> sqlParamDic = null);

        /// <summary>
        /// 加载数据库到内存
        /// </summary>
        public abstract void FillRamData();

        /// <summary>
        /// 获取待删文件，group by disk
        /// </summary>
        /// <param name="diskDescribe"></param>
        /// <returns></returns>
        public abstract int[] GetDeleteDataFromFilmInfoGroupByDisk(
            String diskDescribe = null);

        /// <summary>
        /// 查找所有已存在数据库名
        /// </summary>
        /// <returns></returns>
        public abstract List<String> QueryAllDataBaseNames();

        /// <summary>
        /// 创建数据库
        /// </summary>
        /// <param name="databaseName"></param>
        public abstract void CreateDataBase(String databaseName);

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
        virtual protected void CreateFilmInfoTable()
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
        virtual protected void CreateDiskInfoTable()
        {
            String cmdText = String.Format(@"create table if not exists {0} ( ", "disk_info");
            cmdText += String.Format(@"{0} integer primary key auto_increment, ", "id");
            cmdText += String.Format(@"{0} varchar(256) unique key not null, ", "disk_desc");
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
        virtual protected void CreateSearchLogTable()
        {
            String cmdText = String.Format(@"create table if not exists {0} ( ", "search_log");
            cmdText += String.Format(@"{0} integer primary key auto_increment, ", "id");
            cmdText += String.Format(@"{0} varchar(256) not null, ", "search_key");
            cmdText += String.Format(@"{0} integer not null, ", "result_count");
            cmdText += String.Format(@"{0} datetime not null );", "search_time");

            ExecuteNonQueryGetAffected(cmdText, null);
        }

        /// <summary>
        /// 更新 CommonString.RealOrFake4KDiskName 数据库修改时间
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public int UpdateDiskRealOrFake4KInModifyTimeFromDiskInfo(
            DateTime dateTime)
        {
            String cmdText = String.Format(
                "update {0} set modify_t = @modify_t where disk_desc = @disk_desc;",
                "film_info");

            Dictionary<String, Object> sqlParamDic = new Dictionary<string, object>();
            sqlParamDic.Add("@modify_t", dateTime);
            sqlParamDic.Add("@disk_desc", CommonString.RealOrFake4KDiskName);

            return ExecuteNonQueryGetAffected(cmdText, sqlParamDic);
        }

        virtual public void InsertDataToFilmInfo(DataTable dt)
        {
            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i += 500)
                {
                    InsertDataToFilmInfo(dt, i, 500);
                }
            }
        }

        /// <summary>
        /// 向film_info数据库插入数据，注意dt的列为FillFilmInfoColumn生成
        /// </summary>
        /// <param name="dt">插入的数据</param>
        /// <param name="start">起始，从0开始</param>
        /// <param name="count">个数</param>
        protected void InsertDataToFilmInfo(DataTable dt, int start, int count)
        {
            String cmdText = String.Format(
                @"insert into {0} (
                id, name, path, size, create_t, modify_t, is_folder, 
                to_watch, to_watch_ex, s_w_t, to_delete, to_delete_ex, 
                s_d_t, content, pid, max_cid, disk_desc) values",
                "film_info");

            Dictionary<String, Object> sqlParamDic = new Dictionary<string, object>();
            for (int i = start, j = 0; i < dt.Rows.Count && j < count; i++, j++)
            {
                cmdText += String.Format(
                    @"(@id{0}, @name{0}, @path{0}, @size{0}, @create_t{0}, @modify_t{0}, 
                    @is_folder{0}, @to_watch{0}, @to_watch_ex{0}, @s_w_t{0}, 
                    @to_delete{0}, @to_delete_ex{0}, @s_d_t{0}, @content{0}, 
                    @pid{0}, @max_cid{0}, @disk_desc{0}){1}",
                    i, i == dt.Rows.Count - 1 || j == count - 1 ? ";" : ",");

                sqlParamDic.Add(String.Format("@id{0}", i),
                    dt.Rows[i]["id"]);
                sqlParamDic.Add(String.Format("@name{0}", i),
                    dt.Rows[i]["name"]);
                sqlParamDic.Add(String.Format("@path{0}", i),
                    dt.Rows[i]["path"]);
                sqlParamDic.Add(String.Format("@size{0}", i),
                    dt.Rows[i]["size"]);
                sqlParamDic.Add(String.Format("@create_t{0}", i),
                    dt.Rows[i]["create_t"]);
                sqlParamDic.Add(String.Format("@modify_t{0}", i),
                    dt.Rows[i]["modify_t"]);
                sqlParamDic.Add(String.Format("@is_folder{0}", i),
                    dt.Rows[i]["is_folder"]);
                sqlParamDic.Add(String.Format("@to_watch{0}", i),
                    dt.Rows[i]["to_watch"]);
                sqlParamDic.Add(String.Format("@to_watch_ex{0}", i),
                    dt.Rows[i]["to_watch_ex"]);
                sqlParamDic.Add(String.Format("@s_w_t{0}", i),
                    dt.Rows[i]["s_w_t"]);
                sqlParamDic.Add(String.Format("@to_delete{0}", i),
                    dt.Rows[i]["to_delete"]);
                sqlParamDic.Add(String.Format("@to_delete_ex{0}", i),
                    dt.Rows[i]["to_delete_ex"]);
                sqlParamDic.Add(String.Format("@s_d_t{0}", i),
                    dt.Rows[i]["s_d_t"]);
                sqlParamDic.Add(String.Format("@content{0}", i),
                    dt.Rows[i]["content"]);
                sqlParamDic.Add(String.Format("@pid{0}", i),
                    dt.Rows[i]["pid"]);
                sqlParamDic.Add(String.Format("@max_cid{0}", i),
                    dt.Rows[i]["max_cid"]);
                sqlParamDic.Add(String.Format("@disk_desc{0}", i),
                    dt.Rows[i]["disk_desc"]);
            }

            ExecuteNonQueryGetAffected(cmdText, sqlParamDic);
        }

        /// <summary>
        /// 向disk_info数据库插入或更新数据，diskDescribe唯一性
        /// </summary>
        /// <param name="diskDescribe">磁盘描述</param>
        /// <param name="freeSpace">剩余大小</param>
        /// <param name="totalSize">总大小</param>
        /// <param name="completeScan">完全扫描</param>
        /// <param name="scanLayer">扫描的层数</param>
        virtual public void InsertOrUpdateDataToDiskInfo(
            String diskDescribe, Int64 freeSpace, Int64 totalSize,
            Boolean completeScan, int scanLayer)
        {
            String cmdText = String.Format(
                @"insert into {0} (
                disk_desc, free_space, total_size, complete_scan, scan_layer) values(
                @disk_desc, @free_space, @total_size, @complete_scan, @scan_layer) 
                on duplicate key update free_space = values(free_space),
                total_size = values(total_size), complete_scan = values(complete_scan),
                scan_layer = values(scan_layer);", "disk_info");

            Dictionary<String, Object> sqlParamDic = new Dictionary<string, object>();
            sqlParamDic.Add("@disk_desc", diskDescribe);
            sqlParamDic.Add("@free_space", freeSpace);
            sqlParamDic.Add("@total_size", totalSize);
            sqlParamDic.Add("@complete_scan", completeScan);
            sqlParamDic.Add("@scan_layer", scanLayer);

            ExecuteNonQueryGetAffected(cmdText, sqlParamDic);
        }

        /// <summary>
        /// 向search_log数据库插入数据
        /// </summary>
        /// <param name="searchKey">搜索关键字</param>
        /// <param name="resultCount">搜索结果条数</param>
        /// <param name="searchTime">搜索时间</param>
        public void InsertDataToSearchLog(
            String searchKey, int resultCount, DateTime searchTime)
        {
            String cmdText = String.Format(
                @"insert into {0} (search_key, result_count, search_time) values(
                @search_key, @result_count, @search_time);", "search_log");

            Dictionary<String, Object> sqlParamDic = new Dictionary<string, object>();
            sqlParamDic.Add("@search_key", searchKey);
            sqlParamDic.Add("@result_count", resultCount);
            sqlParamDic.Add("@search_time", searchTime);

            ExecuteNonQueryGetAffected(cmdText, sqlParamDic);
        }

        /// <summary>
        /// 在 film_info 中根据指定的 id列表 获取数据
        /// </summary>
        /// <param name="idList">id列表</param>
        /// <returns></returns>
        virtual public DataTable SelectDataByIDList(int[] idList)
        {
            if (idList == null || idList.Length == 0) return null;

            String cmdText = String.Format(
                "select * from {0} where id in ({1}) order by field(id, {1});",
                "film_info", String.Join(",", idList));

            return ExecuteReaderGetAll(cmdText, null);
        }

        /// <summary>
        /// 搜索 film_info 表
        /// </summary>
        /// <param name="keyWord">搜索关键字</param>
        /// <param name="diskDescribe">null时所有磁盘，否则特定磁盘</param>
        /// <returns></returns>
        public int[] SearchKeyWordFromFilmInfo(
            String keyWord, String diskDescribe = null)
        {
            String[] keyWords = keyWord.ToLower().Split(searchKeyWordSplitter,
                StringSplitOptions.RemoveEmptyEntries);
            if (keyWords.Length == 0) return null;

            if (ramDataCompleted)
            {
                List<int> rstList = new List<int>();

                if (diskDescribe == null)
                {
                    foreach (List<FileNamePathID> fnpiList in ramData.Values)
                        foreach (FileNamePathID fnpi in fnpiList)
                            if (fnpi.IsFileNameContainStrings(keyWords))
                                rstList.Add(fnpi.FileDataBaseID);
                }
                else
                {
                    if (!ramData.ContainsKey(diskDescribe)) return null;

                    List<FileNamePathID> fnpiList = ramData[diskDescribe];
                    foreach (FileNamePathID fnpi in fnpiList)
                        if (fnpi.IsFileNameContainStrings(keyWords))
                            rstList.Add(fnpi.FileDataBaseID);
                }

                return rstList.ToArray();
            }
            else
            {
                String strTemp = String.Empty;
                for (int i = 0; i < keyWords.Length; i++)
                {
                    strTemp += String.Format(" locate(@{0}, name)>0 {1}",
                        i, i == keyWords.Length - 1 ? "" : "and");
                }
                String cmdText = String.Format(
                    "select id from {0} where {1} {2} order by id;", "film_info",
                    strTemp, diskDescribe == null ? "" : "and disk_desc = @disk_desc");

                Dictionary<String, Object> sqlParamDic = new Dictionary<string, object>();
                for (int i = 0; i < keyWords.Length; i++)
                {
                    sqlParamDic.Add(String.Format("@{0}", i), keyWords[i]);
                }
                if (diskDescribe != null)
                {
                    sqlParamDic.Add("@disk_desc", diskDescribe);
                }

                return ExecuteReaderGetIDs(cmdText, sqlParamDic);
            }
        }

        /// <summary>
        /// 查看film_info表结构的详细信息
        /// </summary>
        /// <returns></returns>
        public DataTable GetDescriptionOfFilmInfo()
        {
            String cmdText = String.Format("desc {0}", "film_info");

            return ExecuteReaderGetAll(cmdText, null);
        }

        /// <summary>
        /// 根据 sql 语句生成 MySqlCommand 对象
        /// </summary>
        /// <param name="cmdText">sql 语句</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns>sql 语句解析出错时返回 null</returns>
        private bool DealWithSqlQueryText(String cmdText,
            ref String actualCmdText,
            ref Dictionary<string, object> sqlParamDic,
            ref String errMsg)
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
            if (tempChar != ' ') { errMsg = "error sql statement"; return false; }

            int index = 0;
            int n = 0;
            actualCmdText = string.Empty;
            foreach (KeyValuePair<int, int> kv in dict2)
            {
                actualCmdText += cmdText.Substring(index, kv.Key - index);
                actualCmdText += String.Format("@{0}", n++);
                index = kv.Value + 1;
            }
            actualCmdText += cmdText.Substring(index);

            n = 0;
            foreach (KeyValuePair<int, int> kv in dict2)
            {
                sqlParamDic.Add(String.Format("@{0}", n++),
                    cmdText.Substring(kv.Key + 1, kv.Value - kv.Key - 1));
            }

            return true;
        }

        /// <summary>
        /// 用 sql 语句查询所有列数据
        /// </summary>
        /// <param name="cmdText">sql 语句</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns>sql 语句解析出错时返回 null</returns>
        public DataTable SelectAllDataBySqlText(String cmdText, ref String errMsg)
        {
            String actualCmdText = String.Empty;
            Dictionary<string, object> sqlParamDic = new Dictionary<string, object>();

            if (DealWithSqlQueryText(
                cmdText, ref actualCmdText, ref sqlParamDic, ref errMsg))
            {
                return ExecuteReaderGetAll(cmdText, sqlParamDic);
            }
            else return null;
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

            String actualCmdText = String.Empty;
            Dictionary<string, object> sqlParamDic = new Dictionary<string, object>();

            if (DealWithSqlQueryText("SELECT id FROM" + cmdText.Substring(prefix.Length),
                ref actualCmdText, ref sqlParamDic, ref errMsg))
            {
                return ExecuteReaderGetIDs(cmdText, sqlParamDic);
            }
            else return null;
        }

        /// <summary>
        /// 获取disk_info数据库所有数据
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllDataFromDiskInfo()
        {
            String cmdText = String.Format("select * from {0};", "disk_info");

            return ExecuteReaderGetAll(cmdText, null);
        }

        /// <summary>
        /// 获取所有的磁盘根目录（pid = -1）
        /// </summary>
        /// <returns></returns>
        public DataTable GetAllRootDirectoryFromFilmInfo()
        {
            String cmdText = String.Format(
                "select * from {0} where pid = -1;", "film_info");

            return ExecuteReaderGetAll(cmdText, null);
        }

        /// <summary>
        /// 获取指定id的数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataTable GetDataByIdFromFilmInfo(int id)
        {
            String cmdText = String.Format(
                "select * from {0} where id = @id;", "film_info");

            Dictionary<String, Object> sqlParamDic = new Dictionary<string, object>();
            sqlParamDic.Add("@id", id);

            return ExecuteReaderGetAll(cmdText, sqlParamDic);
        }

        /// <summary>
        /// 获取指定pid的数据
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public int[] GetDataByPidFromFilmInfo(int pid)
        {
            String cmdText = String.Format(
                "select id from {0} where pid = @pid order by id;",
                "film_info");

            Dictionary<String, Object> sqlParamDic = new Dictionary<string, object>();
            sqlParamDic.Add("@pid", pid);

            return ExecuteReaderGetIDs(cmdText, sqlParamDic);
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

            Dictionary<String, Object> sqlParamDic = null;
            if (diskDescribe != null)
            {
                sqlParamDic = new Dictionary<string, object>();
                sqlParamDic.Add("@disk_desc", diskDescribe);
            }

            return ExecuteReaderGetIDs(cmdText, sqlParamDic);
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

            Dictionary<String, Object> sqlParamDic = null;
            if (diskDescribe != null)
            {
                sqlParamDic = new Dictionary<string, object>();
                sqlParamDic.Add("@disk_desc", diskDescribe);
            }

            return ExecuteReaderGetIDs(cmdText, sqlParamDic);
        }

        public void UpdateWatchOrDeleteStateFromFilmInfo(bool isWatch,
            List<SetStateStruct> setStateStructList, DateTime setTime, bool setTo)
        {
            if (setStateStructList == null || setStateStructList.Count == 0) return;
            // 用 pid 排序（最顶层的文件夹在前面）
            setStateStructList.Sort((x, y) => x.pid.CompareTo(y.pid));

            Dictionary<string, object> sqlParamDic = new Dictionary<string, object>();

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
                    if ((isWatch ?
                        setStateStructList[i].to_watch_ex :
                        setStateStructList[i].to_delete_ex) ||
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

                    sqlParamDic.Add(String.Format("@{0}", pi++), setTime);
                }
            }
            else
            {
                List<TreeSetState> nodeTreeSetStateList = new List<TreeSetState>();

                for (int i = 0; i < setStateStructList.Count; i++)
                {
                    if ((!(isWatch ?
                        setStateStructList[i].to_watch_ex :
                        setStateStructList[i].to_delete_ex)) ||
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
                            upNodeSetState.name =
                                Convert.ToString(pdt.Rows[0]["name"]);
                            upNodeSetState.id = id;
                            upNodeSetState.max_cid =
                                Convert.ToInt32(pdt.Rows[0]["max_cid"]);
                            upNodeSetState.cancelIDList = new List<TreeSetState>();
                            upNodeSetState.Add(nodeSetState);

                            nodeSetState = upNodeSetState;
                        }
                        else break;
                    }

                    if (setTimeWithPositive != setTime)
                    {
                        sqlParamDic.Add(String.Format("@{0}", pi++), setTimeWithPositive);
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
                                _upNodeSetState.name =
                                    Convert.ToString(_pdt.Rows[0]["name"]);
                                _upNodeSetState.id = _id;
                                _upNodeSetState.max_cid =
                                    Convert.ToInt32(_pdt.Rows[0]["max_cid"]);
                                _upNodeSetState.cancelIDList = new List<TreeSetState>();
                                _upNodeSetState.Add(_nodeSetState);

                                _nodeSetState = _upNodeSetState;
                            }

                            nodeSetState.Add(_nodeSetState);
                        }
                    }

                    nodeTreeSetStateList.Add(nodeSetState);
                }

                sqlParamDic.Add("@cancel", setTime);

                pi = 0;
                foreach (TreeSetState nodeSetState in nodeTreeSetStateList)
                {
                    List<string> tc3 = new List<string>();

                    GenerateConditionString(nodeSetState, ref c1, ref c2, ref tc3, ref c4);

                    if (tc3.Count > 0)
                        s1.Add(String.Format(
                            "when {0} then @{1}", string.Join(" or ", tc3), pi++));

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

                String cmdText = String.Format(@"update {0} set 
                    {6} = (case {1} {2} else {6} end),
                    {7} = (case {3} {4} else {7} end),
                    {8} = (case {5} else {8} end);",
                    "film_info", str1, str2, str3, str4, string.Join(" ", s1), k1, k2, k3);

                ExecuteNonQueryGetAffected(cmdText, sqlParamDic);
            }
        }

        private void GenerateConditionString(
            TreeSetState nodeSetState, ref List<string> c1,
            ref List<string> c2, ref List<string> c3, ref List<string> c4)
        {
            if (nodeSetState == null) return;

            if (nodeSetState.cancelIDList == null)
            {
                c2.Add(String.Format("(pid={0} or id={0})", nodeSetState.id));
                c4.Add(String.Format("(id>={0} and id<={1})",
                    nodeSetState.id, nodeSetState.max_cid));
            }
            else
            {
                // 不可能为空
                Debug.Assert(nodeSetState.cancelIDList.Count > 0);

                List<int> idList =
                    nodeSetState.cancelIDList.Select(x => x.id).ToList();
                List<int> maxcidList =
                    nodeSetState.cancelIDList.Select(x => x.max_cid).ToList();

                idList.Sort();
                maxcidList.Sort();

                string strC3Range = GenerateRangeString(nodeSetState.id,
                    nodeSetState.max_cid, idList, maxcidList);

                string strTemp = String.Join(",", idList);

                c1.Add(String.Format(
                    "(pid={0} and id not in ({1}))", nodeSetState.id, strTemp));
                c2.Add(String.Format(
                    "(id={0})", nodeSetState.id));
                if (!string.IsNullOrEmpty(strC3Range))
                {
                    c3.Add(String.Format("({0})", strC3Range));
                }
                c4.Add(String.Format("(id={0})", nodeSetState.id));

                foreach (TreeSetState _nodeSetState in nodeSetState.cancelIDList)
                    GenerateConditionString(_nodeSetState, ref c1, ref c2, ref c3, ref c4);
            }
        }

        private string GenerateRangeString(
            int id, int max_cid, List<int> idList, List<int> maxcidList)
        {
            Debug.Assert(
                idList != null &&
                maxcidList != null &&
                idList.Count == maxcidList.Count);
            Debug.Assert(
                id <= idList[0] &&
                max_cid >= maxcidList[maxcidList.Count - 1]);

            List<string> strList = new List<string>();

            int pos = id;
            for (int i = 0; i < idList.Count; i++)
            {
                if (idList[i] > pos)
                {
                    strList.Add(string.Format("(id>{0} and id<{1})", pos, idList[i]));
                }
                pos = maxcidList[i];
            }
            if (pos < max_cid)
            {
                strList.Add(string.Format("(id>{0} and id<{1})", pos, max_cid));
            }

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

            return ExecuteNonQueryGetAffected(cmdText, null);
        }

        /// <summary>
        /// 从film_info中删除指定磁盘描述的数据
        /// </summary>
        /// <param name="diskDescribe"></param>
        /// <returns></returns>
        public int DeleteByDiskDescribeFromFilmInfo(String diskDescribe)
        {
            String cmdText = String.Format("delete from {0} where disk_desc = @disk_desc;",
                "film_info");

            Dictionary<String, Object> sqlParamDic = new Dictionary<string, object>();
            sqlParamDic.Add("@disk_desc", diskDescribe);

            return ExecuteNonQueryGetAffected(cmdText, sqlParamDic);
        }

        /// <summary>
        /// 从disk_info中删除指定磁盘描述的数据
        /// </summary>
        /// <param name="diskDescribe"></param>
        /// <returns></returns>
        public int DeleteByDiskDescribeFromDiskInfo(String diskDescribe)
        {
            String cmdText = String.Format("delete from {0} where disk_desc = @disk_desc;",
                "disk_info");

            Dictionary<String, Object> sqlParamDic = new Dictionary<string, object>();
            sqlParamDic.Add("@disk_desc", diskDescribe);

            return ExecuteNonQueryGetAffected(cmdText, sqlParamDic);
        }

        /// <summary>
        /// 更新film_info磁盘描述
        /// </summary>
        /// <param name="fromDiskDescribe">原</param>
        /// <param name="toDiskDescribe">新</param>
        /// <returns></returns>
        public int UpdateDiskDescribeFromFilmInfo(
            String fromDiskDescribe, String toDiskDescribe)
        {
            String cmdText = String.Format(
                "update {0} set disk_desc = @t_disk_desc where disk_desc = @f_disk_desc;",
                "film_info");

            Dictionary<String, Object> sqlParamDic = new Dictionary<string, object>();
            sqlParamDic.Add("@t_disk_desc", toDiskDescribe);
            sqlParamDic.Add("@f_disk_desc", fromDiskDescribe);

            return ExecuteNonQueryGetAffected(cmdText, sqlParamDic);
        }

        /// <summary>
        /// 更新disk_info磁盘描述
        /// </summary>
        /// <param name="fromDiskDescribe">原</param>
        /// <param name="toDiskDescribe">新</param>
        /// <returns></returns>
        public int UpdateDiskDescribeFromDiskInfo(
            String fromDiskDescribe, String toDiskDescribe)
        {
            String cmdText = String.Format(
                "update {0} set disk_desc = @t_disk_desc where disk_desc = @f_disk_desc;",
                "disk_info");

            Dictionary<String, Object> sqlParamDic = new Dictionary<string, object>();
            sqlParamDic.Add("@t_disk_desc", toDiskDescribe);
            sqlParamDic.Add("@f_disk_desc", fromDiskDescribe);

            return ExecuteNonQueryGetAffected(cmdText, sqlParamDic);
        }

        /// <summary>
        /// 获取文件夹下的文件夹
        /// </summary>
        /// <param name="folderID">文件夹ID</param>
        /// <returns></returns>
        public DataTable GetChildFolderFromFilmInfo(int folderID)
        {
            String cmdText = String.Format(
                "select * from {0} where pid = @pid and is_folder = 1;",
                "film_info");

            Dictionary<String, Object> sqlParamDic = new Dictionary<string, object>();
            sqlParamDic.Add("@pid", folderID);

            return ExecuteReaderGetAll(cmdText, sqlParamDic);
        }

        /// <summary>
        /// 获取文件夹下的文件
        /// </summary>
        /// <param name="folderID">文件夹ID</param>
        /// <returns></returns>
        public DataTable GetChildFileFromFilmInfo(int folderID)
        {
            String cmdText = String.Format(
                "select * from {0} where pid = @pid and is_folder = 0;",
                "film_info");

            Dictionary<String, Object> sqlParamDic = new Dictionary<string, object>();
            sqlParamDic.Add("@pid", folderID);

            return ExecuteReaderGetAll(cmdText, sqlParamDic);
        }

        public int GetMaxIdOfFilmInfo()
        {
            String cmdText = String.Format("select max(id) from {0};", "film_info");

            return ExecuteScalarGetNum(cmdText, null);
        }

        public int CountRowsOfDiskFromFilmInfo(string diskDescribe)
        {
            String cmdText = String.Format(
                "select count(*) from {0} where disk_desc=@disk_desc;", "film_info");

            Dictionary<String, Object> sqlParamDic = new Dictionary<string, object>();
            sqlParamDic.Add("@disk_desc", diskDescribe);

            return ExecuteScalarGetNum(cmdText, sqlParamDic);
        }

        /// <summary>
        /// 查询 search_log 表行数
        /// </summary>
        /// <returns></returns>
        public int CountRowsFromSearchLog()
        {
            String cmdText = String.Format("select count(*) from {0};", "search_log");

            return ExecuteScalarGetNum(cmdText, null);
        }
    }
}