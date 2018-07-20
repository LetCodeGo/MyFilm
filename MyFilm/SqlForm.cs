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

        public Action<string, HashSet<String>> SqlQueryAction = null;

        public Action SqlFormColsedAction = null;

        public SqlForm(SqlData sqlData, string strInfo)
        {
            InitializeComponent();
            this.textBoxSql.Init("SELECT * FROM film_info WHERE ");
            this.sqlData = sqlData;
            this.richTextBoxInfo.Text = "mysql> desc film_info" + Environment.NewLine;
            this.richTextBoxInfo.AppendText(strInfo + Environment.NewLine);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            HashSet<String> noOutSet = new HashSet<String>();

            CheckBox[] cbs = new CheckBox[]
            {
            this.cbid,
            this.cbname,
            this.cbpath,
            this.cbsize,
            this.cbcreate_t,
            this.cbto_delete,
            this.cbs_w_t,
            this.cbto_watch,
            this.cbis_folder,
            this.cbmodify_t,
            this.cbdisk_desc,
            this.cbmax_cid,
            this.cbpid,
            this.cbcontent,
            this.cbs_d_t,
            };

            foreach (CheckBox cb in cbs)
            {
                if (!cb.Checked) noOutSet.Add(cb.Text);
            }

            String sqlStr = this.textBoxSql.Text;
            this.richTextBoxInfo.AppendText(string.Format("mysql> {0}", sqlStr) + Environment.NewLine);

            if (this.cbNoGrid.Checked)
            {
                String errStr = String.Empty;
                String rstStr = String.Empty;
                DataTable dt = sqlData.GetDataTableDataBySql(sqlStr, ref errStr);

                if (dt == null || dt.Rows.Count == 0) rstStr = errStr;
                else rstStr = CommonDataTable.DataTableFormatToString(dt, noOutSet);
                this.richTextBoxInfo.AppendText(rstStr + Environment.NewLine);

                richTextBoxInfo.SelectionStart = richTextBoxInfo.Text.Length;
                richTextBoxInfo.SelectionLength = 0;
                richTextBoxInfo.Focus();
            }
            else
            {
                SqlQueryAction?.Invoke(sqlStr, noOutSet);
            }
        }

        public void UpdateRichTextBox(string updateText)
        {
            this.richTextBoxInfo.AppendText(updateText + Environment.NewLine);

            richTextBoxInfo.SelectionStart = richTextBoxInfo.Text.Length;
            richTextBoxInfo.SelectionLength = 0;
        }

        private void textBoxSql_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                btnSearch_Click(null, null);
                e.Handled = false;
            }
        }

        private void SqlForm_Load(object sender, EventArgs e)
        {
            this.cbid.Checked = false;
            this.cbname.Checked = true;
            this.cbpath.Checked = true;
            this.cbsize.Checked = true;
            this.cbcreate_t.Checked = true;
            this.cbto_delete.Checked = true;
            this.cbs_w_t.Checked = true;
            this.cbto_watch.Checked = true;
            this.cbis_folder.Checked = true;
            this.cbmodify_t.Checked = true;
            this.cbdisk_desc.Checked = true;
            this.cbmax_cid.Checked = false;
            this.cbpid.Checked = false;
            this.cbcontent.Checked = false;
            this.cbs_d_t.Checked = true;
        }

        private void SqlForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            SqlFormColsedAction?.Invoke();
        }

        private void cbNoGrid_CheckedChanged(object sender, EventArgs e)
        {
            this.textBoxSql.PrefixEnabled(!this.cbNoGrid.Checked);
        }
    }
}
