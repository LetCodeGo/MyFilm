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
            FILE,
            QUERY
        }

        public enum QueryTypeEnum
        {
            DISK_ROOT,
            SEARCH,
            DATABASE_ID,
            DATABASE_PID,
            TO_DELETE_BY_TIME,
            TO_DELETE_BY_DISK,
            TO_WATCH
        }

        private static readonly Regex CheckRegex =
            new Regex(@"^/(todeletebytime|todeletebydisk|towatch)?(\?(.+))?$");

        private static readonly string[] ParamKeys =
            new string[] { "search", "databaseid", "databasepid", "diskdesc" };

        public bool IsValid { get; private set; }

        public RequestTypeEnum RequestType { get; private set; }
        public QueryTypeEnum QueryType { get; private set; }

        public string SearchKeyWord { get; private set; }
        public string DiskDescribe { get; set; }
        public int DataBaseId { get; private set; }
        public int DataBasePid { get; private set; }
        public int Offset { get; private set; }

        public RequestURL(string rawURL)
        {
            string URL = Uri.UnescapeDataString(rawURL);

            IsValid = true;

            RequestType = RequestTypeEnum.QUERY;
            QueryType = QueryTypeEnum.DISK_ROOT;

            SearchKeyWord = "";
            DiskDescribe = "";
            DataBaseId = -2;
            DataBasePid = -2;
            Offset = 0;

            if (URL == "/")
            {
                RequestType = RequestTypeEnum.QUERY;
                QueryType = QueryTypeEnum.DISK_ROOT;

                DataBasePid = -1;
            }
            else
            {
                Match match = CheckRegex.Match(rawURL);
                if (match.Success)
                {
                    RequestType = RequestTypeEnum.QUERY;

                    switch (match.Groups[1].Value)
                    {
                        case "todeletebytime":
                            QueryType = QueryTypeEnum.TO_DELETE_BY_TIME;
                            break;
                        case "todeletebydisk":
                            QueryType = QueryTypeEnum.TO_DELETE_BY_DISK;
                            break;
                        case "towatch":
                            QueryType = QueryTypeEnum.TO_WATCH;
                            break;
                        default:
                            break;
                    }

                    List<string> queryKeyList = new List<string>();
                    string[] kvs = match.Groups[3].Value.Split(
                        new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string kv in kvs)
                    {
                        int index = kv.IndexOf('=');
                        if (index > 0 && index < kv.Length - 1)
                        {
                            string s1 = Uri.UnescapeDataString(kv.Substring(0, index));
                            string s2 = Uri.UnescapeDataString(kv.Substring(index + 1));

                            if (queryKeyList.Contains(s1)) { IsValid = false; return; }
                            else queryKeyList.Add(s1);

                            switch (s1)
                            {
                                case "offset":
                                    try { Offset = Convert.ToInt32(s2); }
                                    catch { IsValid = false; return; }

                                    if (Offset < 0) { IsValid = false; return; }
                                    break;
                                case "diskdesc":
                                    if (queryKeyList.Contains("databaseid") ||
                                        queryKeyList.Contains("databasepid"))
                                    { IsValid = false; return; }
                                    else DiskDescribe = s2;
                                    break;
                                case "search":
                                    if (queryKeyList.Contains("databaseid") ||
                                        queryKeyList.Contains("databasepid"))
                                    { IsValid = false; return; }
                                    else
                                    {
                                        QueryType = QueryTypeEnum.SEARCH;
                                        SearchKeyWord = s2;
                                    }
                                    break;
                                case "databaseid":
                                    if (queryKeyList.Contains("diskdesc") ||
                                        queryKeyList.Contains("search") ||
                                        queryKeyList.Contains("databasepid"))
                                    { IsValid = false; return; }
                                    else
                                    {
                                        QueryType = QueryTypeEnum.DATABASE_ID;
                                        try { DataBaseId = Convert.ToInt32(s2); }
                                        catch { IsValid = false; return; }
                                        if (DataBaseId < 0) { IsValid = false; return; }
                                    }
                                    break;
                                case "databasepid":
                                    if (queryKeyList.Contains("diskdesc") ||
                                        queryKeyList.Contains("search") ||
                                        queryKeyList.Contains("databaseid"))
                                    { IsValid = false; return; }
                                    else
                                    {
                                        QueryType = QueryTypeEnum.DATABASE_PID;
                                        try { DataBasePid = Convert.ToInt32(s2); }
                                        catch { IsValid = false; return; }
                                        if (DataBasePid < -1) { IsValid = false; return; }
                                    }
                                    break;
                                default:
                                    IsValid = false;
                                    return;
                            }
                        }
                        else { IsValid = false; return; }
                    }
                }
                else RequestType = RequestTypeEnum.FILE;
            }
        }
    }
}
