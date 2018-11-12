using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace MyFilm
{
    public partial class SqlForm : Form
    {
        /// <summary>
        /// 输出设置
        /// </summary>
        private HashSet<string> outputSet = null;

        public Action<string> SqlQueryAction = null;

        public Action SqlFormColsedAction = null;

        private CheckBox[] cbs = null;

        public SqlForm()
        {
            InitializeComponent();

            this.textBoxSql.Init("SELECT * FROM film_info WHERE ");
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            String cmdText = this.textBoxSql.Text;

            if (this.cbNoGrid.Checked)
            {
                this.richTextBoxInfo.AppendText(
                    string.Format("mysql> {0}", cmdText) + Environment.NewLine);

                String errMsg = String.Empty;
                String outputText = String.Empty;

                DataTable dt = SqlData.GetInstance().SelectAllDataBySqlText(cmdText, ref errMsg);

                if (dt == null)
                {
                    outputText = errMsg + Environment.NewLine;
                    UpdateRichTextBox(outputText);
                }
                else UpdateRichTextBox(CommonDataTable.ConvertFilmInfoToGrid(dt));
            }
            else
            {
                SqlQueryAction?.Invoke(cmdText);
            }

            this.richTextBoxInfo.Focus();
        }

        public void UpdateRichTextBox(DataTable dt)
        {
            this.richTextBoxInfo.AppendText(
                CommonDataTable.DataTableFormatToString(dt, this.outputSet) + Environment.NewLine);

            richTextBoxInfo.SelectionStart = richTextBoxInfo.Text.Length;
            richTextBoxInfo.SelectionLength = 0;
        }

        public void UpdateRichTextBox(String outputText)
        {
            this.richTextBoxInfo.AppendText(outputText + Environment.NewLine);

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
            this.cbindex.Checked = true;
            this.cbid.Checked = false;
            this.cbname.Checked = true;
            this.cbpath.Checked = true;
            this.cbsize.Checked = true;
            this.cbcreate_t.Checked = true;
            this.cbto_delete.Checked = true;
            this.cbto_delete_ex.Checked = true;
            this.cbs_w_t.Checked = true;
            this.cbto_watch.Checked = true;
            this.cbto_watch_ex.Checked = true;
            this.cbis_folder.Checked = true;
            this.cbmodify_t.Checked = true;
            this.cbdisk_desc.Checked = true;
            this.cbmax_cid.Checked = false;
            this.cbpid.Checked = false;
            this.cbcontent.Checked = false;
            this.cbs_d_t.Checked = true;

            cbs = new CheckBox[]
            {
                this.cbindex,
                this.cbid,
                this.cbname,
                this.cbpath,
                this.cbsize,
                this.cbcreate_t,
                this.cbto_delete,
                this.cbto_delete_ex,
                this.cbs_w_t,
                this.cbto_watch,
                this.cbto_watch_ex,
                this.cbis_folder,
                this.cbmodify_t,
                this.cbdisk_desc,
                this.cbmax_cid,
                this.cbpid,
                this.cbcontent,
                this.cbs_d_t,
            };

            this.outputSet = new HashSet<string>();

            foreach (CheckBox cb in cbs)
            {
                if (cb.Checked) this.outputSet.Add(cb.Text);

                cb.CheckedChanged += this.output_CheckedChanged;
            }

            this.cbOutputCtl.Checked = true;
            this.cbOutputCtl.CheckedChanged += this.outputCtl_CheckedChanged;

            this.richTextBoxInfo.Text = "mysql> desc film_info" + Environment.NewLine;

            String descFilmInfo = CommonDataTable.DataTableFormatToString(
                SqlData.GetInstance().GetDescriptionOfFilmInfo(), null);
            UpdateRichTextBox(descFilmInfo);
        }

        private void SqlForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            SqlFormColsedAction?.Invoke();
        }

        private void cbNoGrid_CheckedChanged(object sender, EventArgs e)
        {
            this.textBoxSql.PrefixEnabled(!this.cbNoGrid.Checked);
        }

        private void output_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb.Checked)
            {
                if (!this.outputSet.Contains(cb.Text)) this.outputSet.Add(cb.Text);
            }
            else if (this.outputSet.Contains(cb.Text)) this.outputSet.Remove(cb.Text);
        }

        private void outputCtl_CheckedChanged(object sender, EventArgs e)
        {
            if (this.cbOutputCtl.Checked)
            {
                this.outputSet = new HashSet<string>();

                foreach (CheckBox cb in cbs)
                {
                    cb.Enabled = true;
                    if (cb.Checked) this.outputSet.Add(cb.Text);
                }
            }
            else
            {
                foreach (CheckBox cb in cbs)
                {
                    cb.Enabled = false;
                }

                this.outputSet = null;
            }
        }
    }
}
