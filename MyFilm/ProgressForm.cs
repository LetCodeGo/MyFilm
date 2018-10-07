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
    public partial class ProgressForm : Form
    {
        public ProgressForm()
        {
            InitializeComponent();
        }

        private void ProgressForm_Load(object sender, EventArgs e)
        {
            this.labelFile.Text = string.Empty;
            this.labelProgress.Text = "0.00%";
        }

        public void SetView(double pos, string msgStr)
        {
            this.Invoke(new Action(() =>
            {
                this.progressBar.Value = Convert.ToInt32(pos);
                this.labelFile.Text = msgStr;
                this.labelProgress.Text = pos.ToString("F2") + "%";
            }));
        }

        public void CloseForm()
        {
            this.Invoke(new Action(() => this.Close()));
        }
    }
}
