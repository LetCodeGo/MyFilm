using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyFilm
{
    public class TableControlEx : System.Windows.Forms.TabControl
    {
        public TableControlEx()
        {
            base.SetStyle(
                ControlStyles.UserPaint |                      // 控件将自行绘制，而不是通过操作系统来绘制
                ControlStyles.OptimizedDoubleBuffer |          // 该控件首先在缓冲区中绘制，而不是直接绘制到屏幕上，这样可以减少闪烁
                ControlStyles.AllPaintingInWmPaint |           // 控件将忽略 WM_ERASEBKGND 窗口消息以减少闪烁
                ControlStyles.ResizeRedraw |                   // 在调整控件大小时重绘控件
                ControlStyles.SupportsTransparentBackColor,    // 控件接受 alpha 组件小于 255 的 BackColor 以模拟透明
                true);                                         // 设置以上值为 true
            base.UpdateStyles();

            //this.SizeMode = TabSizeMode.Fixed;
            this.ItemSize = new Size(65, 25);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            for (int i = 0; i < this.TabCount; i++)
            {
                Rectangle pageRect = this.GetTabRect(i);

                Brush stringBrush = SystemBrushes.ControlText;

                if (this.SelectedIndex == i)
                {
                    stringBrush = new SolidBrush(Color.Red);

                    e.Graphics.DrawLines(Pens.Gray, new Point[] {
                        new Point(2, pageRect.Y + pageRect.Height),
                        new Point(pageRect.X, pageRect.Y + pageRect.Height),
                        new Point(pageRect.X, pageRect.Y),
                        new Point(pageRect.X + pageRect.Width, pageRect.Y),
                        new Point(pageRect.X + pageRect.Width, pageRect.Y + pageRect.Height),
                        new Point(this.Width-2, pageRect.Y + pageRect.Height),
                        new Point(this.Width-2, this.Height-2),
                        new Point(2, this.Height-2),
                        new Point(2, pageRect.Y + pageRect.Height)});
                }

                StringFormat stringFormat = new StringFormat()
                {
                    LineAlignment = StringAlignment.Center,
                    Alignment = StringAlignment.Center
                };

                e.Graphics.DrawString(this.TabPages[i].Text,
                    this.Font, stringBrush, pageRect, stringFormat);
            }
        }
    }
}