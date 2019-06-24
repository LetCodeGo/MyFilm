using System;
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

            [XmlElement("MySQLConfig")]
            public MySQLConfig mysqlConfig;

            [XmlElement("SQLiteConfig")]
            public SQLiteConfig sqliteConfig;
        }

        private static LoginConfigData GetInitLoginConfigData()
        {
            LoginConfigData initLoginConfigData = new LoginConfigData()
            {
                dataBaseType = DataBaseType.MYSQL,
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
                    selectedDataBasePath = CommonString.SqliteDateBasePath,
                    dataBaseConfigs = new List<DataBaseConfig>() {
                        new DataBaseConfig() {
                            Name = CommonString.SqliteDateBasePath,
                            WebDataCaptureTime =
                            DateTime.MinValue.ToString("yyyy-MM-dd HHH:mm:ss")
                        }
                    }
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
                    selectedDataBasePath = CommonString.SqliteDateBasePath,
                    dataBaseConfigs = new List<DataBaseConfig>() {
                        new DataBaseConfig() {
                            Name = CommonString.SqliteDateBasePath,
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
                        CommonString.SqliteDateBasePath;
                if (loginConfigData.sqliteConfig.dataBaseConfigs == null)
                    loginConfigData.sqliteConfig.dataBaseConfigs =
                        new List<DataBaseConfig>() {
                            new DataBaseConfig() {
                                Name = CommonString.SqliteDateBasePath,
                                WebDataCaptureTime =
                                    DateTime.MinValue.ToString("yyyy-MM-dd HHH:mm:ss")
                        }
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
