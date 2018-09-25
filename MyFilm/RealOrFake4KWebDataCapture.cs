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
        public readonly static string webPageAddress =
            "https://digiraw.com/4K-UHD-ripping-service/the-real-or-fake-4K-list/";

        public static List<string> CrawlData(ref string errMsg)
        {
            HtmlWeb htmlWeb = new HtmlWeb();
            HtmlDocument document = null;

            try
            {
                document = htmlWeb.Load(webPageAddress);
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return null;
            }

            if (document == null) return null;

            var divLabels = document.DocumentNode.Descendants("div").Where(
                x => x.Attributes.Contains("class") &&
                x.Attributes["class"].Value == "stacks_in_4400_page34_accordion_label");
            var divContents = document.DocumentNode.Descendants("div").Where(
                x => x.Attributes.Contains("class") &&
                x.Attributes["class"].Value == "stacks_in_4400_page34_accordion_content");

            if (divLabels == null || divContents == null) return null;

            int divLabelCount = divLabels.Count();
            int divContentCount = divContents.Count();
            Debug.Assert(divLabelCount == divContentCount);

            List<string> resultStringList = new List<string>();

            foreach (HtmlNode divNode in divContents)
            {
                var lis = divNode.Descendants("li");
                if (lis == null) continue;

                foreach (HtmlNode liNode in lis)
                {
                    string tempText = liNode.InnerText.Trim().Replace("&rsquo;", "'");

                    if (!(string.IsNullOrWhiteSpace(tempText) ||
                        tempText.Contains("none just yet!")))
                    {
                        resultStringList.Add(tempText);
                    }
                }
            }

            return resultStringList;
        }
    }
}
