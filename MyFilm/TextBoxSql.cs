using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyFilm
{
    public class TextBoxSql : TextBox
    {
        private string prefix = string.Empty;
        private bool prefixEnabled = true;

        public void Init(string prefix)
        {
            this.prefix = prefix;
            this.Text = prefix;
            this.SelectionStart = this.prefix.Length;
            this.SelectionLength = 0;
        }

        public void PrefixEnabled(bool enabled)
        {
            if (this.prefixEnabled != enabled)
            {
                this.prefixEnabled = enabled;

                if (this.prefixEnabled)
                {
                    this.Text = prefix;
                }
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // 0x0301：复制（包括ctrl + c）message ID
            // 0x0302：粘贴（包括ctrl + v）message ID
            // 光标在 prifix 之前禁止复制、粘贴
            if (this.prefixEnabled && this.SelectionStart < this.prefix.Length &&
                (keyData == (Keys.Control | Keys.C) || keyData == (Keys.Control | Keys.V)))
                return true;
            else
                return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override void WndProc(ref Message m)
        {
            // 禁止右键
            if (m.Msg != 0x007B) base.WndProc(ref m);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            e.Handled = this.prefixEnabled && (
                this.SelectionStart < this.prefix.Length ||
                (this.SelectionStart == this.prefix.Length && e.KeyChar == (char)8));
            // 回车
            if (e.KeyChar == (char)13) e.Handled = false;
        }
    }
}
