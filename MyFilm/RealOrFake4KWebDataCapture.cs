using System.Collections.Generic;
using System.Data;
using System.Linq;
using HtmlAgilityPack;
using System.Diagnostics;
using System;

namespace MyFilm
{
    public class RealOrFake4KWebDataCapture
    {
        public class RealOrFake4KWebDataCaptureResult
        {
            public int code;
            public string strMsg;
            public DateTime crawlTime;
        }

        public delegate void ThreadWebDataCaptureCallback(RealOrFake4KWebDataCaptureResult rst);
        public delegate void ThreadWebDataCaptureFinish();

        private ThreadWebDataCaptureCallback threadWebDataCaptureCallback = null;
        private ThreadWebDataCaptureFinish threadWebDataCaptureFinish = null;

        public RealOrFake4KWebDataCapture(ThreadWebDataCaptureCallback threadCallback,
            ThreadWebDataCaptureFinish threadFinish)
        {
            this.threadWebDataCaptureCallback = threadCallback;
            this.threadWebDataCaptureFinish = threadFinish;
        }

        private static DataTable CrawlData(ref string errMsg)
        {
            HtmlWeb htmlWeb = new HtmlWeb();
            HtmlDocument document = null;

            try
            {
                document = htmlWeb.Load(CommonString.CrawlURL);
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return null;
            }

            if (document == null) return null;

            var tableHeadContents = document.DocumentNode.SelectSingleNode(
                "//*[@id='grid-iron-stacks_in_4955_page34']/thead");
            var tableBodyContents = document.DocumentNode.SelectSingleNode(
                "//*[@id='grid-iron-stacks_in_4955_page34']/tbody");

            if (tableHeadContents == null || tableBodyContents == null) return null;
            var tableHeadTr = tableHeadContents.SelectNodes(".//tr");
            Debug.Assert(tableHeadTr.Count == 1);

            DataTable dt = new DataTable();
            foreach (var th in tableHeadTr[0].SelectNodes(".//th"))
            {
                dt.Columns.Add(th.InnerText.Trim(), typeof(string));
            }
            Debug.Assert(dt.Columns.Count == 6);

            var tableBodyTr = tableBodyContents.SelectNodes(".//tr");
            foreach (HtmlNode rowNode in tableBodyTr)
            {
                DataRow dr = dt.NewRow();
                int i = 0;
                foreach (var td in rowNode.SelectNodes(".//td"))
                {
                    dr[i++] = td.InnerText.Replace("&amp;", "&").Trim();
                }
                dt.Rows.Add(dr);
            }

            return dt;
        }

        /// <summary>
        /// -1 从网页抓取数据失败，0 从网页抓取数据条数和数据库相同只更新时间
        /// 返回抓取数据条数
        /// </summary>
        /// <param name="strMsg">输出信息，用于显示</param>
        /// <param name="crawlTime">抓取时间</param>
        /// <returns></returns>
        public void Update4KInfo()
        {
            RealOrFake4KWebDataCaptureResult rst = new RealOrFake4KWebDataCaptureResult();
            rst.crawlTime = DateTime.Now;

            string errMsg = "";
            DataTable crawlDt = CrawlData(ref errMsg);
            if (crawlDt == null || crawlDt.Rows.Count == 0)
            {
                rst.strMsg = string.Format("从网页\n{0}\n抓取数据失败\n{1}",
                    CommonString.CrawlURL, errMsg);
                rst.code = -1;
                this.threadWebDataCaptureCallback?.Invoke(rst);
                this.threadWebDataCaptureFinish?.Invoke();
                return;
            }

            int diskCount = SqlData.GetSqlData().CountRowsOfDiskFromFilmInfo(
                CommonString.RealOrFake4KDiskName);
            if ((diskCount - 1) >= crawlDt.Rows.Count)
            {
                // 更新时间
                int affectedCount =
                    SqlData.GetSqlData().UpdateDiskRealOrFake4KInModifyTimeFromDiskInfo(
                        rst.crawlTime);
                Debug.Assert(diskCount == affectedCount);

                rst.strMsg = string.Format(
                    "从网页\n{0}\n抓取数据条数 {1} 小于或等于数据库已存在条数 {2}\n不更新数据库信息",
                    CommonString.CrawlURL, crawlDt.Rows.Count, diskCount - 1);
                rst.code = 0;
                this.threadWebDataCaptureCallback?.Invoke(rst);
                this.threadWebDataCaptureFinish?.Invoke();
                return;
            }

            SqlData.GetSqlData().DeleteByDiskDescribeFromFilmInfo(CommonString.RealOrFake4KDiskName);

            int maxId = SqlData.GetSqlData().GetMaxIdOfFilmInfo();
            int startId = maxId + 1;
            int diskId = startId;

            DataTable dt = CommonDataTable.GetFilmInfoDataTable();

            DataRow drDisk = dt.NewRow();
            drDisk["id"] = startId++;
            drDisk["name"] = CommonString.RealOrFake4KDiskName;
            drDisk["path"] = "------";
            drDisk["size"] = -1;
            drDisk["create_t"] = rst.crawlTime;
            drDisk["modify_t"] = rst.crawlTime;
            drDisk["is_folder"] = true;
            drDisk["to_watch"] = false;
            drDisk["to_watch_ex"] = false;
            drDisk["s_w_t"] = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
            drDisk["to_delete"] = false;
            drDisk["to_delete_ex"] = false;
            drDisk["s_d_t"] = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
            drDisk["content"] = String.Empty;
            drDisk["pid"] = -1;
            drDisk["max_cid"] = startId - 1 + crawlDt.Rows.Count;
            drDisk["disk_desc"] = CommonString.RealOrFake4KDiskName;
            dt.Rows.Add(drDisk);

            foreach (DataRow crawlDr in crawlDt.Rows)
            {
                DataRow dr = dt.NewRow();
                dr["id"] = startId++;
                dr["name"] = string.Join(" | ", crawlDr.ItemArray.Cast<string>().
                    Where(x => !string.IsNullOrWhiteSpace(x)));
                dr["path"] = "------";
                dr["size"] = -1;
                dr["create_t"] = rst.crawlTime;
                dr["modify_t"] = rst.crawlTime;
                dr["is_folder"] = false;
                dr["to_watch"] = false;
                dr["to_watch_ex"] = false;
                dr["s_w_t"] = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
                dr["to_delete"] = false;
                dr["to_delete_ex"] = false;
                dr["s_d_t"] = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
                dr["content"] = String.Empty;
                dr["pid"] = diskId;
                dr["max_cid"] = startId - 1;
                dr["disk_desc"] = CommonString.RealOrFake4KDiskName;
                dt.Rows.Add(dr);
            }

            SqlData.GetSqlData().InsertDataToFilmInfo(dt);

            rst.strMsg = string.Format("从网页\n{0}\n抓取数据 {1} 条，已写入数据库",
                CommonString.CrawlURL, crawlDt.Rows.Count);
            rst.code = crawlDt.Rows.Count;
            this.threadWebDataCaptureCallback?.Invoke(rst);
            this.threadWebDataCaptureFinish?.Invoke();
        }
    }
}
