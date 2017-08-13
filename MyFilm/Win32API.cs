using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MyFilm
{
    public class Win32API
    {
        #region [ API: 记事本 ] 

        /// <summary> 
        /// 传递消息给记事本 
        /// </summary> 
        /// <param name="hWnd"></param> 
        /// <param name="Msg"></param> 
        /// <param name="wParam"></param> 
        /// <param name="lParam"></param> 
        /// <returns></returns> 
        [DllImport("User32.DLL")]
        public static extern int SendMessage(
            IntPtr hWnd, int Msg, int wParam, string lParam);

        /// <summary> 
        /// 查找句柄 
        /// </summary> 
        /// <param name="hwndParent"></param> 
        /// <param name="hwndChildAfter"></param> 
        /// <param name="lpszClass"></param> 
        /// <param name="lpszWindow"></param> 
        /// <returns></returns> 
        [DllImport("User32.DLL")]
        public static extern IntPtr FindWindowEx(
            IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        /// <summary> 
        /// 记事本需要的常量 
        /// </summary> 
        public const int WM_SETTEXT = 0x000C;

        #endregion

        #region 

        /// <summary>
        /// 自定义的结构
        /// </summary>
        public struct My_lParam
        {
            public int i;
            public string s;
        }

        /// <summary>
        /// 使用COPYDATASTRUCT来传递字符串
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData;
        }

        //消息发送API
        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        public static extern int SendMessage(
            IntPtr hWnd,        // 信息发往的窗口的句柄
            int Msg,            // 消息ID
            int wParam,         // 参数1
            int lParam          // 参数2
        );

        //消息发送API
        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        public static extern int SendMessage(
            IntPtr hWnd,         // 信息发往的窗口的句柄
            int Msg,             // 消息ID
            int wParam,          // 参数1
            ref My_lParam lParam // 参数2
        );

        //消息发送API
        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        public static extern int SendMessage(
            IntPtr hWnd,               // 信息发往的窗口的句柄
            int Msg,                   // 消息ID
            int wParam,                // 参数1
            ref COPYDATASTRUCT lParam  // 参数2
        );

        //消息发送API
        [DllImport("User32.dll", EntryPoint = "PostMessage")]
        public static extern int PostMessage(
            IntPtr hWnd,          // 信息发往的窗口的句柄
            int Msg,              // 消息ID
            int wParam,           // 参数1
            int lParam            // 参数2
        );

        //消息发送API
        [DllImport("User32.dll", EntryPoint = "PostMessage")]
        public static extern int PostMessage(
            IntPtr hWnd,         // 信息发往的窗口的句柄
            int Msg,             // 消息ID
            int wParam,          // 参数1
            ref My_lParam lParam // 参数2
        );

        //异步消息发送API
        [DllImport("User32.dll", EntryPoint = "PostMessage")]
        public static extern int PostMessage(
            IntPtr hWnd,        // 信息发往的窗口的句柄
            int Msg,            // 消息ID
            int wParam,         // 参数1
            ref COPYDATASTRUCT lParam  // 参数2
        );

        public readonly static int WM_USER = 0x0400;
        public const int WM_SEARCH = 0x0410;

        #endregion

        #region

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
        public static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);

        /// <summary>
        ///  设置窗口的显示状态
        ///  Win32 函数定义为：http://msdn.microsoft.com/en-us/library/windows/desktop/ms633548(v=vs.85).aspx
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <param name="cmdShow">指示窗口如何被显示</param>
        /// <returns>如果窗体之前是可见，返回值为非零；如果窗体之前被隐藏，返回值为零</returns>
        [DllImport("user32.dll", EntryPoint = "ShowWindow", CharSet = CharSet.Auto)]
        public static extern int ShowWindow(IntPtr hwnd, int nCmdShow);
        public const int SW_RESTORE = 9;

        #endregion
    }
}
