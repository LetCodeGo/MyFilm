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
    public partial class WaitingForm : Form
    {
        public WaitingForm()
        {
            InitializeComponent();
        }

        private Bitmap bitmap = null;

        private Image m_Image = null;

        private EventHandler evtHandler = null;

        // 重载的当前winform的OnPaint方法，当界面被重绘时去显示当前gif显示某一帧
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (m_Image != null)
            {
                // 获得当前gif动画下一步要渲染的帧。
                UpdateImage();

                Graphics.FromImage(bitmap).DrawImage(m_Image, 0, 0, bitmap.Width, bitmap.Height);
                Graphics.FromImage(bitmap).DrawString("正在从网页抓取数据，请等待！",
                    new Font("微软雅黑", 20, FontStyle.Bold), Brushes.Orange, 12, 200);
                e.Graphics.DrawImage(bitmap, 0, 0, this.Width, this.Height);

                // 将获得的当前gif动画需要渲染的帧显示在界面上的某个位置。
                //e.Graphics.DrawImage(m_Image, new Rectangle(0, 0, this.Width, this.Height));
            }
        }

        // 开始动画方法
        private void BeginAnimate()
        {
            if (m_Image != null)
            {
                // 当gif动画每隔一定时间后，都会变换一帧，那么就会触发一事件，
                // 该方法就是将当前image每变换一帧时，都会调用当前这个委托所关联的方法。
                ImageAnimator.Animate(m_Image, evtHandler);
            }
        }

        // 委托所关联的方法
        private void OnImageAnimate(Object sender, EventArgs e)
        {
            // 该方法中，只是使得当前这个winfor重绘，
            // 然后去调用该winform的OnPaint（）方法进行重绘)
            this.Invalidate();
        }

        // 获得当前gif动画的下一步需要渲染的帧，
        // 当下一步任何对当前gif动画的操作都是对该帧进行操作)
        private void UpdateImage()
        {
            ImageAnimator.UpdateFrames(m_Image);
        }

        // 关闭显示动画，该方法可以在winform关闭时，或者某个按钮的触发事件中进行调用，
        // 以停止渲染当前gif动画。
        private void StopAnimate()
        {
            m_Image = null;
            ImageAnimator.StopAnimate(m_Image, evtHandler);
        }

        public void SetFinish()
        {
            this.Invoke(new Action(() => { StopAnimate(); this.Close(); }));
        }

        private void WaitingForm_Load(object sender, EventArgs e)
        {
            bitmap = new Bitmap(this.Width, this.Height);

            // 为委托关联一个处理方法
            evtHandler = new EventHandler(OnImageAnimate);

            // 获取要加载的gif动画文件
            //m_Image = Image.FromFile(Application.StartupPath + "\\waiting.gif");
            m_Image = MyFilm.Properties.Resources.waiting;

            // 调用开始动画方法
            BeginAnimate();
        }
    }
}
