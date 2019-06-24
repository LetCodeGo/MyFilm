using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace MyFilm
{
    public class SqlDataInMySql : SqlData
    {
        /// <summary>
        /// 打开数据库
        /// </summary>
        private static String SQLOpenCmdText =
            String.Format(
            @"server = {0}; uid = {1}; pwd = {2}; database = {3};
            sslmode = none; convert zero datetime = true;
            allow zero datetime = true;",
            CommonString.DbIP, CommonString.DbUserName,
            CommonString.DbPassword, CommonString.DbName);

        private static SqlDataInMySql mySqlData = null;
        private static readonly object locker = new object();

        private SqlDataInMySql() { }

        public static SqlDataInMySql GetInstance()
        {
            if (mySqlData == null)
            {
                lock (locker)
                {
                    // 如果类的实例不存在则创建，否则直接返回
                    if (mySqlData == null)
                    {
                        mySqlData = new SqlDataInMySql();
                    }
                }
            }
            return mySqlData;
        }

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

        public override List<String> QueryAllDataBaseNames()
        {
            List<String> nameList = new List<String>();

            using (MySqlConnection sqlCon = new MySqlConnection(
                String.Format(@"Data Source={0};Persist Security Info=yes;
                SslMode = none;UserId={1}; PWD={2};",
                CommonString.DbIP, CommonString.DbUserName, CommonString.DbPassword)))
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

        public override void CreateDataBase(String databaseName)
        {
            using (MySqlConnection sqlCon = new MySqlConnection(
                String.Format(@"Data Source={0};Persist Security Info=yes;
                SslMode = none;UserId={1}; PWD={2};",
                CommonString.DbIP, CommonString.DbUserName, CommonString.DbPassword)))
            {
                sqlCon.Open();
                using (MySqlCommand sqlCmd = new MySqlCommand(
                    String.Format("create database {0};", databaseName), sqlCon))
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

        public override int[] GetDeleteDataFromFilmInfoGroupByDisk(
            String diskDescribe = null)
        {
            String cmdText = String.Format(
                @"select group_concat(id order by id), count(id) as id_count from {0} where 
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
