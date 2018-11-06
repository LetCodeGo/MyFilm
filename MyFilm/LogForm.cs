using System;
using System.Windows.Forms;

namespace MyFilm
{
    public partial class LogForm : Form
    {
        public LogForm(String log)
        {
            InitializeComponent();
            this.richTextBox.Text = log;
            this.Icon = Properties.Resources.Film;
        }
    }
}
