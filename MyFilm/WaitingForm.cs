using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static MyFilm.RealOrFake4KWebDataCapture;

namespace MyFilm
{
    public partial class WaitingForm : Form
    {
        private Bitmap bitmap = null;

        private Image image = null;

        private Mutex mutex = new Mutex();

        private RealOrFake4KWebDataCapture webDataCapture = null;

        public WaitingForm(ThreadWebDataCaptureCallback threadCallback)
        {
            InitializeComponent();

            bitmap = new Bitmap(this.Width, this.Height);
            image = MyFilm.Properties.Resources.waiting;

            ImageAnimator.Animate(image, this.OnImageAnimate);

            this.webDataCapture = new RealOrFake4KWebDataCapture(
                threadCallback, SetFinish);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            mutex.WaitOne();
            if (image != null)
            {
                ImageAnimator.UpdateFrames(image);

                Graphics.FromImage(bitmap).DrawImage(image, 0, 0, bitmap.Width, bitmap.Height);
                Graphics.FromImage(bitmap).DrawString("正在从网页抓取数据，请等待！",
                    new Font("微软雅黑", 20, FontStyle.Bold), Brushes.Orange, 12, 200);
                e.Graphics.DrawImage(bitmap, 0, 0, this.Width, this.Height);
            }
            mutex.ReleaseMutex();
        }

        private void OnImageAnimate(Object sender, EventArgs e)
        {
            this.Invalidate();
        }

        // 获得当前gif动画的下一步需要渲染的帧，
        // 当下一步任何对当前gif动画的操作都是对该帧进行操作)
        private void UpdateImage()
        {
            ImageAnimator.UpdateFrames(image);
        }

        public void SetFinish()
        {
            this.Invoke(new Action(() =>
            {
                mutex.WaitOne();
                ImageAnimator.StopAnimate(image, this.OnImageAnimate);
                image = null;
                mutex.ReleaseMutex();
                this.Close();
            }));
        }

        private void WaitingForm_Load(object sender, EventArgs e)
        {
            Thread threadWebDataCapture = new Thread(
                new ThreadStart(this.webDataCapture.Update4KInfo));
            threadWebDataCapture.Start();
        }
    }
}
