using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyFilm
{
    public partial class ProgressForm : Form
    {
        private ThreadScanDisk threadScanDisk = null;

        public ProgressForm(ThreadScanDisk threadScanDisk)
        {
            InitializeComponent();
            this.ControlBox = false;

            this.threadScanDisk = threadScanDisk;
            this.threadScanDisk.SetProgressMsg(
                new ThreadScanDisk.ThreadSacnDiskProgressSetView(SetPosAndMsg),
                new ThreadScanDisk.ThreadSacnDiskProgressFinish(SetFinish));
        }

        private void ProgressForm_Load(object sender, EventArgs e)
        {
            this.btnFinish.Enabled = false;
            this.richTextBox.Text = string.Empty;
            this.Text = "Progress [0.00%]";

            Thread threadScan = new Thread(new ThreadStart(this.threadScanDisk.ScanDisk));
            threadScan.Start();
        }

        public void SetPosAndMsg(double pos, string msgStr)
        {
            this.Invoke(new Action(() =>
            {
                this.progressBar.Value = Convert.ToInt32(pos);
                this.richTextBox.Text += string.Format("[{0}]  {1}\r\n",
                    DateTime.Now.ToString("yyyy-MM-dd HHH:mm:ss"), msgStr);
                this.Text = string.Format("Progress [{0}%]", pos.ToString("F2"));
            }));
        }

        public void SetFinish()
        {
            this.Invoke(new Action(() => { this.btnFinish.Enabled = true; }));
        }

        private void richTextBox_TextChanged(object sender, EventArgs e)
        {
            this.richTextBox.SelectionStart = this.richTextBox.Text.Length;
            this.richTextBox.ScrollToCaret();
        }

        private void btnFinish_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
