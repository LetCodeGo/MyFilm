using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace MyFilm
{
    public class ProcessReceiveData
    {
        #region Win32 API

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
        #endregion

        public static bool receiveExit = false;
        public const int WM_SEARCH = 0x0410;

        public static void ReceiveDataByPipe(object hWnd)
        {
            using (NamedPipeServerStream pipeServer =
                new NamedPipeServerStream(
                    "myfilmpipe", PipeDirection.InOut, 1, PipeTransmissionMode.Byte))
            {
                while (!receiveExit)
                {
                    pipeServer.WaitForConnection();

                    byte[] bytes = new byte[1024];
                    int length = pipeServer.Read(bytes, 0, 1024);
                    CommonString.WebSearchKeyWord = Encoding.Default.GetString(bytes, 0, length);

                    if (!receiveExit)
                    {
                        PostMessage((IntPtr)hWnd, WM_SEARCH, 0, 0);
                    }

                    pipeServer.Disconnect();
                }
            }
        }

        public static void ReceiveDataBySharedMemory(object hWnd)
        {
            long capacity = 1 << 10;

            // 打开共享内存
            using (var mmf = MemoryMappedFile.OpenExisting("myfilmMMF"))
            {
                // 使用CreateViewStream方法返回stream实例  
                using (var mmViewStream = mmf.CreateViewStream(0, capacity))
                {
                    // 这里要制定Unicode编码否则会出问题  
                    using (BinaryReader br = new BinaryReader(mmViewStream, Encoding.Unicode))
                    {
                        while (!receiveExit)
                        {
                            mmViewStream.Seek(0, SeekOrigin.Begin);

                            int length = br.ReadInt32();
                            char[] chars = br.ReadChars(length);
                            CommonString.WebSearchKeyWord = new String(chars);

                            if (!receiveExit)
                            {
                                PostMessage((IntPtr)hWnd, WM_SEARCH, 0, 0);
                            }
                        }
                    }
                }
            }
        }
    }
}
