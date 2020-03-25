using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyFilm
{
    public partial class SettingForm : Form
    {
        public Action<LoginConfig.CrawlConfig, LoginConfig.WebServerConfig>
            SettingFormApplyAction = null;

        public SettingForm(LoginConfig.CrawlConfig crawlConfig,
            LoginConfig.WebServerConfig webServerConfig,
            bool buttonApplyVisible = false)
        {
            InitializeComponent();

            this.cbIsCrawl.Checked = crawlConfig.IsCrawl;
            this.tbCrawlAddr.Text = crawlConfig.CrawlURL;
            this.tbIntervalDays.Text = crawlConfig.IntervalDays.ToString();

            this.cbStartWebServer.Checked = webServerConfig.IsStartWebServer;
            this.tbPort.Text = webServerConfig.Port.ToString();
            this.tbRowsPerPage.Text = webServerConfig.RowsPerPage.ToString();

            this.btnApply.Visible = buttonApplyVisible;
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            LoginConfig.CrawlConfig crawlConfig = new LoginConfig.CrawlConfig()
            {
                IsCrawl = this.cbIsCrawl.Checked,
                CrawlURL = this.tbCrawlAddr.Text.Trim(),
                IntervalDays = Convert.ToInt32(this.tbIntervalDays.Text)
            };
            LoginConfig.WebServerConfig webServerConfig = new LoginConfig.WebServerConfig()
            {
                IsStartWebServer = this.cbStartWebServer.Checked,
                Port = Convert.ToInt32(this.tbPort.Text),
                RowsPerPage = Convert.ToInt32(this.tbRowsPerPage.Text)
            };
            SettingFormApplyAction?.Invoke(crawlConfig, webServerConfig);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            btnApply_Click(null, null);
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
