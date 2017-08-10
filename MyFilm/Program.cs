using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyFilm
{
    static class Program
    {
        #region DLL IMPORT

        /// <summary>
        /// 找到某个窗口与给出的类别名和窗口名相同窗口
        /// 非托管定义为：http://msdn.microsoft.com/en-us/library/windows/desktop/ms633499(v=vs.85).aspx
        /// </summary>
        /// <param name="lpClassName">类别名</param>
        /// <param name="lpWindowName">窗口名</param>
        /// <returns>成功找到返回窗口句柄,否则返回null</returns>
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        /// <summary>
        /// 切换到窗口并把窗口设入前台,类似 SetForegroundWindow方法的功能
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <param name="fAltTab">True代表窗口正在通过Alt/Ctrl +Tab被切换</param>
        [DllImport("user32.dll ", SetLastError = true)]
        static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);

        ///// <summary>
        /////  设置窗口的显示状态
        /////  Win32 函数定义为：http://msdn.microsoft.com/en-us/library/windows/desktop/ms633548(v=vs.85).aspx
        ///// </summary>
        ///// <param name="hWnd">窗口句柄</param>
        ///// <param name="cmdShow">指示窗口如何被显示</param>
        ///// <returns>如果窗体之前是可见，返回值为非零；如果窗体之前被隐藏，返回值为零</returns>
        [DllImport("user32.dll", EntryPoint = "ShowWindow", CharSet = CharSet.Auto)]
        public static extern int ShowWindow(IntPtr hwnd, int nCmdShow);
        public const int SW_RESTORE = 9;
        public static IntPtr formhwnd = IntPtr.Zero;
        #endregion

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                string webSearchAddr = Uri.UnescapeDataString(args[0]);
                CommonString.WebSearchKeyWord = webSearchAddr.Substring(webSearchAddr.IndexOf(':') + 1).Trim();
            }

            bool createNew = true;
            //  createdNew:
            // 在此方法返回时，如果创建了局部互斥体（即，如果 name 为 null 或空字符串）或指定的命名系统互斥体，则包含布尔值 true；
            // 如果指定的命名系统互斥体已存在，则为false
            using (Mutex mutex = new Mutex(true, Application.ProductName, out createNew))
            {
                if (createNew)
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);

                    LoginForm loginForm = new LoginForm();
                    loginForm.ShowDialog();

                    if (loginForm.DialogResult == DialogResult.OK)
                    {
                        Application.Run(new MainForm());
                    }
                }
                // 程序已经运行
                else
                {
                    Process currentproc = Process.GetCurrentProcess();
                    Process[] processcollection = Process.GetProcessesByName(currentproc.ProcessName);
                    Debug.Assert(processcollection.Length >= 1);

                    //ProcessSendData.SendDataByPipe(CommonString.WebSearchKeyWord);

                    foreach (Process process in processcollection)
                    {
                        if (process.Id != currentproc.Id)
                        {
                            // 如果进程的句柄为0，即代表没有找到该窗体，即该窗体隐藏的情况时
                            if (process.MainWindowHandle.ToInt32() == 0)
                            {
                                // 获得窗体句柄
                                formhwnd = FindWindow(null, "文件");
                                // 重新显示该窗体并切换到带入到前台
                                ShowWindow(formhwnd, SW_RESTORE);
                                SwitchToThisWindow(formhwnd, true);
                            }
                            else
                            {
                                // 如果窗体没有隐藏，就直接切换到该窗体并带入到前台
                                // 因为窗体除了隐藏到托盘，还可以最小化
                                SwitchToThisWindow(process.MainWindowHandle, true);
                            }

                            if (!String.IsNullOrWhiteSpace(CommonString.WebSearchKeyWord))
                                ProcessSendData.SendDataByPipe(CommonString.WebSearchKeyWord);
                        }
                    }
                }
            }
        }
    }
}
