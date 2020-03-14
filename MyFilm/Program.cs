using System;
using System.Threading;
using System.Windows.Forms;

namespace MyFilm
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // 有参数时，界面显示为搜索结果而不是根目录
            if (args.Length == 1)
            {
                // 网络地址需要转换，当然命名行传参也能正确处理
                string webSearchAddr = Uri.UnescapeDataString(args[0]);
                // 地址格式为 MyFilmWebSearch:KeyWord
                CommonString.WebSearchKeyWord = webSearchAddr.Substring(webSearchAddr.IndexOf(':') + 1).Trim();
            }

            bool createNew = true;
            // 如果指定的命名系统互斥体已存在，createNew则为false，说明已存在另一实例
            using (Mutex mutex = new Mutex(true, CommonString.AppMutexName, out createNew))
            {
                if (createNew)
                {
                    Application.ThreadException += Application_ThreadException;
                    Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                    AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);

                    LoginForm loginForm = new LoginForm();
                    loginForm.ShowDialog();

                    if (loginForm.DialogResult == DialogResult.OK)
                    {
                        Application.Run(new MainForm(loginForm.GetGeneratedSqlData()));
                    }
                }
                // 程序已经运行
                else
                {
                    // 进程间通信发送要搜索的关键字给已在运行的另一实例
                    if (!String.IsNullOrWhiteSpace(CommonString.WebSearchKeyWord))
                    {
                        ProcessSendData.SendData(CommonString.WebSearchKeyWord);
                    }

                    //Process currentproc = Process.GetCurrentProcess();
                    //Process[] processcollection = Process.GetProcessesByName(currentproc.ProcessName);
                    //Debug.Assert(processcollection.Length >= 1);

                    //foreach (Process process in processcollection)
                    //{
                    //    if (process.Id != currentproc.Id)
                    //    {
                    //        // 如果进程的句柄为0，即代表没有找到该窗体，即该窗体隐藏的情况时
                    //        if (process.MainWindowHandle.ToInt32() == 0)
                    //        {
                    //            // 获得窗体句柄
                    //            IntPtr formhwnd = Win32API.FindWindow(null, "文件");
                    //            // 重新显示该窗体并切换到带入到前台
                    //            Win32API.ShowWindow(formhwnd, Win32API.SW_RESTORE);
                    //            Win32API.SwitchToThisWindow(formhwnd, true);
                    //        }
                    //        else
                    //        {
                    //            // 如果窗体没有隐藏，就直接切换到该窗体并带入到前台
                    //            // 因为窗体除了隐藏到托盘，还可以最小化
                    //            Win32API.SwitchToThisWindow(process.MainWindowHandle, true);
                    //        }

                    //        // 进程间通信发送要搜索的关键字给已在运行的另一实例
                    //        if (!String.IsNullOrWhiteSpace(CommonString.WebSearchKeyWord))
                    //        {
                    //            ProcessSendData.SendData(CommonString.WebSearchKeyWord);
                    //        }
                    //    }
                    //}
                }
            }
        }

        /// <summary>
        /// 处理应用程序域内的未处理异常（非UI线程异常）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                Exception ex = e.ExceptionObject as Exception;
                MessageBox.Show(ex.InnerException.Message);
            }
            catch { }
        }

        /// <summary>
        /// 处理应用程序的未处理异常（UI线程异常）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            try
            {
                MessageBox.Show(e.Exception.Message);
            }
            catch { }
        }
    }
}
