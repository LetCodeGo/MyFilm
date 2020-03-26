using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace MyFilm
{
    public class TextBoxEx : TextBox
    {
        private int TextMaxLength = 3;
        private int? MinValue = 1;
        private int? MaxValue = null;

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

        public void SetTextMaxLength(int length)
        {
            TextMaxLength = length;
        }

        public void SetMinValue(int? minValue)
        {
            this.MinValue = minValue;
        }

        public void SetMaxValue(int? maxValue)
        {
            this.MaxValue = maxValue;
        }

        protected override void OnValidating(CancelEventArgs e)
        {
            int num = 0;
            if (Int32.TryParse(this.Text, out num))
            {
                if (MinValue == null)
                {
                    if (MaxValue != null && num > MaxValue)
                    {
                        MessageBox.Show(string.Format(
                            "输入的 \'{0}\' 不合法，请输入一个小于或等于 \'{1}\' 的数",
                            this.Text, MaxValue), "提示",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        e.Cancel = true;
                    }
                }
                else
                {
                    if (MaxValue == null && num < MinValue)
                    {
                        MessageBox.Show(string.Format(
                            "输入的 \'{0}\' 不合法，请输入一个大于或等于 \'{1}\' 的数",
                            this.Text, MinValue), "提示",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        e.Cancel = true;
                    }
                    else if (MaxValue != null && (num < MinValue || num > MaxValue))
                    {
                        MessageBox.Show(string.Format(
                            "输入的 \'{0}\' 不合法，请输入一个 \'{1}\' 到 \'{2}\' 的数",
                            this.Text, MinValue, MaxValue), "提示",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        e.Cancel = true;
                    }
                }
            }
            else
            {
                MessageBox.Show(string.Format("输入的 \'{0}\' 不合法，请输入一个有效的数",
                    this.Text), "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Cancel = true;
            }
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            // 允许输入数字，限制输入长度为3
            e.Handled = (!Char.IsDigit(e.KeyChar)) ||
                ((this.Text.Length - this.SelectionLength) >= TextMaxLength);
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
