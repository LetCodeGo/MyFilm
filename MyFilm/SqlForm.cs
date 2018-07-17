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
    public partial class SqlForm : Form
    {
        private SqlData sqlData = null;

        public SqlForm(SqlData sqlData, string strInfo)
        {
            InitializeComponent();

            this.sqlData = sqlData;
            this.richTextBoxInfo.Text = "mysql> desc film_info" + Environment.NewLine;
            this.richTextBoxInfo.AppendText(strInfo + Environment.NewLine);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            this.richTextBoxInfo.AppendText(string.Format("mysql> {0}", this.textBoxSql.Text) + Environment.NewLine);
            this.richTextBoxInfo.AppendText(sqlData.GetDataBySql(this.textBoxSql.Text) + Environment.NewLine);

            richTextBoxInfo.SelectionStart = richTextBoxInfo.Text.Length;
            richTextBoxInfo.SelectionLength = 0;
            richTextBoxInfo.Focus();
        }
    }
}
