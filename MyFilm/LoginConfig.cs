﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace MyFilm
{
    public class LoginConfig
    {
        [XmlRoot("DataBaseConfig")]
        public class DataBaseConfig
        {
            [XmlAttribute("Name")]
            public String Name;

            [XmlAttribute("WebDataCaptureTime")]
            public String WebDataCaptureTime;
        }

        [XmlRoot("UserNameAndPassWord")]
        public class UserNameAndPassWord
        {
            [XmlAttribute("UserName")]
            public String UserName;

            [XmlAttribute("PassWord")]
            public String PassWord;
        }

        [XmlRoot("CrawlConfig")]
        public class CrawlConfig
        {
            [XmlAttribute("IsCrawl")]
            public Boolean IsCrawl;

            [XmlAttribute("IntervalDays")]
            public Int32 IntervalDays;

            [XmlAttribute("CrawlURL")]
            public String CrawlURL;
        }

        [XmlRoot("WebServerConfig")]
        public class WebServerConfig
        {
            [XmlAttribute("IsStartWebServer")]
            public Boolean IsStartWebServer;

            [XmlAttribute("Port")]
            public Int32 Port;

            [XmlAttribute("RowsPerPage")]
            public Int32 RowsPerPage;
        }

        [Serializable]
        [XmlRoot("MySQLConfig")]
        public class MySQLConfig
        {
            [XmlAttribute("SelectedIP")]
            public String selectedIP;

            [XmlAttribute("SelectedUserName")]
            public String selectedUserName;

            [XmlAttribute("SelectedDataBaseName")]
            public String selectedDataBaseName;

            [XmlArrayItem("HostIPs")]
            public List<String> hostIPs;

            [XmlArrayItem("UserNameAndPassWords")]
            public List<UserNameAndPassWord> userNameAndPassWords;

            [XmlArrayItem("DataBaseConfigs")]
            public List<DataBaseConfig> dataBaseConfigs;

            [XmlIgnore]
            public int UserNameMaxLength
            {
                get
                {
                    int maxLength = 0;
                    userNameAndPassWords.ForEach(x =>
                    {
                        if (maxLength < x.UserName.Length)
                            maxLength = x.UserName.Length;
                    });
                    return maxLength;
                }
            }

            [XmlIgnore]
            public String SelectedPassWord
            {
                get
                {
                    UserNameAndPassWord temp = userNameAndPassWords.Find(
                        x => x.UserName == selectedUserName);
                    if (temp == null) return null;
                    else return temp.PassWord;
                }
            }

            [XmlIgnore]
            public String SelectedDataBaseWebDataCaptureTime
            {
                get
                {
                    DataBaseConfig temp = dataBaseConfigs.Find(
                        x => x.Name == selectedDataBaseName);
                    if (temp == null) return null;
                    else return temp.WebDataCaptureTime;
                }
                set
                {
                    DataBaseConfig temp = dataBaseConfigs.Find(
                        x => x.Name == selectedDataBaseName);
                    if (temp != null) temp.WebDataCaptureTime = value;
                }
            }
        }

        [Serializable]
        [XmlRoot("SQLiteConfig")]
        public class SQLiteConfig
        {
            [XmlAttribute("SelectedDataBasePath")]
            public String selectedDataBasePath;

            [XmlArrayItem("DataBaseList")]
            public List<DataBaseConfig> dataBaseConfigs;

            [XmlIgnore]
            public String SelectedDataBaseWebDataCaptureTime
            {
                get
                {
                    DataBaseConfig temp = dataBaseConfigs.Find(
                        x => x.Name == selectedDataBasePath);
                    if (temp == null) return null;
                    else return temp.WebDataCaptureTime;
                }
                set
                {
                    DataBaseConfig temp = dataBaseConfigs.Find(
                        x => x.Name == selectedDataBasePath);
                    if (temp != null) temp.WebDataCaptureTime = value;
                }
            }
        }

        public enum DataBaseType
        {
            [XmlEnum("MYSQL")]
            MYSQL,
            [XmlEnum("SQLITE")]
            SQLITE
        }

        [Serializable]
        [XmlRoot("LoginConfigData")]
        public class LoginConfigData
        {
            [XmlAttribute("DataBaseType")]
            public DataBaseType dataBaseType;

            [XmlAttribute("RowsPerPage")]
            public Int32 rowsPerPage;

            [XmlElement("MySQLConfig")]
            public MySQLConfig mysqlConfig;

            [XmlElement("SQLiteConfig")]
            public SQLiteConfig sqliteConfig;

            [XmlElement("CrawlConfig")]
            public CrawlConfig crawlConfig;

            [XmlElement("WebServerConfig")]
            public WebServerConfig webServerConfig;
        }

        private static LoginConfigData GetInitLoginConfigData()
        {
            LoginConfigData initLoginConfigData = new LoginConfigData()
            {
                dataBaseType = DataBaseType.SQLITE,
                rowsPerPage = 20,
                mysqlConfig = new MySQLConfig
                {
                    selectedIP = "127.0.0.1",
                    selectedUserName = "",
                    selectedDataBaseName = "",
                    hostIPs = new List<string>() { "127.0.0.1" },
                    userNameAndPassWords = new List<UserNameAndPassWord>(),
                    dataBaseConfigs = new List<DataBaseConfig>()
                },
                sqliteConfig = new SQLiteConfig
                {
                    selectedDataBasePath = SqlDataInSqlite.SqliteDefaultDateBasePath,
                    dataBaseConfigs = new List<DataBaseConfig>() {
                        new DataBaseConfig() {
                            Name = SqlDataInSqlite.SqliteDefaultDateBasePath,
                            WebDataCaptureTime =
                            DateTime.MinValue.ToString("yyyy-MM-dd HHH:mm:ss")
                        }
                    }
                },
                crawlConfig = new CrawlConfig
                {
                    IsCrawl = true,
                    IntervalDays = 10,
                    CrawlURL = CommonString.CrawlURL
                },
                webServerConfig = new WebServerConfig
                {
                    IsStartWebServer = true,
                    Port = 5555,
                    RowsPerPage = 20
                }
            };

            return initLoginConfigData;
        }

        public static LoginConfigData LoadXml(string xmlPath)
        {
            LoginConfigData loginConfigData = null;
            XmlSerializer ser = new XmlSerializer(typeof(LoginConfigData));
            if (File.Exists(xmlPath))
            {
                using (FileStream fs = File.OpenRead(xmlPath))
                {
                    try { loginConfigData = ser.Deserialize(fs) as LoginConfigData; }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK);
                    }
                }
            }

            if (loginConfigData == null) loginConfigData = GetInitLoginConfigData();

            if (loginConfigData.rowsPerPage <= 0)
                loginConfigData.rowsPerPage = 20;

            if (loginConfigData.mysqlConfig == null)
            {
                loginConfigData.mysqlConfig = new MySQLConfig
                {
                    selectedIP = "127.0.0.1",
                    selectedUserName = "",
                    selectedDataBaseName = "",
                    hostIPs = new List<string>() { "127.0.0.1" },
                    userNameAndPassWords = new List<UserNameAndPassWord>(),
                    dataBaseConfigs = new List<DataBaseConfig>()
                };
            }
            else
            {
                if (loginConfigData.mysqlConfig.hostIPs == null)
                    loginConfigData.mysqlConfig.hostIPs = new List<string>();

                if (loginConfigData.mysqlConfig.userNameAndPassWords == null)
                    loginConfigData.mysqlConfig.userNameAndPassWords =
                        new List<UserNameAndPassWord>();

                if (loginConfigData.mysqlConfig.dataBaseConfigs == null)
                    loginConfigData.mysqlConfig.dataBaseConfigs =
                        new List<DataBaseConfig>();
            }

            if (loginConfigData.sqliteConfig == null)
            {
                loginConfigData.sqliteConfig = new SQLiteConfig
                {
                    selectedDataBasePath = SqlDataInSqlite.SqliteDefaultDateBasePath,
                    dataBaseConfigs = new List<DataBaseConfig>() {
                        new DataBaseConfig() {
                            Name = SqlDataInSqlite.SqliteDefaultDateBasePath,
                            WebDataCaptureTime =
                            DateTime.MinValue.ToString("yyyy-MM-dd HHH:mm:ss")
                        }
                    }
                };
            }
            else
            {
                if (loginConfigData.sqliteConfig.selectedDataBasePath == null)
                    loginConfigData.sqliteConfig.selectedDataBasePath =
                        SqlDataInSqlite.SqliteDefaultDateBasePath;
                if (loginConfigData.sqliteConfig.dataBaseConfigs == null)
                    loginConfigData.sqliteConfig.dataBaseConfigs =
                        new List<DataBaseConfig>() {
                            new DataBaseConfig() {
                                Name = SqlDataInSqlite.SqliteDefaultDateBasePath,
                                WebDataCaptureTime =
                                    DateTime.MinValue.ToString("yyyy-MM-dd HHH:mm:ss")
                        }
                    };
            }

            if (loginConfigData.crawlConfig == null)
            {
                loginConfigData.crawlConfig = new CrawlConfig
                {
                    IsCrawl = true,
                    IntervalDays = 10,
                    CrawlURL = CommonString.CrawlURL
                };
            }
            else
            {
                if (String.IsNullOrWhiteSpace(loginConfigData.crawlConfig.CrawlURL))
                    loginConfigData.crawlConfig.CrawlURL = CommonString.CrawlURL;
                if (loginConfigData.crawlConfig.IntervalDays < 1)
                    loginConfigData.crawlConfig.IntervalDays = 10;
            }

            if (loginConfigData.webServerConfig == null)
            {
                loginConfigData.webServerConfig = new WebServerConfig
                {
                    IsStartWebServer = true,
                    Port = 5555,
                    RowsPerPage = 20
                };
            }

            if (String.IsNullOrWhiteSpace(loginConfigData.mysqlConfig.selectedIP))
                loginConfigData.mysqlConfig.selectedIP = "127.0.0.1";

            if (!loginConfigData.mysqlConfig.hostIPs.Contains(
                loginConfigData.mysqlConfig.selectedIP))
                loginConfigData.mysqlConfig.hostIPs.Add(
                    loginConfigData.mysqlConfig.selectedIP);

            return loginConfigData;
        }

        public static void SaveXml(LoginConfigData loginConfigData, string xmlPath)
        {
            XmlSerializer ser = new XmlSerializer(typeof(LoginConfigData));
            using (FileStream fs = File.Create(xmlPath))
            {
                ser.Serialize(fs, loginConfigData);
            }
        }
    }
}
