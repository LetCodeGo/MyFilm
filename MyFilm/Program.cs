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

                    //ProcessSendData.SendData(CommonString.WebSearchKeyWord);

                    foreach (Process process in processcollection)
                    {
                        if (process.Id != currentproc.Id)
                        {
                            // 如果进程的句柄为0，即代表没有找到该窗体，即该窗体隐藏的情况时
                            if (process.MainWindowHandle.ToInt32() == 0)
                            {
                                // 获得窗体句柄
                                IntPtr formhwnd = Win32API.FindWindow(null, "文件");
                                // 重新显示该窗体并切换到带入到前台
                                Win32API.ShowWindow(formhwnd, Win32API.SW_RESTORE);
                                Win32API.SwitchToThisWindow(formhwnd, true);
                            }
                            else
                            {
                                // 如果窗体没有隐藏，就直接切换到该窗体并带入到前台
                                // 因为窗体除了隐藏到托盘，还可以最小化
                                Win32API.SwitchToThisWindow(process.MainWindowHandle, true);
                            }

                            if (!String.IsNullOrWhiteSpace(CommonString.WebSearchKeyWord))
                            {
                                ProcessSendData.SendData(CommonString.WebSearchKeyWord);
                            }
                        }
                    }
                }
            }
        }
    }
}
