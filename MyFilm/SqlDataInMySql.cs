using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace MyFilm
{
    public class SqlDataInMySql : SqlData
    {
        public readonly String DbIP = "127.0.0.1";
        private readonly String DbUserName = string.Empty;
        private readonly String DbPassword = string.Empty;
        public readonly String DbName = "myfilm";
        private readonly String SQLOpenCmdText = string.Empty;

        public SqlDataInMySql(String dbIP, String dbUserName, String dbPassword, String dbName)
        {
            this.DbIP = dbIP;
            this.DbUserName = dbUserName;
            this.DbPassword = dbPassword;
            this.DbName = dbName;
            this.SQLOpenCmdText = String.Format(
                @"server = {0}; uid = {1}; pwd = {2}; database = {3};
                sslmode = none; convert zero datetime = true;
                allow zero datetime = true;",
                dbIP, dbUserName, dbPassword, dbName);
        }

        public override LoginConfig.DataBaseType GetDataBaseType()
        {
            return LoginConfig.DataBaseType.MYSQL;
        }

        public override string GetIdentString()
        {
            return string.Format("mysql_{0}_{1}", this.DbIP, this.DbName);
        }

        //public override void DeleteAllDataFormAllTable()
        //{
        //    string[] tableNames = new string[] { "film_info", "disk_info", "search_log" };
        //    foreach (string tableName in tableNames)
        //    {
        //        ExecuteNonQueryGetAffected(
        //            string.Format("truncate table {0};", tableName), null);
        //        ExecuteNonQueryGetAffected(
        //            string.Format("update statistics low for table {0};", tableName), null);
        //    }
        //}

        protected override int ExecuteNonQueryGetAffected(
            String cmdText, Dictionary<String, Object> sqlParamDic = null)
        {
            int affected = 0;
            using (MySqlConnection sqlCon = new MySqlConnection(SQLOpenCmdText))
            {
                sqlCon.Open();
                using (MySqlCommand sqlCmd = new MySqlCommand(cmdText, sqlCon))
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
            using (MySqlConnection sqlCon = new MySqlConnection(SQLOpenCmdText))
            {
                sqlCon.Open();
                using (MySqlCommand sqlCmd = new MySqlCommand(cmdText, sqlCon))
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
            using (MySqlConnection sqlCon = new MySqlConnection(SQLOpenCmdText))
            {
                sqlCon.Open();
                using (MySqlCommand sqlCmd = new MySqlCommand(cmdText, sqlCon))
                {
                    if (sqlParamDic != null)
                    {
                        foreach (KeyValuePair<String, Object> kv in sqlParamDic)
                        {
                            sqlCmd.Parameters.AddWithValue(kv.Key, kv.Value);
                        }
                    }

                    using (MySqlDataReader sqlDataReader = sqlCmd.ExecuteReader())
                    {
                        dt = new DataTable();
                        for (int i = 0; i < sqlDataReader.FieldCount; i++)
                        {
                            dt.Columns.Add(sqlDataReader.GetName(i),
                                sqlDataReader.GetFieldType(i) ==
                                typeof(MySql.Data.Types.MySqlDateTime) ?
                                typeof(DateTime) : sqlDataReader.GetFieldType(i));
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
            using (MySqlConnection sqlCon = new MySqlConnection(SQLOpenCmdText))
            {
                sqlCon.Open();
                using (MySqlCommand sqlCmd = new MySqlCommand(cmdText, sqlCon))
                {
                    if (sqlParamDic != null)
                    {
                        foreach (KeyValuePair<String, Object> kv in sqlParamDic)
                        {
                            sqlCmd.Parameters.AddWithValue(kv.Key, kv.Value);
                        }
                    }

                    using (MySqlDataReader sqlDataReader = sqlCmd.ExecuteReader())
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

        /// <summary>
        /// 查找所有已存在数据库名
        /// </summary>
        /// <returns></returns>
        public static List<String> QueryAllDataBaseNames(
            String dbIP, String dbUserName, String dbPassword)
        {
            List<String> nameList = new List<String>();

            using (MySqlConnection sqlCon = new MySqlConnection(
                String.Format(@"Data Source={0};Persist Security Info=yes;
                SslMode = none;UserId={1}; PWD={2};",
                dbIP, dbUserName, dbPassword)))
            {
                sqlCon.Open();
                using (MySqlCommand sqlCmd = new MySqlCommand("show databases;", sqlCon))
                {
                    using (MySqlDataReader sqlDataReader = sqlCmd.ExecuteReader())
                    {
                        while (sqlDataReader.Read())
                            nameList.Add(sqlDataReader[0].ToString());
                    }
                }
                sqlCon.Close();
            }

            return nameList;
        }

        public static void CreateDataBase(
            String dbIP, String dbUserName, String dbPassword, String dbName)
        {
            using (MySqlConnection sqlCon = new MySqlConnection(
                String.Format(@"Data Source={0};Persist Security Info=yes;
                SslMode = none;UserId={1}; PWD={2};",
                dbIP, dbUserName, dbPassword)))
            {
                sqlCon.Open();
                using (MySqlCommand sqlCmd = new MySqlCommand(
                    String.Format("create database {0};", dbName), sqlCon))
                {
                    sqlCmd.ExecuteNonQuery();
                }
                sqlCon.Close();
            }
        }

        public override void FillRamData()
        {
            String cmdText = String.Format(
                "select id, name, path, disk_desc from {0};", "film_info");

            using (MySqlConnection sqlCon = new MySqlConnection(SQLOpenCmdText))
            {
                sqlCon.Open();
                using (MySqlCommand sqlCmd = new MySqlCommand(cmdText, sqlCon))
                {
                    using (MySqlDataReader sqlDataReader = sqlCmd.ExecuteReader())
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

            using (MySqlConnection sqlCon = new MySqlConnection(SQLOpenCmdText))
            {
                sqlCon.Open();
                using (MySqlCommand sqlCmd = new MySqlCommand(cmdText, sqlCon))
                {
                    sqlCmd.Parameters.AddWithValue("@disk_desc", CommonString.RealOrFake4KDiskName);
                    using (MySqlDataReader sqlDataReader = sqlCmd.ExecuteReader())
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

            using (MySqlConnection sqlCon = new MySqlConnection(SQLOpenCmdText))
            {
                sqlCon.Open();
                using (MySqlCommand sqlCmd = new MySqlCommand(cmdText, sqlCon))
                {
                    using (MySqlDataReader sqlDataReader = sqlCmd.ExecuteReader())
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

            using (MySqlConnection sqlCon = new MySqlConnection(SQLOpenCmdText))
            {
                sqlCon.Open();
                using (MySqlCommand sqlCmd = new MySqlCommand(cmdText, sqlCon))
                {
                    using (MySqlDataReader sqlDataReader = sqlCmd.ExecuteReader())
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
                @"select group_concat(id order by s_d_t desc, id asc), 
                count(id) as id_count from {0} where 
                to_delete = 1 {1} group by disk_desc order by id_count desc;",
                "film_info", diskDescribe == null ? "" : "and disk_desc = @disk_desc");

            int[] ids = null;
            using (MySqlConnection sqlCon = new MySqlConnection(SQLOpenCmdText))
            {
                sqlCon.Open();
                using (MySqlCommand sqlCmd = new MySqlCommand(cmdText, sqlCon))
                {
                    if (diskDescribe != null)
                    {
                        sqlCmd.Parameters.AddWithValue("@disk_desc", diskDescribe);
                    }

                    using (MySqlDataReader sqlDataReader = sqlCmd.ExecuteReader())
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
