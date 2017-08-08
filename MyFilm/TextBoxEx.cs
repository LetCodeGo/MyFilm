using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyFilm
{
    public class TextBoxEx : TextBox
    {
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.C) || keyData == (Keys.Control | Keys.V))
                return true;
            else
                return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg != 0x007B) base.WndProc(ref m);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            // 允许输入数字，限制输入长度为3
            e.Handled = (!Char.IsDigit(e.KeyChar)) || (this.Text.Length >= 3);
            // 允许输入退格
            if (e.KeyChar == (char)8)
            {
                // 当长度为0时，覆盖
                if (this.Text.Length == 1)
                {
                    this.SelectionStart = 0;
                    this.SelectionLength = 1;
                    e.Handled = true;
                }
                else
                {
                    e.Handled = false;
                }
            }
        }
    }
}
