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
            this.tbPort.SetTextMaxLength(5);
            this.tbPort.SetMinValue(1024);
            this.tbPort.SetMaxValue(65535);

            this.cbIsCrawl.Checked = crawlConfig.IsCrawl;
            this.tbCrawlAddr.Text = crawlConfig.CrawlURL;
            this.tbIntervalDays.Text = crawlConfig.IntervalDays.ToString();

            this.cbStartWebServer.Checked = webServerConfig.IsStartWebServer;
            this.tbPort.Text = webServerConfig.Port.ToString();
            this.tbRowsPerPage.Text = webServerConfig.RowsPerPage.ToString();

            this.btnApply.Visible = buttonApplyVisible;
            this.Icon = Properties.Resources.Film;
        }

        private bool ApplyAction()
        {
            if (Helper.PortInUse(Convert.ToInt32(this.tbPort.Text)))
            {
                MessageBox.Show(string.Format("端口号 \'{0}\' 已被占用", this.tbPort.Text),
                    "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

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

            return true;
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            ApplyAction();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (ApplyAction()) this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
