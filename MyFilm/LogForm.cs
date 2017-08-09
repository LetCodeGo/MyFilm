﻿using System;
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
    public partial class LogForm : Form
    {
        public LogForm(String log)
        {
            InitializeComponent();
            this.richTextBox.Text = log;
            this.Icon = Properties.Resources.ico;
        }
    }
}
