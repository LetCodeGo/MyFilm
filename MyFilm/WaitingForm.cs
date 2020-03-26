using System;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using static MyFilm.RealOrFake4KWebDataCapture;

namespace MyFilm
{
    public partial class WaitingForm : Form
    {
        private enum WaitingType
        {
            RealOrFake4KWebDataCapture,
            CopyDataBaseData
        }

        private Bitmap bitmap = null;

        private Image image = null;

        private Mutex mutex = new Mutex();

        private RealOrFake4KWebDataCapture webDataCapture = null;

        private SqlData sqlDataFrom = null;
        private SqlData sqlDataTo = null;

        private WaitingType waitingType = WaitingType.RealOrFake4KWebDataCapture;

        public WaitingForm(
            ThreadWebDataCaptureCallback threadCallback, SqlData sqlData, String crawlURL)
        {
            InitializeComponent();

            waitingType = WaitingType.RealOrFake4KWebDataCapture;
            bitmap = new Bitmap(this.Width, this.Height);
            image = MyFilm.Properties.Resources.waiting;

            ImageAnimator.Animate(image, this.OnImageAnimate);

            this.webDataCapture = new RealOrFake4KWebDataCapture(
                threadCallback, SetFinish, sqlData, crawlURL);
        }

        public WaitingForm(SqlData sqlDataFrom, SqlData sqlDataTo)
        {
            InitializeComponent();

            waitingType = WaitingType.CopyDataBaseData;
            this.sqlDataFrom = sqlDataFrom;
            this.sqlDataTo = sqlDataTo;

            bitmap = new Bitmap(this.Width, this.Height);
            image = MyFilm.Properties.Resources.waiting;

            ImageAnimator.Animate(image, this.OnImageAnimate);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            mutex.WaitOne();
            if (image != null)
            {
                ImageAnimator.UpdateFrames(image);

                Graphics.FromImage(bitmap).DrawImage(image, 0, 0, bitmap.Width, bitmap.Height);
                StringFormat stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Center;
                int y = ((bitmap.Height / 2) + 60);
                RectangleF rectangleF = new Rectangle(0, y, bitmap.Width, bitmap.Height - y);
                Graphics.FromImage(bitmap).DrawString(
                    this.waitingType == WaitingType.RealOrFake4KWebDataCapture ? "正在从网页抓取数据，请等待" : "正在复制数据库数据，请等待",
                    new Font("微软雅黑", 16, FontStyle.Bold), Brushes.Orange, rectangleF, stringFormat);

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
            if (this.waitingType == WaitingType.RealOrFake4KWebDataCapture)
            {
                Thread threadWebDataCapture = new Thread(
                    new ThreadStart(this.webDataCapture.Update4KInfo));
                threadWebDataCapture.Start();
            }
            else
            {
                Thread threadCopyDataBaseData = new Thread(new ThreadStart(this.CopyDataBaseData));
                threadCopyDataBaseData.Start();
            }
        }

        private void CopyDataBaseData()
        {
            DataTable filmInfoDataTable = sqlDataFrom.GetFilmInfoDatabaseTransferData();
            DataTable diskInfoDataTable = sqlDataFrom.GetDiskInfoDatabaseTransferData();
            DataTable searchLogDataTable = sqlDataFrom.GetSearchLogDatabaseTransferData();

            if (CommonString.NeedDeleteAllTableData)
            {
                sqlDataTo.DeleteAllDataFormAllTable();
            }

            sqlDataTo.InsertDataToFilmInfo(filmInfoDataTable);
            sqlDataTo.InsertDataToDiskInfo(diskInfoDataTable);
            sqlDataTo.InsertDataToSearchLog(searchLogDataTable);

            SetFinish();
        }
    }
}
