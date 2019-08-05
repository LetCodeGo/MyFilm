using HttpServer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MyFilm
{
    public class WebServer : HttpServer.HttpServer
    {
        private static readonly int PageItemCount = 20;

        private string SearchKeyWord = "";
        private int DataBaseId = -1;
        private int DataBasePid = -2;

        private int TotalItemCount = 0;
        private int PageCount = 0;
        private int PageIndex = -1;
        private int ItemIndex = -1;

        private int UpFolderId = -2;
        private int CurrentFolderId = -1;

        private string TitleSuffix = "";

        private Regex iconRegex = new Regex(@"^icon\.(\w*)\.ico$");
        private Dictionary<string, byte[]> ImageBytesDic = new Dictionary<string, byte[]>();
        private Dictionary<string, byte[]> ExistResourcesBytesDic = new Dictionary<string, byte[]>();

        private static readonly string[] ColumnNames =
            new string[] { "索引", "名称", "路径", "大小", "修改日期", "磁盘" };
        private static readonly string[] ColumnNamesInDataTable =
            new string[] { "index", "name", "path", "size", "modify_t", "disk_desc" };
        private static readonly string[] ColumnClassHeaders =
            new string[] { "indexheader", "nameheader", "pathheader",
                "sizeheader", "modifiedheader", "discheader" };
        private static readonly string[] ColumnClassDatas =
            new string[] { "indexdata", "namedata", "pathdata",
                "sizedata", "modifieddata", "discdata" };

        private DataTable gridData = null;
        private QueryDataType queryDataType = QueryDataType.DISK_ROOT;

        private enum QueryDataType
        {
            DISK_ROOT,
            SEARCH,
            DATABASE_ID,
            DATABASE_PID
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="ipAddress">IP地址</param>
        /// <param name="port">端口号</param>
        public WebServer(string ipAddress, int port)
            : base(ipAddress, port)
        {
            if (CommonString.DataBaseType == LoginConfig.DataBaseType.MYSQL)
            {
                TitleSuffix = String.Format("[MyFilm v{0}][MySQL]",
                    System.Windows.Forms.Application.ProductVersion);
            }
            else if (CommonString.DataBaseType == LoginConfig.DataBaseType.SQLITE)
            {
                TitleSuffix = String.Format("[MyFilm v{0}][SQLite]",
                    System.Windows.Forms.Application.ProductVersion);
            }

            ExistResourcesBytesDic.Add("chara.png",
                Helper.ImageToBytes(Properties.Resources.chara, ImageFormat.Png));
            ExistResourcesBytesDic.Add("favicon.ico",
                Helper.IconToBytes(Properties.Resources.Film));
            ExistResourcesBytesDic.Add("file.gif",
                Helper.ImageToBytes(Properties.Resources.file, ImageFormat.Gif));
            ExistResourcesBytesDic.Add("folder.gif",
                Helper.ImageToBytes(Properties.Resources.folder, ImageFormat.Gif));
            ExistResourcesBytesDic.Add("up.gif",
                Helper.ImageToBytes(Properties.Resources.up, ImageFormat.Gif));
            ExistResourcesBytesDic.Add("updir.gif",
                Helper.ImageToBytes(Properties.Resources.updir, ImageFormat.Gif));
            ExistResourcesBytesDic.Add("myfilm.png",
                Helper.ImageToBytes(Properties.Resources.myfilm, ImageFormat.Png));
            ExistResourcesBytesDic.Add("warn.png",
                Helper.ImageToBytes(Properties.Resources.warn, ImageFormat.Png));

            ExistResourcesBytesDic.Add("main.css",
                Encoding.UTF8.GetBytes(Properties.Resources.main_css));
        }

        public override void OnPost(HttpRequest request, HttpResponse response)
        {
            //获取客户端传递的参数
            string data = request.Params == null ? "" : string.Join(";", request.Params.Select(x => x.Key + "=" + x.Value).ToArray());

            //设置返回信息
            string content = string.Format("这是通过Post方式返回的数据:{0}", data);

            //构造响应报文
            response.SetContent(content);
            response.Content_Encoding = "utf-8";
            response.StatusCode = "200";
            response.Content_Type = "text/html; charset=UTF-8";
            response.Headers["Server"] = "ExampleServer";

            //发送响应
            response.Send();
        }

        public override void OnGet(HttpRequest request, HttpResponse response)
        {
            bool BadURL = false;

            // 查询
            if (request.URL.StartsWith("/?") || request.URL == "/")
            {
                SearchKeyWord = "";
                DataBaseId = -1;
                DataBasePid = -2;

                TotalItemCount = 0;
                PageCount = 0;
                PageIndex = -1;
                ItemIndex = -1;

                UpFolderId = -2;
                CurrentFolderId = -1;

                int offset = 0;

                bool SearchExist = false;
                bool DataBaseIdExist = false;
                bool DataBasePidExist = false;
                bool OffsetExist = false;

                if (request.Params != null)
                {
                    SearchExist = request.Params.ContainsKey("search");
                    DataBaseIdExist = request.Params.ContainsKey("databaseid");
                    DataBasePidExist = request.Params.ContainsKey("databasepid");
                    OffsetExist = request.Params.ContainsKey("offset");
                }

                // 搜索或查询ID不能同时进行
                if ((SearchExist && DataBaseIdExist) ||
                    (SearchExist && DataBasePidExist) ||
                    (DataBasePidExist && DataBaseIdExist)) BadURL = true;
                else if (DataBaseIdExist && (!Int32.TryParse(request.Params["databaseid"], out DataBaseId)))
                    BadURL = true;
                else if (DataBasePidExist && (!Int32.TryParse(request.Params["databasepid"], out DataBasePid)))
                    BadURL = true;
                else if (OffsetExist && (!Int32.TryParse(request.Params["offset"], out offset)))
                    BadURL = true;
                else if ((DataBaseIdExist && DataBaseId < 0) ||
                    (OffsetExist && offset < 0) ||
                    (DataBasePidExist && DataBasePid < -1)) BadURL = true;

                if (!BadURL)
                {
                    if (SearchExist) SearchKeyWord = request.Params["search"];
                    if (offset >= 0)
                    {
                        PageIndex = offset / PageItemCount;
                        ItemIndex = offset % PageItemCount;
                    }

                    DataTable diskRootDataTable = SqlData.GetSqlData().DiskRootDataTable;

                    if ((!string.IsNullOrWhiteSpace(SearchKeyWord)) ||
                        (DataBaseIdExist && DataBaseId >= 0) ||
                        (DataBasePidExist && DataBasePid >= -1))
                    {
                        if (DataBaseIdExist && DataBaseId >= 0)
                        {
                            queryDataType = QueryDataType.DATABASE_ID;
                            DataTable dt = SqlData.GetSqlData().GetDataByIdFromFilmInfo(DataBaseId);

                            if (dt != null && dt.Rows.Count == 1)
                            {
                                gridData = CommonDataTable.ConvertFilmInfoToGrid(dt);
                                TotalItemCount = 1;
                            }
                            else BadURL = true;
                        }
                        else
                        {
                            int[] idList = null;
                            if (DataBasePidExist && DataBasePid >= -1)
                            {
                                queryDataType = QueryDataType.DATABASE_PID;
                                idList = SqlData.GetSqlData().GetDataByPidFromFilmInfo(DataBasePid);
                                if (idList != null) TotalItemCount = idList.Length;
                            }
                            else
                            {
                                queryDataType = QueryDataType.SEARCH;
                                idList = SqlData.GetSqlData().SearchKeyWordFromFilmInfo(SearchKeyWord);
                                if (idList != null) TotalItemCount = idList.Length;

                                SqlData.GetSqlData().InsertDataToSearchLog(
                                    SearchKeyWord, TotalItemCount, DateTime.Now);
                            }

                            if (offset == 0 && TotalItemCount == 0)
                                gridData = diskRootDataTable.Clone();
                            else if (offset >= 0 && offset < TotalItemCount)
                            {
                                DataTable dt = SqlData.GetSqlData().SelectDataByIDList(
                                    Helper.ArraySlice(idList, PageIndex * PageItemCount, PageItemCount));
                                gridData = CommonDataTable.ConvertFilmInfoToGrid(dt);
                            }
                            else BadURL = true;
                        }
                    }
                    else
                    {
                        queryDataType = QueryDataType.DISK_ROOT;
                        TotalItemCount = diskRootDataTable.Rows.Count;

                        if (offset == 0 && TotalItemCount == 0)
                        {
                            gridData = diskRootDataTable.Clone();
                        }
                        else if (offset >= 0 && offset < TotalItemCount)
                        {
                            gridData = diskRootDataTable
                                .AsEnumerable()
                                .Where((row, index) => index >= PageIndex * PageItemCount && index < (PageIndex + 1) * PageItemCount)
                                .CopyToDataTable();
                        }
                        else BadURL = true;
                    }

                    if (!BadURL)
                    {
                        string strTitle = SearchKeyWord;
                        PageCount = TotalItemCount / PageItemCount +
                            (TotalItemCount % PageItemCount == 0 ? 0 : 1);

                        if (queryDataType == QueryDataType.DISK_ROOT) strTitle = "Index";
                        else if (queryDataType == QueryDataType.DATABASE_ID)
                        {
                            CurrentFolderId = Convert.ToInt32(gridData.Rows[0]["pid"]);
                            if (CurrentFolderId >= 0)
                            {
                                DataTable dt = SqlData.GetSqlData().GetDataByIdFromFilmInfo(CurrentFolderId);
                                UpFolderId = Convert.ToInt32(dt.Rows[0]["pid"]);
                            }
                            strTitle = string.Format("ID[{0}]", DataBaseId);
                        }
                        else if (queryDataType == QueryDataType.DATABASE_PID)
                        {
                            CurrentFolderId = DataBasePid;
                            if (CurrentFolderId >= 0)
                            {
                                DataTable dt = SqlData.GetSqlData().GetDataByIdFromFilmInfo(CurrentFolderId);
                                UpFolderId = Convert.ToInt32(dt.Rows[0]["pid"]);
                            }
                            strTitle = string.Format("PID[{0}]", DataBasePid);
                        }

                        StringBuilder sb = new StringBuilder("<head>\n", 2048);
                        sb.AppendFormat("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"><meta name=\"viewport\" content=\"width=512\"><title>{0}</title>\n",
                            string.Format("{0} - {1}", strTitle, TitleSuffix));
                        sb.AppendLine("<link rel=\"stylesheet\" href=\"/main.css\" type=\"text/css\">");
                        sb.AppendLine("<link rel=\"shortcut icon\" href=\"/favicon.ico\" type=\"image/x-icon\">");
                        sb.AppendLine("</head>");

                        SetBodyHtml(ref sb);

                        response.SetContent(sb.ToString());
                        response.StatusCode = "200";
                        response.Content_Type = "text/html; charset=UTF-8";
                    }
                }
            }
            // 文件
            else
            {
                string requestURL = request.URL.TrimStart(new char[] { '/' });
                Match match = iconRegex.Match(requestURL);
                if (match.Success && (!string.IsNullOrWhiteSpace(match.Groups[1].Value)))
                {
                    string strTemp = string.Format("*.{0}", match.Groups[1].Value);
                    byte[] iconBytes = null;
                    if (ImageBytesDic.ContainsKey(strTemp))
                    {
                        iconBytes = ImageBytesDic[strTemp];
                    }
                    else
                    {
                        try
                        {
                            iconBytes = Helper.IconToBytes(IconReader.GetFileIcon(
                                strTemp, IconReader.IconSize.Small, false));
                        }
                        catch
                        {
                            iconBytes = ExistResourcesBytesDic["warn.png"];
                        }
                        ImageBytesDic.Add(strTemp, iconBytes);
                    }

                    response.SetContent(iconBytes);
                    response.StatusCode = "200";
                    response.Content_Type = "text/html; charset=UTF-8";
                }
                else if (ExistResourcesBytesDic.ContainsKey(requestURL))
                {
                    response.SetContent(ExistResourcesBytesDic[requestURL]);
                    response.StatusCode = "200";
                    response.Content_Type = "text/html; charset=UTF-8";
                }
                else
                {
                    string requestFile = Path.Combine(ServerRoot, requestURL);

                    //判断地址中是否存在扩展名
                    string extension = Path.GetExtension(requestFile);

                    //根据有无扩展名按照两种不同链接进行处
                    if (extension != "")
                    {
                        //从文件中返回HTTP响应
                        response = response.FromFile(requestFile);
                    }
                    else BadURL = true;
                }
            }

            if (BadURL) SetPageNotFindHtml(ref response);

            //发送HTTP响应
            response.Send();
        }

        public override void OnDefault(HttpRequest request, HttpResponse response)
        {

        }

        private void SetBodyHtml(ref StringBuilder sb)
        {
            string urlEncodedSearch = Uri.EscapeDataString(SearchKeyWord);

            sb.AppendLine("<body><center><br>");
            sb.AppendFormat("<a href=\"/\"><img class=\"logo\" src=\"/myfilm.png\" alt=\"{0}\"></a>", TitleSuffix);
            sb.AppendLine("<br><br>");
            sb.AppendFormat("<form id=\"searchform\" action=\"/\" method=\"get\"><input class=\"searchbox\" style=\"width:480px\" id=\"search\" name=\"search\" type=\"text\" title=\"搜索 {0}\" value=\"{1}\" ></form>",
                TitleSuffix, string.IsNullOrWhiteSpace(SearchKeyWord) ? "" : SearchKeyWord);
            SetTableHtml(ref sb);
            sb.AppendLine("</center>");

            string hrefData = "";
            if (queryDataType == QueryDataType.DATABASE_ID)
                hrefData = string.Format("databaseid={0}&amp;", DataBaseId);
            else if (queryDataType == QueryDataType.DATABASE_PID)
                hrefData = string.Format("databasepid={0}&amp;", DataBasePid);
            else if (queryDataType == QueryDataType.SEARCH)
                hrefData = string.Format("search={0}&amp;", urlEncodedSearch);
            if (PageCount > 1) SetCutPageHtml(ref sb, hrefData);

            sb.AppendLine("</body>");
        }

        private void SetTableHtml(ref StringBuilder sb)
        {
            string explain1 = String.Format(
                "总共 {0} 条记录，当前第 {1} 页，共 {2} 页",
                TotalItemCount, PageIndex + 1, PageCount);
            string explain2 = "";
            switch (queryDataType)
            {
                case QueryDataType.SEARCH:
                    explain2 = string.Format("搜索 \'{0}\'", SearchKeyWord);
                    break;
                case QueryDataType.DISK_ROOT:
                    explain2 = "索引 根目录";
                    break;
                case QueryDataType.DATABASE_ID:
                    explain2 = string.Format("查询 数据库中 id 为 {0} 的数据", DataBaseId);
                    break;
                case QueryDataType.DATABASE_PID:
                    explain2 = string.Format("查询 数据库中所有 pid 为 {0} 的数据", DataBasePid);
                    break;
                default:
                    explain2 = "索引 根目录";
                    break;
            }

            sb.AppendLine("<table cellspacing=\"0\" width=\"480px\">");
            sb.AppendFormat(
                "<tr><td colspan=\"6\"><p class=\"numresults\">{0} （{1}）</p></td></tr>\n",
                explain1, explain2);

            if (queryDataType == QueryDataType.DATABASE_ID || queryDataType == QueryDataType.DATABASE_PID)
            {
                string href = "/";
                // -1 是为根目录，不显示上一目录
                if (UpFolderId >= 0)
                {
                    int[] idList = SqlData.GetSqlData().GetDataByPidFromFilmInfo(UpFolderId);
                    href = string.Format("/?databasepid={0}&amp;offset={1}",
                        UpFolderId, Array.IndexOf(idList, CurrentFolderId));
                }

                sb.AppendFormat(
                    "<tr><td class=\"updir\" colspan=\"6\"><a href=\"{0}\"><img class=\"icon\" src=\"/updir.gif\" alt=\"\">上一目录...</a></td></tr>\n",
                    href);
            }

            sb.Append("<tr>");
            for (int i = 0; i < ColumnNames.Length; i++)
            {
                sb.AppendFormat("<td class=\"{0}\"><span class=\"nobr\"><nobr>{1}</nobr></span></td>",
                    ColumnClassHeaders[i], ColumnNames[i]);
            }
            sb.AppendLine("</tr>");

            sb.AppendLine("<tr><td colspan=\"6\" class=\"lineshadow\" height=\"1\"></td></tr>");

            int startIndex = PageIndex * PageItemCount;

            for (int i = 0; i < gridData.Rows.Count; i++, startIndex++)
            {
                string rowDataClass = (i == ItemIndex) ? "offsetrow" : (i % 2 == 0 ? "trdata1" : "trdata2");
                sb.AppendFormat("<tr class=\"{0}\">", rowDataClass);

                string strStyle = "";
                string strTdStyleAndClass = "";

                for (int j = 0; j < 6; j++)
                {
                    if (i == ItemIndex)
                    {
                        if (j == 0) strStyle = "style=\"border-left: thin solid; border-top: thin solid; border-bottom: thin solid; \"";
                        else if (j == 5) strStyle = "style=\"border-top: thin solid; border-bottom: thin solid; border-right: thin solid; \"";
                        else strStyle = "style=\"border-top: thin solid; border-bottom: thin solid; \"";
                    }
                    strTdStyleAndClass = string.Format("{0}class=\"{1}\"", strStyle, ColumnClassDatas[j]);

                    if (j == 1)
                    {
                        if (Convert.ToBoolean(gridData.Rows[i]["is_folder"]))
                        {
                            sb.AppendFormat("<td {0}><span class=\"nobr\"><nobr><a href=\"/?databasepid={1}\"><img class=\"icon\" src=\"/folder.gif\" alt=\"\">{2}</a></nobr></span></td>",
                                strTdStyleAndClass, gridData.Rows[i]["id"], gridData.Rows[i]["name"]);
                        }
                        else
                        {
                            string icoPath = "file.gif";
                            if (gridData.Rows[i]["disk_desc"].ToString() == CommonString.RealOrFake4KDiskName)
                                icoPath = "chara.png";
                            else
                            {
                                string nameExtenSion = Path.GetExtension(gridData.Rows[i]["name"].ToString());
                                if (!string.IsNullOrWhiteSpace(nameExtenSion))
                                    icoPath = string.Format("icon{0}.ico", nameExtenSion);
                            }

                            sb.AppendFormat("<td {0}><span class=\"nobr\"><nobr><img class=\"icon\" src=\"/{1}\" alt=\"\">{2}</nobr></span></td>",
                                strTdStyleAndClass, icoPath, gridData.Rows[i]["name"]);
                        }
                    }
                    else if (j == 2)
                    {
                        if (Convert.ToInt32(gridData.Rows[i]["pid"]) >= 0)
                        {
                            sb.AppendFormat("<td {0}><span class=\"nobr\"><nobr><a href=\"/?databasepid={1}\"><span class=\"nobr\"><nobr>{2}</nobr></span></a></td>",
                                strTdStyleAndClass, gridData.Rows[i]["pid"], gridData.Rows[i]["path"]);
                        }
                        else
                        {
                            sb.AppendFormat("<td {0}><span class=\"nobr\"><nobr><span class=\"nobr\"><nobr>{1}</nobr></span></td>",
                                strTdStyleAndClass, gridData.Rows[i]["path"]);
                        }
                    }
                    else
                    {
                        sb.AppendFormat("<td {0}><span class=\"nobr\"><nobr>{1}</nobr></span></td>",
                            strTdStyleAndClass, j == 0 ? startIndex : gridData.Rows[i][ColumnNamesInDataTable[j]]);
                    }
                }
                sb.AppendLine("</tr>");
            }

            sb.AppendLine("</table>");
        }

        private void SetCutPageHtml(ref StringBuilder sb, string hrefData)
        {
            bool showPrePage = (PageIndex > 0 && PageIndex <= (PageCount - 1));
            bool showNextPage = (PageIndex >= 0 && PageIndex < (PageCount - 1));

            int offset = PageIndex * PageItemCount + ItemIndex;

            sb.AppendLine("<center><br>");
            if (showPrePage)
            {
                sb.AppendFormat("<span class=\"prevnext\"><a href=\"/?{0}offset={1}\">&lt; 上一页</a></span>",
                    hrefData, Math.Max(offset - PageItemCount, 0));
            }

            // < 上一个 1 2 3 4 5 6 7 8 9 10 11 下一个 >
            if (PageCount < 12)
            {
                for (int i = 0; i < PageCount; i++)
                {
                    if (i == PageIndex)
                    {
                        sb.AppendFormat("<span class=\"nav\"><b>{0}</b></span>", i + 1);
                    }
                    else
                    {
                        sb.AppendFormat(
                            "<span class=\"nav\"><a class=num href=\"/?{0}offset={1}\">{2}</a></span>",
                            hrefData, i * PageItemCount, i + 1);
                    }
                }
            }
            else
            {
                bool afterEllipsis = (PageIndex < (PageCount - 3));
                bool beforeEllipsis = (PageIndex >= 3);

                sb.AppendFormat(
                    "<span class=\"nav\"><a class=num href=\"/?{0}offset=0\">1</a></span>",
                     hrefData);
                if (beforeEllipsis) sb.Append("<span class=\"nav\">...</span>");
                for (int i = Math.Max(PageIndex - 1, 1); i <= Math.Min(PageIndex + 1, PageCount - 2); i++)
                {
                    if (i == PageIndex)
                    {
                        sb.AppendFormat("<span class=\"nav\"><b>{0}</b></span>", i + 1);
                    }
                    else
                    {
                        sb.AppendFormat(
                            "<span class=\"nav\"><a class=num href=\"/?{0}offset={1}\">{2}</a></span>",
                            hrefData, i * PageItemCount, i + 1);
                    }
                }
                if (afterEllipsis) sb.Append("<span class=\"nav\">...</span>");
                sb.AppendFormat(
                    "<span class=\"nav\"><a class=num href=\"/?{0}offset={1}\">{2}</a></span>",
                    hrefData, (PageCount - 1) * PageItemCount, PageCount);
            }

            if (showNextPage)
            {
                sb.AppendFormat("<span class=\"prevnext\"><a href=\"/?{0}offset={1}\">下一页 &gt;</a></span>",
                    hrefData, Math.Min(offset + PageItemCount, TotalItemCount - 1));
            }
            sb.AppendLine("</center>");
        }

        private void SetPageNotFindHtml(ref HttpResponse response)
        {
            response.SetContent("<html><body><h1>404 - Page Not Found</h1></body></html>");
            response.StatusCode = "404";
            response.Content_Type = "text/html";
        }

        private string ConvertPath(string[] urls)
        {
            string html = string.Empty;
            int length = ServerRoot.Length;
            foreach (var url in urls)
            {
                var s = url.StartsWith("..") ? url : url.Substring(length).TrimEnd('\\');
                html += String.Format("<li><a href=\"{0}\">{0}</a></li>", s);
            }

            return html;
        }

        private string ListDirectory(string requestDirectory, string requestURL)
        {
            //列举子目录
            var folders = requestURL.Length > 1 ? new string[] { "../" } : new string[] { };
            folders = folders.Concat(Directory.GetDirectories(requestDirectory)).ToArray();
            var foldersList = ConvertPath(folders);

            //列举文件
            var files = Directory.GetFiles(requestDirectory);
            var filesList = ConvertPath(files);

            //构造HTML
            StringBuilder builder = new StringBuilder();
            builder.Append(string.Format("<html><head><title>{0}</title></head>", requestDirectory));
            builder.Append(string.Format("<body><h1>{0}</h1><br/><ul>{1}{2}</ul></body></html>",
                 requestURL, filesList, foldersList));

            return builder.ToString();
        }
    }
}
