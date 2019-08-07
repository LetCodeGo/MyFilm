using HttpServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MyFilm
{
    public class RequestURL
    {
        public enum RequestTypeEnum
        {
            INDEX,
            FILE,
            QUERY
        }

        public enum QueryTypeEnum
        {
            DISK_ROOT,
            SEARCH,
            DATABASE_ID,
            DATABASE_PID
        }

        private static readonly string[] ParamKeys =
            new string[] { "search", "databaseid", "databasepid" };

        public bool IsValid { get; private set; }

        public RequestTypeEnum RequestType { get; private set; }
        public QueryTypeEnum QueryType { get; private set; }

        public string SearchKeyWord { get; private set; }
        public int DataBaseId { get; private set; }
        public int DataBasePid { get; private set; }
        public int Offset { get; private set; }

        public RequestURL(string rawURL)
        {
            string URL = Uri.UnescapeDataString(rawURL);

            IsValid = true;

            RequestType = RequestTypeEnum.INDEX;
            QueryType = QueryTypeEnum.DISK_ROOT;

            SearchKeyWord = "";
            DataBaseId = -2;
            DataBasePid = -2;
            Offset = 0;

            if (URL == "/")
            {
                RequestType = RequestTypeEnum.INDEX;
                QueryType = QueryTypeEnum.DISK_ROOT;

                DataBasePid = -1;
            }
            else
            {
                if (URL.StartsWith("/?"))
                {
                    RequestType = RequestTypeEnum.QUERY;

                    string strType = "";
                    string strValue = "";
                    string[] kvs = rawURL.Substring(2).Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string kv in kvs)
                    {
                        int index = kv.IndexOf('=');
                        if (index > 0 && index < kv.Length - 1)
                        {
                            string s1 = Uri.UnescapeDataString(kv.Substring(0, index));
                            string s2 = Uri.UnescapeDataString(kv.Substring(index + 1));

                            if (s1 == "offset")
                            {
                                try { Offset = Convert.ToInt32(s2); }
                                catch { IsValid = false; return; }

                                if (Offset < 0) { IsValid = false; return; }
                            }
                            else if (ParamKeys.Contains(s1))
                            {
                                if (strType == "")
                                {
                                    strType = s1;
                                    strValue = s2;
                                }
                                else { IsValid = false; return; }
                            }
                        }
                        else { IsValid = false; return; }
                    }

                    switch (strType)
                    {
                        case "search":
                            QueryType = QueryTypeEnum.SEARCH;
                            SearchKeyWord = strValue;
                            break;
                        case "databaseid":
                            QueryType = QueryTypeEnum.DATABASE_ID;
                            try { DataBaseId = Convert.ToInt32(strValue); }
                            catch { IsValid = false; return; }
                            if (DataBaseId < 0) IsValid = false;
                            break;
                        case "databasepid":
                            QueryType = QueryTypeEnum.DATABASE_PID;
                            try { DataBasePid = Convert.ToInt32(strValue); }
                            catch { IsValid = false; return; }
                            if (DataBasePid < -1) IsValid = false;
                            break;
                        case "":
                            RequestType = RequestTypeEnum.INDEX;
                            QueryType = QueryTypeEnum.DISK_ROOT;

                            DataBasePid = -1;
                            break;
                        default:
                            IsValid = false;
                            break;
                    }
                }
                else RequestType = RequestTypeEnum.FILE;
            }
        }
    }
}
